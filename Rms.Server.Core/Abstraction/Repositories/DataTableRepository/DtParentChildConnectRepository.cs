using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Rms.Server.Core.Abstraction.Pollies;
using Rms.Server.Core.Utility;
using Rms.Server.Core.Utility.Exceptions;
using Rms.Server.Core.Utility.Extensions;
using Rms.Server.Core.Utility.Models.Entites;
using System;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;

namespace Rms.Server.Core.Abstraction.Repositories
{
    /// <summary>
    /// DT_PARENT_CHILD_CONNECTテーブルのリポジトリ
    /// </summary>
    public partial class DtParentChildConnectRepository : IDtParentChildConnectRepository
    {
        /// <summary>SQLパラメータ（親機器SID）</summary>
        /// <remarks>SQLインジェクション対策</remarks>
        private static readonly string ParentDeviceSidQuery = "@ParentDeviceSid";

        /// <summary>SQLパラメータ（子機器SID）</summary>
        /// <remarks>SQLインジェクション対策</remarks>
        private static readonly string ChildDeviceSidQuery = "@ChildDeviceSid";

        /// <summary>
        /// 親子間通信データテーブルSELECT用SQL文（親機フラグ=true）
        /// Select時にロックをかける
        /// </summary>
        private static readonly string SelectParentSqlCommand = "SELECT * FROM [core].[DT_PARENT_CHILD_CONNECT] AS [e]"
            + " WITH (UPDLOCK, ROWLOCK) "
            + " WHERE [e].[PARENT_DEVICE_SID] = " + ParentDeviceSidQuery
            + " AND [e].[CHILD_DEVICE_SID] = " + ChildDeviceSidQuery
            + " AND [e].[PARENT_RESULT] IS NOT NULL";

