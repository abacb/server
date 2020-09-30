using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Rms.Server.Core.Abstraction.Models;
using Rms.Server.Core.Abstraction.Repositories;
using Rms.Server.Core.Service.Models;
using Rms.Server.Core.Utility;
using Rms.Server.Core.Utility.Exceptions;
using Rms.Server.Core.Utility.Extensions;
using Rms.Server.Core.Utility.Models;
using Rms.Server.Core.Utility.Models.Entites;
using Rms.Server.Core.Utility.Properties;
using System;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Transactions;

namespace Rms.Server.Core.Service.Services
{
    /// <summary>
    /// デバイスサービス
    /// </summary>
    public class DeviceService : IDeviceService
    {
        /// <summary>
        /// 設定
        /// </summary>
        private readonly AppSettings _settings;

        /// <summary>
        /// ロガー
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// IRequestDeviceRepository
        /// </summary>
        private readonly IRequestDeviceRepository _requestDeviceRepository;

        /// <summary>
        /// IDtDeviceRepository
        /// </summary>
        private readonly IDtDeviceRepository _dtDeviceRepository;

        /// <summary>
        /// IDeliveringRepository
        /// </summary>
        private readonly IDeliveringRepository _deliveringRepository;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="settings">設定</param>
        /// <param name="logger">ロガー</param>
        /// <param name="requestDeviceRepository">requestDeviceRepository</param>
        /// <param name="dtDeviceRepository">dtDeviceRepository</param>
        /// <param name="deliveringRepository">deliveringRepository</param>
        public DeviceService(
            AppSettings settings,
            ILogger<DeviceService> logger,
            IRequestDeviceRepository requestDeviceRepository,
            IDtDeviceRepository dtDeviceRepository,
            IDeliveringRepository deliveringRepository)
        {
            Assert.IfNull(settings);
            Assert.IfNull(logger);
            Assert.IfNull(requestDeviceRepository);
            Assert.IfNull(dtDeviceRepository);
            Assert.IfNull(deliveringRepository);

            _settings = settings;
            _logger = logger;
            _requestDeviceRepository = requestDeviceRepository;
            _dtDeviceRepository = dtDeviceRepository;
            _deliveringRepository = deliveringRepository;
        }

        /// <summary>
        /// RequestRemoteAsyncStatusメソッド呼び出し中の処理ステータス
        /// </summary>
        private enum RequestRemoteAsyncStatus
        {
            /// <summary>
            /// ReadDtDevice呼び出し中
            /// </summary>
            ReadDtDevice = 0,

            /// <summary>
            /// GetDeviceConnectionInfoAsync呼び出し中
            /// </summary>
            GetDeviceConnectionInfoAsync,

            /// <summary>
            /// SendMessageAsync呼び出し中
            /// </summary>
            SendMessageAsync,
        }

        /// <summary>
        /// CreateメソッドとUpdateメソッド呼び出し中の処理ステータス
        /// </summary>
        private enum CreateAndUpdateStatus
        {
            /// <summary>
            /// CreateDtDevice呼び出し中
            /// </summary>
            CreateDtDevice = 0,

            /// <summary>
            /// UpdateDtDevice呼び出し中
            /// </summary>
            UpdateDtDevice,

            /// <summary>
            /// MakeArchiveFile呼び出し中
            /// </summary>
            MakeArchiveFile,

            /// <summary>
            /// Upload呼び出し中
            /// </summary>
            Upload,

            /// <summary>
            /// その他メソッド呼び出し中
            /// </summary>
            Other,
        }

