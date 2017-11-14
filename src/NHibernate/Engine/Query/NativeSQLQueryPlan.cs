using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using NHibernate.Action;
using NHibernate.Engine.Query.Sql;
using NHibernate.Event;
using NHibernate.Exceptions;
using NHibernate.Hql;
using NHibernate.Impl;
using NHibernate.Loader.Custom.Sql;
using NHibernate.Param;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Engine.Query
{
	/// <summary> Defines a query execution plan for a native-SQL query. </summary>
	[Serializable]
	public partial class NativeSQLQueryPlan
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
				var parametersSpecifications = customQuery.CollectedParametersSpecifications.ToList();
				SqlString sql = ExpandDynamicFilterParameters(customQuery.SQL, parametersSpecifications, session);
				// After the last modification to the SqlString we can collect all parameters types.
				parametersSpecifications.ResetEffectiveExpectedType(queryParameters);

				var sqlParametersList = sql.GetParameters().ToList();
				SqlType[] sqlTypes = parametersSpecifications.GetQueryParameterTypes(sqlParametersList, session.Factory);
				
				var ps = session.Batcher.PrepareCommand(CommandType.Text, sql, sqlTypes);

				try
				{
					if (selection != null && selection.Timeout != RowSelection.NoValue)
					{
						// NH Difference : set Timeout for native query
						ps.CommandTimeout = selection.Timeout;
					}

					foreach (IParameterSpecification parameterSpecification in parametersSpecifications)
					{
						parameterSpecification.Bind(ps, sqlParametersList, queryParameters, session);
					}
					
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

		private SqlString ExpandDynamicFilterParameters(SqlString sqlString, ICollection<IParameterSpecification> parameterSpecs, ISessionImplementor session)
		{
			var enabledFilters = session.EnabledFilters;
			if (enabledFilters.Count == 0 || sqlString.ToString().IndexOf(ParserHelper.HqlVariablePrefix) < 0)
			{
				return sqlString;
			}

			Dialect.Dialect dialect = session.Factory.Dialect;
			string symbols = ParserHelper.HqlSeparators + dialect.OpenQuote + dialect.CloseQuote;

			var result = new SqlStringBuilder();
			foreach (var sqlPart in sqlString)
			{
				var parameter = sqlPart as Parameter;
				if (parameter != null)
				{
					result.Add(parameter);
					continue;
				}

				var sqlFragment = sqlPart.ToString();
				var tokens = new StringTokenizer(sqlFragment, symbols, true);

				foreach (string token in tokens)
				{
					if (token.StartsWith(ParserHelper.HqlVariablePrefix))
					{
						string filterParameterName = token.Substring(1);
						string[] parts = StringHelper.ParseFilterParameterName(filterParameterName);
						string filterName = parts[0];
						string parameterName = parts[1];
						var filter = (FilterImpl)enabledFilters[filterName];

						object value = filter.GetParameter(parameterName);
						IType type = filter.FilterDefinition.GetParameterType(parameterName);
						int parameterColumnSpan = type.GetColumnSpan(session.Factory);
						var collectionValue = value as ICollection;
						int? collectionSpan = null;

						// Add query chunk
						string typeBindFragment = string.Join(", ", Enumerable.Repeat("?", parameterColumnSpan).ToArray());
						string bindFragment;
						if (collectionValue != null && !type.ReturnedClass.IsArray)
						{
							collectionSpan = collectionValue.Count;
							bindFragment = string.Join(", ", Enumerable.Repeat(typeBindFragment, collectionValue.Count).ToArray());
						}
						else
						{
							bindFragment = typeBindFragment;
						}

						// dynamic-filter parameter tracking
						var filterParameterFragment = SqlString.Parse(bindFragment);
						var dynamicFilterParameterSpecification = new DynamicFilterParameterSpecification(filterName, parameterName, type, collectionSpan);
						var parameters = filterParameterFragment.GetParameters().ToArray();
						var sqlParameterPos = 0;
						var paramTrackers = dynamicFilterParameterSpecification.GetIdsForBackTrack(session.Factory);
						foreach (var paramTracker in paramTrackers)
						{
							parameters[sqlParameterPos++].BackTrack = paramTracker;
						}

						parameterSpecs.Add(dynamicFilterParameterSpecification);
						result.Add(filterParameterFragment);
					}
					else
					{
						result.Add(token);
					}
				}
			}
			return result.ToSqlString();
		}
	}
}
