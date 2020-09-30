using Rms.Server.Core.Utility;
using Rms.Server.Core.Utility.Models.Entites;
using System.Linq;
using System.Text.Json.Serialization;

namespace Rms.Server.Core.Service.Models
{
    /// <summary>
    /// 配信情報
    /// </summary>
    public class RequestDelivery
    {
        /// <summary>
        /// ファイルパス
        /// </summary>
        [JsonPropertyName("FilePath")]
        public string FilePath { get; set; }

        /// <summary>
        /// 配信ファイル種別
        /// </summary>
        [JsonPropertyName("DeliveryFileType")]
        public string DeliveryFileType { get; set; }

        /// <summary>
        /// 機種コード
        /// </summary>
        [JsonPropertyName("TypeCodes")]
        public string[] TypeCodes { get; set; }

        /// <summary>
        /// バージョン
        /// </summary>
        [JsonPropertyName("Version")]
        public string Version { get; set; }

        /// <summary>
        /// 適用（インストール）可能バージョン
        /// </summary>
        [JsonPropertyName("InstallableVersions")]
        public string[] InstallableVersions { get; set; }

        /// <summary>
        /// ダウンロード遅延時間
        /// </summary>
        [JsonPropertyName("DownloadDelayTime")]
        public ushort DownloadDelayTime { get; set; }

        /// <summary>
        /// 配信対象
        /// </summary>
        [JsonPropertyName("Targets")]
        public Target[] Targets { get; set; }

        /// <summary>
        /// 配信メッセージオブジェクトを作成する(配信対象以外)
        /// </summary>
        /// <param name="deliveryGroup">配信グループデータ</param>
        /// <returns>配信メッセージオブジェクト</returns>
        /// <remarks>配信対象プロパティは別途設定する必要がある</remarks>
        public static RequestDelivery CreateDeliveryMessageObject(DtDeliveryGroup deliveryGroup)
        {
            Assert.IfNull(deliveryGroup);

            RequestDelivery request = new RequestDelivery();

            // ファイルパスの取得
            request.FilePath = deliveryGroup.DtDeliveryFile?.FilePath;

            // 配信ファイル種別の取得
            request.DeliveryFileType = deliveryGroup.DtDeliveryFile?.MtDeliveryFileType?.Code;

            // 機種コードの取得
            switch (deliveryGroup.DtDeliveryFile?.MtDeliveryFileType?.Code)
            {
                // 配信ファイル種別によって集めるコードを変える
                case Const.DeliveryFileType.AlSoft:
                    // インストールタイプコード(1個)
                    request.TypeCodes = new string[] { deliveryGroup.DtDeliveryFile?.MtInstallType?.Code };
                    break;
                case Const.DeliveryFileType.HotFixConsole:
                case Const.DeliveryFileType.HotFixHobbit:
                    // 機器型式のコード
                    request.TypeCodes = deliveryGroup.DtDeliveryFile?.DtDeliveryModel
                        ?.Where(x => x.MtEquipmentModel != null)
                        .Select(x => x.MtEquipmentModel.Code)
                        .ToArray();
                    break;
                case Const.DeliveryFileType.Package:
                    // 空配列
                    request.TypeCodes = new string[0];
                    break;
                default:
                    // 想定外のパラメータはnullを設定
                    request.TypeCodes = null;
                    break;
            }

            // バージョンの取得
            request.Version = deliveryGroup.DtDeliveryFile?.Version;

            // 適用（インストール）可能バージョンの取得
            request.InstallableVersions = deliveryGroup.DtDeliveryFile?.InstallableVersion?.Split(',');

            // ダウンロード遅延時間の取得
            request.DownloadDelayTime = (ushort)deliveryGroup.DownloadDelayTime.GetValueOrDefault();

            return request;
        }

        /// <summary>
        /// 配信対象データクラス
        /// </summary>
        public class Target
        {
            /// <summary>
            /// 配信結果SID
            /// </summary>
            [JsonPropertyName("DeliveryResultSID")]
            public string DeliveryResultSID { get; set; }

            /// <summary>
            /// 配信対象機器UID
            /// </summary>
            [JsonPropertyName("EquipmentUID")]
            public string EquipmentUID { get; set; }
        }
    }
}
