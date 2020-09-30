using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace Rms.Server.Core.Azure.Utility.Validations
{
    /// <summary>
    /// 配列が要素を1以上持つかのValidationを行う
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class RequiredAtLeastOneElementAttribute : ValidationAttribute
    {
        /// <summary>
        ///  エラーメッセージ
        /// </summary>
        private const string DefaultError = "'{0}' must have at least one element.";

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public RequiredAtLeastOneElementAttribute() : base(DefaultError)
        {
        }

        /// <summary>
        /// IsValid
        /// </summary>
        /// <param name="value">対象値</param>
        /// <returns>ValidationResult</returns>
        public override bool IsValid(object value)
        {
            return value is IList list && list.Count > 0;
        }

        /// <summary>
        /// エラーメッセージを返す
        /// </summary>
        /// <param name="name">対象名</param>
        /// <returns>エラーメッセージ</returns>
        public override string FormatErrorMessage(string name)
        {
            return string.Format(this.ErrorMessageString, name);
        }
    }
}
