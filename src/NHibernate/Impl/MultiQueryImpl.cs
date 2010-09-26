using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using Iesi.Collections;

using NHibernate.Cache;
using NHibernate.Driver;
using NHibernate.Engine;
using NHibernate.Hql;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;
using NHibernate.Transform;
using NHibernate.Type;
using Iesi.Collections.Generic;
using System.Collections.Generic;

namespace NHibernate.Impl
{
	public class MultiQueryImpl : IMultiQuery
	{
		private static readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof(MultiQueryImpl));

		private readonly List<IQuery> queries = new List<IQuery>();
		private readonly List<IQueryTranslator> translators = new List<IQueryTranslator>(); 
		private readonly IList<System.Type> resultCollectionGenericType = new List<System.Type>(); 
		private readonly List<QueryParameters> parameters = new List<QueryParameters>(); 
		private IList queryResults;
		private readonly Dictionary<string, int> queryResultPositions = new Dictionary<string, int>();
		private string cacheRegion;
		private int commandTimeout = RowSelection.NoValue;
		private bool isCacheable;
		private readonly ISessionImplementor session;
		private IResultTransformer resultTransformer;
		private readonly List<SqlType> types = new List<SqlType>();
		private SqlString sqlString;
		private readonly Dialect.Dialect dialect;
		private bool forceCacheRefresh;
		private QueryParameters combinedParameters;
		private FlushMode flushMode = FlushMode.Unspecified;
		private FlushMode sessionFlushMode = FlushMode.Unspecified;

		public MultiQueryImpl(ISessionImplementor session)
		{
			IDriver driver = session.Factory.ConnectionProvider.Driver;
			if (!driver.SupportsMultipleQueries)
			{
				throw new NotSupportedException(
					string.Format("The driver {0} does not support multiple queries.", driver.GetType().FullName));
			}
			dialect = session.Factory.Dialect;
			this.session = session;
		}

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
			commandTimeout = timeout;
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

		public IMultiQuery SetParameterList(string name, ICollection vals, IType type)
		{
			foreach (IQuery query in queries)
			{
				query.SetParameterList(name, vals, type);
			}
			return this;
		}

		public IMultiQuery SetParameterList(string name, ICollection vals)
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

