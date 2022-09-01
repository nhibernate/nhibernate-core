using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using NHibernate.Transaction;

namespace NHibernate
{
	/// <summary>
	/// Allows the application to define units of work, while maintaining abstraction from the
	/// underlying transaction implementation
	/// </summary>
	/// <remarks>
	/// A transaction is associated with a <c>ISession</c> and is usually instantiated by a call to
	/// <c>ISession.BeginTransaction()</c>. A single session might span multiple transactions since 
	/// the notion of a session (a conversation between the application and the datastore) is of
	/// coarser granularity than the notion of a transaction. However, it is intended that there be
	/// at most one uncommitted <c>ITransaction</c> associated with a particular <c>ISession</c>
	/// at a time. Implementors are not intended to be threadsafe.
	/// </remarks>
	public partial interface ITransaction : IDisposable
	{
		/// <summary>
		/// Begin the transaction with the default isolation level.
		/// </summary>
		void Begin();

		/// <summary>
		/// Begin the transaction with the specified isolation level.
		/// </summary>
		/// <param name="isolationLevel">Isolation level of the transaction</param>
		void Begin(IsolationLevel isolationLevel);

		/// <summary>
		/// Flush the associated <c>ISession</c> and end the unit of work.
		/// </summary>
		/// <remarks>
		/// This method will commit the underlying transaction if and only if the transaction
		/// was initiated by this object.
		/// </remarks>
		void Commit();

		/// <summary>
		/// Force the underlying transaction to roll back.
		/// </summary>
		void Rollback();

		/// <summary>
		/// Is the transaction in progress
		/// </summary>
		bool IsActive { get; }

		/// <summary>
		/// Was the transaction rolled back or set to rollback only?
		/// </summary>
		bool WasRolledBack { get; }

		/// <summary>
		/// Was the transaction successfully committed?
		/// </summary>
		/// <remarks>
		/// This method could return <see langword="false" /> even after successful invocation of <c>Commit()</c>
		/// </remarks>
		bool WasCommitted { get; }

		/// <summary>
		/// Enlist the <see cref="DbCommand"/> in the current Transaction.
		/// </summary>
		/// <param name="command">The <see cref="DbCommand"/> to enlist.</param>
		/// <remarks>
		/// It is okay for this to be a no op implementation.
		/// </remarks>
		void Enlist(DbCommand command);

		// Obsolete since 5.2
		/// <summary>
		/// Register a user synchronization callback for this transaction.
		/// </summary>
		/// <param name="synchronization">The <see cref="ISynchronization"/> callback to register.</param>
		[Obsolete("Use RegisterSynchronization(ITransactionCompletionSynchronization) extension method instead. " +
			"If implementing ITransaction, implement a 'public void " +
			"RegisterSynchronization(ITransactionCompletionSynchronization)': the TransactionExtensions extension " +
			"method will call it.")]
		void RegisterSynchronization(ISynchronization synchronization);
	}

	// 6.0 TODO: merge into ITransaction
	public static class TransactionExtensions
	{
		/// <summary>
		/// Register an user synchronization callback for this transaction.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="synchronization">The <see cref="ISynchronization"/> callback to register.</param>
		public static void RegisterSynchronization(
			this ITransaction transaction,
			ITransactionCompletionSynchronization synchronization)
		{
			if (transaction is AdoTransaction adoTransaction)
			{
				adoTransaction.RegisterSynchronization(synchronization);
				return;
			}

			// Use reflection for supporting custom transaction factories and transaction implementations.
			var registerMethod = transaction.GetType().GetMethod(
				nameof(AdoTransaction.RegisterSynchronization),
				new[] { typeof(ITransactionCompletionSynchronization) });
			if (registerMethod == null)
				throw new NotSupportedException(
					$"{transaction.GetType()} does not support {nameof(ITransactionCompletionSynchronization)}");
			registerMethod.Invoke(transaction, new object[] { synchronization });
		}
		
		public static Task BeginAsync(this ITransaction transaction, IsolationLevel isolationLevel)
		{
			try
			{
				transaction.Begin(isolationLevel);
				return Task.CompletedTask;
			}
			catch (Exception ex)
			{
				return Task.FromException(ex);
			}
		}

		public static Task BeginAsync(this ITransaction transaction)
		{
			try
			{
				transaction.Begin();
				return Task.CompletedTask;
			}
			catch (Exception ex)
			{
				return Task.FromException(ex);
			}
		}
	}
}
