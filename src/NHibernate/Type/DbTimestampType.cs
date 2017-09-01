using System;
using System.Data;
using System.Data.Common;

using NHibernate.Engine;
using NHibernate.Exceptions;
using NHibernate.Impl;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;

namespace NHibernate.Type
{
	/// <summary> An extension of <see cref="TimestampType"/> which
	/// maps to the database's current timestamp, rather than the vm's
	/// current timestamp.
	/// </summary>
	/// <remarks>
	/// Note: May/may-not cause issues on dialects which do not properly support
	/// a true notion of timestamp
	/// </remarks>
	[Serializable]
	public partial class DbTimestampType : TimestampType, IVersionType
	{
		private static readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof (DbTimestampType));
		private static readonly SqlType[] EmptyParams = new SqlType[0];

		public override string Name
		{
			get { return "DbTimestamp"; }
		}

		public override object Seed(ISessionImplementor session)
		{
			if (session == null)
			{
				log.Debug("incoming session was null; using current vm time");
				return base.Seed(session);
			}
			else if (!session.Factory.Dialect.SupportsCurrentTimestampSelection)
			{
				log.Debug("falling back to vm-based timestamp, as dialect does not support current timestamp selection");
				return base.Seed(session);
			}
			else
			{
				return GetCurrentTimestamp(session);
			}
		}

		private object GetCurrentTimestamp(ISessionImplementor session)
		{
			Dialect.Dialect dialect = session.Factory.Dialect;
			string timestampSelectString = dialect.CurrentTimestampSelectString;
			return UsePreparedStatement(timestampSelectString, session);
		}

		protected virtual object UsePreparedStatement(string timestampSelectString, ISessionImplementor session)
		{
			var tsSelect = new SqlString(timestampSelectString);
			DbCommand ps = null;
			DbDataReader rs = null;
			using (new SessionIdLoggingContext(session.SessionId)) 
			try
			{
				ps = session.Batcher.PrepareCommand(CommandType.Text, tsSelect, EmptyParams);
				rs = session.Batcher.ExecuteReader(ps);
				rs.Read();
				DateTime ts = rs.GetDateTime(0);
				if (log.IsDebugEnabled)
				{
					log.Debug("current timestamp retreived from db : " + ts + " (tiks=" + ts.Ticks + ")");
				}
				return ts;
			}
			catch (DbException sqle)
			{
				throw ADOExceptionHelper.Convert(session.Factory.SQLExceptionConverter, sqle,
				                                 "could not select current db timestamp", tsSelect);
			}
			finally
			{
				if (ps != null)
				{
					try
					{
						session.Batcher.CloseCommand(ps, rs);
					}
					catch (DbException sqle)
					{
						log.Warn("unable to clean up prepared statement", sqle);
					}
				}
			}
		}
	}
}