using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rms.Server.Core.Abstraction.Pollies;
using Rms.Server.Core.Utility;
using Rms.Server.Core.Utility.Exceptions;
using Rms.Server.Utility.Utility.Extensions;
using Rms.Server.Utility.Utility.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rms.Server.Utility.Abstraction.Repositories
{
    /// <summary>
    /// DT_PARENT_CHILD_CONNECTテーブルのリポジトリ
    /// </summary>
    public partial class DtParentChildConnectRepository : IDtParentChildConnectRepository
    {
        /// <summary>ロガー</summary>
        private readonly ILogger<DtParentChildConnectRepository> _logger;

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
        public DtParentChildConnectRepository(ILogger<DtParentChildConnectRepository> logger, ITimeProvider timePrivder, DBPolly dbPolly, AppSettings appSettings)
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
        /// DT_PARENT_CHILD_CONNECTテーブルからDtParentChildConnectを取得する
        /// </summary>
        /// <returns>取得したデータ</returns>
        public IEnumerable<DtParentChildConnect> ReadDtParentChildConnect()
        {
            IEnumerable<DtParentChildConnect> models = null;
            try
            {
                _logger.Enter();

                _dbPolly.Execute(() =>
                {
                    using (DBAccessor.Models.RmsDbContext db = new DBAccessor.Models.RmsDbContext(_appSettings))
                    {
                        if (!db.DtParentChildConnect.Any())
                        {
                            // 親子間通信データテーブルにデータがない場合は正常系
                            models = new List<DtParentChildConnect>();
                        }
                        else
                        {
                            // インベントリデータテーブルは更新ではなく新規追加なので端末SID毎に最新のデータを取得して結合する
                            var inventoryLatests = db.DtInventory.GroupBy(x => x.DeviceSid).Select(y => y.OrderByDescending(z => z.CreateDatetime).FirstOrDefault());

                            var joinEntities = db.DtParentChildConnect
                                                    .Join(
                                                        db.DtDevice,
                                                        x => x.ChildDeviceSid,
                                                        y => y.Sid,
                                                        (parentChildConnet, device) => new
                                                        {
                                                            ParentChildConnet = parentChildConnet,
                                                            DeviceTableSid = device.Sid,
                                                            DeviceTableEquipmentUid = device.EquipmentUid,
                                                            DeviceTableInstallTypeSid = device.InstallTypeSid
                                                        })
                                                    .Join(
                                                        db.MtInstallType,
                                                        x => x.DeviceTableInstallTypeSid,
                                                        y => y.Sid,
                                                        (joinTable, installType) => new
                                                        {
                                                            joinTable.ParentChildConnet,
                                                            joinTable.DeviceTableSid,
                                                            joinTable.DeviceTableEquipmentUid,
                                                            InstallTypeTableCode = installType.Code,
                                                        })
                                                    .Join(
                                                        inventoryLatests,
                                                        x => x.ParentChildConnet.Sid,
                                                        y => y.DeviceSid,
                                                        (joinTable, inventory) => new
                                                        {
                                                            joinTable.ParentChildConnet,
                                                            joinTable.DeviceTableEquipmentUid,
                                                            joinTable.InstallTypeTableCode,
                                                            InventoryDetailInfo = inventory.DetailInfo
                                                        }).ToList();

                            if (!joinEntities.Any())
                            {
                                throw new RmsException("JoinによりDT_PARENT_CHILD_CONNECTテーブルのデータ抽出数が0になりました。");
                            }
                            else
                            {
                                models = new List<DtParentChildConnect>();

                                foreach (var entity in joinEntities)
                                {
                                    var detailInfo = JsonConvert.DeserializeObject<JObject>(entity.InventoryDetailInfo);

                                    // アラーム判定・アラーム情報の生成に必要な属性のみ取得
                                    var model = new DtParentChildConnect
                                    {
                                        EquipmentUid = entity.DeviceTableEquipmentUid,
                                        TypeCode = entity.InstallTypeTableCode,
                                        TemperatureSensor = (string)detailInfo[Utility.Const.InventoryAlSoftwareName][Utility.Const.InventorySettingInfoName][Utility.Const.InventoryOptionName][Utility.Const.InventoryTemperatureSensorName],
                                        Dxa = (string)detailInfo[Utility.Const.InventoryAlSoftwareName][Utility.Const.InventorySettingInfoName][Utility.Const.InventoryOptionName][Utility.Const.InventoryDxaName],
                                        ParentLastConnectDatetime = entity.ParentChildConnet.ParentLastConnectDatetime,
                                        ChildLastConnectDatetime = entity.ParentChildConnet.ChildLastConnectDatetime
                                    };
                                    models.Append(model);
                                }
                            }
                        }
                    }
                });

                return models;
            }
            catch (Exception e)
            {
                throw new RmsException("DT_PARENT_CHILD_CONNECTテーブルのSelectに失敗しました。", e);
            }
            finally
            {
                _logger.LeaveJson("{0}", models);
            }
        }
    }
}
