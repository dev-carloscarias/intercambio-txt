
namespace com.InnovaMD.Provider.Models.Security
{
    public class Feature
    {
        public virtual int FeatureId { get; set; }
        public virtual FeatureGroup Group { get; set; }
        public virtual ApplicationDomainContext Cotext { get; set; }
        public virtual string Name { get; set; }
        public virtual bool IsShareable { get; set; }
        public virtual string Description { get; set; }
        public virtual bool IsBreakTheGlassAllowed { get; set; }

        public bool? IsActive { get; set; }
    }
}
