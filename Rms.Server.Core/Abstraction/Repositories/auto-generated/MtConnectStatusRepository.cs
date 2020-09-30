//------------------------------------------------------------------------------
// <auto-generated>
//     このコードはテンプレートから生成されました。
//
//     このファイルを手動で変更すると、アプリケーションで予期しない動作が発生する可能性があります。
//     このファイルに対する手動の変更は、コードが再生成されると上書きされます。
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using Microsoft.Data.SqlClient;
using Rms.Server.Core.Utility.Exceptions;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Rms.Server.Core.Utility.Models.Entites;
using Microsoft.Extensions.Logging;
using Rms.Server.Core.Utility;
using Rms.Server.Core.Utility.Extensions;
using Rms.Server.Core.Abstraction.Pollies;

namespace Rms.Server.Core.Abstraction.Repositories
{
    /// <summary>
    /// MT_CONNECT_STATUSテーブルのリポジトリ
    /// </summary>
    public partial class MtConnectStatusRepository : IMtConnectStatusRepository
    {
        /// <summary>ロガー</summary>
        private readonly ILogger<MtConnectStatusRepository> _logger;
    
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
        public MtConnectStatusRepository(ILogger<MtConnectStatusRepository> logger, ITimeProvider timePrivder, DBPolly dbPolly, AppSettings appSettings)
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
        /// 引数に指定したMtConnectStatusをMT_CONNECT_STATUSテーブルへ登録する
        /// </summary>
        /// <param name="inData">登録するデータ</param>
        /// <returns>処理結果</returns>
        public MtConnectStatus CreateMtConnectStatus(MtConnectStatus inData)
        {
            MtConnectStatus model = null;
            try
            {
                _logger.EnterJson("{0}", inData);

                Rms.Server.Core.DBAccessor.Models.MtConnectStatus entity = new Rms.Server.Core.DBAccessor.Models.MtConnectStatus(inData);

                _dbPolly.Execute(() =>
                {
                    entity.CreateDatetime = _timePrivder.UtcNow;

                    using (Rms.Server.Core.DBAccessor.Models.RmsDbContext db = new Rms.Server.Core.DBAccessor.Models.RmsDbContext(_appSettings))
                    {
                        var dbdata = db.MtConnectStatus.Add(entity).Entity;
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
                throw new RmsException("MT_CONNECT_STATUSテーブルへのInsertに失敗しました。", e);
            }
            finally
            {
                _logger.LeaveJson("{0}", model);
            }
        }
    }
}
