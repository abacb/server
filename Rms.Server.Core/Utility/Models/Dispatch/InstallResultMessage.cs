using Newtonsoft.Json;
using Rms.Server.Core.Utility.Models.Entites;
using System;
using System.ComponentModel.DataAnnotations;

namespace Rms.Server.Core.Utility.Models.Dispatch
{
    /// <summary>
    /// 適用結果
    /// </summary>
    public class InstallResultMessage : IConvertibleModel<DtInstallResult>
    {
        /// <summary>
        /// 発生元機器UID
        /// </summary>
        [Required]
        [JsonProperty(nameof(SourceEquipmentUID))]
        public string SourceEquipmentUID { get; set; }

        /// <summary>
        /// 収集日時
        /// </summary>
        [Required]
        [JsonProperty(nameof(CollectDT))]
        public DateTime? CollectDT { get; set; }

        /// <summary>
        /// 配信結果SID
        /// </summary>
        [JsonProperty(nameof(DeliveryResultSID))]
        public long DeliveryResultSID { get; set; }

        /// <summary>
        /// 自動適用
        /// </summary>
        [Required]
        [JsonProperty(nameof(Auto))]
        public bool? Auto { get; set; }

        /// <summary>
        /// インストール方法
        /// </summary>
        [Required]
        [JsonProperty(nameof(Method))]
        public string Method { get; set; }

        /// <summary>
        /// アップデート処理プロセス
        /// </summary>
        [JsonProperty(nameof(Process))]
        public string Process { get; set; }

        /// <summary>
        /// アップデート処理開始日時
        /// </summary>
        [Required]
        [JsonProperty(nameof(UpdateStart))]
        public DateTime? UpdateStart { get; set; }

        /// <summary>
        /// アップデート処理終了日時
        /// </summary>
        [Required]
        [JsonProperty(nameof(UpdateEnd))]
        public DateTime? UpdateEnd { get; set; }

        /// <summary>
        /// コンピュータ名
        /// </summary>
        [JsonProperty(nameof(ComputerName))]
        public string ComputerName { get; set; }

        /// <summary>
        /// IPアドレス
        /// </summary>
        [JsonProperty(nameof(IpAddress))]
        public string IpAddress { get; set; }

        /// <summary>
        /// サーバ・クライアント種別
        /// </summary>
        [JsonProperty(nameof(ServerClientKind))]
        public string ServerClientKind { get; set; }

        /// <summary>
        /// アップデート処理前の内部バージョン
        /// </summary>
        [JsonProperty(nameof(BeforeVersion))]
        public string BeforeVersion { get; set; }

        /// <summary>
        /// アップデート処理後の内部バージョン
        /// </summary>
        [JsonProperty(nameof(AfterVersion))]
        public string AfterVersion { get; set; }

        /// <summary>
        /// 成功（成否）
        /// </summary>
        [Required]
        [JsonProperty(nameof(Success))]
        public bool? Success { get; set; }

        /// <summary>
        /// 状態
        /// </summary>
        [Required]
        [JsonProperty(nameof(State))]
        public string State { get; set; }

        /// <summary>
        /// エラーコード
        /// </summary>
        [JsonProperty(nameof(ErrorCode))]
        public string ErrorCode { get; set; }

        /// <summary>
        /// エラー内容
        /// </summary>
        [JsonProperty(nameof(ErrorDescription))]
        public string ErrorDescription { get; set; }

        /// <summary>
        /// リリースバージョン
        /// </summary>
        [JsonProperty(nameof(ReleaseVersion))]
        public string ReleaseVersion { get; set; }

        /// <summary>
        /// 機種コード
        /// </summary>
        [JsonProperty(nameof(TypeCode))]
        public string TypeCode { get; set; }

        /// <summary>
        /// イベント発生日時
        /// </summary>
        [Required]
        [JsonProperty(nameof(EventDT))]
        public DateTime? EventDT { get; set; }

        /// <summary>
        /// 変換
        /// </summary>
        /// <param name="deviceId">デバイスSID</param>
        /// <param name="eventData">イベント情報</param>
        /// <returns>DtInstallResult</returns>
        public DtInstallResult Convert(long deviceId, RmsEvent eventData)
        {
            return new DtInstallResult
            {
                //// Sid
                //// TypeCodeはDBに入れない
                DeviceSid = deviceId,
                DeliveryResultSid = DeliveryResultSID,
                //// InstallResultStatusSidはStateを使ってマスタテーブルから該当するステータスSIDを取得して設定する
                SourceEquipmentUid = SourceEquipmentUID,
                ReleaseVersion = ReleaseVersion,
                BeforeVersion = BeforeVersion,
                AfterVervion = AfterVersion,
                IsSuccess = Success,
                ErrorCode = ErrorCode,
                ErrorDescription = ErrorDescription,
                IsAuto = Auto,
                Method = Method,
                Process = Process,
                UpdateStratDatetime = UpdateStart,
                UpdateEndDatetime = UpdateEnd,
                ComputerName = ComputerName,
                IpAddress = IpAddress,
                ServerClientKind = ServerClientKind,
                HasRepairReport = null,
                EventDatetime = EventDT,
                CollectDatetime = CollectDT,
                MessageId = eventData?.MessageId,
                //// CreateDatetime
                //// DtDeliveryResult
                //// DtDevice
                //// MtInstallResultStatus = new MtInstallResultStatus() { Code = State } // 嘘のマスタデータオブジェクトを作成するのは嫌なので、別途渡すようにする。
            };
        }
    }
}
