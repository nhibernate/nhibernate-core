using System;
using System.Data;
using System.Collections;
using NHibernate.Engine;

namespace NHibernate.Impl {

	/// <summary>
	/// Manages prepared statements and batching. Class exists to enfores seperation of concerns
	/// </summary>
	public abstract class BatcherImpl : IBatcher {
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(BatcherImpl));

		public static int open;

		protected ISessionImplementor session;
		protected ISessionFactoryImplementor factory;

		private IDbCommand batchUpdate;
		private string batchUpdateSQL;

		private ArrayList statementsToClose = new ArrayList();

		public BatcherImpl(ISessionImplementor session) {
			this.session = session;
			this.factory = session.Factory;
		}

		protected IDbCommand GetStatement() {
			return batchUpdate;
		}

		public IDbCommand PrepareStatement(string sql) {
			ExecuteBatch();
			LogOpen();
			return factory.GetPreparedStatement( session.Connection, sql, false);
		}
		public IDbCommand PrepareQueryStatement(string sql) {
			LogOpen();
			IDbCommand st = factory.GetPreparedStatement( session.Connection, sql, false );
			factory.SetFetchSize(st);
			statementsToClose.Add(st);
			return st;
		}

		public void CloseQueryStatement(IDbCommand st) {
			statementsToClose.Remove(st);
			LogClose();
			factory.ClosePreparedStatement(st);
		}

		public void CloseStatement(IDbCommand ps) {
			LogClose();
			factory.ClosePreparedStatement(ps);
		}

		public IDbCommand PrepareBatchStatement(string sql) {
			if ( !sql.Equals(batchUpdateSQL) ) {
				batchUpdate = PrepareStatement(sql);
				batchUpdateSQL=sql;
			}
			return batchUpdate;
		}

		public void ExecuteBatch() {
			if ( batchUpdate!=null ) {
				IDbCommand ps = batchUpdate;
				batchUpdate = null;
				batchUpdateSQL = null;
				try {
					DoExecuteBatch(ps);
				} finally {
					CloseStatement(ps);
				}
			}
		}

		public void CloseStatements() {
			foreach( IDbCommand cmd in statementsToClose ) {
				try {
					CloseStatement(cmd);
				} catch(Exception e) {
					// no big deal
					log.Warn("Could not close a JDBC statement", e);
				}
			}
			statementsToClose.Clear();
		}

		protected abstract void DoExecuteBatch(IDbCommand ps) ;
		public abstract void AddToBatch(int expectedCount);

		private static void LogOpen() {
			if ( log.IsDebugEnabled ) {
				open++;
				log.Debug( open + " open PreparedStatements" );
			}
		}

		private static void LogClose() {
			if ( log.IsDebugEnabled )
				open--;
		}
	}
}
