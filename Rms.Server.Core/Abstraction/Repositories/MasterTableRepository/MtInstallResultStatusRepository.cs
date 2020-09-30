using Microsoft.Extensions.Logging;
using Rms.Server.Core.Abstraction.Pollies;
using Rms.Server.Core.Utility;
using Rms.Server.Core.Utility.Exceptions;
using Rms.Server.Core.Utility.Extensions;
using Rms.Server.Core.Utility.Models.Entites;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Rms.Server.Core.Abstraction.Repositories
{
    /// <summary>
    /// MT_INSTALL_RESULT_STATUSテーブルのリポジトリ
    /// </summary>
    public partial class MtInstallResultStatusRepository : IMtInstallResultStatusRepository
    {
        /// <summary>ロガー</summary>
        private readonly ILogger<MtInstallResultStatusRepository> _logger;

        /// <summary>DateTimeの提供元</summary>
        private readonly ITimeProvider _timePrivder;

        /// <summary>DB接続用のPolly</summary>
        private readonly DBPolly _dbPolly;

        /// <summary>アプリケーション設定</summary>
        private readonly AppSettings _appSettings;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="logger">ロガー</param>
        /// <param name="timePrivder">DateTimeの提供元</param>
        /// <param name="dbPolly">DB接続用のPolly</param>
        /// <param name="appSettings">アプリケーション設定</param>
        public MtInstallResultStatusRepository(ILogger<MtInstallResultStatusRepository> logger, ITimeProvider timePrivder, DBPolly dbPolly, AppSettings appSettings)
        {
            Assert.IfNull(logger);
            Assert.IfNull(timePrivder);
            Assert.IfNull(dbPolly);
            Assert.IfNull(appSettings);
            _logger = logger;
            _timePrivder = timePrivder;
            _dbPolly = dbPolly;
            _appSettings = appSettings;
        }

        /// <summary>
        /// 引数に指定したMtInstallResultStatusをMT_INSTALL_RESULT_STATUSテーブルへ登録する
        /// </summary>
        /// <param name="inData">登録するデータ</param>
        /// <returns>処理結果</returns>
        public MtInstallResultStatus CreateMtInstallResultStatus(MtInstallResultStatus inData)
        {
            MtInstallResultStatus model = null;
            try
            {
                _logger.EnterJson("{0}", inData);

                DBAccessor.Models.MtInstallResultStatus entity = new DBAccessor.Models.MtInstallResultStatus(inData);

                _dbPolly.Execute(() =>
                {
                    entity.CreateDatetime = _timePrivder.UtcNow;

                    using (DBAccessor.Models.RmsDbContext db = new DBAccessor.Models.RmsDbContext(_appSettings))
                    {
                        var dbdata = db.MtInstallResultStatus.Add(entity).Entity;
                        db.SaveChanges();
                        model = dbdata.ToModel();
                    }
                });

                return model;
            }
            catch (ValidationException e)
            {
                throw new RmsParameterException(e.ValidationResult.ErrorMessage, e);
            }
            catch (Exception e)
            {
                throw new RmsException("MT_INSTALL_RESULT_STATUSテーブルへのInsertに失敗しました。", e);
            }
            finally
            {
                _logger.LeaveJson("{0}", model);
            }
        }

        /// <summary>
        /// MT_INSTALL_RESULT_STATUSテーブルからMtInstallResultStatusを取得する
        /// </summary>
        /// <param name="code">取得するデータのCode</param>
        /// <returns>取得したデータ</returns>
        public MtInstallResultStatus ReadMtInstallResultStatus(string code)
        {
            MtInstallResultStatus model = null;
            try
            {
                _logger.Enter($"{nameof(code)}={code}");
    
                DBAccessor.Models.MtInstallResultStatus entity = null;
                _dbPolly.Execute(() =>
                {
                    using (DBAccessor.Models.RmsDbContext db = new DBAccessor.Models.RmsDbContext(_appSettings))
                    {
                        entity = db.MtInstallResultStatus.FirstOrDefault(x => x.Code.Equals(code));
                    }
                });
    
                if (entity != null)
                {
                    model = entity.ToModel();
                }
    
                return model;
            }
            catch (Exception e)
            {
                throw new RmsException("MT_INSTALL_RESULT_STATUSテーブルのSelectに失敗しました。", e);
            }
            finally
            {
                _logger.LeaveJson("{0}", model);
            }
        }
    }
}
