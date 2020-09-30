using System;
using System.Collections.Generic;
using System.Text;

namespace Rms.Server.Core.Utility.Models.Dispatch
{
    /// <summary>
    /// IConvertibleModel
    /// </summary>
    /// <typeparam name="T">型</typeparam>
    public interface IConvertibleModel<T>
    {
        /// <summary>
        /// 変換
        /// </summary>
        /// <param name="deviceId">デバイスSID</param>
        /// <param name="eventData">イベント情報</param>
        /// <returns>変換後のデータ</returns>
        T Convert(long deviceId, RmsEvent eventData);
    }
}
