using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using NHibernate.Cache;
using NHibernate.Criterion;
using NHibernate.Driver;
using NHibernate.Engine;
using NHibernate.Exceptions;
using NHibernate.Loader.Criteria;
using NHibernate.SqlCommand;
using NHibernate.Transform;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Impl
{
	public class MultiCriteriaImpl : IMultiCriteria
	{
		private static readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof(MultiCriteriaImpl));
		private readonly IList<ICriteria> criteriaQueries = new List<ICriteria>();
		private readonly IList<System.Type> resultCollectionGenericType = new List<System.Type>();

		private readonly SessionImpl session;
		private readonly ISessionFactoryImplementor factory;
		private readonly List<CriteriaQueryTranslator> translators = new List<CriteriaQueryTranslator>();
		private readonly List<QueryParameters> parameters = new List<QueryParameters>();
		private readonly List<CriteriaLoader> loaders = new List<CriteriaLoader>();
		private readonly List<int> loaderCriteriaMap = new List<int>();
		private readonly Dialect.Dialect dialect;
		private IList criteriaResults;
		private readonly Dictionary<string, int> criteriaResultPositions = new Dictionary<string, int>();
		private bool isCacheable = false;
		private bool forceCacheRefresh = false;
		private string cacheRegion;
		private IResultTransformer resultTransformer;
		private readonly IResultSetsCommand resultSetsCommand;

		/// <summary>
		/// Initializes a new instance of the <see cref="MultiCriteriaImpl"/> class.
		/// </summary>
		/// <param name="session">The session.</param>
		/// <param name="factory">The factory.</param>
		internal MultiCriteriaImpl(SessionImpl session, ISessionFactoryImplementor factory)
		{
			IDriver driver = session.Factory.ConnectionProvider.Driver;
			dialect = session.Factory.Dialect;
			resultSetsCommand = driver.GetResultSetsCommand(session);
			this.session = session;
			this.factory = factory;
		}

		public SqlString SqlString
		{
			get
			{
				return resultSetsCommand.Sql;
			}
		}

		public IList List()
		{
			using (new SessionIdLoggingContext(session.SessionId))
			{
				bool cacheable = session.Factory.Settings.IsQueryCacheEnabled && isCacheable;

				CreateCriteriaLoaders();
				CombineCriteriaQueries();

				if (log.IsDebugEnabled)
				{
					log.DebugFormat("Multi criteria with {0} criteria queries.", criteriaQueries.Count);
					for (int i = 0; i < criteriaQueries.Count; i++)
					{
						log.DebugFormat("Query #{0}: {1}", i, criteriaQueries[i]);
					}
				}

				if (cacheable)
				{
					criteriaResults = ListUsingQueryCache();
				}
				else
				{
					criteriaResults = ListIgnoreQueryCache();
				}

				return criteriaResults;
			}
		}

		private IList ListUsingQueryCache()
		{
			IQueryCache queryCache = session.Factory.GetQueryCache(cacheRegion);

			ISet<FilterKey> filterKeys = FilterKey.CreateFilterKeys(session.EnabledFilters, session.EntityMode);

			ISet<string> querySpaces = new HashSet<string>();
			List<IType[]> resultTypesList = new List<IType[]>();
			int[] maxRows = new int[loaders.Count];
			int[] firstRows = new int[loaders.Count];
			for (int i = 0; i < loaders.Count; i++)
			{
				querySpaces.UnionWith(loaders[i].QuerySpaces);
				resultTypesList.Add(loaders[i].ResultTypes);
				firstRows[i] = parameters[i].RowSelection.FirstRow;
				maxRows[i] = parameters[i].RowSelection.MaxRows;
			}

			MultipleQueriesCacheAssembler assembler = new MultipleQueriesCacheAssembler(resultTypesList);
			QueryParameters combinedParameters = CreateCombinedQueryParameters();
			QueryKey key = new QueryKey(session.Factory, SqlString, combinedParameters, filterKeys, null)
				.SetFirstRows(firstRows)
				.SetMaxRows(maxRows);

			IList result =
				assembler.GetResultFromQueryCache(session,
												  combinedParameters,
												  querySpaces,
												  queryCache,
												  key);

			if (result == null)
			{
				log.Debug("Cache miss for multi criteria query");
				IList list = DoList();
				queryCache.Put(key, new ICacheAssembler[] { assembler }, new object[] { list }, combinedParameters.NaturalKeyLookup, session);
				result = list;
			}

			return GetResultList(result);
		}

		private IList ListIgnoreQueryCache()
		{
			return GetResultList(DoList());
		}

		protected virtual IList GetResultList(IList results)
		{
			var resultCollections = new List<object>(resultCollectionGenericType.Count);
			for (int i = 0; i < criteriaQueries.Count; i++)
			{
				if (resultCollectionGenericType[i] == typeof(object))
				{
					resultCollections.Add(new List<object>());
				}
				else
				{
					resultCollections.Add(Activator.CreateInstance(typeof(List<>).MakeGenericType(resultCollectionGenericType[i])));
				}
			}

			for (int i = 0; i < loaders.Count; i++)
			{
				CriteriaLoader loader = loaders[i];
				var resultList = loader.GetResultList((IList)results[i], parameters[i].ResultTransformer);
				var criteriaIndex = loaderCriteriaMap[i];
				ArrayHelper.AddAll((IList)resultCollections[criteriaIndex], resultList);
			}

			if (resultTransformer != null)
			{
				for (int i = 0; i < results.Count; i++)
				{
					resultCollections[i] = resultTransformer.TransformList((IList)resultCollections[i]);
				}
			}

			return resultCollections;
		}

		private IList DoList()
		{
			List<IList> results = new List<IList>();
			GetResultsFromDatabase(results);
			return results;
		}

		private void CombineCriteriaQueries()
		{
			foreach (CriteriaLoader loader in loaders)
			{
				CriteriaQueryTranslator translator = loader.Translator;
				translators.Add(translator);
				QueryParameters queryParameters = translator.GetQueryParameters();
				parameters.Add(queryParameters);
				ISqlCommand singleCommand = loader.CreateSqlCommand(queryParameters, session);
				resultSetsCommand.Append(singleCommand);
			}
		}

		private void GetResultsFromDatabase(IList results)
		{
			bool statsEnabled = session.Factory.Statistics.IsStatisticsEnabled;
			var stopWatch = new Stopwatch();
			if (statsEnabled)
			{
				stopWatch.Start();
			}
			int rowCount = 0;

			try
			{
				using (var reader = resultSetsCommand.GetReader(null))
				{
					var hydratedObjects = new List<object>[loaders.Count];
					List<EntityKey[]>[] subselectResultKeys = new List<EntityKey[]>[loaders.Count];
					bool[] createSubselects = new bool[loaders.Count];
					for (int i = 0; i < loaders.Count; i++)
					{
						CriteriaLoader loader = loaders[i];
						int entitySpan = loader.EntityPersisters.Length;
						hydratedObjects[i] = entitySpan == 0 ? null : new List<object>(entitySpan);
						EntityKey[] keys = new EntityKey[entitySpan];
						QueryParameters queryParameters = parameters[i];
						IList tmpResults = new List<object>();

						RowSelection selection = parameters[i].RowSelection;
						createSubselects[i] = loader.IsSubselectLoadingEnabled;
						subselectResultKeys[i] = createSubselects[i] ? new List<EntityKey[]>() : null;
						int maxRows = Loader.Loader.HasMaxRows(selection) ? selection.MaxRows : int.MaxValue;
						if (!dialect.SupportsLimitOffset || !loader.UseLimit(selection, dialect))
						{
							Loader.Loader.Advance(reader, selection);
						}
						int count;
						for (count = 0; count < maxRows && reader.Read(); count++)
						{
							rowCount++;

							object o =
								loader.GetRowFromResultSet(reader, session, queryParameters, loader.GetLockModes(queryParameters.LockModes),
																					 null, hydratedObjects[i], keys, true);
							if (createSubselects[i])
							{
								subselectResultKeys[i].Add(keys);
								keys = new EntityKey[entitySpan]; //can't reuse in this case
							}
							tmpResults.Add(o);
						}

						results.Add(tmpResults);
						reader.NextResult();
					}

					for (int i = 0; i < loaders.Count; i++)
					{
						CriteriaLoader loader = loaders[i];
						loader.InitializeEntitiesAndCollections(hydratedObjects[i], reader, session, false);

						if (createSubselects[i])
						{
							loader.CreateSubselects(subselectResultKeys[i], parameters[i], session);
						}
					}
				}
			}
			catch (Exception sqle)
			{
				var message = string.Format("Failed to execute multi criteria: [{0}]", resultSetsCommand.Sql);
				log.Error(message, sqle);
				throw ADOExceptionHelper.Convert(session.Factory.SQLExceptionConverter, sqle, "Failed to execute multi criteria", resultSetsCommand.Sql);
			}
			if (statsEnabled)
			{
				stopWatch.Stop();
				session.Factory.StatisticsImplementor.QueryExecuted(string.Format("{0} queries (MultiCriteria)", loaders.Count), rowCount, stopWatch.Elapsed);
			}
		}

		private void CreateCriteriaLoaders()
		{
			//a criteria can use more than a single query (polymorphic queries), need to have a 
			//way to correlate a loader to a result index
			int criteriaIndex = 0;
			foreach (CriteriaImpl criteria in criteriaQueries)
			{
				string[] implementors = factory.GetImplementors(criteria.EntityOrClassName);
				int size = implementors.Length;

				ISet<string> spaces = new HashSet<string>();

				for (int i = 0; i < size; i++)
				{
					CriteriaLoader loader = new CriteriaLoader(
						session.GetOuterJoinLoadable(implementors[i]),
						factory,
						criteria,
						implementors[i],
						session.EnabledFilters
						);
					loaders.Add(loader);
					loaderCriteriaMap.Add(criteriaIndex);
					spaces.UnionWith(loader.QuerySpaces);
				}
				criteriaIndex += 1;
			}
		}

		public IMultiCriteria Add(System.Type resultGenericListType, ICriteria criteria)
		{
			criteriaQueries.Add(criteria);
			resultCollectionGenericType.Add(resultGenericListType);

			return this;
		}

		public IMultiCriteria Add(ICriteria criteria)
		{
			return Add<object>(criteria);
		}

		public IMultiCriteria Add(string key, ICriteria criteria)
		{
			return Add<object>(key, criteria);
		}

		public IMultiCriteria Add(DetachedCriteria detachedCriteria)
		{
			return Add<object>(detachedCriteria);
		}

		public IMultiCriteria Add(string key, DetachedCriteria detachedCriteria)
		{
			return Add<object>(key, detachedCriteria);
		}

		public IMultiCriteria Add<T>(ICriteria criteria)
		{
			criteriaQueries.Add(criteria);
			resultCollectionGenericType.Add(typeof(T));

			return this;
		}

		public IMultiCriteria Add<T>(string key, ICriteria criteria)
		{
			ThrowIfKeyAlreadyExists(key);
			criteriaQueries.Add(criteria);
			criteriaResultPositions.Add(key, criteriaQueries.Count - 1);
			resultCollectionGenericType.Add(typeof(T));

			return this;
		}

		public IMultiCriteria Add<T>(DetachedCriteria detachedCriteria)
		{
			criteriaQueries.Add(
				detachedCriteria.GetExecutableCriteria(session)
				);
			resultCollectionGenericType.Add(typeof(T));

			return this;
		}

		public IMultiCriteria Add<T>(string key, DetachedCriteria detachedCriteria)
		{
			ThrowIfKeyAlreadyExists(key);
			criteriaQueries.Add(detachedCriteria.GetExecutableCriteria(session));
			criteriaResultPositions.Add(key, criteriaQueries.Count - 1);
			resultCollectionGenericType.Add(typeof(T));

			return this;
		}

		public IMultiCriteria Add(System.Type resultGenericListType, IQueryOver queryOver)
		{
			return Add(resultGenericListType, queryOver.RootCriteria);
		}

		public IMultiCriteria Add<T>(IQueryOver<T> queryOver)
		{
			return Add<T>(queryOver.RootCriteria);
		}

		public IMultiCriteria Add<U>(IQueryOver queryOver)
		{
			return Add<U>(queryOver.RootCriteria);
		}

		public IMultiCriteria Add<T>(string key, IQueryOver<T> queryOver)
		{
			return Add<T>(key, queryOver.RootCriteria);
		}

		public IMultiCriteria Add<U>(string key, IQueryOver queryOver)
		{
			return Add<U>(key, queryOver.RootCriteria);
		}

		public IMultiCriteria SetCacheable(bool cachable)
		{
			isCacheable = cachable;
			return this;
		}

		public IMultiCriteria ForceCacheRefresh(bool forceRefresh)
		{
			forceCacheRefresh = forceRefresh;
			return this;
		}

		#region IMultiCriteria Members

		public IMultiCriteria SetResultTransformer(IResultTransformer resultTransformer)
		{
			this.resultTransformer = resultTransformer;
			return this;
		}

		public object GetResult(string key)
		{
			if (criteriaResults == null) List();

			int criteriaResultPosition;
			if (!criteriaResultPositions.TryGetValue(key, out criteriaResultPosition))
				throw new InvalidOperationException(String.Format("The key '{0}' is unknown", key));

			return criteriaResults[criteriaResultPosition];
		}

		#endregion

		public IMultiCriteria SetCacheRegion(string cacheRegion)
		{
			this.cacheRegion = cacheRegion;
			return this;
		}

		private QueryParameters CreateCombinedQueryParameters()
		{
			QueryParameters combinedQueryParameters = new QueryParameters();
			combinedQueryParameters.ForceCacheRefresh = forceCacheRefresh;
			combinedQueryParameters.NamedParameters = new Dictionary<string, TypedValue>();
			var positionalParameterTypes = new List<IType>();
			var positionalParameterValues = new List<object>();
			int index = 0;
			foreach (QueryParameters queryParameters in parameters)
			{
				foreach (KeyValuePair<string, TypedValue> dictionaryEntry in queryParameters.NamedParameters)
				{
					combinedQueryParameters.NamedParameters.Add(dictionaryEntry.Key + index, dictionaryEntry.Value);
				}
				index += 1;
				positionalParameterTypes.AddRange(queryParameters.PositionalParameterTypes);
				positionalParameterValues.AddRange(queryParameters.PositionalParameterValues);
			}
			combinedQueryParameters.PositionalParameterTypes = positionalParameterTypes.ToArray();
			combinedQueryParameters.PositionalParameterValues = positionalParameterValues.ToArray();
			return combinedQueryParameters;
		}

		private void ThrowIfKeyAlreadyExists(string key)
		{
			if (criteriaResultPositions.ContainsKey(key))
				throw new InvalidOperationException(String.Format("The key '{0}' already exists", key));
		}
	}
}