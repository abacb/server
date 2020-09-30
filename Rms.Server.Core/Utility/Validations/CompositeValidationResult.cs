using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Rms.Server.Core.Azure.Utility.Validations
{
    /// <summary>
    /// 複数のValidation結果を格納できる
    /// </summary>
    public class CompositeValidationResult : ValidationResult
    {
        /// <summary>
        /// CompositeValidationResult
        /// </summary>
        /// <param name="errorMessage">エラーメッセージ</param>
        public CompositeValidationResult(string errorMessage) : base(errorMessage) { }

        /// <summary>
        /// CompositeValidationResult
        /// </summary>
        /// <param name="errorMessage">エラーメッセージ</param>
        /// <param name="memberNames">メンバ名</param>
        public CompositeValidationResult(string errorMessage, IEnumerable<string> memberNames) : base(errorMessage, memberNames) { }

        /// <summary>
        /// CompositeValidationResult
        /// </summary>
        /// <param name="validationResult">結果</param>
        protected CompositeValidationResult(ValidationResult validationResult) : base(validationResult) { }

        /// <summary>
        /// 複数のValidation結果
        /// </summary>
        public IEnumerable<ValidationResult> Results { get; } = new List<ValidationResult>();

        /// <summary>
        /// エラーを登録
        /// </summary>
        /// <param name="validationResult">ValidationResult</param>
        public void AddResult(ValidationResult validationResult)
        {
            (this.Results as IList<ValidationResult>).Add(validationResult);
        }

        /// <summary>
        /// エラーを登録
        /// </summary>
        /// <param name="validationResults">ValidationResults</param>
        public void AddResults(IEnumerable<ValidationResult> validationResults)
        {
            (this.Results as List<ValidationResult>).AddRange(validationResults);
        }
    }
}
