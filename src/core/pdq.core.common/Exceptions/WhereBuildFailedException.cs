using System;
using System.Runtime.Serialization;

namespace pdq.Exceptions
{
    [Serializable]
	public class WhereBuildFailedException : Exception
	{
		const string message =
			@"The where clause was unable to be built because there was more that one root element.
			  Consider whether you need to wrap your additional clauses or change the default clause handling behaviour.";
		public WhereBuildFailedException() : base(message) { }

        protected WhereBuildFailedException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}

