using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using NHibernate.Dialect;
using NHibernate.Engine;
using NHibernate.Engine.Transaction;
using NHibernate.Exceptions;
using NHibernate.Impl;

namespace NHibernate.Transaction
{
	/// <summary>
	/// Minimal <see cref="ITransaction"/> factory implementation.
	/// Does not support system <see cref="System.Transactions.Transaction"/>.
	/// </summary>
	public partial class AdoNetTransactionFactory : ITransactionFactory
	{
		private readonly INHibernateLogger isolaterLog = NHibernateLogger.For(typeof(ITransactionFactory));

		/// <inheritdoc />
		public virtual ITransaction CreateTransaction(ISessionImplementor session)
		{
			return new AdoTransaction(session);
		}

		/// <inheritdoc />
		public virtual void EnlistInSystemTransactionIfNeeded(ISessionImplementor session)
		{
			// nothing need to do here, we only support local transactions with this factory
		}

		/// <inheritdoc />
		public virtual void ExplicitJoinSystemTransaction(ISessionImplementor session)
		{
			throw new NotSupportedException("The current transaction factory does not support system transactions.");
		}

		/// <inheritdoc />
		public virtual bool IsInActiveSystemTransaction(ISessionImplementor session)
		{
			return false;
		}

		/// <inheritdoc />
		public virtual void ExecuteWorkInIsolation(ISessionImplementor session, IIsolatedWork work, bool transacted)
		{
			if (session == null)
				throw new ArgumentNullException(nameof(session));
			if (work == null)
				throw new ArgumentNullException(nameof(work));

			DbConnection connection = null;
			DbTransaction trans = null;
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
				using (session.BeginContext())
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
						isolaterLog.Debug(ignore, "Unable to rollback transaction");
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

				try
				{
					trans?.Dispose();
				}
				catch (Exception ignore)
				{
					isolaterLog.Warn(ignore, "Unable to dispose transaction");
				}

				if (connection != null && session.Factory.Dialect is SQLiteDialect == false)
					session.Factory.ConnectionProvider.CloseConnection(connection);
			}
		}

		/// <inheritdoc />
		public virtual void Configure(IDictionary<string, string> props)
		{
		}
	}
}
