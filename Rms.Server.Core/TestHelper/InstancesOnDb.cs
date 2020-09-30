using Rms.Server.Core.Utility.Models.Entites;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TestHelper
{
    /// <summary>
    /// DBに設定したインスタンスを保持するクラス
    /// </summary>
    public class InstancesOnDb
    {
        private Dictionary<Type, IList<Object>> dic = new Dictionary<Type, IList<Object>>();
        public void Add<T>(T obj)
        {
            if (dic.TryGetValue(typeof(T), out var list))
            {
                list.Add(obj);
            }
            else
            {
                var newList = new List<Object>(){
                obj
            };
                dic.Add(typeof(T), newList);
            }
        }
        public List<T> Get<T>()
        {
            var list = dic[typeof(T)];
            return list.Select(x => (T)(object)x).ToList();
        }

        // 以下は使用頻度の高そうな処理のsyntax sugar。
        public long GetMtDeliveryGroupStatusSid(string code = null)
        {
            return code == null ?
                Get<MtDeliveryGroupStatus>().First().Sid :
                Get<MtDeliveryGroupStatus>().First(x => x.Code == code).Sid;
        }
        public MtDeliveryFileType GetMtDeliveryFileType()
        {
            return Get<MtDeliveryFileType>().First();
        }
        public long GetMtConnectStatusSid()
        {
            return Get<MtConnectStatus>().First().Sid;
        }
        public long GetMtEquipmentlTypeSid(string code = null)
        {
            return code == null ?
                Get<MtEquipmentType>().First().Sid :
                Get<MtEquipmentType>().First(x => x.Code == code).Sid;
        }
        public MtEquipmentModel GetMtEquipmentModel(string code = null)
        {
            return code == null ?
                Get<MtEquipmentModel>().First() :
                Get<MtEquipmentModel>().First(x => x.Code == code);
        }
        public MtInstallType GetMtInstallType(string code = null)
        {
            return code == null ?
                Get<MtInstallType>().First() :
                Get<MtInstallType>().First(x => x.Code == code);
        }
        public long GetDtDeviceSid()
        {
            return Get<DtDevice>().First().Sid;
        }
        public long GetDtDeliveryFileSid()
        {
            return Get<DtDeliveryFile>().First().Sid;
        }
    }

}
