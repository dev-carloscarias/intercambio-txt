using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.InnovaMD.Provider.Models.ClinicalConsultations.Response
{
    public class CreateConsultationConfigurationsResponseModel
    {
        public string CancelConsultationCreateMessage { get; set; }
        public string AlertConsultationCreateCompleteStepMessage { get; set; }
        public string SubmitValidationMessage { get; set; }
    }
}
