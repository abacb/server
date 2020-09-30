using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rms.Server.Core.Utility.Exceptions;
using Rms.Server.Core.Utility.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Rms.Server.Core.Azure.Functions.WebApi.Utility
{
    /// <summary>
    /// WebAPIHelper
    /// </summary>
    public static class WebApiHelper
    {
        /// <summary>
        /// long型パラメータをバイト配列型に変換する。並びはビッグエンディアン固定。
        /// </summary>
        /// <param name="param">long型パラメータ</param>
        /// <returns>変換後バイト配列</returns>
        public static byte[] ConvertLongToByteArray(long param)
        {
            byte[] byteArray = BitConverter.GetBytes(param);

            // ビッグエンディアン形式に変換する
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(byteArray);
            }

            return byteArray;
        }

        /// <summary>
        /// バイト配列型パラメータをlong型に変換する。並びはビッグエンディアンとして処理する。
        /// </summary>
        /// <param name="param">バイト配列型パラメータ</param>
        /// <returns>変換後long</returns>
        public static long ConvertByteArrayToLong(byte[] param)
        {
            if (param == null)
            {
                return 0;
            }

            // 副作用を避けるために配列をコピーする
            byte[] copyParam = new byte[param.Length];
            param.CopyTo(copyParam, 0);

            // ビッグエンディアン形式に変換する
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(copyParam);
            }

            return BitConverter.ToInt64(copyParam);
        }
    }
}
