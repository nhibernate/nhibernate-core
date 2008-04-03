using System.Collections;
using NHibernate;
using NHibernate.AdoNet;
using NHibernate.Engine;

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
	public interface ITransactionFactory
	{
		/// <summary>
		/// Configure from the given properties
		/// </summary>
		/// <param name="props"></param>
		void Configure(IDictionary props);

		/// <summary>
		/// Create a new transaction and return it without starting it.
		/// </summary>
		ITransaction CreateTransaction(ISessionImplementor session);

		/// <summary>
		/// Get the default connection release mode
		/// </summary>
		ConnectionReleaseMode DefaultReleaseMode { get; }

		//TODO: do we need this?
		//bool IsTransactionManagerRequired { get; }

		//TODO: do we need this?
		//bool AreCallbacksLocalToHibernateTransactions { get; }

		/// <summary>
		/// Determine whether an underlying transaction is in progress.
		///
		/// Mainly this is used in determining whether to register a
		/// synchronization as well as whether or not to circumvent
		/// auto flushing outside transactions. 
		/// </summary>
		/// <returns>true if an underlying transaction is know to be in effect.</returns>
		bool IsTransactionInProgress(AdoNetContext adoNetContext, ITransactionContext transactionContext, ITransaction transaction);
	}
}