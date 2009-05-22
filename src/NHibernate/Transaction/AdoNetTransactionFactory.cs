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

		public void EnlistInDistributedTransactionIfNeeded(ISessionImplementor session)
		{
			// nothing need to do here, we only support local transactions with this factory
		}

		public bool IsInDistributedActiveTransaction(ISessionImplementor session)
		{
			return false;
		}

		public void Configure(IDictionary props)
		{
		}
	}
}