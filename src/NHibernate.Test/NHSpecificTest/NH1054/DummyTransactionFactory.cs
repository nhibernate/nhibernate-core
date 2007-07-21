using System;
using System.Collections.Generic;
using System.Text;

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
	}
}
