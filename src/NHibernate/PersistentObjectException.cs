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
		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		public PersistentObjectException( string message ) : base( message )
		{
		}
	}
}