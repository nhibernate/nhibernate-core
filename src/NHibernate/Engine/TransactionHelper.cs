using System.Data;
using System.Data.Common;
using NHibernate.Engine.Transaction;
using NHibernate.Exceptions;

namespace NHibernate.Engine
{
	/// <summary>
	/// Allows work to be done outside the current transaction, by suspending it,
	/// and performing work in a new transaction
	/// </summary>
	public abstract class TransactionHelper
	{
		public class Work : IIsolatedWork
		{
			private readonly ISessionImplementor session;
			private readonly TransactionHelper owner;
			internal object generatedValue;

			public Work(ISessionImplementor session, TransactionHelper owner)
			{
				this.session = session;
				this.owner = owner;
			}

			#region Implementation of IIsolatedWork

			public void DoWork(IDbConnection connection, IDbTransaction transaction)
			{
				try
				{
					generatedValue = owner.DoWorkInCurrentTransaction(session, connection, transaction);
				}
				catch (DbException sqle)
				{
					throw ADOExceptionHelper.Convert(session.Factory.SQLExceptionConverter, sqle, "could not get or update next value", null);
				}
			}

			#endregion
		}

		/// <summary> The work to be done</summary>
		public abstract object DoWorkInCurrentTransaction(ISessionImplementor session, IDbConnection conn, IDbTransaction transaction);

		/// <summary> Suspend the current transaction and perform work in a new transaction</summary>
		public virtual object DoWorkInNewTransaction(ISessionImplementor session)
		{
			Work work = new Work(session, this);
			Isolater.DoIsolatedWork(work, session);
			return work.generatedValue;
		}
	}
}