		public IMultiQuery SetTimestamp(string name, DateTime val)
		{
			foreach (IQuery query in queries)
			{
				query.SetTimestamp(name, val);
			}
			return this;
		}

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
			using (new SessionIdLoggingContext(session.SessionId))
			{
				bool cacheable = session.Factory.Settings.IsQueryCacheEnabled && isCacheable;
				combinedParameters = CreateCombinedQueryParameters();

				if (log.IsDebugEnabled)
				{
					log.DebugFormat("Multi query with {0} queries.", queries.Count);
					for (int i = 0; i < queries.Count; i++)
					{
						log.DebugFormat("Query #{0}: {1}", i, queries[i]);
					}
				}

				try
				{
					Before();
					return cacheable ? ListUsingQueryCache() : ListIgnoreQueryCache();
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
			var multiqueryHolderInstatiator = GetMultiQueryHolderInstatiator();
			int len = results.Count;
			for (int i = 0; i < len; ++i)
			{
				// First use the transformer of each query transformig each row and then the list
				results[i] = GetTransformedResults((IList)results[i], GetQueryHolderInstantiator(i));
				// then use the MultiQueryTransformer (if it has some sense...) using, as source, the transformed result.
				results[i] = GetTransformedResults((IList)results[i], multiqueryHolderInstatiator);
			}
			return results;
		}

		private IList GetTransformedResults(IList source, HolderInstantiator holderInstantiator)
		{
			if (!holderInstantiator.IsRequired)
			{
				return source;
			}
			for (int j = 0; j < source.Count; j++)
			{
				object[] row = source[j] as object[] ?? new[] { source[j] };
				source[j] = holderInstantiator.Instantiate(row);
			}

			return holderInstantiator.ResultTransformer.TransformList(source);
		}

		private HolderInstantiator GetQueryHolderInstantiator(int queryPosition)
		{
			// TODO : we need a test to check the behavior when the query has a 'new' istead a trasformer
			// we should take the HolderInstantiator directly from QueryTranslator... taking care with Parameters.
			return Parameters[queryPosition].ResultTransformer != null ? 
				new HolderInstantiator(Parameters[queryPosition].ResultTransformer, translators[queryPosition].ReturnAliases) 
				: HolderInstantiator.NoopInstantiator;
		}

		private HolderInstantiator GetMultiQueryHolderInstatiator()
		{
			return HasMultiQueryResultTrasformer() ? new HolderInstantiator(resultTransformer, null) : HolderInstantiator.NoopInstantiator;
		}

		private bool HasMultiQueryResultTrasformer()
		{
			return resultTransformer != null;
		}

		protected ArrayList DoList()
		{
			bool statsEnabled = session.Factory.Statistics.IsStatisticsEnabled;
			var stopWatch = new Stopwatch();
			if (statsEnabled)
			{
				stopWatch.Start();
			}
			int rowCount = 0;

			IDbCommand command = PrepareQueriesCommand();

			BindParameters(command);

			ArrayList results = new ArrayList();

			log.Info(command.CommandText);
			if (commandTimeout != RowSelection.NoValue)
				command.CommandTimeout = commandTimeout;
			ArrayList[] hydratedObjects = new ArrayList[Translators.Count];
			List<EntityKey[]>[] subselectResultKeys = new List<EntityKey[]>[Translators.Count];
			bool[] createSubselects = new bool[Translators.Count];
			IDataReader reader = session.Batcher.ExecuteReader(command);
			try
			{
				if (log.IsDebugEnabled)
					log.DebugFormat("Executing {0} queries", translators.Count);
				for (int i = 0; i < translators.Count; i++)
				{
					IQueryTranslator translator = Translators[i];
					QueryParameters parameter = Parameters[i];
					IList tempResults;
					if (resultCollectionGenericType[i] == typeof(object))
					{
						tempResults = new ArrayList();
					}
					else
					{
						tempResults = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(resultCollectionGenericType[i]));
					}
					int entitySpan = translator.Loader.EntityPersisters.Length;
					hydratedObjects[i] = entitySpan > 0 ? new ArrayList() : null;
					RowSelection selection = parameter.RowSelection;
					int maxRows = Loader.Loader.HasMaxRows(selection) ? selection.MaxRows : int.MaxValue;
					if (!dialect.SupportsLimitOffset || !Loader.Loader.UseLimit(selection, dialect))
					{
						Loader.Loader.Advance(reader, selection);
					}

					LockMode[] lockModeArray = translator.Loader.GetLockModes(parameter.LockModes);
					EntityKey optionalObjectKey = Loader.Loader.GetOptionalObjectKey(parameter, session);

					createSubselects[i] = translator.Loader.IsSubselectLoadingEnabled;
					subselectResultKeys[i] = createSubselects[i] ? new List<EntityKey[]>() : null;

					translator.Loader.HandleEmptyCollections(parameter.CollectionKeys, reader, session);
					EntityKey[] keys = new EntityKey[entitySpan]; // we can reuse it each time

					if (log.IsDebugEnabled)
					{
						log.Debug("processing result set");
					}

					int count;
					for (count = 0; count < maxRows && reader.Read(); count++)
					{
						if (log.IsDebugEnabled)
						{
							log.Debug("result set row: " + count);
						}

						rowCount++;

						object result =
							translator.Loader.GetRowFromResultSet(reader,
														   session,
														   parameter,
														   lockModeArray,
														   optionalObjectKey,
														   hydratedObjects[i],
														   keys,
														   true);

						tempResults.Add(result);

						if (createSubselects[i])
						{
							subselectResultKeys[i].Add(keys);
							keys = new EntityKey[entitySpan]; //can't reuse in this case
						}
					}

					if (log.IsDebugEnabled)
					{
						log.Debug(string.Format("done processing result set ({0} rows)", count));
					}

					results.Add(tempResults);

					if (log.IsDebugEnabled)
					{
						log.DebugFormat("Query {0} returned {1} results", i, tempResults.Count);
					}

					reader.NextResult();
				}
			}
			catch (Exception ex)
			{
				log.Error("Failed to execute multi query: [" + command.CommandText + "]", ex);
				throw new HibernateException("Failed to execute multi query: [" + command.CommandText + "]", ex);
			}
			finally
			{
				session.Batcher.CloseCommand(command, reader);
			}
			for (int i = 0; i < translators.Count; i++)
			{
				IQueryTranslator translator = translators[i];
				QueryParameters parameter = parameters[i];

				translator.Loader.InitializeEntitiesAndCollections(hydratedObjects[i], reader, session, false);

				if (createSubselects[i])
				{
					translator.Loader.CreateSubselects(subselectResultKeys[i], parameter, session);
				}
			}
			if (statsEnabled)
			{
				stopWatch.Stop();
				session.Factory.StatisticsImplementor.QueryExecuted(string.Format("{0} queries (MultiQuery)", translators.Count), rowCount, stopWatch.Elapsed);
			}
			return results;
		}

