using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.InnovaMD.Provider.Models.ClinicalConsultations
{
    public class ClinicalConsultationServicingNonPPNReason
    {
        public long? ClinicalConsultationId { get; set; }
        public long? ClinicalConsultationNoPPNReasonId { get; set; }
        public int? NoPPNReasonId { get; set; }
        public string NoPPNReasonDescription { get; set; }
    }
}
