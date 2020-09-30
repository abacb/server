using Newtonsoft.Json;
using Rms.Server.Core.Utility.Models.Entites;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Rms.Server.Core.Utility.Models.Dispatch
{
    /// <summary>
    /// サーバ設定同期
    /// </summary>
    public class ServerSyncMessage
    {
        /// <summary>
        /// サーバ設定同期クラスコンストラクタ
        /// </summary>
        public ServerSyncMessage()
        {
            // nullは許可しないので初期化しておく
            Storage = new Dictionary<string, StorageConfig>();
            Script = new Dictionary<string, ScriptConfig>();
        }

        /// <summary>
        /// ストレージ
        /// </summary>
        [Required]
        [JsonProperty(nameof(Storage))]
        public Dictionary<string, StorageConfig> Storage { get; set; }

        /// <summary>
        /// スクリプト
        /// </summary>
        [Required]
        [JsonProperty(nameof(Script))]
        public Dictionary<string, ScriptConfig> Script { get; set; }

        #region Static Methods

        /// <summary>
        /// メッセージクラスをシリアライズしてJSON文字列を取得する
        /// nullの要素には初期値が設定される
        /// </summary>
        /// <param name="message">メッセージクラスオブジェクト</param>
        /// <returns>JSON文字列。メッセージクラスオブジェクトがnullの場合にはnullを返す</returns>
        public static string CreateJsonString(ServerSyncMessage message)
        {
            if (message == null)
            {
                return null;
            }

            return JsonConvert.SerializeObject(message);
        }

        /// <summary>
        /// JSON文字列をデシリアライズしてメッセージクラスに変換する
        /// </summary>
        /// <param name="jsonString">JSON文字列</param>
        /// <returns>メッセージクラス</returns>
        /// <remarks>
        /// エラー時には以下の例外が発生する
        /// - 引数がnull: ArgumentNullException
        /// - JSONのデシリアライズに失敗: JsonException
        /// </remarks>
        public static ServerSyncMessage Deserialize(string jsonString)
        {
            return JsonConvert.DeserializeObject<ServerSyncMessage>(jsonString);
        }

        /// <summary>
        /// 2つのメッセージクラスオブジェクトを比較する
        /// </summary>
        /// <param name="m1">メッセージクラスオブジェクト1</param>
        /// <param name="m2">メッセージクラスオブジェクト2</param>
        /// <returns>比較結果。内容が完全に一致していればtrueを返す</returns>
        public static bool Equals(ServerSyncMessage m1, ServerSyncMessage m2)
        {
            // ストレージ設定またはスクリプト設定のいずれかが一致していなければ不一致と判定する
            return ScriptConfigEquals(m1, m2) && StorageConfigEquals(m1, m2);
        }

        #endregion

        /// <summary>
        /// ストレージ設定をプロパティに設定する
        /// </summary>
        /// <param name="configs">設定リスト</param>
        /// <returns>正常系はtrueを返し、設定リストがnullの場合にはfalseを返す</returns>
        public bool SetStorageConfigs(List<DtStorageConfig> configs)
        {
            if (configs == null)
            {
                return false;
            }

            // 設定時に過去のデータはクリアしておく
            Storage = new Dictionary<string, StorageConfig>();

            foreach (DtStorageConfig config in configs)
            {
                StorageConfig c = new StorageConfig();
                c.Url = config.Url ?? string.Empty;
                c.Sas = config.Sas ?? string.Empty;

                // DBのユニーク制約により、Name（キー）が同一のレコードが複数存在することはないため、
                // キー重複チェックは行わない
                Storage.Add(config.Name, c);
            }

            return true;
        }

        /// <summary>
        /// スクリプト設定をプロパティに設定する
        /// DBには同名の設定に対して複数のバージョンが格納されているため、
        /// 同名の設定のうちバージョンが最新のデータのみを設定する。
        /// </summary>
        /// <param name="configs">設定リスト</param>
        /// <returns>正常系はtrueを返し、設定リストがnullの場合にはfalseを返す</returns>
        public bool SetScriptConfigs(List<DtScriptConfig> configs)
        {
            if (configs == null)
            {
                return false;
            }

            // 設定時に過去のデータはクリアしておく
            Script = new Dictionary<string, ScriptConfig>();

            foreach (DtScriptConfig config in configs)
            {
                ScriptConfig c = new ScriptConfig();
                c.FileName = config.FileName ?? string.Empty;
                c.Version = config.Version ?? 0;
                c.Location = config.Location ?? string.Empty;

                if (Script.ContainsKey(config.Name))
                {
                    // 同名のキーが存在する場合は、バージョンが新しい設定で上書きする
                    if (Script[config.Name].Version < c.Version)
                    {
                        Script[config.Name] = c;
                    }
                }
                else
                {
                    // 同名のキーが存在しない場合には要素を追加する
                    Script.Add(config.Name, c);
                }
            }

            return true;
        }

        /// <summary>
        /// メッセージクラス内のストレージ設定同士を比較する
        /// </summary>
        /// <param name="m1">メッセージクラスオブジェクト1</param>
        /// <param name="m2">メッセージクラスオブジェクト2</param>
        /// <returns>比較結果。内容が完全に一致していればtrueを返す</returns>
        private static bool StorageConfigEquals(ServerSyncMessage m1, ServerSyncMessage m2)
        {
            List<string> keys1 = new List<string>(m1.Storage.Keys);
            List<string> keys2 = new List<string>(m2.Storage.Keys);

            // キーの数を比較
            if (keys1.Count != keys2.Count)
            {
                return false;
            }

            // 要素の比較
            for (int i1 = 0; i1 < keys1.Count; i1++)
            {
                string key1 = keys1[i1];

                for (int i2 = 0; i2 < keys2.Count; i2++)
                {
                    string key2 = keys2[i2];

                    // キーが一致しているか?
                    if (key1.Equals(key2))
                    {
                        // 同じキーを持つ設定不一致のため終了
                        if (!StorageConfig.Equals(m1.Storage[key1], m2.Storage[key2]))
                        {
                            return false;
                        }

                        break;
                    }
                    else
                    {
                        // key1に一致するkey2が存在しなかった場合は設定不一致のため終了
                        if (i2 == keys2.Count - 1)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// メッセージクラス内のスクリプト設定同士を比較する
        /// </summary>
        /// <param name="m1">メッセージクラスオブジェクト1</param>
        /// <param name="m2">メッセージクラスオブジェクト2</param>
        /// <returns>比較結果。内容が完全に一致していればtrueを返す</returns>
        private static bool ScriptConfigEquals(ServerSyncMessage m1, ServerSyncMessage m2)
        {
            List<string> keys1 = new List<string>(m1.Script.Keys);
            List<string> keys2 = new List<string>(m2.Script.Keys);

            // キーの数を比較
            if (keys1.Count != keys2.Count)
            {
                return false;
            }

            // 要素の比較
            for (int i1 = 0; i1 < keys1.Count; i1++)
            {
                string key1 = keys1[i1];

                for (int i2 = 0; i2 < keys2.Count; i2++)
                {
                    string key2 = keys2[i2];

                    // キーが一致しているか?
                    if (key1.Equals(key2))
                    {
                        // 同じキーを持つ設定不一致のため終了
                        if (!ScriptConfig.Equals(m1.Script[key1], m2.Script[key2]))
                        {
                            return false;
                        }

                        break;
                    }
                    else
                    {
                        // key1に一致するkey2が存在しなかった場合は設定不一致のため終了
                        if (i2 == keys2.Count - 1)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        #region Inner Classes

        /// <summary>
        /// ストレージ設定
        /// </summary>
        public class StorageConfig
        {
            /// <summary>
            /// ストレージのURL
            /// </summary>
            [Required]
            [JsonProperty(nameof(Url))]
            public string Url { get; set; }

            /// <summary>
            /// ストレージのSASキー
            /// </summary>
            [Required]
            [JsonProperty(nameof(Sas))]
            public string Sas { get; set; }

            /// <summary>
            /// 2つのストレージ設定クラスオブジェクトの内容が完全一致かどうか判定する
            /// </summary>
            /// <param name="c1">ストレージ設定クラスオブジェクト1</param>
            /// <param name="c2">ストレージ設定クラスオブジェクト2</param>
            /// <returns>完全一致かどうか</returns>
            public static bool Equals(StorageConfig c1, StorageConfig c2)
            {
                return c1.Url.Equals(c2.Url) && c1.Sas.Equals(c2.Sas);
            }
        }

        /// <summary>
        /// スクリプト設定
        /// </summary>
        public class ScriptConfig
        {
            /// <summary>
            /// 取得（ダウンロード）するスクリプトファイル名
            /// </summary>
            [Required]
            [JsonProperty(nameof(FileName))]
            public string FileName { get; set; }

            /// <summary>
            /// スクリプトのバージョン
            /// </summary>
            [Required]
            [JsonProperty(nameof(Version))]
            public int Version { get; set; }

            /// <summary>
            /// 取得（ダウンロード）したスクリプトの配置先のパス
            /// </summary>
            [Required]
            [JsonProperty(nameof(Location))]
            public string Location { get; set; }

            /// <summary>
            /// 2つのスクリプト設定クラスオブジェクトの内容が完全一致かどうか判定する
            /// </summary>
            /// <param name="c1">スクリプト設定クラスオブジェクト1</param>
            /// <param name="c2">スクリプト設定クラスオブジェクト2</param>
            /// <returns>完全一致かどうか</returns>
            public static bool Equals(ScriptConfig c1, ScriptConfig c2)
            {
                return c1.FileName.Equals(c2.FileName) && c1.Location.Equals(c2.Location) && (c1.Version == c2.Version);
            }
        }

        #endregion
    }
}
