using System;

namespace NHibernate 
{
	/// <summary>
	/// Indicated that a transaction could not be begun, committed, or rolled back
	/// </summary>
	[Serializable]
	public class TransactionException : HibernateException 
	{
		public TransactionException(string message, Exception root) : base(message, root) {}

		public TransactionException(string message) : base(message) {}
	}
}
