using Newtonsoft.Json;
using Rms.Server.Core.Utility.Models.Entites;
using System;
using System.ComponentModel.DataAnnotations;

namespace Rms.Server.Core.Utility.Models.Dispatch
{
    /// <summary>
    /// 親子間通信情報
    /// </summary>
    public class ParentChildConnectionInfoMessage 
    {
        /// <summary>
        /// 発生元機器UID
        /// </summary>
        [Required]
        [JsonProperty(nameof(SourceEquipmentUID))]
        public string SourceEquipmentUID { get; set; }

        /// <summary>
        /// 通信先機器UID
        /// </summary>
        [Required]
        [JsonProperty(nameof(ConnectEquipmentUID))]
        public string ConnectEquipmentUID { get; set; }

        /// <summary>
        /// 親機フラグ
        /// </summary>
        [Required]
        [JsonProperty(nameof(Parent))]
        public bool? Parent { get; set; }

        /// <summary>
        /// 成否
        /// </summary>
        [Required]
        [JsonProperty(nameof(Success))]
        public bool? Success { get; set; }

        /// <summary>
        /// 確認日時
        /// </summary>
        [Required]
        [JsonProperty(nameof(ConfirmDT))]
        public DateTime? ConfirmDT { get; set; }

        /// <summary>
        /// 親機器UIDを取得する
        /// </summary>
        /// <returns>親機器UID</returns>
        public string GetParentUID()
        {
            if (IsParent())
            {
                return SourceEquipmentUID;
            }

            return ConnectEquipmentUID;
        }

        /// <summary>
        /// 子機器UIDを取得する
        /// </summary>
        /// <returns>子機器UID</returns>
        public string GetChildUID()
        {
            if (IsParent())
            {
                return ConnectEquipmentUID;
            }

            return SourceEquipmentUID;
        }

        /// <summary>
        /// 親機かどうかを判定する
        /// </summary>
        /// <returns>親機フラグ（親機であればtrue）</returns>
        public bool IsParent()
        {
            if (!Parent.HasValue)
            {
                return false;
            }

            return Parent.Value;
        }

        /// <summary>
        /// 変換（親フラグ=true）の場合
        /// </summary>
        /// <returns>DtParentChildConnect</returns>
        public DtParentChildConnectFromParent ConvertForParent()
        {
            DtParentChildConnectFromParent result = new DtParentChildConnectFromParent();
            result.ParentDeviceUid = SourceEquipmentUID;
            result.ChildDeviceUid = ConnectEquipmentUID;
            result.ParentResult = Success;
            result.ParentConfirmDatetime = ConfirmDT;

            if (Success != null && Success.Value)
            {
                // 成功だった場合には最終通信日時にも設定する
                // 失敗した場合にはDB設定時に（レコード）生成日時を設定する
                result.ParentLastConnectDatetime = ConfirmDT;
            }

            return result;
        }

        /// <summary>
        /// 変換（親フラグ=false）の場合
        /// </summary>
        /// <returns>DtParentChildConnect</returns>
        public DtParentChildConnectFromChild ConvertForChild()
        {
            DtParentChildConnectFromChild result = new DtParentChildConnectFromChild();
            result.ParentDeviceUid = ConnectEquipmentUID;
            result.ChildDeviceUid = SourceEquipmentUID;
            result.ChildResult = Success;
            result.ChildConfirmDatetime = ConfirmDT;

            if (Success != null && Success.Value)
            {
                // 成功だった場合には最終通信日時にも設定する
                // 失敗した場合にはDB設定時に（レコード）生成日時を設定する
                result.ChildLastConnectDatetime = ConfirmDT;
            }           

            return result;
        }
    }
}