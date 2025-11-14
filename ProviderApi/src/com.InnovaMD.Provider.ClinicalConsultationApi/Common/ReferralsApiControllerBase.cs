using com.InnovaMD.Utilities.Provider.Security;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;


namespace com.InnovaMD.Provider.ClinicalConsultationApi.Common
{
    public class ClinicalConsultationApiControllerBase : ControllerBase
    {

        private readonly IDataProtectionProvider _provider;
        private IDataProtector _protector;
        protected IDataProtector Protector
        {
            get
            {
                if (_protector == null)
                {
                    _protector = _provider?.CreateProtector($"{User.GetApplicationDomainId()}-{User.GetUsername()}");
                }
                return _protector;
            }
            private set { }
        }

        protected ClinicalConsultationApiControllerBase(IDataProtectionProvider provider)
        {
            _provider = provider;
        }
        protected ClinicalConsultationApiControllerBase()
        {
        }
    }
}
