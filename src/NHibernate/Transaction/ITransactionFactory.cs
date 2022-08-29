using System;
using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Engine.Transaction;

namespace NHibernate.Transaction
{
	/// <summary>
	/// <para>
	/// A factory interface for <see cref="ITransaction"/> instances.
	/// Concrete implementations are specified by <c>transaction.factory_class</c> 
	/// configuration property.
	/// </para>
	/// <para>
	/// Implementors must be threadsafe and should declare a public default constructor. 
	/// <seealso cref="ITransactionContext"/>
	/// </para>
	/// </summary>
	public partial interface ITransactionFactory
	{
		/// <summary>
		/// Configure from the given properties.
		/// </summary>
		/// <param name="props">The configuration properties.</param>
		void Configure(IDictionary<string, string> props);

		/// <summary>
		/// Create a new <see cref="ITransaction"/> and return it without starting it.
		/// </summary>
		/// <param name="session">The session for which to create a new transaction.</param>
		/// <return>The created transaction.</return>
		ITransaction CreateTransaction(ISessionImplementor session);

		/// <summary>
		/// <para>
		/// If supporting system <see cref="System.Transactions.Transaction"/>, enlist the session in
		/// the ambient transaction if any. This method may be call multiple times for the same ambient
		/// transaction, and must support it. (Avoid re-enlisting the session if already enlisted.)
		/// </para>
		/// <para>Do nothing if the transaction factory does not support system transaction, or
		/// if the session auto-join transaction option is disabled.</para>
		/// </summary>
		/// <param name="session">The session having to participate in the ambient system transaction if any.</param>
		void EnlistInSystemTransactionIfNeeded(ISessionImplementor session);

		/// <summary>
		/// Enlist the session in the current system <see cref="System.Transactions.Transaction"/>.
		/// </summary>
		/// <param name="session">The session to enlist.</param>
		/// <exception cref="NotSupportedException">Thrown if the transaction factory does not support system
		/// transactions.</exception>
		/// <exception cref="HibernateException">Thrown if there is no current transaction.</exception>
		void ExplicitJoinSystemTransaction(ISessionImplementor session);

		/// <summary>
		/// If supporting system <see cref="System.Transactions.Transaction"/>, indicate whether the given
		/// <paramref name="session"/> is currently enlisted in an system transaction. Otherwise
		/// <see langword="false" />.
		/// </summary>
		/// <param name="session"></param>
		/// <returns><see langword="true" /> if the session is enlisted in an system transaction.</returns>
		/// <remarks>
		/// When a <see cref="System.Transactions.Transaction"/> is distributed, a number of processing will run
		/// on dedicated threads, and may call this. This method must not rely on
		/// <see cref="System.Transactions.Transaction.Current" />: it may not be relevant for the
		/// <paramref name="session"/>.
		/// </remarks>
		bool IsInActiveSystemTransaction(ISessionImplementor session);

		/// <summary>
		/// Execute a work outside of the current transaction (if any).
		/// </summary>
		/// <param name="session">The session for which an isolated work has to be executed.</param>
		/// <param name="work">The work to execute.</param>
		/// <param name="transacted"><see langword="true" /> for encapsulating the work in a dedicated
		/// transaction, <see langword="false" /> for not transacting it.</param>
		void ExecuteWorkInIsolation(ISessionImplementor session, IIsolatedWork work, bool transacted);
	}
}
