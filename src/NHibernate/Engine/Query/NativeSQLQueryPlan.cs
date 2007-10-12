using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using log4net;
using NHibernate.Action;
using NHibernate.Engine.Query.Sql;
using NHibernate.Event;
using NHibernate.Exceptions;
using NHibernate.Loader.Custom.Sql;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Engine.Query
{
	/// <summary> Defines a query execution plan for a native-SQL query. </summary>
	[Serializable]
	public class NativeSQLQueryPlan
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(NativeSQLQueryPlan));

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

		/// <summary> 
		/// Bind positional parameter values to the <tt>PreparedStatement</tt>
		/// (these are parameters specified by a JDBC-style ?).
		/// </summary>
		private int BindPositionalParameters(IDbCommand st, QueryParameters queryParameters, int start, ISessionImplementor session)
		{
			object[] values = queryParameters.FilteredPositionalParameterValues;
			IType[] types = queryParameters.FilteredPositionalParameterTypes;
			int span = 0;
			for (int i = 0; i < values.Length; i++)
			{
				types[i].NullSafeSet(st, values[i], start + span, session);
				span += types[i].GetColumnSpan(session.Factory);
			}
			return span;
		}

		/// <summary> 
		/// Bind named parameters to the <tt>PreparedStatement</tt>. This has an
		/// empty implementation on this superclass and should be implemented by
		/// subclasses (queries) which allow named parameters.
		/// </summary>
		private int BindNamedParameters(IDbCommand ps, IDictionary namedParams, int start, ISessionImplementor session)
		{
			if (namedParams != null)
			{
				// assumes that types are all of span 1
				int result = 0;
				foreach (DictionaryEntry param in namedParams)
				{
					string name = (string)param.Key;
					TypedValue typedval = (TypedValue)param.Value;

					int[] locs = GetNamedParameterLocs(name);
					for (int i = 0; i < locs.Length; i++)
					{
						if (log.IsDebugEnabled)
						{
							log.Debug(string.Format("BindNamedParameters() {0} -> {1} [{2}]", typedval.Value, name, (locs[i] + start)));
						}
						typedval.Type.NullSafeSet(ps, typedval.Value, locs[i] + start, session);
					}
					result += locs.Length;
				}
				return result;
			}
			else
			{
				return 0;
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

		// TODO : H3.2 Executable query (now can be supported for named SQL query/ storedProcedure)
		public int PerformExecuteUpdate(QueryParameters queryParameters, ISessionImplementor session)
		{
			CoordinateSharedCacheCleanup(session);

			if (queryParameters.Callable)
			{
				throw new ArgumentException("callable not yet supported for native queries");
			}

			int result = 0;
			//try
			//{
			//  queryParameters.ProcessFilters(customQuery.SQL, session);
			//  SqlString sql = queryParameters.FilteredSQL;

			//  IDbCommand ps = session.Batcher.PrepareCommand(CommandType.Text, sql);

			//  try
			//  {
			//    int col = 1;
			//    col += BindPositionalParameters(ps, queryParameters, col, session);
			//    BindNamedParameters(ps, queryParameters.NamedParameters, col, session);
			//    result =  TransactionManager.manager.ExecuteUpdate(ps);
			//  }
			//  finally
			//  {
			//    if (ps != null)
			//    {
			//      session.Batcher.CloseCommand(ps);
			//    }
			//  }
			//}
			//catch (HibernateException)
			//{
			//  throw;
			//}
			//catch (Exception sqle)
			//{
			//  throw ADOExceptionHelper.Convert(sqle, "could not execute native bulk manipulation query:"+ sourceQuery);
			//  //throw ADOExceptionHelper.Convert(session.Factory.SQLExceptionConverter, sqle, "could not execute native bulk manipulation query", this.sourceQuery);
			//}

			return result;
		}
	}
}
