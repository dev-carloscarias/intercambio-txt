using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.InnovaMD.Provider.Models.ClinicalConsultations.Filters
{
    public class ZipCode
    {
        public int ZipCodeId { get; set; }
        public int CountryId { get; set; }
        public int CountyId { get; set; }
        public int StateId { get; set; }
        public string ZipCodeValue { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Region { get; set; }
        public string RegionEnglish { get; set; }
        public string RegionSpanis { get; set; }
        public string? ZipCodeValueProtected { get; set; }
    }
}
