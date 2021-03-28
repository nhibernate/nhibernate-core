using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using NHibernate.Dialect;
using NHibernate.Engine;
using NHibernate.Engine.Transaction;
using NHibernate.Exceptions;

namespace NHibernate.Transaction
{
	/// <summary>
	/// Minimal <see cref="ITransaction"/> factory implementation.
	/// Does not support system <see cref="System.Transactions.Transaction"/>.
	/// </summary>
	public partial class AdoNetTransactionFactory : ITransactionFactory
	{
		private static readonly INHibernateLogger _isolatorLog = NHibernateLogger.For(typeof(ITransactionFactory));

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
			try
			{
				// We make an exception for SQLite and use the session's connection,
				// since SQLite only allows one connection to the database.
				connection = session.Factory.Dialect is SQLiteDialect
					? session.Connection
					: session.Factory.ConnectionProvider.GetConnection();

				if (transacted)
				{
					trans = connection.BeginTransaction();
				}

				work.DoWork(connection, trans);

				if (transacted)
				{
					trans.Commit();
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
						_isolatorLog.Debug(ignore, "Unable to rollback transaction");
					}

					switch (t)
					{
						case HibernateException _:
							throw;
						case DbException _:
							throw ADOExceptionHelper.Convert(session.Factory.SQLExceptionConverter, t,
							                                 "error performing isolated work");
						default:
							throw new HibernateException("error performing isolated work", t);
					}
				}
			}
			finally
			{
				try
				{
					trans?.Dispose();
				}
				catch (Exception ignore)
				{
					_isolatorLog.Warn(ignore, "Unable to dispose transaction");
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
