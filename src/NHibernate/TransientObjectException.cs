using System;

namespace NHibernate
{
	/// <summary>
	/// Throw when the user passes a transient instance to a <c>ISession</c> method that expects
	/// a persistent instance
	/// </summary>
	[Serializable]
	public class TransientObjectException : HibernateException
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		public TransientObjectException( string message ) : base( message )
		{
		}
	}
}