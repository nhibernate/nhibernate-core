﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


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
	using System.Threading.Tasks;
	using System.Threading;
	public partial class DbTimestampType : AbstractDateTimeType
	{

		/// <inheritdoc />
		public override async Task<object> SeedAsync(ISessionImplementor session, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
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
			return await (GetCurrentTimestampAsync(session, cancellationToken)).ConfigureAwait(false);
		}

		/// <summary>
		/// Retrieves the current timestamp in database.
		/// </summary>
		/// <param name="session">The session to use for retrieving the timestamp.</param>
		/// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
		/// <returns>A datetime.</returns>
		protected virtual async Task<DateTime> GetCurrentTimestampAsync(ISessionImplementor session, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			var dialect = session.Factory.Dialect;
			// Need to round notably for Sql Server DateTime with Odbc, which has a 3.33ms resolution,
			// causing stale data update failure 2/3 of times if not rounded to 10ms.
			return Round(
				await (UsePreparedStatementAsync(GetCurrentTimestampSelectString(dialect), session, cancellationToken)).ConfigureAwait(false),
				dialect.TimestampResolutionInTicks);
		}

		protected virtual async Task<DateTime> UsePreparedStatementAsync(string timestampSelectString, ISessionImplementor session, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			var tsSelect = new SqlString(timestampSelectString);
			DbCommand ps = null;
			DbDataReader rs = null;
			using (session.BeginProcess())
			{
				try
				{
					ps = await (session.Batcher.PrepareCommandAsync(CommandType.Text, tsSelect, EmptyParams, cancellationToken)).ConfigureAwait(false);
					rs = await (session.Batcher.ExecuteReaderAsync(ps, cancellationToken)).ConfigureAwait(false);
					await (rs.ReadAsync(cancellationToken)).ConfigureAwait(false);
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
