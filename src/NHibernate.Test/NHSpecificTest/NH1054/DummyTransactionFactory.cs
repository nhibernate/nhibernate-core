using System;
using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Engine.Transaction;
using NHibernate.Transaction;

namespace NHibernate.Test.NHSpecificTest.NH1054
{
	public partial class DummyTransactionFactory : ITransactionFactory
	{
		public void Configure(IDictionary<string, string> props)
		{
		}

		public ITransaction CreateTransaction(ISessionImplementor session)
		{
			throw new NotImplementedException();
		}

		public void EnlistInSystemTransactionIfNeeded(ISessionImplementor session)
		{
			throw new NotImplementedException();
		}

		public void ExplicitJoinSystemTransaction(ISessionImplementor session)
		{
			throw new NotImplementedException();
		}

		public bool IsInActiveSystemTransaction(ISessionImplementor session)
		{
			return false;
		}

		public void ExecuteWorkInIsolation(ISessionImplementor session, IIsolatedWork work, bool transacted)
		{
			throw new NotImplementedException();
		}
	}
}
