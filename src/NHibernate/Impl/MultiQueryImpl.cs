using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NHibernate.Cache;
using NHibernate.Driver;
using NHibernate.Engine;
using NHibernate.Engine.Query.Sql;
using NHibernate.Exceptions;
using NHibernate.Hql;
using NHibernate.Loader.Custom;
using NHibernate.Loader.Custom.Sql;
using NHibernate.SqlCommand;
using NHibernate.Transform;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Impl
{
	// Since v5.2
	[Obsolete("Use Multi.QueryBatch instead, obtainable with ISession.CreateQueryBatch.")]
	public partial class MultiQueryImpl : IMultiQuery
	{
		private static readonly INHibernateLogger log = NHibernateLogger.For(typeof(MultiQueryImpl));

		private readonly List<IQuery> queries = new List<IQuery>();
		private readonly List<ITranslator> translators = new List<ITranslator>();
		private readonly List<int> translatorQueryMap = new List<int>();
		private readonly IList<System.Type> resultCollectionGenericType = new List<System.Type>();
		private readonly List<QueryParameters> parameters = new List<QueryParameters>();
		private IList queryResults;
		private readonly Dictionary<string, int> queryResultPositions = new Dictionary<string, int>();
		private string cacheRegion;
		private int? _timeout;
		private bool isCacheable;
		private readonly ISessionImplementor session;
		private IResultTransformer resultTransformer;
		private readonly Dialect.Dialect dialect;
		private bool forceCacheRefresh;
		private QueryParameters combinedParameters;
		private FlushMode flushMode = FlushMode.Unspecified;
		private FlushMode sessionFlushMode = FlushMode.Unspecified;
		private readonly IResultSetsCommand resultSetsCommand;

		public MultiQueryImpl(ISessionImplementor session)
		{
			IDriver driver = session.Factory.ConnectionProvider.Driver;
			dialect = session.Factory.Dialect;
			resultSetsCommand = driver.GetResultSetsCommand(session);
			this.session = session;
		}

		#region Parameters setting

		public IMultiQuery SetResultTransformer(IResultTransformer transformer)
		{
			resultTransformer = transformer;
			return this;
		}

		public IMultiQuery SetForceCacheRefresh(bool cacheRefresh)
		{
			forceCacheRefresh = cacheRefresh;
			return this;
		}

		public IMultiQuery SetTimeout(int timeout)
		{
			_timeout = timeout == RowSelection.NoValue ? (int?) null : timeout;
			return this;
		}

		public IMultiQuery SetParameter(string name, object val, IType type)
		{
			foreach (IQuery query in queries)
			{
				query.SetParameter(name, val, type);
			}
			return this;
		}

		public IMultiQuery SetParameter(string name, object val)
		{
			foreach (IQuery query in queries)
			{
				query.SetParameter(name, val);
			}
			return this;
		}

		public IMultiQuery SetParameterList(string name, IEnumerable vals, IType type)
		{
			foreach (IQuery query in queries)
			{
				query.SetParameterList(name, vals, type);
			}
			return this;
		}

		public IMultiQuery SetParameterList(string name, IEnumerable vals)
		{
			foreach (IQuery query in queries)
			{
				query.SetParameterList(name, vals);
			}
			return this;
		}

		public IMultiQuery SetAnsiString(string name, string val)
		{
			foreach (IQuery query in queries)
			{
				query.SetAnsiString(name, val);
			}
			return this;
		}

		public IMultiQuery SetBinary(string name, byte[] val)
		{
			foreach (IQuery query in queries)
			{
				query.SetBinary(name, val);
			}
			return this;
		}

		public IMultiQuery SetBoolean(string name, bool val)
		{
			foreach (IQuery query in queries)
			{
				query.SetBoolean(name, val);
			}
			return this;
		}

		public IMultiQuery SetByte(string name, byte val)
		{
			foreach (IQuery query in queries)
			{
				query.SetByte(name, val);
			}
			return this;
		}

		public IMultiQuery SetCharacter(string name, char val)
		{
			foreach (IQuery query in queries)
			{
				query.SetCharacter(name, val);
			}
			return this;
		}

		public IMultiQuery SetDateTime(string name, DateTime val)
		{
			foreach (IQuery query in queries)
			{
				query.SetDateTime(name, val);
			}
			return this;
		}

		public IMultiQuery SetDateTimeNoMs(string name, DateTime val)
		{
			foreach (var query in queries)
			{
				query.SetDateTimeNoMs(name, val);
			}
			return this;
		}

		// Since v5.0
		[Obsolete("Use SetDateTime instead, it uses DateTime2 with dialects supporting it.")]
		public IMultiQuery SetDateTime2(string name, DateTime val)
		{
			foreach (IQuery query in queries)
			{
				query.SetParameter(name, val, NHibernateUtil.DateTime2);
			}
			return this;
		}

		public IMultiQuery SetTimeSpan(string name, TimeSpan val)
		{
			foreach (IQuery query in queries)
			{
				query.SetParameter(name, val, NHibernateUtil.TimeSpan);
			}
			return this;
		}

		public IMultiQuery SetTimeAsTimeSpan(string name, TimeSpan val)
		{
			foreach (IQuery query in queries)
			{
				query.SetParameter(name, val, NHibernateUtil.TimeAsTimeSpan);
			}
			return this;
		}

		public IMultiQuery SetDateTimeOffset(string name, DateTimeOffset val)
		{
			foreach (IQuery query in queries)
			{
				query.SetParameter(name, val, NHibernateUtil.DateTimeOffset);
			}
			return this;
		}

		public IMultiQuery SetDecimal(string name, decimal val)
		{
			foreach (IQuery query in queries)
			{
				query.SetDecimal(name, val);
			}
			return this;
		}

		public IMultiQuery SetDouble(string name, double val)
		{
			foreach (IQuery query in queries)
			{
				query.SetDouble(name, val);
			}
			return this;
		}

		public IMultiQuery SetEntity(string name, object val)
		{
			foreach (IQuery query in queries)
			{
				query.SetEntity(name, val);
			}
			return this;
		}

		public IMultiQuery SetEnum(string name, Enum val)
		{
			foreach (IQuery query in queries)
			{
				query.SetEnum(name, val);
			}
			return this;
		}

		public IMultiQuery SetInt16(string name, short val)
		{
			foreach (IQuery query in queries)
			{
				query.SetInt16(name, val);
			}
			return this;
		}

		public IMultiQuery SetInt32(string name, int val)
		{
			foreach (IQuery query in queries)
			{
				query.SetInt32(name, val);
			}
			return this;
		}

		public IMultiQuery SetInt64(string name, long val)
		{
			foreach (IQuery query in queries)
			{
				query.SetInt64(name, val);
			}
			return this;
		}

		public IMultiQuery SetSingle(string name, float val)
		{
			foreach (IQuery query in queries)
			{
				query.SetSingle(name, val);
			}
			return this;
		}

		public IMultiQuery SetString(string name, string val)
		{
			foreach (IQuery query in queries)
			{
				query.SetString(name, val);
			}
			return this;
		}

		public IMultiQuery SetGuid(string name, Guid val)
		{
			foreach (IQuery query in queries)
			{
				query.SetGuid(name, val);
			}
			return this;
		}

		public IMultiQuery SetTime(string name, DateTime val)
		{
			foreach (IQuery query in queries)
			{
				query.SetTime(name, val);
			}
			return this;
		}

		// Since v5.0
		[Obsolete("Use SetDateTime instead.")]
		public IMultiQuery SetTimestamp(string name, DateTime val)
		{
			foreach (IQuery query in queries)
			{
				query.SetTimestamp(name, val);
			}
			return this;
		}

		#endregion

		public IMultiQuery AddNamedQuery<T>(string key, string namedQuery)
		{
			ThrowIfKeyAlreadyExists(key);
			return Add<T>(key, session.GetNamedQuery(namedQuery));
		}

		public IMultiQuery Add(System.Type resultGenericListType, IQuery query)
		{
			AddQueryForLaterExecutionAndReturnIndexOfQuery(resultGenericListType, query);

			return this;
		}

		public IMultiQuery Add(string key, IQuery query)
		{
			return Add<object>(key, query);
		}

		public IMultiQuery Add(IQuery query)
		{
			return Add<object>(query);
		}

		public IMultiQuery Add(string key, string hql)
		{
			return Add<object>(key, hql);
		}

		public IMultiQuery Add(string hql)
		{
			return Add<object>(hql);
		}

		public IMultiQuery AddNamedQuery(string queryName)
		{
			return AddNamedQuery<object>(queryName);
		}

		public IMultiQuery AddNamedQuery(string key, string namedQuery)
		{
			return AddNamedQuery<object>(key, namedQuery);
		}

		public IMultiQuery Add<T>(IQuery query)
		{
			AddQueryForLaterExecutionAndReturnIndexOfQuery(typeof(T), query);
			return this;
		}

		public IMultiQuery Add<T>(string key, IQuery query)
		{
			ThrowIfKeyAlreadyExists(key);
			queryResultPositions.Add(key, AddQueryForLaterExecutionAndReturnIndexOfQuery(typeof(T), query));
			return this;
		}

		public IMultiQuery Add<T>(string hql)
		{
			return Add<T>(((ISession)session).CreateQuery(hql));
		}

		public IMultiQuery Add<T>(string key, string hql)
		{
			ThrowIfKeyAlreadyExists(key);
			return Add<T>(key, ((ISession)session).CreateQuery(hql));
		}

		public IMultiQuery AddNamedQuery<T>(string queryName)
		{
			return Add<T>(session.GetNamedQuery(queryName));
		}

		public IMultiQuery SetCacheable(bool cacheable)
		{
			isCacheable = cacheable;
			return this;
		}

		public IMultiQuery SetCacheRegion(string region)
		{
			cacheRegion = region;
			return this;
		}

		/// <summary>
		/// Return the query results of all the queries
		/// </summary>
		public IList List()
		{
			using (session.BeginProcess())
			{
				bool cacheable = session.Factory.Settings.IsQueryCacheEnabled && isCacheable;
				combinedParameters = CreateCombinedQueryParameters();

				if (log.IsDebugEnabled())
				{
					log.Debug("Multi query with {0} queries.", queries.Count);
					for (int i = 0; i < queries.Count; i++)
					{
						log.Debug("Query #{0}: {1}", i, queries[i]);
					}
				}

				try
				{
					Before();

					var querySpaces = new HashSet<string>(Translators.SelectMany(t => t.QuerySpaces));
					if (resultSetsCommand.HasQueries)
					{
						session.AutoFlushIfRequired(querySpaces);
					}

					return cacheable ? ListUsingQueryCache(querySpaces) : ListIgnoreQueryCache();
				}
				finally
				{
					After();
				}
			}
		}

		public IMultiQuery SetFlushMode(FlushMode mode)
		{
			flushMode = mode;
			return this;
		}

		protected void Before()
		{
			if (flushMode != FlushMode.Unspecified)
			{
				sessionFlushMode = session.FlushMode;
				session.FlushMode = flushMode;
			}
		}

		protected void After()
		{
			if (sessionFlushMode != FlushMode.Unspecified)
			{
				session.FlushMode = sessionFlushMode;
				sessionFlushMode = FlushMode.Unspecified;
			}
		}

		protected virtual IList GetResultList(IList results)
		{
			var rawResultCollections = new List<IList>(resultCollectionGenericType.Count);
			for (var i = 0; i < queries.Count; i++)
			{
				var query = queries[i] as ExpressionQueryImpl;
				// Linq queries may override the query type, finishing the work with a post execute transformer,
				// which with multi queries are executed through the multi-query result trasformer.
				var rawElementType = query?.QueryExpression?.Type ?? resultCollectionGenericType[i];
				var resultList = rawElementType == typeof(object)
					? new List<object>()
					: (IList) Activator.CreateInstance(typeof(List<>).MakeGenericType(rawElementType));
				rawResultCollections.Add(resultList);
			}

			for (var i = 0; i < results.Count; i++)
			{
				// First use the transformer of each query transforming each row and then the list
				// DONE: The behavior when the query has a 'new' instead a transformer is delegated to the Loader
				var resultList = translators[i].Loader.GetResultList((IList)results[i], Parameters[i].ResultTransformer);

				var queryIndex = translatorQueryMap[i];
				ArrayHelper.AddAll(rawResultCollections[queryIndex], resultList);
			}

			var resultCollections = new List<object>(resultCollectionGenericType.Count);
			for (var i = 0; i < queries.Count; i++)
			{
				// Once polymorpic queries aggregated in one result per query (previous loop), use the
				// MultiQueryTransformer using, as source, the aggregated result.
				var resultList = GetTransformedResults(rawResultCollections[i]);
				resultCollections.Add(resultList);
			}

			return resultCollections;
		}

		private  IList GetTransformedResults(IList source)
		{
			if (resultTransformer == null)
				return source;

			//MultiCriteria does not call TransformTuple here
			for (var j = 0; j < source.Count; j++)
			{
				var row = source[j] as object[] ?? new[] {source[j]};
				source[j] = resultTransformer.TransformTuple(row, null);
			}

			return resultTransformer.TransformList(source);
		}

		protected List<object> DoList()
		{
			Stopwatch stopWatch = null;
			if (session.Factory.Statistics.IsStatisticsEnabled)
			{
				stopWatch = Stopwatch.StartNew();
			}
			int rowCount = 0;

			var results = new List<object>();

			var hydratedObjects = new List<object>[Translators.Count];
			List<EntityKey[]>[] subselectResultKeys = new List<EntityKey[]>[Translators.Count];
			bool[] createSubselects = new bool[Translators.Count];
			var cacheBatcher = new CacheBatcher(session);

			try
			{
				using (var reader = resultSetsCommand.GetReader(_timeout))
				{
					if (log.IsDebugEnabled())
					{
						log.Debug("Executing {0} queries", translators.Count);
					}
					for (int i = 0; i < translators.Count; i++)
					{
						ITranslator translator = Translators[i];
						QueryParameters parameter = Parameters[i];

						int entitySpan = translator.Loader.EntityPersisters.Length;
						hydratedObjects[i] = entitySpan > 0 ? new List<object>() : null;
						RowSelection selection = parameter.RowSelection;
						int maxRows = Loader.Loader.HasMaxRows(selection) ? selection.MaxRows : int.MaxValue;
						if (!dialect.SupportsLimitOffset || !translator.Loader.UseLimit(selection, dialect))
						{
							Loader.Loader.Advance(reader, selection);
						}

						if (parameter.HasAutoDiscoverScalarTypes)
						{
							translator.Loader.AutoDiscoverTypes(reader, parameter, null);
						}

						LockMode[] lockModeArray = translator.Loader.GetLockModes(parameter.LockModes);
						EntityKey optionalObjectKey = Loader.Loader.GetOptionalObjectKey(parameter, session);

						createSubselects[i] = translator.Loader.IsSubselectLoadingEnabled;
						subselectResultKeys[i] = createSubselects[i] ? new List<EntityKey[]>() : null;

						translator.Loader.HandleEmptyCollections(parameter.CollectionKeys, reader, session);
						EntityKey[] keys = new EntityKey[entitySpan]; // we can reuse it each time

						if (log.IsDebugEnabled())
						{
							log.Debug("processing result set");
						}

						IList tempResults = new List<object>();
						int count;
						for (count = 0; count < maxRows && reader.Read(); count++)
						{
							if (log.IsDebugEnabled())
							{
								log.Debug("result set row: {0}", count);
							}

							rowCount++;
							object result = translator.Loader.GetRowFromResultSet(
								reader, session, parameter, lockModeArray, optionalObjectKey, hydratedObjects[i], keys, true, null, null,
								(persister, data) => cacheBatcher.AddToBatch(persister, data));
							tempResults.Add(result);

							if (createSubselects[i])
							{
								subselectResultKeys[i].Add(keys);
								keys = new EntityKey[entitySpan]; //can't reuse in this case
							}
						}

						if (log.IsDebugEnabled())
						{
							log.Debug("done processing result set ({0} rows)", count);
						}

						results.Add(tempResults);

						if (log.IsDebugEnabled())
						{
							log.Debug("Query {0} returned {1} results", i, tempResults.Count);
						}

						reader.NextResult();
					}

					for (int i = 0; i < translators.Count; i++)
					{
						ITranslator translator = translators[i];
						QueryParameters parameter = parameters[i];

						translator.Loader.InitializeEntitiesAndCollections(hydratedObjects[i], reader, session, false, cacheBatcher);

						if (createSubselects[i])
						{
							translator.Loader.CreateSubselects(subselectResultKeys[i], parameter, session);
						}
					}

					cacheBatcher.ExecuteBatch();
				}
			}
			catch (Exception sqle)
			{
				log.Error(sqle, "Failed to execute multi query: [{0}]", resultSetsCommand.Sql);
				throw ADOExceptionHelper.Convert(session.Factory.SQLExceptionConverter, sqle, "Failed to execute multi query", resultSetsCommand.Sql);
			}

			if (stopWatch != null)
			{
				stopWatch.Stop();
				session.Factory.StatisticsImplementor.QueryExecuted(string.Format("{0} queries (MultiQuery)", translators.Count), rowCount, stopWatch.Elapsed);
			}
			return results;
		}

		protected SqlString SqlString
		{
			get
			{
				if (!resultSetsCommand.HasQueries)
					AggregateQueriesInformation();
				return resultSetsCommand.Sql;
			}
		}

		private void AggregateQueriesInformation()
		{
			int queryIndex = 0;
			foreach (AbstractQueryImpl query in queries)
			{
				query.VerifyParameters();
				QueryParameters queryParameters = query.GetQueryParameters();
				queryParameters.ValidateParameters();
				foreach (var translator in query.GetTranslators(session, queryParameters))
				{
					translators.Add(translator);
					translatorQueryMap.Add(queryIndex);
					parameters.Add(queryParameters);
					ISqlCommand singleCommand = translator.Loader.CreateSqlCommand(queryParameters, session);
					resultSetsCommand.Append(singleCommand);
				}
				queryIndex++;
			}
		}

		public object GetResult(string key)
		{
			if (queryResults == null)
			{
				queryResults = List();
			}

			int queryResultPosition;
			if (!queryResultPositions.TryGetValue(key, out queryResultPosition))
				throw new InvalidOperationException(String.Format("The key '{0}' is unknown", key));

			return queryResults[queryResultPosition];
		}

		public override string ToString()
		{
			return "Multi Query: [" + SqlString + "]";
		}

		#region Implementation

		private IList ListIgnoreQueryCache()
		{
			return GetResultList(DoList());
		}

		private IList ListUsingQueryCache(HashSet<string> querySpaces)
		{
			IQueryCache queryCache = session.Factory.GetQueryCache(cacheRegion);

			ISet<FilterKey> filterKeys = FilterKey.CreateFilterKeys(session.EnabledFilters);

			List<IType[]> resultTypesList = new List<IType[]>(Translators.Count);
			for (int i = 0; i < Translators.Count; i++)
			{
				ITranslator queryTranslator = Translators[i];
				resultTypesList.Add(queryTranslator.ReturnTypes);
			}
			int[] firstRows = new int[Parameters.Count];
			int[] maxRows = new int[Parameters.Count];
			for (int i = 0; i < Parameters.Count; i++)
			{
				RowSelection rowSelection = Parameters[i].RowSelection;
				firstRows[i] = rowSelection.FirstRow;
				maxRows[i] = rowSelection.MaxRows;
			}

			MultipleQueriesCacheAssembler assembler = new MultipleQueriesCacheAssembler(resultTypesList);

			QueryKey key = new QueryKey(session.Factory, SqlString, combinedParameters, filterKeys, null)
				.SetFirstRows(firstRows)
				.SetMaxRows(maxRows);

			IList result = assembler.GetResultFromQueryCache(session, combinedParameters, querySpaces, queryCache, key);

			if (result == null)
			{
				log.Debug("Cache miss for multi query");
				var list = DoList();
				queryCache.Put(key, combinedParameters, new ICacheAssembler[] { assembler }, new object[] { list }, session);
				result = list;
			}

			return GetResultList(result);
		}

		private IList<ITranslator> Translators
		{
			get
			{
				if (!resultSetsCommand.HasQueries)
				{
					AggregateQueriesInformation();
				}
				return translators;
			}
		}

		private QueryParameters CreateCombinedQueryParameters()
		{
			QueryParameters combinedQueryParameters = new QueryParameters();
			combinedQueryParameters.ForceCacheRefresh = forceCacheRefresh;
			combinedQueryParameters.NamedParameters = new Dictionary<string, TypedValue>();
			var positionalParameterTypes = new List<IType>();
			var positionalParameterValues = new List<object>();
			int index = 0;
			foreach (QueryParameters queryParameters in Parameters)
			{
				foreach (KeyValuePair<string, TypedValue> dictionaryEntry in queryParameters.NamedParameters)
				{
					combinedQueryParameters.NamedParameters.Add(dictionaryEntry.Key + "_" + index, dictionaryEntry.Value);
				}
				index += 1;
				positionalParameterTypes.AddRange(queryParameters.PositionalParameterTypes);
				positionalParameterValues.AddRange(queryParameters.PositionalParameterValues);
			}
			combinedQueryParameters.PositionalParameterTypes = positionalParameterTypes.ToArray();
			combinedQueryParameters.PositionalParameterValues = positionalParameterValues.ToArray();
			return combinedQueryParameters;
		}

		private IList<QueryParameters> Parameters
		{
			get
			{
				if (!resultSetsCommand.HasQueries)
					AggregateQueriesInformation();
				return parameters;
			}
		}

		private void ThrowIfKeyAlreadyExists(string key)
		{
			if (queryResultPositions.ContainsKey(key))
				throw new InvalidOperationException(String.Format("The key '{0}' already exists", key));
		}

		private int AddQueryForLaterExecutionAndReturnIndexOfQuery(System.Type resultGenericListType, IQuery query)
		{
			((AbstractQueryImpl)query).SetIgnoreUknownNamedParameters(true);
			queries.Add(query);
			resultCollectionGenericType.Add(resultGenericListType);
			return queries.Count - 1;
		}

		#endregion
	}

	public interface ITranslator
	{
		Loader.Loader Loader { get; }
		IType[] ReturnTypes { get; }
		string[] ReturnAliases { get; }
		ICollection<string> QuerySpaces { get; }
	}

	internal class HqlTranslatorWrapper : ITranslator
	{
		private readonly IQueryTranslator innerTranslator;

		public HqlTranslatorWrapper(IQueryTranslator translator)
		{
			innerTranslator = translator;
		}

		public Loader.Loader Loader
		{
			get { return innerTranslator.Loader; }
		}

		public IType[] ReturnTypes
		{
			get { return innerTranslator.ActualReturnTypes; }
		}

		public ICollection<string> QuerySpaces
		{
			get { return innerTranslator.QuerySpaces; }
		}

		public string[] ReturnAliases
		{
			get { return innerTranslator.ReturnAliases; }
		}
	}

	internal class SqlTranslator : ITranslator
	{
		private readonly CustomLoader loader;

		public SqlTranslator(ISQLQuery sqlQuery, ISessionFactoryImplementor sessionFactory)
		{
			var sqlQueryImpl = (SqlQueryImpl) sqlQuery;
			NativeSQLQuerySpecification sqlQuerySpec = sqlQueryImpl.GenerateQuerySpecification(sqlQueryImpl.NamedParams);
			var sqlCustomQuery = new SQLCustomQuery(sqlQuerySpec.SqlQueryReturns, sqlQuerySpec.QueryString, sqlQuerySpec.QuerySpaces, sessionFactory);
			loader = new CustomLoader(sqlCustomQuery, sessionFactory);
		}

		public IType[] ReturnTypes
		{
			get { return loader.ResultTypes; }
		}

		public Loader.Loader Loader
		{
			get { return loader; }
		}

		public ICollection<string> QuerySpaces
		{
			get { return loader.QuerySpaces; }
		}

		public string[] ReturnAliases
		{
			get { return loader.ReturnAliases; }
		}
	}
}
