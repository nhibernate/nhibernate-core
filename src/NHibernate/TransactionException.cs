using System;

namespace NHibernate
{
	/// <summary>
	/// Indicated that a transaction could not be begun, committed, or rolled back
	/// </summary>
	[Serializable]
	public class TransactionException : HibernateException
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="root"></param>
		public TransactionException( string message, Exception root ) : base( message, root )
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		public TransactionException( string message ) : base( message )
		{
		}
	}
}