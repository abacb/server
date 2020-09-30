using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;
using Rms.Server.Core.Utility;
using Rms.Server.Core.Utility.Exceptions;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Rms.Server.Core.DBAccessor.Models
{
    /// <summary>
    /// DBコンテキスト
    /// </summary>
    public partial class RmsDbContext : DbContext
    {
        /// <summary>
        /// ログファクトリ
        /// </summary>
        public static readonly LoggerFactory LoggerFactory = new LoggerFactory(new[] { new DebugLoggerProvider((category, level) => category == DbLoggerCategory.Database.Command.Name && level == LogLevel.Information) });

        /// <summary>アプリケーション設定</summary>
        private readonly AppSettings _appSettings;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="appSettings">アプリケーション設定</param>
        public RmsDbContext(AppSettings appSettings)
        {
#if DEBUG
            // 呼び出し元がDbPolly#Executeではない場合に例外を投げる
            ContainsDbPollyExecuteInStackTrace();
#endif

            this._appSettings = appSettings;
        }

        /// <summary>
        /// 変更・追加されたデータをバリデーションチェックにかけてから反映する。
        /// </summary>
        /// <returns>反映数</returns>
        public override int SaveChanges()
        {
            // 変更・追加データの抽出
            var entries = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            foreach (var entry in entries)
            {
                var entity = entry.Entity;
                if (entity == null)
                {
                    continue;
                }

                // メタデータを生成してバリデーションチェック
                // 不正値があれば例外が飛ぶ
                var metaDataEntityInstance = this.CreateMetadataInstance(entity, entry.State);
                var validationContext = new ValidationContext(metaDataEntityInstance);
                Validator.ValidateObject(
                    metaDataEntityInstance,
                    validationContext,
                    validateAllProperties: true);
            }

            return base.SaveChanges();
        }

        /// <summary>
        /// 変更・追加のある作成日時・更新日時をもつEntityに現在時刻を設定してSaveChangesする
        /// </summary>
        /// <param name="provider">タイムプロバイダー</param>
        /// <returns>SaveChangesの戻り値</returns>
        public int SaveChanges(ITimeProvider provider)
        {
            if (provider == null)
            {
                return base.SaveChanges();
            }

            var utcNow = provider.UtcNow;
            this.SetCreateDateTime(utcNow);
            this.SetUpdateDateTime(utcNow);
            return this.SaveChanges();
        }

        /// <summary>
        /// エンティティデータを基にメタデータインスタンスを生成する
        /// </summary>
        /// <param name="entityInstance">エンティティデータ</param>
        /// <param name="state">EntityState</param>
        /// <returns>メタデータインスタンス</returns>
        private object CreateMetadataInstance(object entityInstance, EntityState state)
        {
            // エンティティデータからメタデータタイプを生成する
            var entityType = entityInstance.GetType();
            var metadataType = Type.GetType(entityType.FullName + "ModelMetaData");
            if (metadataType == null)
            {
                throw new Exception(entityInstance.ToString() + "のメタデータタイプを特定できませんでした");
            }

            // 動的にメタデータインスタンスを生成する
            var metadataInstance = Activator.CreateInstance(metadataType);
            foreach (var property in metadataType.GetProperties())
            {
                // プロパティの取り出し
                if (entityType.GetProperty(property.Name) == null)
                {
                    throw new Exception(property.Name + "を特定できませんでした");
                }

                // エンティティデータをメタデータにコピーする
                if (property.Name.Equals("RowVersion") && state == EntityState.Added)
                {
                    // RowVersionは追加時nullで入るので、チェック回避のために適当に値を入れる
                    property.SetValue(
                        metadataInstance,
                        new byte[] { });
                }
                else
                {
                    property.SetValue(
                        metadataInstance,
                        entityType.GetProperty(property.Name).GetValue(entityInstance));
                }
            }

            return metadataInstance;
        }

        /// <summary>
        /// 変更・追加されたEntityのうち、更新日時のカラムをもつものに現在時刻を設定する
        /// </summary>
        /// <param name="utcNow">現在時刻</param>
        private void SetUpdateDateTime(DateTime utcNow)
        {
            var entities = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified)
                .Where(e => e.CurrentValues.Properties.FirstOrDefault(p => p.Name.Equals("UpdateDatetime")) != null)
                .Select(e => e.Entity);

            foreach (dynamic entity in entities)
            {
                entity.UpdateDatetime = utcNow;
            }
        }

        /// <summary>
        /// 追加されたEntityのうち、作成日時のカラムをもつものに現在時刻を設定する
        /// </summary>
        /// <param name="utcNow">現在時刻</param>
        private void SetCreateDateTime(DateTime utcNow)
        {
            var entities = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added)
                .Where(e => e.CurrentValues.Properties.FirstOrDefault(p => p.Name.Equals("CreateDatetime")) != null)
                .Select(e => e.Entity);

            foreach (dynamic entity in entities)
            {
                entity.CreateDatetime = utcNow;
            }
        }

        /// <summary>
        /// スタックトレースを確認して、DbPolly#Executeが呼び出し元に無い場合に例外を投げる
        /// </summary>
        /// <remarks>デバッグビルド時のみ呼び出される</remarks>
        private void ContainsDbPollyExecuteInStackTrace()
        {
            StackTrace stackTrace = new StackTrace();
            StackFrame[] frames = stackTrace.GetFrames();

            foreach (StackFrame frame in frames)
            {
                MethodBase methodBase = frame.GetMethod();
                string methodName = methodBase.Name;
                string className = methodBase.ReflectedType.ToString();

                if (methodName.Equals("Execute") && className.Contains("DBPolly"))
                {
                    return;
                }
            }

            throw new RmsException("RmsDbContextがDBPolly#Executeを経由せずにインスタンス化されました。");
        }
    }
}
