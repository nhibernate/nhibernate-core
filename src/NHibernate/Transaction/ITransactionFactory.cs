using System.Collections;
using NHibernate.Engine;

namespace NHibernate.Transaction
{
	/// <summary>
	/// An abstract factory for <c>ITransaction</c> instances.
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