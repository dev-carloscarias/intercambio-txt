using com.InnovaMD.Provider.Models.Common;

namespace com.InnovaMD.Provider.Data.Common
{
    public abstract class RepositoryBase
    {
        protected readonly ConnectionStringOptions connectionStringOptions;

        public RepositoryBase(ConnectionStringOptions connectionStringOptions)
        {
            this.connectionStringOptions = connectionStringOptions;
        }
    }
}
