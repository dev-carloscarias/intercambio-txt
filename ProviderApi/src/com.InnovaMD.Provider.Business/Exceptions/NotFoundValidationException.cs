using System;
using System.Runtime.Serialization;

namespace com.InnovaMD.Provider.Business.Exceptions
{
    public class NotFoundValidationException : Exception, ISerializable
    {
        public NotFoundValidationException()
            : base()
        { }
        public NotFoundValidationException(string message)
            : base(message)
        { }
        public NotFoundValidationException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
