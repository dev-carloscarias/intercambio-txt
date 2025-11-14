using System.Collections.Generic;

namespace com.InnovaMD.Provider.Models.ClinicalConsultations.Response
{
    public class ServicesResponse
    {
        public IEnumerable<ProcedureBundle> Procedures { get; set; }
        public bool AllowAutoSelect { get; set; }
        public string ErrorMessageRequired { get; set; }
        public string ErrorMessageMaximum { get; set; }
        public string ErrorMessageMinimum { get; set; }
    }
}
