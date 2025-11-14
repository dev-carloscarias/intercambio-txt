using com.InnovaMD.Provider.Models.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace com.InnovaMD.Provider.Business.Exceptions
{
    public class RequestModelValidationException : ValidationException, ISerializable
    {
        public RequestModelValidationException()
            : base()
        { }
        public RequestModelValidationException(string message, BusinessResponseModel validationResults)
            : base(message, null, validationResults)
        { }
        public RequestModelValidationException(string message, List<ValidationResult> validationResults)
            : base(message, null, validationResults)
        { }
        public RequestModelValidationException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
