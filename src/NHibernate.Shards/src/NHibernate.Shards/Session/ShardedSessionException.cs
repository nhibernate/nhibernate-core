using System;
using System.Runtime.Serialization;

namespace NHibernate.Shards.Session
{
	/// <summary>
	/// Main exception used in Hibernate Shards.
	/// </summary>
	public class ShardedSessionException : Exception
	{
		public ShardedSessionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public ShardedSessionException(string message, Exception innerException) : base(message, innerException)
		{
		}

		public ShardedSessionException(string message) : base(message)
		{
		}

		public ShardedSessionException()
		{
		}
	}
}