using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Rms.Server.Core.Azure.Utility.Validations
{
    /// <summary>
    /// RequestValidator
    /// </summary>
    public static class RequestValidator
    {
        /// <summary>
        /// リクエストが不正なパラメータかチェックする
        /// </summary>
        /// <typeparam name="T">リクエストの型</typeparam>
        /// <param name="request">リクエスト</param>
        /// <param name="errorMessage">(エラーがある場合は)エラーメッセージ</param>
        /// <returns>true: 不正なパラメータ / false: 正常なパラメータ</returns>
        public static bool IsBadRequestParameter<T>(T request, out string errorMessage)
        {
            if (request == null)
            {
                errorMessage = "Request body is null";
                return true;
            }
            
            if (IsNullAllInstanceProperties(request))
            {
                errorMessage = "All request property is null";
                return true;
            }

            // プロパティがすべてnullの場合、BadRequestとする。
            // [理由]
            // WebAPIのリクエストに意図しない値（datetimeに不正な値が含まれる等）が入る場合、
            // jsonのデシリアライズに失敗してすべてnullの値でWebAPIにわたってくる。
            // そのケースを判別するために、ここでエラー内容を区別する。
            // (いずれにせよ結果的にRequired不足によりエラーにはなるが、
            // その場合個別のエラーメッセージが書かれるため、可読性のために本処理を追加する。)
            if (IsNullAllInstanceProperties(request))
            {
                errorMessage = "All request property is null";
                return true;
            }

            // リクエストパラメータチェック
            var validationResults = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(request, new ValidationContext(request), validationResults, true);
            if (!isValid)
            {
                // エラーメッセージをwebapiに返却
                string message = string.Empty;
                validationResults.ForEach(result => message = string.Join(" / ", message, result.ErrorMessage));

                errorMessage = message;
                return true;
            }

            // エラーなし
            errorMessage = string.Empty;
            return false;
        }

        /// <summary>
        /// オブジェクトやクラスインスタンスのプロパティが全てNullかチェックする
        /// </summary>
        /// <typeparam name="T">型</typeparam>
        /// <param name="obj">プロパティを持つインスタンス、またはオブジェクト</param>
        /// <returns>true：プロパティのすべてがNull / false：Null以外の値を持つプロパティが存在する</returns>
        private static bool IsNullAllInstanceProperties<T>(T obj)
        {
            foreach (PropertyInfo property in obj.GetType().GetProperties())
            {
                object value = property.GetValue(obj);
                ////リクエストのプロパティのNullチェック
                if (value != null)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
