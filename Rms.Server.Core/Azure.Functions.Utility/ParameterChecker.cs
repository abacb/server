using System;
using System.ComponentModel.DataAnnotations;

namespace Rms.Server.Core.Azure.Functions.Utility
{
    /// <summary>
    /// ParameterCheckerクラス用ResultCode
    /// </summary>
    public enum ParameterCheckerResultCode
    {
        /// <summary>
        /// 成功
        /// </summary>
        Succeed,

        /// <summary>
        /// null引数エラー
        /// </summary>
        NullArgError,

        /// <summary>
        /// 必須パラメータエラー
        /// </summary>
        RequiredError,

        /// <summary>
        /// 文字数制限パラメータエラー
        /// </summary>
        LengthError,
    }

    /// <summary>
    /// パラメータのチェックを行うクラス
    /// </summary>
    public static class ParameterChecker
    {
        /// <summary>
        /// 属性設定に反するプロパティがないかチェックする
        /// </summary>
        /// <typeparam name="T">ジェネリッククラス</typeparam>
        /// <param name="param">チェックするインスタンス</param>
        /// <returns>true:無効プロパティあり / false:無効プロパティなし</returns>
        public static Result<ParameterCheckerResultCode> HasInvalidProperty<T>(T param)
        {
            if (param == null)
            {
                return new Result<ParameterCheckerResultCode>(
                    ParameterCheckerResultCode.NullArgError,
                    "argument is null");
            }

            foreach (var p in param.GetType().GetProperties())
            {
                var value = p.GetValue(param);

                // 必須パラメータかチェック
                RequiredAttribute reqAttr = Attribute.GetCustomAttribute(p, typeof(RequiredAttribute)) as RequiredAttribute;
                if (reqAttr != null && value == null)
                {
                    return new Result<ParameterCheckerResultCode>(
                        ParameterCheckerResultCode.RequiredError,
                        p.Name + " is required, but null");
                }

                // 文字数制限違反してないかチェック
                StringLengthAttribute lenAttr = Attribute.GetCustomAttribute(p, typeof(StringLengthAttribute)) as StringLengthAttribute;
                if (lenAttr != null)
                {
                    if (value == null)
                    {
                        continue;
                    }

                    int length = value.ToString().Length;
                    if ((length > lenAttr.MaximumLength) ||
                        (length < lenAttr.MinimumLength))
                    {
                        return new Result<ParameterCheckerResultCode>(
                            ParameterCheckerResultCode.LengthError,
                            lenAttr.MinimumLength + " <= " + p.Name + " length <= " + lenAttr.MaximumLength + ", but length is " + length);
                    }
                }
            }

            return new Result<ParameterCheckerResultCode>(
                ParameterCheckerResultCode.Succeed,
                "Parameter OK");
        }
    }
}
