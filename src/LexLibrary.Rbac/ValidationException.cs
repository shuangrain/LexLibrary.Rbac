using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace LexLibrary.Rbac
{
    public class ValidationException : Exception
    {
        private readonly IList<ValidationResult> _validationResults = null;

        public Type _targetType { get; }

        public ValidationException(Type targetType, IList<ValidationResult> validationResults)
        {
            _targetType = targetType;
            _validationResults = validationResults;
        }

        public override string Message
        {
            get
            {
                return string.Concat(_targetType.ToString(),
                                     Environment.NewLine,
                                     string.Join(", ", _validationResults.Select(x => x.ErrorMessage)));
            }
        }
    }
}
