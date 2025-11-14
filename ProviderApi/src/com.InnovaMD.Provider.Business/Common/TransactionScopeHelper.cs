using System;
using System.Transactions;

namespace com.InnovaMD.Provider.Business.Common
{
    public static class TransactionScopeHelper
    {
        public static TransactionScope CreateTransactionScope(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            var transactionOptions = new TransactionOptions
            {
                IsolationLevel = isolationLevel,
                Timeout = TimeSpan.FromMinutes(10)
            };
            return new TransactionScope(TransactionScopeOption.Required, transactionOptions, TransactionScopeAsyncFlowOption.Enabled);
        }
    }
}
