using System.Collections;
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
			AdoTransaction tx = new AdoTransaction( session );
			tx.Begin();
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