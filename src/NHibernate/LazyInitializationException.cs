using System;

namespace NHibernate 
{
	/// <summary>
	/// A problem occurred trying to lazily initialize a collection or proxy (for example the session
	/// was closed) or iterate query results.
	/// </summary>
	[Serializable]
	public class LazyInitializationException : Exception 
	{
		public LazyInitializationException(Exception root) : base("Hibernate lazy instantiation problem", root) {}

		public LazyInitializationException(string msg) : base(msg) 
		{
			log4net.LogManager.GetLogger( typeof(LazyInitializationException) ).Error(msg, this);
		}

		public LazyInitializationException(string msg, Exception root) : base(msg, root) { }

	}
}
