using System;

namespace NHibernate 
{
	/// <summary>
	/// Thrown when the user passes a persistent instance to a <c>ISession</c> method that expects a
	/// transient instance
	/// </summary>
	[Serializable]
	public class PersistentObjectException : HibernateException 
	{
		public PersistentObjectException(string s) : base(s) {}
	}
}
