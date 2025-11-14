using System;
using System.Collections.Generic;
using System.Text;

namespace com.InnovaMD.Provider.Models.Security
{
    public class FeatureGroup
    {
        public virtual int FeatureGroupId { get; set; }
        public virtual string Name { get; set; }

        public virtual IEnumerable<Feature> Features { get; set; }
    }
}
