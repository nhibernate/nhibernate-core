using System;

namespace NHibernate 
{
	/// <summary>
	/// Indicated that a transaction could not be begun, committed, or rolled back
	/// </summary>
	[Serializable]
	public class TransactionException : HibernateException 
	{
		public TransactionException(string msg, Exception root) : base(msg, root) {}

		public TransactionException(string msg) : base(msg) {}
	}
}
