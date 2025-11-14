using System;

namespace com.InnovaMD.Provider.Models.ClinicalConsultations
{
    public class ProcedureBundle
    {
        public int ProcedureBundleId { get; set; }
        public string Description { get; set; }
        public int LineOfBusinessId { get; set; }
        public int DefaultUnits { get; set; }
        public int MinimumUnits { get; set; }
        public int MaximumUnits { get; set; }
        public string ReferenceCode { get; set; }
        public int? SortOrder { get; set; }
        public string ServiceTypeCode { get; set; }
        public string ProcedureBundleIdProtected { get; set; }
        public string LineOfBusinessIdProtected { get; set; }
    }
}
