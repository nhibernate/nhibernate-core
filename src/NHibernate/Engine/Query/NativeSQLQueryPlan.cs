using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

using NHibernate.Action;
using NHibernate.Engine.Query.Sql;
using NHibernate.Event;
using NHibernate.Exceptions;
using NHibernate.Loader.Custom.Sql;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Engine.Query
{
	/// <summary> Defines a query execution plan for a native-SQL query. </summary>
	[Serializable]
	public class NativeSQLQueryPlan
	{
		private static readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof(NativeSQLQueryPlan));

		private readonly string sourceQuery;
		private readonly SQLCustomQuery customQuery;

		public NativeSQLQueryPlan(NativeSQLQuerySpecification specification, ISessionFactoryImplementor factory)
		{
			sourceQuery = specification.QueryString;
			customQuery = new SQLCustomQuery(specification.SqlQueryReturns, specification.QueryString, specification.QuerySpaces, factory);
		}

		public string SourceQuery
		{
			get { return sourceQuery; }
		}

		public SQLCustomQuery CustomQuery
		{
			get { return customQuery; }
		}

		private int[] GetNamedParameterLocs(string name)
		{
			object loc = customQuery.NamedParameterBindPoints[name];
			if (loc == null)
			{
				throw new QueryException("Named parameter does not appear in Query: " + name, customQuery.SQL.ToString());
			}
			if (Convert.GetTypeCode(loc) == TypeCode.Int32)
			{
				return new int[] {Convert.ToInt32(loc)};
			}
			else
			{
				return ArrayHelper.ToIntArray((IList)loc);
			}
		}

		private void CoordinateSharedCacheCleanup(ISessionImplementor session)
		{
			BulkOperationCleanupAction action = new BulkOperationCleanupAction(session, CustomQuery.QuerySpaces);

			action.Init();

			if (session.IsEventSource)
			{
				((IEventSource)session).ActionQueue.AddAction(action);
			}
		}

		// DONE : H3.2 Executable query (now can be supported for named SQL query/ storedProcedure)
		public int PerformExecuteUpdate(QueryParameters queryParameters, ISessionImplementor session)
		{
			CoordinateSharedCacheCleanup(session);

			if (queryParameters.Callable)
			{
				throw new ArgumentException("callable not yet supported for native queries");
			}

			RowSelection selection = queryParameters.RowSelection;

			int result;
			try
			{
				queryParameters.ProcessFilters(customQuery.SQL, session);
				SqlString sql = queryParameters.FilteredSQL;
				SqlType[] sqlTypes = queryParameters.PrepareParameterTypes(sql, session.Factory, GetNamedParameterLocs, 0, false, false);
				
				IDbCommand ps = session.Batcher.PrepareCommand(CommandType.Text, sql, sqlTypes);

				try
				{
					if (selection != null && selection.Timeout != RowSelection.NoValue)
					{
						// NH Difference : set Timeout for native query
						ps.CommandTimeout = selection.Timeout;
					}
					// NH Different behavior:
					// The inital value is 0 (initialized to 1 in JAVA)
					// The responsibility of parameter binding was entirely moved to QueryParameters
					// to deal with positionslParameter+NamedParameter+ParameterOfFilters
					
					queryParameters.BindParameters(ps, 0, session);
					result = session.Batcher.ExecuteNonQuery(ps);
				}
				finally
				{
					if (ps != null)
					{
						session.Batcher.CloseCommand(ps, null);
					}
				}
			}
			catch (HibernateException)
			{
				throw;
			}
			catch (Exception sqle)
			{
				throw ADOExceptionHelper.Convert(session.Factory.SQLExceptionConverter, sqle,
				                                 "could not execute native bulk manipulation query:" + sourceQuery);
			}

			return result;
		}
	}
}
