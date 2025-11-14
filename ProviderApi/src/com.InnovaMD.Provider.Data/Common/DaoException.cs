using System;
using System.Runtime.Serialization;

namespace com.InnovaMD.Provider.Data.Common
{
    public class DaoException : Exception, ISerializable
    {
        public DaoException()
            : base()
        { }

        public DaoException(string message)
            : base(message)
        { }

        public DaoException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
