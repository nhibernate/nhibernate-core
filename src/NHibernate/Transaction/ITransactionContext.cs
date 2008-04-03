using NHibernate.Engine;

namespace NHibernate.Transaction
{
	public interface ITransactionContext
	{
		/// Since C# doesn't support interfaces into interfaces ITransactionContext
		/// represent the inner interface at 
		/// org.hibernate.transaction.TransactionFactory.Context

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