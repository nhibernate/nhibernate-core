using System;
using System.Collections;
using System.Data;
using System.Data.Common;

using NHibernate.Dialect;
using NHibernate.Engine;
using NHibernate.Engine.Transaction;
using NHibernate.Exceptions;
using NHibernate.Impl;

namespace NHibernate.Transaction
{
	public class AdoNetTransactionFactory : ITransactionFactory
	{
		private readonly IInternalLogger isolaterLog = LoggerProvider.LoggerFor(typeof(ITransactionFactory));

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

		public void ExecuteWorkInIsolation(ISessionImplementor session, IIsolatedWork work, bool transacted)
		{
			IDbConnection connection = null;
			IDbTransaction trans = null;
			// bool wasAutoCommit = false;
			try
			{
				// We make an exception for SQLite and use the session's connection,
				// since SQLite only allows one connection to the database.
				if (session.Factory.Dialect is SQLiteDialect)
					connection = session.Connection;
				else
					connection = session.Factory.ConnectionProvider.GetConnection();

				if (transacted)
				{
					trans = connection.BeginTransaction();
					// TODO NH: a way to read the autocommit state is needed
					//if (TransactionManager.GetAutoCommit(connection))
					//{
					//  wasAutoCommit = true;
					//  TransactionManager.SetAutoCommit(connection, false);
					//}
				}

				work.DoWork(connection, trans);

				if (transacted)
				{
					trans.Commit();
					//TransactionManager.Commit(connection);
				}
			}
			catch (Exception t)
			{
				using (new SessionIdLoggingContext(session.SessionId))
				{
					try
					{
						if (trans != null && connection.State != ConnectionState.Closed)
						{
							trans.Rollback();
						}
					}
					catch (Exception ignore)
					{
						isolaterLog.Debug("unable to release connection on exception [" + ignore + "]");
					}

					if (t is HibernateException)
					{
						throw;
					}
					else if (t is DbException)
					{
						throw ADOExceptionHelper.Convert(session.Factory.SQLExceptionConverter, t,
														 "error performing isolated work");
					}
					else
					{
						throw new HibernateException("error performing isolated work", t);
					}
				}
			}
			finally
			{
				//if (transacted && wasAutoCommit)
				//{
				//  try
				//  {
				//    // TODO NH: reset autocommit
				//    // TransactionManager.SetAutoCommit(connection, true);
				//  }
				//  catch (Exception)
				//  {
				//    log.Debug("was unable to reset connection back to auto-commit");
				//  }
				//}
				if (session.Factory.Dialect is SQLiteDialect == false)
					session.Factory.ConnectionProvider.CloseConnection(connection);
			}
		}

		public void Configure(IDictionary props)
		{
		}
	}
}