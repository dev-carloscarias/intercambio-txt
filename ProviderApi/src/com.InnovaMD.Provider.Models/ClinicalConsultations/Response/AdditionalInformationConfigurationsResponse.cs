using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.InnovaMD.Provider.Models.ClinicalConsultations.Response
{
    public class AdditionalInformationConfigurationsResponse
    {
        public int ConsultationDaysBackAllowed { get; set; }
        public string CreateRequestConsultationMaximumAllowedMessage { get; set; }
        public int RequestConsultationMaximumAllowedValue { get; set; }
        public string RuleOutsMessage { get; set; }
    }
} 
