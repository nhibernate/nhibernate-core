using System;
using System.Data;
using NHibernate.Engine;
namespace NHibernate.Transaction {

	/// <summary>
	/// Wraps an ADO.NET transaction to implements the <c>ITransaction</c> interface
	/// </summary>
	public class Transaction : ITransaction {
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(Transaction));
		private ISessionImplementor session;
		private IDbTransaction trans;
		private bool begun;
		private bool committed;
		private bool rolledBack;

		public Transaction(ISessionImplementor session) {
			this.session = session;
		}

		public IDbTransaction AdoTransaction {
			get { return trans; }
		}

		public void Begin() {
			log.Debug("begin");

			try {
				trans = session.Connection.BeginTransaction();
			} catch (Exception e) {
				log.Error("Begin transaction failed", e);
				throw new TransactionException("Begin failed with SQL exception", e);
			}

			begun = true;
		}

		public void Commit() {
			if (!begun) throw new TransactionException("Transaction not successfully started");

			log.Debug("commit");

			try {
				if ( session.FlushMode != FlushMode.Never ) session.Flush();
				try {
					trans.Commit();
					committed = true;
				} catch (Exception e) {
					log.Error("Commit failed", e);
					throw new TransactionException("Commit failed with SQL exception", e);
				}
			} finally {
				session.AfterTransactionCompletion();
			}
		}

		public void Rollback() {
			if (!begun) throw new TransactionException("Transaction not successfully started");

			log.Debug("rollback");
			try {
				trans.Rollback();
				rolledBack = true;
			} catch(Exception e) {
				log.Error("Rollback failed", e);
				throw new TransactionException("Rollback failed with SQL Exception", e);
			} finally {
				session.AfterTransactionCompletion();
			}
		}

		public bool WasRolledBack {
			get { return rolledBack; }
		}

		public bool WasCommitted {
			get { return committed; }
		}

	}
}
