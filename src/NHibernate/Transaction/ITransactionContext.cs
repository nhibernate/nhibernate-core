using NHibernate.Engine;

namespace NHibernate.Transaction
{
	/// <summary>
	/// Represent the inner interface in TransactionFactory class.
	/// See at Hibernate org.hibernate.transaction.TransactionFactory.Context
	/// </summary>
	public interface ITransactionContext
	{
		ISessionFactoryImplementor Factory { get; }
		//bool IsOpen { get; }
		bool IsClosed { get; }

		bool IsFlushModeNever { get; }
		bool IsFlushBeforeCompletionEnabled { get; }
		void ManagedFlush();

		bool ShouldAutoClose { get; }
		void ManagedClose();
	}
}