using Microsoft.Extensions.Logging;
using Rms.Server.Core.Abstraction.Repositories;
using Rms.Server.Core.Utility;
using Rms.Server.Core.Utility.Exceptions;
using Rms.Server.Core.Utility.Extensions;
using Rms.Server.Core.Utility.Models;
using Rms.Server.Core.Utility.Models.Entites;
using Rms.Server.Core.Utility.Properties;
using System;

namespace Rms.Server.Core.Service.Services
{
    /// <summary>
    /// DeliveryGroupService
    /// </summary>
    public class DeliveryGroupService : IDeliveryGroupService
    {
        /// <summary>
        /// ロガー
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// タイムプロバイダ
        /// </summary>
        private readonly ITimeProvider _timeProvider;

        /// <summary>
        /// 配信グループリポジトリ
        /// </summary>
        private readonly IDtDeliveryGroupRepository _dtDeliveryGroupRepository;

        /// <summary>
        /// 適用結果ステータスマスタリポジトリ
        /// </summary>
        private readonly IMtInstallResultStatusRepository _mtInstallResultStatusRepository;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="logger">ロガー</param>
        /// <param name="timeProvider">タイムプロバイダ</param>
        /// <param name="dtDeliveryGroupRepository">配信グループリポジトリ</param>
        /// <param name="mtInstallResultStatusRepository">適用結果ステータスマスタリポジトリ</param>
        public DeliveryGroupService(
            ILogger<DeliveryGroupService> logger,
            ITimeProvider timeProvider,
            IDtDeliveryGroupRepository dtDeliveryGroupRepository,
            IMtInstallResultStatusRepository mtInstallResultStatusRepository)
        {
            Assert.IfNull(logger);
            Assert.IfNull(timeProvider);
            Assert.IfNull(dtDeliveryGroupRepository);
            Assert.IfNull(mtInstallResultStatusRepository);

            _logger = logger;
            _timeProvider = timeProvider;
            _dtDeliveryGroupRepository = dtDeliveryGroupRepository;
            _mtInstallResultStatusRepository = mtInstallResultStatusRepository;
        }

        /// <summary>
        /// 配信グループを追加する
        /// </summary>
        /// <remarks>sd 01-2.配信グループ登録</remarks>
        /// <param name="utilParam">配信グループパラメータ</param>
        /// <returns>DBに追加したパラメータ(Result付き)</returns>
        public Result<DtDeliveryGroup> Create(DtDeliveryGroup utilParam)
        {
            Result<DtDeliveryGroup> result = null;

            try
            {
                _logger.EnterJson("In Param: {0}", utilParam);

                // 適用結果ステータス(notstart)のSIDを取得する
                MtInstallResultStatus status = _mtInstallResultStatusRepository.ReadMtInstallResultStatus(Const.InstallResultStatus.NotStarted);

                // 配信結果に適用結果履歴の初期値を設定する
                foreach (var deliveryResult in utilParam.DtDeliveryResult)
                {
                    deliveryResult.DtInstallResult.Add(new DtInstallResult()
                    {
                        DeviceSid = deliveryResult.DeviceSid,
                        ////DeliveryResultSid
                        InstallResultStatusSid = status.Sid,
                        CollectDatetime = _timeProvider.UtcNow
                    });
                }

                // Sq1.1.1 配信グループを登録する
                DtDeliveryGroup model = _dtDeliveryGroupRepository.CreateDtDeliveryGroup(utilParam);

                _logger.Info(nameof(Resources.CO_API_DGC_004));

                result = new Result<DtDeliveryGroup>(ResultCode.Succeed, Resources.CO_API_DGC_004, model);
                return result;
            }
            catch (RmsParameterException e)
            {
                _logger.Error(e, nameof(Resources.CO_API_DGC_002), new object[] { e.Message });
                result = new Result<DtDeliveryGroup>(ResultCode.ParameterError, string.Format(Resources.CO_API_DGC_002, e.Message), utilParam);
                return result;
            }
            catch (Exception e)
            {
                _logger.Error(e, nameof(Resources.CO_API_DGC_003));
                result = new Result<DtDeliveryGroup>(ResultCode.ServerEerror, Resources.CO_API_DGC_003, utilParam);
                return result;
            }
            finally
            {
                _logger.LeaveJson("Result Param: {0}", result);
            }
        }

