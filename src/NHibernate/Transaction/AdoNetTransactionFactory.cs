using System.Collections;
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
	}
}