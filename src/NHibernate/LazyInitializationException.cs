using System;
using System.Runtime.Serialization;

namespace NHibernate 
{
	/// <summary>
	/// A problem occurred trying to lazily initialize a collection or proxy (for example the session
	/// was closed) or iterate query results.
	/// </summary>
	[Serializable]
	public class LazyInitializationException : Exception 
	{
		public LazyInitializationException(Exception root) : this(root.Message) {}

		public LazyInitializationException(string msg) : base(msg) 
		{
			log4net.LogManager.GetLogger( typeof(LazyInitializationException) ).Error(msg, this);
		}

		public LazyInitializationException(string msg, Exception root) : this(msg + " " + root.Message) {}

		public LazyInitializationException() : this("LazyInitalizationException") {}

		protected LazyInitializationException(SerializationInfo info, StreamingContext context) : base(info, context) { }

	}
}