        /// <summary>
        /// 配信グループを更新する
        /// </summary>
        /// <remarks>sd 01-2.配信グループ更新</remarks>
        /// <param name="utilParam">配信グループパラメータ</param>
        /// <returns>DB更新したパラメータ(Result付き)</returns>
        public Result<DtDeliveryGroup> Update(DtDeliveryGroup utilParam)
        {
            Result<DtDeliveryGroup> result = null;

            try
            {
                _logger.EnterJson("In Param: {0}", utilParam);

                // Sq1.1.1 配信グループを更新する
                DtDeliveryGroup model = _dtDeliveryGroupRepository.UpdateDtDeliveryGroupIfDeliveryNotStart(utilParam);

                if (model == null)
                {
                    _logger.Error(nameof(Resources.CO_API_DGU_005));
                    result = new Result<DtDeliveryGroup>(ResultCode.NotFound, Resources.CO_API_DGU_005, utilParam);
                }
                else
                {
                    _logger.Info(nameof(Resources.CO_API_DGU_007));
                    result = new Result<DtDeliveryGroup>(ResultCode.Succeed, Resources.CO_API_DGU_007, model);
                }

                return result;
            }
            catch (RmsParameterException e)
            {
                _logger.Error(e, nameof(Resources.CO_API_DGU_002), new object[] { e.Message });
                result = new Result<DtDeliveryGroup>(ResultCode.ParameterError, string.Format(Resources.CO_API_DGU_002, e.Message), utilParam);
                return result;
            }
            catch (RmsCannotChangeDeliveredFileException e)
            {
                _logger.Error(e, nameof(Resources.CO_API_DGU_003));
                result = new Result<DtDeliveryGroup>(ResultCode.DoStartedDelivery, Resources.CO_API_DGU_003, utilParam);
                return result;
            }
            catch (RmsConflictException e)
            {
                _logger.Error(e, nameof(Resources.CO_API_DGU_004));
                result = new Result<DtDeliveryGroup>(ResultCode.Conflict, Resources.CO_API_DGU_004, utilParam);
                return result;
            }
            catch (Exception e)
            {
                _logger.Error(e, nameof(Resources.CO_API_DGU_006));
                result = new Result<DtDeliveryGroup>(ResultCode.ServerEerror, Resources.CO_API_DGU_006, utilParam);
                return result;
            }
            finally
            {
                _logger.LeaveJson("Result Param: {0}", result);
            }
        }

        /// <summary>
        /// 配信グループを削除する
        /// </summary>
        /// <param name="sid">削除する配信グループのSID</param>
        /// <param name="rowVersion">削除する配信グループのRowVersion</param>
        /// <returns>DB削除したパラメータ(Result付き)</returns>
        public Result<DtDeliveryGroup> Delete(long sid, byte[] rowVersion)
        {
            Result<DtDeliveryGroup> result = null;

            try
            {
                _logger.Enter($"{nameof(sid)}: {sid} {nameof(rowVersion)}: {rowVersion}");

                // DBから指定SIDのデータ削除を依頼する
                DtDeliveryGroup model = _dtDeliveryGroupRepository.DeleteDtDeliveryGroup(sid, rowVersion);

                if (model == null)
                {
                    _logger.Error(nameof(Resources.CO_API_DGD_005));
                    result = new Result<DtDeliveryGroup>(ResultCode.NotFound, Resources.CO_API_DGD_005, new DtDeliveryGroup() { Sid = sid, RowVersion = rowVersion });
                }
                else
                {
                    _logger.Info(nameof(Resources.CO_API_DGD_007));
                    result = new Result<DtDeliveryGroup>(ResultCode.Succeed, Resources.CO_API_DGD_007, model);
                }

                return result;
            }
            catch (RmsParameterException e)
            {
                _logger.Error(e, nameof(Resources.CO_API_DGD_002), new object[] { e.Message });
                result = new Result<DtDeliveryGroup>(ResultCode.ParameterError, string.Format(Resources.CO_API_DGD_002, e.Message), new DtDeliveryGroup() { Sid = sid, RowVersion = rowVersion });
                return result;
            }
            catch (RmsCannotChangeDeliveredFileException e)
            {
                _logger.Error(e, nameof(Resources.CO_API_DGD_003));
                result = new Result<DtDeliveryGroup>(ResultCode.DoStartedDelivery, Resources.CO_API_DGD_003, new DtDeliveryGroup() { Sid = sid, RowVersion = rowVersion });
                return result;
            }
            catch (RmsConflictException e)
            {
                _logger.Error(e, nameof(Resources.CO_API_DGD_004));
                result = new Result<DtDeliveryGroup>(ResultCode.Conflict, Resources.CO_API_DGD_004, new DtDeliveryGroup() { Sid = sid, RowVersion = rowVersion });
                return result;
            }
            catch (Exception e)
            {
                _logger.Error(e, nameof(Resources.CO_API_DGD_006));
                result = new Result<DtDeliveryGroup>(ResultCode.ServerEerror, Resources.CO_API_DGD_006, new DtDeliveryGroup() { Sid = sid, RowVersion = rowVersion });
                return result;
            }
            finally
            {
                _logger.LeaveJson("Result Param: {0}", result);
            }
        }
    }
}
