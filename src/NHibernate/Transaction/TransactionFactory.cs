using System.Collections;
using NHibernate.Engine;

namespace NHibernate.Transaction
{
	public class TransactionFactory : ITransactionFactory
	{
		public ITransaction CreateTransaction(ISessionImplementor session)
		{
			return new AdoTransaction(session);
		}

		public void Configure(IDictionary props)
		{
		}
	}
}