using System;
using System.Runtime.Serialization;

namespace SQUnit
{
	[Serializable]
	public class InvalidTestFileException : Exception
	{
		public InvalidTestFileException()
		{
		}

		public InvalidTestFileException(string message) : base(message)
		{
		}

		public InvalidTestFileException(string message, Exception inner) : base(message, inner)
		{
		}

		protected InvalidTestFileException(
			SerializationInfo info,
			StreamingContext context) : base(info, context)
		{
		}
	}
}