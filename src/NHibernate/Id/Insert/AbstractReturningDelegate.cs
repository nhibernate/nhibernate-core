using System.Data;
using System.Data.Common;
using NHibernate.Engine;
using NHibernate.Exceptions;
using NHibernate.Impl;
using NHibernate.SqlCommand;

namespace NHibernate.Id.Insert
{
	/// <summary> 
	/// Abstract InsertGeneratedIdentifierDelegate implementation where the
	/// underlying strategy causes the enerated identitifer to be returned as an
	/// effect of performing the insert statement.  Thus, there is no need for an
	/// additional sql statement to determine the generated identitifer. 
	/// </summary>
	public abstract class AbstractReturningDelegate : IInsertGeneratedIdentifierDelegate
	{
		private readonly IPostInsertIdentityPersister persister;

		public AbstractReturningDelegate(IPostInsertIdentityPersister persister)
		{
			this.persister = persister;
		}

		protected virtual internal IPostInsertIdentityPersister Persister
		{
			get { return persister; }
		}

		#region IInsertGeneratedIdentifierDelegate Members

		public abstract IdentifierGeneratingInsert PrepareIdentifierGeneratingInsert();

		public object PerformInsert(SqlString insertSQL, ISessionImplementor session, IBinder binder)
		{
			try
			{
				// prepare and execute the insert
				IDbCommand insert = Prepare(insertSQL, session);
				try
				{
					binder.BindValues(insert);
					return ExecuteAndExtract(insert);
				}
				finally
				{
					ReleaseStatement(insert, session);
				}
			}
			catch (DbException sqle)
			{
				throw ADOExceptionHelper.Convert(sqle, "could not insert: " + MessageHelper.InfoString(persister), insertSQL);
			}
		}

		#endregion

		protected internal virtual void ReleaseStatement(IDbCommand insert, ISessionImplementor session)
		{
			session.Batcher.CloseCommand(insert, null);
		}

		protected internal abstract IDbCommand Prepare(SqlString insertSQL, ISessionImplementor session);

		public abstract object ExecuteAndExtract(IDbCommand insert);

	}
}
