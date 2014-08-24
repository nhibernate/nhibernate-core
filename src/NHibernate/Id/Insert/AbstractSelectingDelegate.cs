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
	public abstract class AbstractSelectingDelegate : IInsertGeneratedIdentifierDelegate
	{
		private readonly IPostInsertIdentityPersister persister;

		protected internal AbstractSelectingDelegate(IPostInsertIdentityPersister persister)
		{
			this.persister = persister;
		}

		#region IInsertGeneratedIdentifierDelegate Members

		public abstract IdentifierGeneratingInsert PrepareIdentifierGeneratingInsert();

		public object PerformInsert(SqlCommandInfo insertSQL, ISessionImplementor session, IBinder binder)
		{
			try
			{
				// prepare and execute the insert
				IDbCommand insert = session.Batcher.PrepareCommand(insertSQL.CommandType, insertSQL.Text, insertSQL.ParameterTypes);
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
				                                 "could not insert: " + persister.GetInfoString(), insertSQL.Text);
			}

			SqlString selectSQL = SelectSQL;
			using (new SessionIdLoggingContext(session.SessionId)) 
			try
			{
				//fetch the generated id in a separate query
				IDbCommand idSelect = session.Batcher.PrepareCommand(CommandType.Text, selectSQL, ParametersTypes);
				try
				{
					BindParameters(session, idSelect, binder.Entity);
					IDataReader rs = session.Batcher.ExecuteReader(idSelect);
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
				                                 insertSQL.Text);
			}
		}

		#endregion

		/// <summary> Get the SQL statement to be used to retrieve generated key values. </summary>
		/// <returns> The SQL command string </returns>
		protected internal abstract SqlString SelectSQL { get; }

		/// <summary> Extract the generated key value from the given result set. </summary>
		/// <param name="session">The session </param>
		/// <param name="rs">The result set containing the generated primay key values. </param>
		/// <param name="entity">The entity being saved. </param>
		/// <returns> The generated identifier </returns>
		protected internal abstract object GetResult(ISessionImplementor session, IDataReader rs, object entity);

		/// <summary> Bind any required parameter values into the SQL command <see cref="SelectSQL"/>. </summary>
		/// <param name="session">The session </param>
		/// <param name="ps">The prepared <see cref="SelectSQL"/> command </param>
		/// <param name="entity">The entity being saved. </param>
		protected internal virtual void BindParameters(ISessionImplementor session, IDbCommand ps, object entity) { }

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