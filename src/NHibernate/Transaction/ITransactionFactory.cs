using System.Collections;
using System.Data;
using NHibernate.Engine;

namespace NHibernate.Transaction
{
	/// <summary>
	/// An abstract factory for <c>ITransaction</c> instances. (NHibernate note: do we need this??)
	/// </summary>
	public interface ITransactionFactory
	{
		/// <summary>
		/// Begin a transaction and return the associated <c>ITransaction</c> instance
		/// </summary>
		/// <param name="session"></param>
		ITransaction BeginTransaction( ISessionImplementor session );

		/// <summary>
		/// Begin a transaction with the specified isolation level and return
		/// the associated <c>ITransaction</c> instance
		/// </summary>
		/// <param name="session"></param>
		/// <param name="isolationLevel"></param>
		/// <returns></returns>
		ITransaction BeginTransaction( ISessionImplementor session, IsolationLevel isolationLevel );

		/// <summary>
		/// Configure from the given properties
		/// </summary>
		/// <param name="props"></param>
		void Configure( IDictionary props );
	}
}