		private IDbCommand PrepareQueriesCommand()
		{
			SqlType[] sqlTypes = types.ToArray();
			return session.Batcher.PrepareQueryCommand(CommandType.Text, SqlString, sqlTypes);
		}

		protected SqlString SqlString
		{
			get
			{
				if (sqlString == null)
					AggregateQueriesInformation();
				return sqlString;
			}
		}

		private void AggregateQueriesInformation()
		{
			sqlString = new SqlString();
			foreach (AbstractQueryImpl query in queries)
			{
				QueryParameters queryParameters = query.GetQueryParameters();
				queryParameters.ValidateParameters();
				query.VerifyParameters();
				IQueryTranslator[] queryTranslators =
					session.GetQueries(query.ExpandParameterLists(queryParameters.NamedParameters), false);
				foreach (IQueryTranslator translator in queryTranslators)
				{
					translators.Add(translator);
					parameters.Add(queryParameters);
					queryParameters = GetFilteredQueryParameters(queryParameters, translator);
					SqlCommandInfo commandInfo = translator.Loader.GetQueryStringAndTypes(session, queryParameters, types.Count);
					sqlString = sqlString.Append(commandInfo.Text).Append(session.Factory.ConnectionProvider.Driver.MultipleQueriesSeparator).Append(Environment.NewLine);
					types.AddRange(commandInfo.ParameterTypes);
				}
			}
		}

		private static QueryParameters GetFilteredQueryParameters(QueryParameters queryParameters, IQueryTranslator translator)
		{
			QueryParameters filteredQueryParameters = queryParameters;
			Dictionary<string, TypedValue> namedParameters = new Dictionary<string, TypedValue>(queryParameters.NamedParameters);
			filteredQueryParameters.NamedParameters.Clear();
			foreach (string paramName in translator.GetParameterTranslations().GetNamedParameterNames())
			{
				TypedValue v;
				if (namedParameters.TryGetValue(paramName, out v))
				{
					filteredQueryParameters.NamedParameters.Add(paramName, v);
				}
			}
			return filteredQueryParameters;
		}

		private void BindParameters(IDbCommand command)
		{
			int colIndex = 0;

			for (int queryIndex = 0; queryIndex < queries.Count; queryIndex++)
			{
				int limitParameterSpan = BindLimitParametersFirstIfNeccesary(command, queryIndex, colIndex);
				colIndex = BindQueryParameters(command, queryIndex, colIndex + limitParameterSpan);
				BindLimitParametersLastIfNeccesary(command, queryIndex, colIndex);
			}
		}

		private void BindLimitParametersLastIfNeccesary(IDbCommand command, int queryIndex, int colIndex)
		{
			QueryParameters parameter = parameters[queryIndex];
			RowSelection selection = parameter.RowSelection;
			if (Loader.Loader.UseLimit(selection, dialect) && !dialect.BindLimitParametersFirst)
			{
				Loader.Loader.BindLimitParameters(command, colIndex, selection, session);
			}
		}

		private int BindQueryParameters(IDbCommand command, int queryIndex, int colIndex)
		{
			QueryParameters parameter = Parameters[queryIndex];
			colIndex += parameter.BindParameters(command, colIndex, session);
			return colIndex;
		}

		public object GetResult(string key)
		{
			if (queryResults == null)
			{
				queryResults = List();
			}

			if (!queryResultPositions.ContainsKey(key))
			{
				throw new InvalidOperationException(String.Format("The key '{0}' is unknown", key));
			}

