using System;
using System.Threading.Tasks;

namespace com.InnovaMD.Provider.Data.ClinicalConsultations
{
    public interface IReportsRepository : IDisposable
    {
        Task<string> GetAzureAppAccessToken();
    }
}
