using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.InnovaMD.Provider.Models.ClinicalConsultations
{
    public class ViewLog
    {
        public long ViewLogId { get; set; }
        public long ClinicalConsultationId { get; set; }
        public int UserId { get; set; }
        public DateTime ViewedOn { get; set; }

}
}
