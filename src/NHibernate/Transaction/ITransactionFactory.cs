using System.Collections;
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
		/// Configure from the given properties
		/// </summary>
		/// <param name="props"></param>
		void Configure( IDictionary props );
	}
}