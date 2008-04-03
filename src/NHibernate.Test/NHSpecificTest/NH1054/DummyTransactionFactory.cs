using System;
using NHibernate.AdoNet;
using NHibernate.Transaction;

namespace NHibernate.Test.NHSpecificTest.NH1054
{
	public class DummyTransactionFactory : NHibernate.Transaction.ITransactionFactory
	{
		void NHibernate.Transaction.ITransactionFactory.Configure(System.Collections.IDictionary props)
		{
			
		}

		ITransaction NHibernate.Transaction.ITransactionFactory.CreateTransaction(NHibernate.Engine.ISessionImplementor session)
		{
			return null;
		}

		public ConnectionReleaseMode DefaultReleaseMode
		{
			get { throw new NotImplementedException(); }
		}

		public bool IsTransactionInProgress(AdoNetContext adoNetContext, ITransactionContext transactionContext,
		                                    ITransaction transaction)
		{
			throw new NotImplementedException();
		}
	}
}
