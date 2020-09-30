using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Rms.Server.Core.Abstraction.Pollies;
using Rms.Server.Core.Utility;
using Rms.Server.Core.Utility.Exceptions;
using Rms.Server.Operation.Utility.Extensions;
using Rms.Server.Operation.Utility.Models;
using System;
using System.Linq;

namespace Rms.Server.Operation.Abstraction.Repositories
{
    /// <summary>
    /// DT_EQUIPMENTテーブルのリポジトリ
    /// </summary>
    public partial class DtEquipmentRepository : IDtEquipmentRepository
    {
        /// <summary>ロガー</summary>
        private readonly ILogger<DtEquipmentRepository> _logger;

        /// <summary>DateTimeの提供元</summary>
        private readonly ITimeProvider _timeProvider;

        /// <summary>DB接続用のPolly</summary>
        private readonly DBPolly _dbPolly;

        /// <summary>アプリケーション設定</summary>
        private readonly AppSettings _appSettings;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="logger">ロガー</param>
        /// <param name="timeProvider">DateTimeの提供元</param>
        /// <param name="dbPolly">DB接続用のPolly</param>
        /// <param name="appSettings">アプリケーション設定</param>
        public DtEquipmentRepository(ILogger<DtEquipmentRepository> logger, ITimeProvider timeProvider, DBPolly dbPolly, AppSettings appSettings)
        {
            Assert.IfNull(logger);
            Assert.IfNull(timeProvider);
            Assert.IfNull(dbPolly);
            Assert.IfNull(appSettings);

            _logger = logger;
            _timeProvider = timeProvider;
            _dbPolly = dbPolly;
            _appSettings = appSettings;
        }

        /// <summary>
        /// DT_EQUIPMENTテーブルからDtEquipmentを取得する
        /// </summary>
        /// <param name="equipmentNumber">機器管理番号</param>
        /// <param name="allowNotExist">取得件数が0件である場合を正常系とする場合はtrueを、異常系とする場合はfalseを指定する</param>
        /// <returns>取得したデータ</returns>
        public DtEquipment ReadDtEquipment(string equipmentNumber, bool allowNotExist = true)
        {
            DtEquipment model = null;
            try
            {
                _logger.EnterJson("{0}", new { equipmentNumber });

                DBAccessor.Models.DtEquipment entity = null;
                _dbPolly.Execute(() =>
                {
                    using (DBAccessor.Models.RmsDbContext db = new DBAccessor.Models.RmsDbContext(_appSettings))
                    {
                        entity = db.DtEquipment.Include(x => x.InstallBaseS).FirstOrDefault(x => x.EquipmentNumber == equipmentNumber);
                    }
                });

                if (entity != null)
                {
                    model = entity.ToModelIncludedInstallBase();
                }
                else
                {
                    if (!allowNotExist)
                    {
                        var info = new { EquipmentNumber = equipmentNumber };
                        throw new RmsException(string.Format("DT_EQUIPMENTテーブルに該当レコードが存在しません。(検索条件: {0})", JsonConvert.SerializeObject(info)));
                    }
                }

                return model;
            }
            catch (Exception e)
            {
                throw new RmsException("DT_EQUIPMENTテーブルのSelectに失敗しました。", e);
            }
            finally
            {
                _logger.LeaveJson("{0}", model);
            }
        }
    }
}
