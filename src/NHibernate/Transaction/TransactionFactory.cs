using System;
using System.Data;
using System.Collections;
using NHibernate.Engine;

namespace NHibernate.Transaction {

	/// <summary>
	/// Summary description for TransactionFactory.
	/// </summary>
	public class TransactionFactory : ITransactionFactory {
		
		public ITransaction BeginTransaction(ISessionImplementor session) {
			Transaction tx = new Transaction(session);
			tx.Begin();
			return tx;
		}

		public void Configure(IDictionary props) {}
	}
}