			return queryResults[queryResultPositions[key]];
		}

		private int BindLimitParametersFirstIfNeccesary(IDbCommand command, int queryIndex, int colIndex)
		{
			int limitParameterSpan = 0;
			QueryParameters parameter = Parameters[queryIndex];
			RowSelection selection = parameter.RowSelection;
			if (Loader.Loader.UseLimit(selection, dialect) && dialect.BindLimitParametersFirst)
			{
				limitParameterSpan += Loader.Loader.BindLimitParameters(command, colIndex, selection, session);
			}
			return limitParameterSpan;
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

		private IList ListUsingQueryCache()
		{
			IQueryCache queryCache = session.Factory.GetQueryCache(cacheRegion);

			ISet filterKeys = FilterKey.CreateFilterKeys(session.EnabledFilters, session.EntityMode);

			ISet<string> querySpaces = new HashedSet<string>();
			List<IType[]> resultTypesList = new List<IType[]>(Translators.Count);
			for (int i = 0; i < Translators.Count; i++)
			{
				IQueryTranslator queryTranslator = Translators[i];
				querySpaces.AddAll(queryTranslator.QuerySpaces);
				resultTypesList.Add(queryTranslator.ActualReturnTypes);
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

			QueryKey key = new QueryKey(session.Factory, SqlString, combinedParameters, filterKeys)
				.SetFirstRows(firstRows)
				.SetMaxRows(maxRows);

			IList result = assembler.GetResultFromQueryCache(session, combinedParameters, querySpaces, queryCache, key);

			if (result == null)
			{
				log.Debug("Cache miss for multi query");
				ArrayList list = DoList();
				queryCache.Put(key, new ICacheAssembler[] { assembler }, new object[] { list }, false, session);
				result = list;
			}

			return GetResultList(result);
		}

		private IList<IQueryTranslator> Translators
		{
			get
			{
				if (sqlString == null)
					AggregateQueriesInformation();
				return translators;
			}
		}

		private QueryParameters CreateCombinedQueryParameters()
		{
			QueryParameters combinedQueryParameters = new QueryParameters();
			combinedQueryParameters.ForceCacheRefresh = forceCacheRefresh;
			combinedQueryParameters.NamedParameters = new Dictionary<string, TypedValue>();
			ArrayList positionalParameterTypes = new ArrayList();
			ArrayList positionalParameterValues = new ArrayList();
			int index = 0;
			foreach (QueryParameters queryParameters in Parameters)
			{
				foreach (KeyValuePair<string, TypedValue> dictionaryEntry in queryParameters.NamedParameters)
				{
					combinedQueryParameters.NamedParameters.Add(dictionaryEntry.Key + index, dictionaryEntry.Value);
				}
				index += 1;
				positionalParameterTypes.AddRange(queryParameters.PositionalParameterTypes);
				positionalParameterValues.AddRange(queryParameters.PositionalParameterValues);
			}
			combinedQueryParameters.PositionalParameterTypes = (IType[])positionalParameterTypes.ToArray(typeof(IType));
			combinedQueryParameters.PositionalParameterValues = (object[])positionalParameterValues.ToArray(typeof(object));
			return combinedQueryParameters;
		}

		private IList<QueryParameters> Parameters
		{
			get
			{
				if (sqlString == null)
					AggregateQueriesInformation();
				return parameters;
			}
		}

		private void ThrowIfKeyAlreadyExists(string key)
		{
			if (queryResultPositions.ContainsKey(key))
			{
				throw new InvalidOperationException(String.Format("The key '{0}' already exists", key));
			}
		}

		private int AddQueryForLaterExecutionAndReturnIndexOfQuery(System.Type resultGenericListType, IQuery query)
		{
			ThrowNotSupportedIfSqlQuery(query);
			((AbstractQueryImpl)query).SetIgnoreUknownNamedParameters(true);
			queries.Add(query);
			resultCollectionGenericType.Add(resultGenericListType);
			return queries.Count - 1;
		}
		protected void ThrowNotSupportedIfSqlQuery(IQuery query)
		{
			if (query is ISQLQuery)
				throw new NotSupportedException("Sql queries in MultiQuery are currently not supported.");
		}

		#endregion
	}
}
