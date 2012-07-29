using System;
using System.Runtime.Serialization;

namespace SQUnit
{
	[Serializable]
	public class TestFailedException : Exception
	{
		public TestFailedException()
		{
		}

		public TestFailedException(string message) : base(message)
		{
		}

		public TestFailedException(string message, Exception inner) : base(message, inner)
		{
		}

		protected TestFailedException(
			SerializationInfo info,
			StreamingContext context) : base(info, context)
		{
		}
	}
}