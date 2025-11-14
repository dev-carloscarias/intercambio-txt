using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace com.InnovaMD.Provider.Models.Common
{
    public class ValidationResultModel
    {
        public bool IsValid { get; set; }
        public List<ValidationResult> ValidationResults { get; set; }
    }
}