        /// <summary>
        /// 機器情報を保存する
        /// </summary>
        /// <remarks>sd 04-01.機器登録</remarks>
        /// <param name="utilParam">Utility型機器情報</param>
        /// <param name="baseConfig">設置設定情報</param>
        /// <returns>結果付きDB保存済み機器情報</returns>
        public Result<DtDevice> Create(DtDevice utilParam, InstallBaseConfig baseConfig)
        {
            Result<DtDevice> result = null;
            CreateAndUpdateStatus status = CreateAndUpdateStatus.Other;

            try
            {
                _logger.EnterJson("In Param: {0}", utilParam);

                // 一連の動きのためトランザクション処理開始
                using (TransactionScope scope = new TransactionScope())
                {
                    // Sq3.2.1:機器情報を保存する
                    status = CreateAndUpdateStatus.CreateDtDevice;
                    DtDevice newDeviceInfo = _dtDeviceRepository.CreateDtDevice(utilParam);
                    if (newDeviceInfo == null)
                    {
                        // unconnectedのデータがないとnullが返る
                        _logger.Error(nameof(Resources.CO_API_DVC_004), new object[] { "接続ステータスが「unconnected」のデータがDBにありません" });
                        return new Result<DtDevice>(
                            ResultCode.ServerEerror,
                            string.Format(Resources.CO_API_DVC_004, new object[] { "接続ステータスが「unconnected」のデータがDBにありません" }),
                            utilParam);
                    }

                    // 設置設定インスタンスにエッジIDを登録する
                    string edgeId = newDeviceInfo.EdgeId.ToString();
                    baseConfig.OwnConfig.EdgeId = edgeId;

                    string configMessage = string.Empty;

                    // Sq3.2.2:設置設定ファイルを生成する(機器情報)
                    status = CreateAndUpdateStatus.MakeArchiveFile;
                    var archiveFile = MakeArchiveFile(
                        baseConfig,
                        newDeviceInfo.EquipmentUid,
                        newDeviceInfo.CreateDatetime,
                        out configMessage);

                    // Sq3.2.3:設置設定ファイルをアップロードする(設置設定ファイル)
                    status = CreateAndUpdateStatus.Upload;
                    _deliveringRepository.Upload(archiveFile, configMessage);

                    // 正常終了
                    status = CreateAndUpdateStatus.Other;
                    _logger.Info(nameof(Resources.CO_API_DVC_007), new object[] { edgeId });
                    result = new Result<DtDevice>(
                        ResultCode.Succeed,
                        string.Format(Resources.CO_API_DVC_007, new object[] { edgeId }),
                        newDeviceInfo);

                    scope.Complete();
                }

                return result;                
            }
            catch (RmsParameterException e)
            {
                // Sq3.2.1
                _logger.Error(e, nameof(Resources.CO_API_DVC_003), new object[] { e.Message });

                return new Result<DtDevice>(
                    ResultCode.ParameterError,
                    string.Format(Resources.CO_API_DVC_003, new object[] { e.Message }),
                    utilParam);
            }
            catch (Exception) when (status == CreateAndUpdateStatus.Other)
            {
                // 想定外のエラーは呼び出し側に返却する
                throw;
            }
            catch (Exception e)
            {
                string errorMessage = string.Empty;

                switch (status)
                {
                    case CreateAndUpdateStatus.CreateDtDevice:
                        // Sq3.2.1
                        _logger.Error(e, nameof(Resources.CO_API_DVC_004), new object[] { e.Message });
                        errorMessage = string.Format(Resources.CO_API_DVC_004, new object[] { e.Message });
                        break;
                    case CreateAndUpdateStatus.MakeArchiveFile:
                        // Sq3.2.2
                        _logger.Error(e, nameof(Resources.CO_API_DVC_005), new object[] { e.Message });
                        errorMessage = string.Format(Resources.CO_API_DVC_005, new object[] { e.Message });
                        break;
                    case CreateAndUpdateStatus.Upload:
                    default:
                        // Sq3.2.3
                        _logger.Error(e, nameof(Resources.CO_API_DVC_006), new object[] { e.Message });
                        errorMessage = string.Format(Resources.CO_API_DVC_006, new object[] { e.Message });
                        break;
                }

                return new Result<DtDevice>(ResultCode.ServerEerror, errorMessage, utilParam);
            }
            finally
            {
                _logger.LeaveJson("Out Param: {0}", result);
            }
        }

