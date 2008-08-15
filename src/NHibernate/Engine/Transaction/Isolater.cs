using System;
using System.Data;
using System.Data.Common;
using log4net;
using NHibernate.Exceptions;

namespace NHibernate.Engine.Transaction
{
	/// <summary>
	/// Class which provides the isolation semantics required by
	/// an <see cref="IIsolatedWork"/>.
	/// </summary>
	/// <remarks>
	/// <list type="bullet">
  /// <listheader>
	///      <description>Processing comes in two flavors:</description>
  ///  </listheader>
  ///  <item>
	///      <term><see cref="DoIsolatedWork"/> </term>
	///      <description>makes sure the work to be done is performed in a seperate, distinct transaction</description>
  ///  </item>
	///  <item>
	///      <term><see cref="DoNonTransactedWork"/> </term>
	///      <description>makes sure the work to be done is performed outside the scope of any transaction</description>
	///  </item>
	/// </list>
	/// </remarks>
	public class Isolater
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(Isolater));

		private static DoWork GetApropieateDelegate()
		{
			bool isAmbientTransation = System.Transactions.Transaction.Current != null;
			if (isAmbientTransation)
			{
				return AmbientDelegateWork;
			}
			else
			{
				return AdoDelegateWork;
			}
		}
		/// <summary> 
		/// Ensures that all processing actually performed by the given work will
		/// occur on a seperate transaction. 
		/// </summary>
		/// <param name="work">The work to be performed. </param>
		/// <param name="session">The session from which this request is originating. </param>
		public static void DoIsolatedWork(IIsolatedWork work, ISessionImplementor session)
		{
			DoWork worker = GetApropieateDelegate();
			worker(session, work, true);
		}

		/// <summary> 
		/// Ensures that all processing actually performed by the given work will
		/// occur outside of a transaction. 
		/// </summary>
		/// <param name="work">The work to be performed. </param>
		/// <param name="session">The session from which this request is originating. </param>
		public static void DoNonTransactedWork(IIsolatedWork work, ISessionImplementor session)
		{
			DoWork worker = GetApropieateDelegate();
			worker(session, work, false);
		}

		private delegate void DoWork(ISessionImplementor session, IIsolatedWork work, bool transacted);

		private static void AdoDelegateWork(ISessionImplementor session, IIsolatedWork work, bool transacted)
		{
			IDbConnection connection = null;
			IDbTransaction trans = null;
			// bool wasAutoCommit = false;
			try
			{
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

				work.DoWork(connection);

				if (transacted)
				{
					trans.Commit();
					//TransactionManager.Commit(connection);
				}
			}
			catch (Exception t)
			{
				try
				{
					if (transacted && connection != null && !(connection.State == ConnectionState.Closed))
					{
						trans.Rollback();
            // TransactionManager.RollBack(connection);
					}
				}
				catch (Exception ignore)
				{
					log.Debug("unable to release connection on exception [" + ignore + "]");
				}

				if (t is HibernateException)
				{
					throw;
				}
				else if (t is DbException)
				{
					throw ADOExceptionHelper.Convert(session.Factory.SQLExceptionConverter, t, "error performing isolated work");
				}
				else
				{
					throw new HibernateException("error performing isolated work", t);
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
				session.Factory.ConnectionProvider.CloseConnection(connection);
			}
		}

		private static void AmbientDelegateWork(ISessionImplementor session, IIsolatedWork work, bool transacted)
		{
			throw new NotSupportedException("Not supported yet.");
		}

	}
}