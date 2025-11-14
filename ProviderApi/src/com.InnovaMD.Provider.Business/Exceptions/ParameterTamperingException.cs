using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace com.InnovaMD.Provider.Business.Exceptions
{
    public class ParameterTamperingException : Exception, ISerializable
    {
        public ParameterTamperingException()
            : base()
        { }

        public ParameterTamperingException(string message)
            : base(message)
        { }

        public ParameterTamperingException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}