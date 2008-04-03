using System.Collections;
using NHibernate.AdoNet;
using NHibernate.Engine;

namespace NHibernate.Transaction
{
	public class AdoNetTransactionFactory : ITransactionFactory
	{
		public ITransaction CreateTransaction(ISessionImplementor session)
		{
			return new AdoTransaction(session);
		}

		public void Configure(IDictionary props)
		{
		}

		public ConnectionReleaseMode DefaultReleaseMode
		{
			get { throw new System.NotImplementedException(); }
		}

		public bool IsTransactionInProgress(AdoNetContext adoNetContext, ITransactionContext transactionContext,
		                                    ITransaction transaction)
		{
			throw new System.NotImplementedException();
		}
	}
}