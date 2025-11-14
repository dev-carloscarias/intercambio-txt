using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.InnovaMD.Provider.Models.ClinicalConsultations
{
    public class ServicingNonPPNReason
    {
        public int ServicingNonPPNReasonId { get; set; }
        public string ServicingNonPPNReasonIdProtected { get; set; }
        public int LineOfBusinessId { get; set; }
        public string Description { get; set; }
    }
}