        /// <summary>
        /// 親子間通信データテーブルSELECT用SQL文（親機フラグ=false）
        /// Select時にロックをかける
        /// </summary>
        private static readonly string SelectChildSqlCommand = "SELECT * FROM [core].[DT_PARENT_CHILD_CONNECT] AS [e]"
            + " WITH (UPDLOCK, ROWLOCK) "
            + " WHERE [e].[PARENT_DEVICE_SID] = " + ParentDeviceSidQuery
            + " AND [e].[CHILD_DEVICE_SID] = " + ChildDeviceSidQuery
            + " AND [e].[CHILD_RESULT] IS NOT NULL";

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
        /// DT_PARENT_CHILD_CONNECTテーブルにメッセージを追加またはメッセージの内容で更新処理を行う（親フラグ=trueの場合）
        /// </summary>
        /// <param name="inData">更新データ</param>
        /// <returns>追加または更新したデータ。「確認日時」が既存レコードより古く更新しなかった場合には、nullを返す</returns>
        public DtParentChildConnect Save(DtParentChildConnectFromParent inData)
        {
            DtParentChildConnect model = null;

            try
            {
                _dbPolly.Execute(() =>
                {
                    using (DBAccessor.Models.RmsDbContext db = new DBAccessor.Models.RmsDbContext(_appSettings))
                    using (var tran = db.Database.BeginTransaction())
                    {
                        // 現在時刻を取得
                        var now = _timePrivder.Now;

                        // メッセージクラス→Modelクラス変換時に、通信が成功だった場合は最終接続日時にデータが格納される
                        // ここで初期値だった場合には通信失敗と判断できる
                        if (inData.ParentLastConnectDatetime == null || inData.ParentLastConnectDatetime == default(DateTime))
                        {
                            inData.ParentLastConnectDatetime = now;
                        }

                        // DB格納データクラス
                        DBAccessor.Models.DtParentChildConnect entity = CreateEntityFromParent(inData, db);

                        // 親機器 - 子機器 - 親機器確認結果の組み合わせをチェックする
                        // 3つの組み合わせでレコードを一意に決めることができる
                        var sids = CreateSqlParameterSet(entity.ParentDeviceSid, entity.ChildDeviceSid);
                        DBAccessor.Models.DtParentChildConnect targetRecord
                            = db.DtParentChildConnect.FromSql(SelectParentSqlCommand, sids).AsNoTracking().FirstOrDefault();

                        if (targetRecord == null || targetRecord.Sid == 0)
                        {
                            // 作成日時+更新日時
                            // 通常はSaveChangesにTimeProvideを渡し、DBが自動的に作成日時と更新日時を設定する。
                            // しかし親子間通信データについては、
                            // 通信失敗のケースにおいて最終更新日時にはレコード作成日時を格納する
                            // （nullを回避しつつ、通信失敗ケースであったことを後から判断できるようにするため）。
                            // DBが自動的に時刻を設定した場合に、リポジトリ内で取得した時刻と時間差が発生する可能性があるため、
                            // リポジトリ内で明示的に時刻を取得してDBにレコードを挿入する。
                            entity.CreateDatetime = now;
                            entity.UpdateDatetime = now;

                            // レコード追加
                            var dbdata = db.DtParentChildConnect.Add(entity).Entity;

                            // 最終日時更新データと作成日時・更新日時を一致させたいので、TimeProviderは渡さずに明示的な日時データを使う
                            db.SaveChanges();
                            model = dbdata.ToModel();
                        }
                        else
                        {
                            // データの時刻チェック: データがDBに格納されたデータよりも新しい場合のみ更新処理を行う
                            // DBよりも古い確認時刻だった場合には更新せずにnullを返す
                            DateTime? dbTime = targetRecord.ParentConfirmDatetime;

                            if (dbTime == null || inData.ParentConfirmDatetime.Value.CompareTo(dbTime.Value) > 0)
                            {
                                // 更新対象のレコードのSIDを設定
                                entity.Sid = targetRecord.Sid;

                                db.DtParentChildConnect.Attach(entity);

                                // 更新のあったデータのみを更新する
                                db.Entry(entity).Property(x => x.ParentConfirmDatetime).IsModified = true;
                                db.Entry(entity).Property(x => x.ParentResult).IsModified = true;
                                if (entity.ParentResult != null && entity.ParentResult.Value)
                                {
                                    db.Entry(entity).Property(x => x.ParentLastConnectDatetime).IsModified = true;
                                }

                                db.SaveChanges(_timePrivder);
                                model = entity.ToModel();
                            }
                        }

                        // トランザクション終了
                        tran.Commit();
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
                throw new RmsException("DT_PARENT_CHILD_CONNECTテーブルの更新処理に失敗しました。", e);
            }
            finally
            {
                _logger.LeaveJson("{0}", model);
            }
        }

        /// <summary>
        /// DT_PARENT_CHILD_CONNECTテーブルにメッセージを追加またはメッセージの内容で更新処理を行う（親フラグ=falseの場合）
        /// </summary>
        /// <param name="inData">更新データ</param>
        /// <returns>追加または更新したデータ。「確認日時」が既存レコードより古く更新しなかった場合には、nullを返す</returns>
        public DtParentChildConnect Save(DtParentChildConnectFromChild inData)
        {
            DtParentChildConnect model = null;

            try
            {
                _dbPolly.Execute(() =>
                {
                    using (DBAccessor.Models.RmsDbContext db = new DBAccessor.Models.RmsDbContext(_appSettings))
                    using (var tran = db.Database.BeginTransaction())
                    {
                        // 現在時刻を取得
                        var now = _timePrivder.Now;

                        // メッセージクラス→Modelクラス変換時に、通信が成功だった場合は最終接続日時にデータが格納される
                        // ここで初期値だった場合には通信失敗と判断できる
                        if (inData.ChildLastConnectDatetime == null || inData.ChildLastConnectDatetime == default(DateTime))
                        {
                            inData.ChildLastConnectDatetime = now;
                        }

                        // DB格納データクラス
                        DBAccessor.Models.DtParentChildConnect entity = CreateEntityFromChild(inData, db);

                        // 親機器 - 子機器 - 子機器確認結果の組み合わせをチェックする
                        // 3つの組み合わせでレコードを一意に決めることができる
                        var sids = CreateSqlParameterSet(entity.ParentDeviceSid, entity.ChildDeviceSid);
                        DBAccessor.Models.DtParentChildConnect targetRecord
                            = db.DtParentChildConnect.FromSql(SelectChildSqlCommand, sids).AsNoTracking().FirstOrDefault();

                        if (targetRecord == null || targetRecord.Sid == 0)
                        {
                            // 作成日時+更新日時
                            // 通常はSaveChangesにTimeProvideを渡し、DBが自動的に作成日時と更新日時を設定する。
                            // しかし親子間通信データについては、
                            // 通信失敗のケースにおいて最終更新日時にはレコード作成日時を格納する
                            // （nullを回避しつつ、通信失敗ケースであったことを後から判断できるようにするため）。
                            // DBが自動的に時刻を設定した場合に、リポジトリ内で取得した時刻と時間差が発生する可能性があるため、
                            // リポジトリ内で明示的に時刻を取得してDBにレコードを挿入する。
                            entity.CreateDatetime = now;
                            entity.UpdateDatetime = now;

                            // レコード追加
                            var dbdata = db.DtParentChildConnect.Add(entity).Entity;

                            // 最終日時更新データと作成日時・更新日時を一致させたいので、TimeProviderは渡さずに明示的な日時データを使う
                            db.SaveChanges();
                            model = dbdata.ToModel();
                        }
                        else
                        {
                            // データの時刻チェック: データがDBに格納されたデータよりも新しい場合のみ更新処理を行う
                            // DBよりも古い確認時刻だった場合には更新せずにnullを返す
                            DateTime? dbTime = targetRecord.ChildConfirmDatetime;

                            if (dbTime == null || inData.ChildConfirmDatetime.Value.CompareTo(dbTime.Value) > 0)
                            {
                                // 更新対象のレコードのSIDを設定
                                entity.Sid = targetRecord.Sid;

                                db.DtParentChildConnect.Attach(entity);

                                // 更新のあったデータのみを更新する
                                db.Entry(entity).Property(x => x.ChildConfirmDatetime).IsModified = true;
                                db.Entry(entity).Property(x => x.ChildResult).IsModified = true;
                                if (entity.ChildResult != null && entity.ChildResult.Value)
                                {
                                    db.Entry(entity).Property(x => x.ChildLastConnectDatetime).IsModified = true;
                                }

                                db.SaveChanges(_timePrivder);
                                model = entity.ToModel();
                            }
                        }

                        // トランザクション終了
                        tran.Commit();
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
                throw new RmsException("DT_PARENT_CHILD_CONNECTテーブルの更新処理に失敗しました。", e);
            }
            finally
            {
                _logger.LeaveJson("{0}", model);
            }
        }

        /// <summary>
        /// UtilityモデルクラスからEntityクラスを生成する（親フラグ=trueの場合）
        /// </summary>
        /// <param name="inData">親子間通信データ</param>
        /// <param name="dbContext">DBコンテキスト</param>
        /// <returns>Entityクラスインスタンス。機器SIDを取得できない場合はnullを返す</returns>
        private DBAccessor.Models.DtParentChildConnect CreateEntityFromParent(DtParentChildConnectFromParent inData, DBAccessor.Models.RmsDbContext dbContext)
        {
            DBAccessor.Models.DtParentChildConnect entity = new DBAccessor.Models.DtParentChildConnect();

            // 機器UIDからSIDを取得する
            var parent = dbContext.DtDevice.FirstOrDefault(x => x.EquipmentUid == inData.ParentDeviceUid);
            var child = dbContext.DtDevice.FirstOrDefault(x => x.EquipmentUid == inData.ChildDeviceUid);

            if (parent == null || child == null || parent.Sid == 0 || child.Sid == 0)
            {
                // SIDを取得できない場合はエラー
                return null;
            }

            long parentDeviceSid = parent.Sid;
            long childDeviceSid = child.Sid;

            // エンティティに必要なデータを設定する
            entity.ParentDeviceSid = parentDeviceSid;
            entity.ChildDeviceSid = childDeviceSid;
            entity.ParentResult = inData.ParentResult;
            entity.ParentConfirmDatetime = inData.ParentConfirmDatetime;
            if (inData.ParentResult.Value)
            {
                entity.ParentLastConnectDatetime = inData.ParentConfirmDatetime;
            }

            return entity;
        }

        /// <summary>
        /// UtilityモデルクラスからEntityクラスに生成する（親フラグ=falseの場合）
        /// </summary>
        /// <param name="inData">親子間通信データ</param>
        /// <param name="dbContext">DBコンテキスト</param>
        /// <returns>Entityクラスインスタンス。機器SIDを取得できない場合はnullを返す</returns>
        private DBAccessor.Models.DtParentChildConnect CreateEntityFromChild(DtParentChildConnectFromChild inData, DBAccessor.Models.RmsDbContext dbContext)
        {
            DBAccessor.Models.DtParentChildConnect entity = new DBAccessor.Models.DtParentChildConnect();

            // 機器UIDからSIDを取得する
            var parent = dbContext.DtDevice.FirstOrDefault(x => x.EquipmentUid == inData.ParentDeviceUid);
            var child = dbContext.DtDevice.FirstOrDefault(x => x.EquipmentUid == inData.ChildDeviceUid);

            if (parent == null || child == null || parent.Sid == 0 || child.Sid == 0)
            {
                // SIDを取得できない場合はエラー
                return null;
            }

            long parentDeviceSid = parent.Sid;
            long childDeviceSid = child.Sid;

            // エンティティに必要なデータを設定する
            entity.ParentDeviceSid = parentDeviceSid;
            entity.ChildDeviceSid = childDeviceSid;
            entity.ChildResult = inData.ChildResult;
            entity.ChildConfirmDatetime = inData.ChildConfirmDatetime;
            if (inData.ChildResult.Value)
            {
                entity.ChildLastConnectDatetime = inData.ChildConfirmDatetime;
            }

            return entity;
        }

        /// <summary>
        /// SIDの組み合わせからSQLパラメータ配列を生成する
        /// </summary>
        /// <param name="parentDeviceSid">親機器SID</param>
        /// <param name="childDeviceSid">子機器SID</param>
        /// <returns>SQLパラメータ配列</returns>
        private SqlParameter[] CreateSqlParameterSet(long parentDeviceSid, long childDeviceSid)
        {
            return new SqlParameter[] 
                {
                    new SqlParameter(ParentDeviceSidQuery, parentDeviceSid),
                    new SqlParameter(ChildDeviceSidQuery, childDeviceSid)
                };
        }
    }
}
