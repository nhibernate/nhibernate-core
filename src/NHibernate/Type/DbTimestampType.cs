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
	/// <summary>
	/// When used as a version, gets seeded and incremented by querying the database's
	/// current timestamp, rather than the application host's current timestamp.
	/// </summary>
	[Serializable]
	public partial class DbTimestampType : AbstractDateTimeType
	{
		private static readonly INHibernateLogger log = NHibernateLogger.For(typeof(DbTimestampType));
		private static readonly SqlType[] EmptyParams = Array.Empty<SqlType>();

		/// <inheritdoc />
		public override string Name => "DbTimestamp";

		/// <inheritdoc />
		public override object Seed(ISessionImplementor session)
		{
			if (session == null)
			{
				log.Debug("incoming session was null; using current application host time");
				return base.Seed(null);
			}
			if (!session.Factory.Dialect.SupportsCurrentTimestampSelection)
			{
				log.Info("falling back to application host based timestamp, as dialect does not support current timestamp selection");
				return base.Seed(session);
			}
			return GetCurrentTimestamp(session);
		}

		protected virtual DateTime GetCurrentTimestamp(ISessionImplementor session)
		{
			var dialect = session.Factory.Dialect;
			// Need to round notably for Sql Server DateTime with Odbc, which has a 3.33ms resolution,
			// causing stale data update failure 2/3 of times if not rounded to 10ms.
			return Round(
				UsePreparedStatement(dialect.CurrentTimestampSelectString, session),
				dialect.TimestampResolutionInTicks);
		}

		protected virtual DateTime UsePreparedStatement(string timestampSelectString, ISessionImplementor session)
		{
			var tsSelect = new SqlString(timestampSelectString);
			DbCommand ps = null;
			DbDataReader rs = null;
			using (session.BeginProcess())
			{
				try
				{
					ps = session.Batcher.PrepareCommand(CommandType.Text, tsSelect, EmptyParams);
					rs = session.Batcher.ExecuteReader(ps);
					rs.Read();
					var ts = rs.GetDateTime(0);
					log.Debug("current timestamp retreived from db : {0} (ticks={1})", ts, ts.Ticks);
					return ts;
				}
				catch (DbException sqle)
				{
					throw ADOExceptionHelper.Convert(
						session.Factory.SQLExceptionConverter,
						sqle,
						"could not select current db timestamp",
						tsSelect);
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
							log.Warn(sqle, "unable to clean up prepared statement");
						}
					}
				}
			}
		}
	}
}
