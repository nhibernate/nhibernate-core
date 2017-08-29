using System.Data;
using System.Data.Common;
using NHibernate.Engine;
using NHibernate.Exceptions;
using NHibernate.Impl;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;

namespace NHibernate.Id.Insert
{
	/// <summary> 
	/// Abstract InsertGeneratedIdentifierDelegate implementation where the
	/// underlying strategy requires an subsequent select after the insert
	/// to determine the generated identifier. 
	/// </summary>
	public abstract partial class AbstractSelectingDelegate : IInsertGeneratedIdentifierDelegate
	{
		private readonly IPostInsertIdentityPersister persister;

		protected internal AbstractSelectingDelegate(IPostInsertIdentityPersister persister)
		{
			this.persister = persister;
		}

		#region IInsertGeneratedIdentifierDelegate Members

		public abstract IdentifierGeneratingInsert PrepareIdentifierGeneratingInsert();

		public object PerformInsert(SqlCommandInfo insertSql, ISessionImplementor session, IBinder binder)
		{
			// NH-2145: Prevent connection releases between insert and select when we cannot perform
			// them as a single statement. Retrieving id most of the time relies on using the same connection.
			session.ConnectionManager.FlushBeginning();
			try
			{
				try
				{
					// prepare and execute the insert
					var insert = session.Batcher.PrepareCommand(insertSql.CommandType, insertSql.Text, insertSql.ParameterTypes);
					try
					{
						binder.BindValues(insert);
						session.Batcher.ExecuteNonQuery(insert);
					}
					finally
					{
						session.Batcher.CloseCommand(insert, null);
					}
				}
				catch (DbException sqle)
				{
					throw ADOExceptionHelper.Convert(session.Factory.SQLExceptionConverter, sqle,
					                                 "could not insert: " + persister.GetInfoString(), insertSql.Text);
				}

				var selectSql = SelectSQL;
				using (new SessionIdLoggingContext(session.SessionId))
				{
					try
					{
						//fetch the generated id in a separate query
						var idSelect = session.Batcher.PrepareCommand(CommandType.Text, selectSql, ParametersTypes);
						try
						{
							BindParameters(session, idSelect, binder.Entity);
							var rs = session.Batcher.ExecuteReader(idSelect);
							try
							{
								return GetResult(session, rs, binder.Entity);
							}
							finally
							{
								session.Batcher.CloseReader(rs);
							}
						}
						finally
						{
							session.Batcher.CloseCommand(idSelect, null);
						}
					}
					catch (DbException sqle)
					{
						throw ADOExceptionHelper.Convert(session.Factory.SQLExceptionConverter, sqle,
						                                 "could not retrieve generated id after insert: " + persister.GetInfoString(),
						                                 insertSql.Text);
					}
				}
			}
			finally
			{
				session.ConnectionManager.FlushEnding();
			}
		}

		#endregion

		/// <summary> Get the SQL statement to be used to retrieve generated key values. </summary>
		/// <returns> The SQL command string </returns>
		protected internal abstract SqlString SelectSQL { get; }

		/// <summary> Extract the generated key value from the given result set. </summary>
		/// <param name="session">The session </param>
		/// <param name="rs">The result set containing the generated primary key values. </param>
		/// <param name="entity">The entity being saved. </param>
		/// <returns> The generated identifier </returns>
		protected internal abstract object GetResult(ISessionImplementor session, DbDataReader rs, object entity);

		/// <summary> Bind any required parameter values into the SQL command <see cref="SelectSQL"/>. </summary>
		/// <param name="session">The session </param>
		/// <param name="ps">The prepared <see cref="SelectSQL"/> command </param>
		/// <param name="entity">The entity being saved. </param>
		protected internal virtual void BindParameters(ISessionImplementor session, DbCommand ps, object entity) { }

		#region NH Specific

		/// <summary>
		/// Types of any required parameter values into the SQL command <see cref="SelectSQL"/>.
		/// </summary>
		protected internal virtual SqlType[] ParametersTypes
		{
			get { return SqlTypeFactory.NoTypes; }
		}

		#endregion
	}
}