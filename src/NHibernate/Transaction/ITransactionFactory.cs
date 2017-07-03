using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Engine.Transaction;

namespace NHibernate.Transaction
{
	/// <summary>
	/// An abstract factory for <see cref="ITransaction"/> instances.
	/// Concrete implementations are specified by <c>transaction.factory_class</c> 
	/// configuration property.
	/// 
	/// Implementors must be threadsafe and should declare a public default constructor. 
	/// <seealso cref="ITransactionContext"/>
	/// </summary>
	public partial interface ITransactionFactory
	{
		/// <summary>
		/// Configure from the given properties
		/// </summary>
		/// <param name="props"></param>
		void Configure(IDictionary<string, string> props);

		/// <summary>
		/// Create a new transaction and return it without starting it.
		/// </summary>
		ITransaction CreateTransaction(ISessionImplementor session);

		void EnlistInSystemTransactionIfNeeded(ISessionImplementor session);

		bool IsInActiveSystemTransaction(ISessionImplementor session);

		void ExecuteWorkInIsolation(ISessionImplementor session, IIsolatedWork work, bool transacted);
	}
}