        /// <summary>
        /// 機器情報を更新する
        /// </summary>
        /// <remarks>sd 04-01.機器登録</remarks>
        /// <param name="utilParam">>Utility型機器情報</param>
        /// <param name="baseConfig">設置設定情報</param>
        /// <returns>結果付きDB保存済み機器情報</returns>
        public Result<DtDevice> Update(DtDevice utilParam, InstallBaseConfig baseConfig)
        {
            Result<DtDevice> result = null;
            CreateAndUpdateStatus status = CreateAndUpdateStatus.Other;

            try
            {
                _logger.EnterJson("In Param: {0}", utilParam);

                // 一連の動きのためトランザクション処理開始
                using (TransactionScope scope = new TransactionScope())
                {
                    // Sq3.2.1:機器情報を保存する
                    status = CreateAndUpdateStatus.UpdateDtDevice;
                    DtDevice updatedDeviceInfo = _dtDeviceRepository.UpdateDtDevice(utilParam);
                    if (updatedDeviceInfo == null)
                    {
                        // 指定SIDのデータがないとnullが返る
                        _logger.Error(nameof(Resources.CO_API_DVU_003), new object[] { "指定したSIDのデータがDBにありません" });
                        return new Result<DtDevice>(
                            ResultCode.NotFound,
                            string.Format(Resources.CO_API_DVU_003, new object[] { "指定したSIDのデータがDBにありません" }),
                            utilParam);
                    }

                    // 設置設定インスタンスにエッジID・機器UIDを登録する
                    string edgeId = updatedDeviceInfo.EdgeId.ToString();
                    baseConfig.OwnConfig.EdgeId = edgeId;
                    baseConfig.OwnConfig.EquipmentUid = updatedDeviceInfo.EquipmentUid;

                    string configMessage = string.Empty;

                    // Sq3.2.2:設置設定ファイルを生成する(機器情報)
                    status = CreateAndUpdateStatus.MakeArchiveFile;
                    var archiveFile = MakeArchiveFile(
                        baseConfig,
                        updatedDeviceInfo.EquipmentUid,
                        updatedDeviceInfo.UpdateDatetime,
                        out configMessage);

                    // Sq3.2.3:設置設定ファイルをアップロードする(設置設定ファイル)
                    status = CreateAndUpdateStatus.Upload;
                    _deliveringRepository.Upload(archiveFile, configMessage);

                    // 正常終了
                    status = CreateAndUpdateStatus.Other;
                    _logger.Info(nameof(Resources.CO_API_DVU_007), new object[] { edgeId });
                    result = new Result<DtDevice>(
                        ResultCode.Succeed,
                        string.Format(Resources.CO_API_DVU_007, new object[] { edgeId }),
                        updatedDeviceInfo);

                    scope.Complete();
                }

                return result;
            }
            catch (RmsParameterException e)
            {
                // Sq3.2.1
                _logger.Error(e, nameof(Resources.CO_API_DVU_003), new object[] { e.Message });

                return new Result<DtDevice>(
                    ResultCode.ParameterError,
                    string.Format(Resources.CO_API_DVU_003, new object[] { e.Message }),
                    utilParam);
            }
            catch (Exception) when (status == CreateAndUpdateStatus.Other)
            {
                // 想定外のエラーは呼び出し側に返却する
                throw;
            }
            catch (Exception e)
            {
                string errorMessage = string.Empty;

                switch (status)
                {
                    case CreateAndUpdateStatus.UpdateDtDevice:
                        // Sq3.2.1
                        _logger.Error(e, nameof(Resources.CO_API_DVU_004), new object[] { e.Message });
                        errorMessage = string.Format(Resources.CO_API_DVU_004, new object[] { e.Message });
                        break;
                    case CreateAndUpdateStatus.MakeArchiveFile:
                        // Sq3.2.2
                        _logger.Error(e, nameof(Resources.CO_API_DVU_005), new object[] { e.Message });
                        errorMessage = string.Format(Resources.CO_API_DVU_005, new object[] { e.Message });
                        break;
                    case CreateAndUpdateStatus.Upload:
                    default:
                        // Sq3.2.3
                        _logger.Error(e, nameof(Resources.CO_API_DVU_006), new object[] { e.Message });
                        errorMessage = string.Format(Resources.CO_API_DVU_006, new object[] { e.Message });
                        break;
                }

                return new Result<DtDevice>(ResultCode.ServerEerror, errorMessage, utilParam);
            }
            finally
            {
                _logger.LeaveJson("Out Param: {0}", result);
            }
        }

