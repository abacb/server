using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Rms.Server.Core.Utility.Models
{
    /// <summary>
    /// 結果コード
    /// </summary>
    public enum ResultCode
    {
        /// <summary>
        /// 成功
        /// </summary>
        Succeed,

        /// <summary>
        /// DB更新の際RowVersionが期待していたものと異なる。
        /// </summary>
        Conflict,

        /// <summary>
        /// 配信情報更新/削除時、すでに開始されている場合
        /// </summary>
        /// <remarks>
        /// サービス層からトランザクションで検知するべきかも？
        /// </remarks>
        DoStartedDelivery,

        /// <summary>
        /// 更新/削除/取得しようとしたデータが見つからない場合
        /// </summary>
        NotFound,

        /// <summary>
        /// 期待していないパラメタ
        /// </summary>
        /// <remarks>
        /// 内部的にあり得ないパラメタエラーでは「ない」点に注意。
        /// </remarks>
        ParameterError,

        /// <summary>
        /// サーバー側の不明な原因によるエラー
        /// </summary>
        ServerEerror
    }

    /// <summary>
    /// Resultクラス、Result＜T＞クラスの拡張メソッド
    /// </summary>
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "関連クラスのためとりまとめる")]
    public static class ResultExtensions
    {
        /// <summary>
        /// nullチェックしたうえで成功判定を行う
        /// </summary>
        /// <param name="result">Resultクラス</param>
        /// <returns>対象がNullでなく成功している場合True。それ以外false。</returns>
        public static bool IsSuccessSafety(this Result result)
        {
            if (result == null)
            {
                return false;
            }

            return result.IsSuccess();
        }

        /// <summary>
        /// nullチェックしたうえで成功判定を行う
        /// </summary>
        /// <typeparam name="T">型</typeparam>
        /// <param name="result">Resultクラス</param>
        /// <returns>対象がNullでなく成功している場合True。それ以外false。</returns>
        public static bool IsSuccessSafety<T>(this Result<T> result)
        {
            if (result == null)
            {
                return false;
            }

            return result.IsSuccess();
        }
    }

    /// <summary>
    /// リポジトリからの結果
    /// </summary>
    /// <typeparam name="T">型</typeparam>
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "関連クラスのためとりまとめる")]
    public class Result<T>
    {
        /// <summary>
        /// 結果
        /// </summary>
        public Result()
        {
        }

        /// <summary>
        /// 結果
        /// </summary>
        /// <param name="resultCode">結果コード</param>
        /// <param name="message">メッセージ</param>
        /// <param name="entity">entity</param>
        public Result(
            ResultCode resultCode,
            string message,
            T entity)
        {
            ResultCode = resultCode;
            Message = message;
            Entity = entity;
        }

        /// <summary>
        /// 結果コード
        /// </summary>
        public ResultCode ResultCode { get; }

        /// <summary>
        /// メッセージ
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Entity
        /// </summary>
        public T Entity { get; }

        /// <summary>
        /// 成功かどうか
        /// </summary>
        /// <returns>true:成功, false:失敗</returns>
        public bool IsSuccess()
        {
            return ResultCode == ResultCode.Succeed;
        }
    }

    /// <summary>
    /// リポジトリからの結果
    /// </summary>
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "関連クラスのためとりまとめる")]
    public class Result
    {
        /// <summary>
        /// 結果
        /// </summary>
        /// <param name="resultCode">結果コード</param>
        /// <param name="message">メッセージ</param>
        /// <param name="ex">例外</param>
        public Result(
            ResultCode resultCode,
            string message = null,
            Exception ex = null)
        {
            ResultCode = resultCode;
            Message = message;
            Exception = ex;
        }

        /// <summary>
        /// 結果コード
        /// </summary>
        public ResultCode ResultCode { get; }

        /// <summary>
        /// メッセージ
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// 例外
        /// </summary>
        public Exception Exception { get; }

        /// <summary>
        /// 成功かどうか
        /// </summary>
        /// <returns>true:成功, false:失敗</returns>
        public bool IsSuccess()
        {
            return ResultCode == ResultCode.Succeed;
        }
    }
}