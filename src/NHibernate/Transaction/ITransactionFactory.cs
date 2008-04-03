using System.Collections;
using NHibernate;
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
	}
}