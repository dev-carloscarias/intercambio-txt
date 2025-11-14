using com.InnovaMD.Provider.Models.Security;
using com.InnovaMD.Utilities.Schema.SchemaModels;
using System.ComponentModel.DataAnnotations;

namespace com.InnovaMD.Provider.Models.Log
{
    public class MyAuditEvent : AuditEvent
    {
        [Required]
        [EnumDataType(typeof(AuditEventTypes))]
        public override string AuditEventType { get ; set; }
        [Required]
        [EnumDataType(typeof(AuditEventGroups))]
        public override string AuditEventGroup { get; set; }
        [Required]
        [EnumDataType(typeof(ApplicationDomains))]
        public override string ApplicationDomain { get; set; }
    }
}
