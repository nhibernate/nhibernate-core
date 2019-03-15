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
			if (!SupportsCurrentTimestampSelection(session.Factory.Dialect))
			{
				log.Info("falling back to application host based timestamp, as dialect does not support current timestamp selection");
				return base.Seed(session);
			}
			return GetCurrentTimestamp(session);
		}

		/// <summary>
		/// Indicates if the dialect support the adequate timestamp selection.
		/// </summary>
		/// <param name="dialect">The dialect to test.</param>
		/// <returns><see langword="true" /> if the dialect supports selecting the adequate timestamp,
		/// <see langword="false" /> otherwise.</returns>
		protected virtual bool SupportsCurrentTimestampSelection(Dialect.Dialect dialect)
		{
			return dialect.SupportsCurrentTimestampSelection;
		}

		/// <summary>
		/// Retrieves the current timestamp in database.
		/// </summary>
		/// <param name="session">The session to use for retrieving the timestamp.</param>
		/// <returns>A datetime.</returns>
		protected virtual DateTime GetCurrentTimestamp(ISessionImplementor session)
		{
			var dialect = session.Factory.Dialect;
			// Need to round notably for Sql Server DateTime with Odbc, which has a 3.33ms resolution,
			// causing stale data update failure 2/3 of times if not rounded to 10ms.
			return Round(
				UsePreparedStatement(GetCurrentTimestampSelectString(dialect), session),
				dialect.TimestampResolutionInTicks);
		}

		/// <summary>
		/// Gets the timestamp selection query.
		/// </summary>
		/// <param name="dialect">The dialect for which retrieving the timestamp selection query.</param>
		/// <returns>A SQL query.</returns>
		protected virtual string GetCurrentTimestampSelectString(Dialect.Dialect dialect)
		{
			return dialect.CurrentTimestampSelectString;
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
					log.Debug("current timestamp retrieved from db : {0} (ticks={1})", ts, ts.Ticks);
					return AdjustDateTime(ts);
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
