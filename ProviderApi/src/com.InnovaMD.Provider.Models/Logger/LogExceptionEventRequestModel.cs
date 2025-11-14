using com.InnovaMD.Utilities.Schema.SchemaModels.Definitions;
using System.ComponentModel.DataAnnotations;

namespace com.InnovaMD.Provider.Models.Log
{
    public class LogExceptionEventRequestModel
    {
        public string Functionality { get; set; }

        [Required (ErrorMessage = "Message is Required")]
        public string Message { get; set; }

        public string Exception { get; set; }

        public string ExceptionType { get; set; }

        public User User { get; set; }

        public RequestProperties RequestProperties { get; set; }
    }
}
