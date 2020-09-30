using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Rms.Server.Core.Azure.Utility.Validations
{
    //// https://qiita.com/mak_in/items/f5c7443d73aca38c039b

    /// <summary>
    /// 子オブジェクトのValidationも行います
    /// </summary>
    public class NestedValidateAttribute : ValidationAttribute
    {
        /// <summary>
        /// IsValid
        /// </summary>
        /// <param name="value">対象値</param>
        /// <param name="validationContext">ValidationContext</param>
        /// <returns>ValidationResult</returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }

            var results = new List<ValidationResult>();

            // TryValidateObject()は配列に対応していないため、自力で行う
            if (typeof(IEnumerable).IsAssignableFrom(value.GetType()))
            {
                foreach (var valueInArray in value as IEnumerable)
                {
                    var innerResults = new List<ValidationResult>();
                    var context = new ValidationContext(valueInArray, null, null);

                    // Validationを実施
                    Validator.TryValidateObject(valueInArray, context, innerResults, true);

                    results.AddRange(innerResults);
                }
            }
            else
            {
                var context = new ValidationContext(value, null, null);
                //// Validationを実施
                Validator.TryValidateObject(value, context, results, true);
            }

            if (results.Count != 0)
            {
                var validationResults = results.Select(x => new ValidationResult(x.ErrorMessage, x.MemberNames.Select(y => $"{validationContext.DisplayName}.{y}"))).ToList();

                // エラーが発生した具体的な値をすべて出力する。
                string message = string.Empty;
                validationResults.ForEach(result => message = string.Join(" / ", message, result.ErrorMessage));

                var compositeResults = new CompositeValidationResult($"{validationContext.DisplayName}.{{{message}}}");
                compositeResults.AddResults(validationResults);
                return compositeResults;
            }

            return ValidationResult.Success;
        }
    }
}
