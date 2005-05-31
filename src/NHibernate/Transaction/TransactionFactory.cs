using System.Collections;
using System.Data;
using NHibernate.Engine;

namespace NHibernate.Transaction
{
	/// <summary>
	/// Summary description for TransactionFactory.
	/// </summary>
	public class TransactionFactory : ITransactionFactory
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="session"></param>
		/// <returns></returns>
		public ITransaction BeginTransaction( ISessionImplementor session )
		{
			return BeginTransaction( session, IsolationLevel.Unspecified );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="session"></param>
		/// <param name="isolationLevel"></param>
		/// <returns></returns>
		public ITransaction BeginTransaction( ISessionImplementor session, IsolationLevel isolationLevel )
		{
			AdoTransaction tx = new AdoTransaction( session );
			tx.Begin( isolationLevel );
			return tx;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="props"></param>
		public void Configure( IDictionary props )
		{
		}
	}
}