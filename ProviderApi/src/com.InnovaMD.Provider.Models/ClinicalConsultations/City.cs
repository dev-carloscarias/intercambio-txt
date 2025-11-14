namespace com.InnovaMD.Provider.Models.ClinicalConsultations
{
    public class City
    {
        public int? ProviderAffiliationId { get; set; }
        public int CityId { get; set; }
        public string Name { get; set; }
        public string ZipCode { get; set; }
        public string CityIdProtected { get; set; }
    }
}