        /// <summary>
        /// 端末へリモート接続のリクエストを行う
        /// </summary>
        /// <remarks>sd 03-2.リモート接続（ワンタイム接続）</remarks>
        /// <param name="request">リクエスト</param>
        /// <returns>結果</returns>
        public async Task<Result> RequestRemoteAsync(RequestRemote request)
        {
            Result result = null;
            RequestRemoteAsyncStatus status = RequestRemoteAsyncStatus.ReadDtDevice;

            try
            {
                _logger.EnterJson("In Param: {0}", request);

                // 端末データからDeviceIDをDeviceEdgeIdを取得する。
                DtDevice model = _dtDeviceRepository.ReadDtDevice(request.DeviceId);
                if (model == null)
                {
                    _logger.Error(nameof(Resources.CO_API_DRC_003));
                    result = new Result(ResultCode.NotFound, Resources.CO_API_DRC_003);
                    return result;
                }

                // Sq3.1.1 接続先情報を取得する
                status = RequestRemoteAsyncStatus.GetDeviceConnectionInfoAsync;
                DeviceConnectionInfo deveiceConnectionInfo = await _requestDeviceRepository.GetDeviceConnectionInfoAsync(model.EdgeId);
                if (deveiceConnectionInfo == null)
                {
                    _logger.Error(nameof(Resources.CO_API_DRC_004));
                    result = new Result(ResultCode.NotFound, Resources.CO_API_DRC_004);
                    return result;
                }

                // Sq3.1.2 メッセージを生成する
                string message = CreateMessage(request, out result);

                // Sq3.1.3 メッセージを送信する
                status = RequestRemoteAsyncStatus.SendMessageAsync;
                await _requestDeviceRepository.SendMessageAsync(deveiceConnectionInfo, message);

                _logger.Info(nameof(Resources.CO_API_DRC_007));
                result = new Result(ResultCode.Succeed, Resources.CO_API_DRC_007);
                return result;
            }
            catch (Exception ex)
            {
                switch (status)
                {
                    // ReadDtDeviceでnullを返さずにExceptionが発生した場合
                    case RequestRemoteAsyncStatus.ReadDtDevice:
                        _logger.Error(nameof(Resources.CO_API_DRC_003));
                        result = new Result(ResultCode.NotFound, Resources.CO_API_DRC_003);
                        break;
                    case RequestRemoteAsyncStatus.GetDeviceConnectionInfoAsync:
                        _logger.Error(ex, nameof(Resources.CO_API_DRC_004));
                        result = new Result(ResultCode.ServerEerror, Resources.CO_API_DRC_004);
                        break;
                    case RequestRemoteAsyncStatus.SendMessageAsync:
                    default:
                        _logger.Error(ex, nameof(Resources.CO_API_DRC_006));
                        result = new Result(ResultCode.ServerEerror, Resources.CO_API_DRC_006);
                        break;
                }

                return result;
            }
            finally
            {
                _logger.LeaveJson("Result Param: {0}", result);
            }
        }

        /// <summary>
        /// 設置設定ファイル用のArchiveFileを作成する
        /// </summary>
        /// <param name="messageObject">設置設定インスタンス</param>
        /// <param name="equipmentUid">機器UID</param>
        /// <param name="createDatetime">作成日時</param>
        /// <param name="configMessage">設置設定ファイル内容</param>
        /// <returns>ArchiveFile</returns>
        private ArchiveFile MakeArchiveFile(
            InstallBaseConfig messageObject,
            string equipmentUid,
            DateTime createDatetime,
            out string configMessage)
        { 
            Assert.IfNull(messageObject);
            Assert.IfNull(equipmentUid);

            configMessage = messageObject.ToStringJsonIndented();

            // メタデータなしで作成
            return new ArchiveFile()
            {
                ContainerName = _settings.DeliveringBlobContainerNameInstallbase,
                FilePath = string.Format(_settings.DeliveringBlobInstallbaseFilePath, equipmentUid),
                CreatedAt = createDatetime
            };
        }

        /// <summary>
        /// メッセージを作成する
        /// </summary>
        /// <param name="request">リモート接続要求</param>
        /// <param name="result">メッセージ作成結果</param>
        /// <returns>メッセージ</returns>
        /// <remarks>
        /// リポジトリにしてもいいかもしれないが、
        /// 自動テストではどちらにせよAppSettingsをDIしており手間がかかる割に恩恵が薄いのでService内にて対応。
        /// </remarks>
        private string CreateMessage(RequestRemote request, out Result result)
        {
            result = null;
            string remoteParamater = _settings.RemoteParameter;
            if (string.IsNullOrWhiteSpace(remoteParamater))
            {
                result = new Result(ResultCode.ServerEerror);
                return string.Empty;
            }

            var messageObject = new RequestRemoteMessage(remoteParamater, request);
            var message = JsonConvert.SerializeObject(messageObject);
            result = new Result(ResultCode.Succeed);

            _logger.Debug("message: {0}", new object[] { message });
            return message;
        }

        /// <summary>
        /// RequestRemoteMessage
        /// </summary>
        public class RequestRemoteMessage
        {
            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="command">リモートコマンド文字列</param>
            /// <param name="request">リクエストリモート</param>
            public RequestRemoteMessage(string command, RequestRemote request)
            {
                Assert.IfNull(request);
                Assert.IfNullOrEmpty(command, nameof(command));

                RemoteParameter = string.Format(command, request.SessionCode);
            }

            /// <summary>
            /// RemoteParameter
            /// </summary>
            [JsonPropertyName("RemoteParameter")]
            public string RemoteParameter { get; }
        }
    }
}
