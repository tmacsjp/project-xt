using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace OA.Core
{
    public class ApiException : Exception
    {
        public ApiException()
        {

        }

        public ApiException(string message)
            : base(message)
        {

        }

        public ApiException(string message, Exception innerException)
            : base(message, innerException)
        {

        }

        public ApiException(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context)
        {

        }
    }
}
