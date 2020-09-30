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
    public class DeliveryFileService : IDeliveryFileService
    {
        /// <summary>
        /// ロガー
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// 配信ファイルリポジトリ
        /// </summary>
        private readonly IDtDeliveryFileRepository _dtDeliveryFileRepository;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="logger">ロガー</param>
        /// <param name="dtDeliveryFileRepository">配信ファイルリポジトリ</param>
        public DeliveryFileService(
            ILogger<DeliveryFileService> logger,
            IDtDeliveryFileRepository dtDeliveryFileRepository)
        {
            Assert.IfNull(logger);
            Assert.IfNull(dtDeliveryFileRepository);

            _logger = logger;
            _dtDeliveryFileRepository = dtDeliveryFileRepository;
        }

        /// <summary>
        /// 配信ファイルを追加する
        /// </summary>
        /// <remarks>sd 01-1.配信ファイル登録</remarks>
        /// <param name="utilParam">配信ファイルパラメータ</param>
        /// <returns>DBに追加したパラメータ(Result付き)</returns>
        public Result<DtDeliveryFile> Create(DtDeliveryFile utilParam)
        {
            Result<DtDeliveryFile> result = null;

            try
            {
                _logger.EnterJson("In Param: {0}", utilParam);

                // Sq3.1.1: 配信ファイル情報を登録する
                DtDeliveryFile model = _dtDeliveryFileRepository.CreateDtDeliveryFile(utilParam);

                _logger.Info(nameof(Resources.CO_API_DFC_004));

                result = new Result<DtDeliveryFile>(ResultCode.Succeed, Resources.CO_API_DFC_004, model);
                return result;
            }
            catch (RmsParameterException e)
            {
                _logger.Error(e, nameof(Resources.CO_API_DFC_002), new object[] { e.Message });
                result = new Result<DtDeliveryFile>(ResultCode.ParameterError, string.Format(Resources.CO_API_DFC_002, e.Message), utilParam);
                return result;
            }
            catch (Exception e)
            {
                _logger.Error(e, nameof(Resources.CO_API_DFC_003));
                result = new Result<DtDeliveryFile>(ResultCode.ServerEerror, Resources.CO_API_DFC_003, utilParam);
                return result;
            }
            finally
            {
                _logger.LeaveJson("Result Param: {0}", result);
            }
        }

        /// <summary>
        /// 配信ファイルを更新する
        /// </summary>
        /// <remarks>sd 01-1.配信ファイル更新</remarks>
        /// <param name="utilParam">配信ファイルパラメータ</param>
        /// <returns>DB更新したパラメータ(Result付き)</returns>
        public Result<DtDeliveryFile> Update(DtDeliveryFile utilParam)
        {
            Result<DtDeliveryFile> result = null;

            try
            {
                _logger.EnterJson("In Param: {0}", utilParam);

                // Sq3.1.1: 配信ファイル情報を更新する
                DtDeliveryFile model = _dtDeliveryFileRepository.UpdateDtDeliveryFileIfNoGroupStarted(utilParam);

                if (model == null)
                {
                    _logger.Error(nameof(Resources.CO_API_DFU_005));
                    result = new Result<DtDeliveryFile>(ResultCode.NotFound, Resources.CO_API_DFU_005, utilParam);
                }
                else
                {
                    _logger.Info(nameof(Resources.CO_API_DFU_007));
                    result = new Result<DtDeliveryFile>(ResultCode.Succeed, Resources.CO_API_DFU_007, model);
                }

                return result;
            }
            catch (RmsParameterException e)
            {
                _logger.Error(e, nameof(Resources.CO_API_DFU_002), new object[] { e.Message });
                result = new Result<DtDeliveryFile>(ResultCode.ParameterError, string.Format(Resources.CO_API_DFU_002, e.Message), utilParam);
                return result;
            }
            catch (RmsCannotChangeDeliveredFileException e)
            {
                _logger.Error(e, nameof(Resources.CO_API_DFU_003));
                result = new Result<DtDeliveryFile>(ResultCode.DoStartedDelivery, Resources.CO_API_DFU_003, utilParam);
                return result;
            }
            catch (RmsConflictException e)
            {
                _logger.Error(e, nameof(Resources.CO_API_DFU_004));
                result = new Result<DtDeliveryFile>(ResultCode.Conflict, Resources.CO_API_DFU_004, utilParam);
                return result;
            }
            catch (Exception e)
            {
                _logger.Error(e, nameof(Resources.CO_API_DFU_006));
                result = new Result<DtDeliveryFile>(ResultCode.ServerEerror, Resources.CO_API_DFU_006, utilParam);
                return result;
            }
            finally
            {
                _logger.LeaveJson("Result Param: {0}", result);
            }
        }

        /// <summary>
        /// 配信ファイルを削除する
        /// </summary>
        /// <param name="sid">削除する配信ファイルのSID</param>
        /// <param name="rowVersion">削除する配信ファイルのRowVersion</param>
        /// <returns>DB削除したパラメータ(Result付き)</returns>
        public Result<DtDeliveryFile> Delete(long sid, byte[] rowVersion)
        {
            Result<DtDeliveryFile> result = null;

            try
            {
                _logger.Enter($"{nameof(sid)}: {sid} {nameof(rowVersion)}: {rowVersion}");

                // 配信ファイル情報を削除する
                DtDeliveryFile model = _dtDeliveryFileRepository.DeleteDtDeliveryFile(sid, rowVersion);

                if (model == null)
                {
                    _logger.Error(nameof(Resources.CO_API_DFD_005));
                    result = new Result<DtDeliveryFile>(ResultCode.NotFound, Resources.CO_API_DFD_005, new DtDeliveryFile() { Sid = sid, RowVersion = rowVersion });
                }
                else
                {
                    _logger.Info(nameof(Resources.CO_API_DFD_007));
                    result = new Result<DtDeliveryFile>(ResultCode.Succeed, Resources.CO_API_DFD_007, model);
                }

                return result;
            }
            catch (RmsParameterException e)
            {
                _logger.Error(e, nameof(Resources.CO_API_DFD_002), new object[] { e.Message });
                result = new Result<DtDeliveryFile>(ResultCode.ParameterError, string.Format(Resources.CO_API_DFD_002, e.Message), new DtDeliveryFile() { Sid = sid, RowVersion = rowVersion });
                return result;
            }
            catch (RmsCannotChangeDeliveredFileException e)
            {
                _logger.Error(e, nameof(Resources.CO_API_DFD_003));
                result = new Result<DtDeliveryFile>(ResultCode.DoStartedDelivery, Resources.CO_API_DFD_003, new DtDeliveryFile() { Sid = sid, RowVersion = rowVersion });
                return result;
            }
            catch (RmsConflictException e)
            {
                _logger.Error(e, nameof(Resources.CO_API_DFD_004));
                result = new Result<DtDeliveryFile>(ResultCode.Conflict, Resources.CO_API_DFD_004, new DtDeliveryFile() { Sid = sid, RowVersion = rowVersion });
                return result;
            }
            catch (Exception e)
            {
                _logger.Error(e, nameof(Resources.CO_API_DFD_006));
                result = new Result<DtDeliveryFile>(ResultCode.ServerEerror, Resources.CO_API_DFD_006, new DtDeliveryFile() { Sid = sid, RowVersion = rowVersion });
                return result;
            }
            finally
            {
                _logger.LeaveJson("Result Param: {0}", result);
            }
        }

        /// <summary>
        /// 中止フラグを更新する
        /// </summary>
        /// <remarks>sd 01-9.配信中止、再開</remarks>
        /// <param name="utilParam">更新する配信ファイルデータ</param>
        /// <returns>DB更新したデータ(Result付き)</returns>
        public Result<DtDeliveryFile> PutCancelFlag(DtDeliveryFile utilParam)
        {
            Result<DtDeliveryFile> result = null;

            try
            {
                _logger.EnterJson("In Param: {0}", utilParam);

                // Sq1.1.1, Sq2.1.1 配信ステータスを更新する
                DtDeliveryFile model = _dtDeliveryFileRepository.UpdateCancelFlag(utilParam);

                if (model == null)
                {
                    _logger.Error(nameof(Resources.CO_API_DSU_004));
                    result = new Result<DtDeliveryFile>(ResultCode.NotFound, Resources.CO_API_DSU_004, utilParam);
                }
                else
                {
                    _logger.Info(nameof(Resources.CO_API_DSU_006));
                    result = new Result<DtDeliveryFile>(ResultCode.Succeed, Resources.CO_API_DSU_006, model);
                }

                return result;
            }
            catch (RmsParameterException e)
            {
                _logger.Error(e, nameof(Resources.CO_API_DSU_002), new object[] { e.Message });
                result = new Result<DtDeliveryFile>(ResultCode.ParameterError, string.Format(Resources.CO_API_DSU_002, e.Message), utilParam);
                return result;
            }
            catch (RmsConflictException e)
            {
                _logger.Error(e, nameof(Resources.CO_API_DSU_003));
                result = new Result<DtDeliveryFile>(ResultCode.Conflict, Resources.CO_API_DSU_003, utilParam);
                return result;
            }
            catch (Exception e)
            {
                _logger.Error(e, nameof(Resources.CO_API_DSU_005));
                result = new Result<DtDeliveryFile>(ResultCode.ServerEerror, Resources.CO_API_DSU_005, utilParam);
                return result;
            }
            finally
            {
                _logger.LeaveJson("Result Param: {0}", result);
            }
        }
    }
}
