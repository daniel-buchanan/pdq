using System;
using System.Runtime.Serialization;

namespace pdq.common.Exceptions
{
    [Serializable]
    public class PdqOptionsInvalidException : Exception
	{
		public PdqOptionsInvalidException(string reason)
            : base(reason) { }


        protected PdqOptionsInvalidException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}

