using System;

namespace NHibernate.Cache 
{
	/// <summary>
	/// Represents any exception from an <c>ICache</c>
	/// </summary>
	[Serializable]
	public class CacheException : HibernateException 
	{
		public CacheException(string message) : base(message) { }

		public CacheException(Exception e) : base(e) { }
	}

}
