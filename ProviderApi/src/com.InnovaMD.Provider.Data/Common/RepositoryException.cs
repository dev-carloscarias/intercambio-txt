using System;
using System.Runtime.Serialization;

namespace com.InnovaMD.Provider.Data.Common
{
    public class RepositoryException : Exception, ISerializable
    {
        public RepositoryException()
            : base()
        { }
        public RepositoryException(string message)
            : base(message)
        { }
        public RepositoryException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
