using System;
using System.Collections;
using System.Data;
using System.Text.RegularExpressions;
using Iesi.Collections;
using log4net;
using NHibernate.Cache;
using NHibernate.Engine;
using NHibernate.Hql;
using NHibernate.Hql.Classic;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;
using NHibernate.Transform;
using NHibernate.Type;

namespace NHibernate.Impl
{
	public class MultiQueryImpl : IMultiQuery
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(MultiQueryImpl));

		private ArrayList queries = new ArrayList();
		private ArrayList translators = new ArrayList();
		private ArrayList parameters = new ArrayList();
		private string cacheRegion;
		private int commandTimeout = RowSelection.NoValue;
		private bool isCacheable = false;
		private ISessionImplementor session;
		private IResultTransformer resultTransformer;
		private ArrayList types = new ArrayList();
		private SqlString sqlString = null;
		private Dialect.Dialect dialect;
		private bool forceCacheRefresh;
		private QueryParameters combinedParameters;
		private IList namedParametersThatAreSafeToDuplicate = new ArrayList();
		private FlushMode flushMode = FlushMode.Unspecified;
		private FlushMode sessionFlushMode = FlushMode.Unspecified;
	    private static readonly Regex parseParameterListOrignialName = new Regex(@"(.*?)\d+_",RegexOptions.Compiled);

	    public MultiQueryImpl(ISessionImplementor session)
		{
			dialect = session.Factory.Dialect;
			if (dialect.SupportsMultipleQueries == false)
			{
				throw new NotSupportedException(
					string.Format("The dialect {0} does not support mutliply queries.", dialect.GetType().FullName));
			}
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
			namedParametersThatAreSafeToDuplicate.Add(name);
			foreach (IQuery query in queries)
			{
				query.SetParameter(name, val, type);
			}
			return this;
		}

		public IMultiQuery SetParameter(string name, object val)
		{
			namedParametersThatAreSafeToDuplicate.Add(name);
			foreach (IQuery query in queries)
			{
				query.SetParameter(name, val);
			}
			return this;
		}

		public IMultiQuery SetParameterList(string name, ICollection vals, IType type)
		{
			namedParametersThatAreSafeToDuplicate.Add(name);
			foreach (IQuery query in queries)
			{
				query.SetParameterList(name, vals, type);
			}
			return this;
		}

		public IMultiQuery SetParameterList(string name, ICollection vals)
		{
			namedParametersThatAreSafeToDuplicate.Add(name);
			foreach (IQuery query in queries)
			{
				query.SetParameterList(name, vals);
			}
			return this;
		}

		public IMultiQuery SetAnsiString(string name, string val)
		{
			namedParametersThatAreSafeToDuplicate.Add(name);
			foreach (IQuery query in queries)
			{
				query.SetAnsiString(name, val);
			}
			return this;
		}

		public IMultiQuery SetBinary(string name, byte[] val)
		{
			namedParametersThatAreSafeToDuplicate.Add(name);
			foreach (IQuery query in queries)
			{
				query.SetBinary(name, val);
			}
			return this;
		}

		public IMultiQuery SetBoolean(string name, bool val)
		{
			namedParametersThatAreSafeToDuplicate.Add(name);
			foreach (IQuery query in queries)
			{
				query.SetBoolean(name, val);
			}
			return this;
		}

		public IMultiQuery SetByte(string name, byte val)
		{
			namedParametersThatAreSafeToDuplicate.Add(name);
			foreach (IQuery query in queries)
			{
				query.SetByte(name, val);
			}
			return this;
		}

		public IMultiQuery SetCharacter(string name, char val)
		{
			namedParametersThatAreSafeToDuplicate.Add(name);
			foreach (IQuery query in queries)
			{
				query.SetCharacter(name, val);
			}
			return this;
		}

		public IMultiQuery SetDateTime(string name, DateTime val)
		{
			namedParametersThatAreSafeToDuplicate.Add(name);
			foreach (IQuery query in queries)
			{
				query.SetDateTime(name, val);
			}
			return this;
		}

		public IMultiQuery SetDecimal(string name, decimal val)
		{
			namedParametersThatAreSafeToDuplicate.Add(name);
			foreach (IQuery query in queries)
			{
				query.SetDecimal(name, val);
			}
			return this;
		}

		public IMultiQuery SetDouble(string name, double val)
		{
			namedParametersThatAreSafeToDuplicate.Add(name);
			foreach (IQuery query in queries)
			{
				query.SetDouble(name, val);
			}
			return this;
		}

		public IMultiQuery SetEntity(string name, object val)
		{
			namedParametersThatAreSafeToDuplicate.Add(name);
			foreach (IQuery query in queries)
			{
				query.SetEntity(name, val);
			}
			return this;
		}

		public IMultiQuery SetEnum(string name, Enum val)
		{
			namedParametersThatAreSafeToDuplicate.Add(name);
			foreach (IQuery query in queries)
			{
				query.SetEnum(name, val);
			}
			return this;
		}

		public IMultiQuery SetInt16(string name, short val)
		{
			namedParametersThatAreSafeToDuplicate.Add(name);
			foreach (IQuery query in queries)
			{
				query.SetInt16(name, val);
			}
			return this;
		}

		public IMultiQuery SetInt32(string name, int val)
		{
			namedParametersThatAreSafeToDuplicate.Add(name);
			foreach (IQuery query in queries)
			{
				query.SetInt32(name, val);
			}
			return this;
		}

		public IMultiQuery SetInt64(string name, long val)
		{
			namedParametersThatAreSafeToDuplicate.Add(name);
			foreach (IQuery query in queries)
			{
				query.SetInt64(name, val);
			}
			return this;
		}

		public IMultiQuery SetSingle(string name, float val)
		{
			namedParametersThatAreSafeToDuplicate.Add(name);
			foreach (IQuery query in queries)
			{
				query.SetSingle(name, val);
			}
			return this;
		}

		public IMultiQuery SetString(string name, string val)
		{
			namedParametersThatAreSafeToDuplicate.Add(name);
			foreach (IQuery query in queries)
			{
				query.SetString(name, val);
			}
			return this;
		}

		public IMultiQuery SetTime(string name, DateTime val)
		{
			namedParametersThatAreSafeToDuplicate.Add(name);
			foreach (IQuery query in queries)
			{
				query.SetTime(name, val);
			}
			return this;
		}

		public IMultiQuery SetTimestamp(string name, DateTime val)
		{
			namedParametersThatAreSafeToDuplicate.Add(name);
			foreach (IQuery query in queries)
			{
				query.SetTimestamp(name, val);
			}
			return this;
		}

		public IMultiQuery Add(IQuery query)
		{
			queries.Add(query);
			return this;
		}

		public IMultiQuery Add(string hql)
		{
			queries.Add(session.CreateQuery(hql));
			return this;
		}
		
		public IMultiQuery AddNamedQuery(string namedQuery)
		{
			queries.Add(session.GetNamedQuery(namedQuery));
			return this;
		}

		public IMultiQuery SetCacheable(bool cacheable)
		{
			isCacheable = cacheable;
			return this;
		}

		public IMultiQuery SetCacheRegion(string cacheRegion)
		{
			this.cacheRegion = cacheRegion;
			return this;
		}

		/// <summary>
		/// Return the query results of all the queries
		/// </summary>
		public IList List()
		{
			bool cacheable = session.Factory.IsQueryCacheEnabled && isCacheable;
			combinedParameters = CreateCombinedQueryParameters();

			if(log.IsDebugEnabled)
			{
				log.DebugFormat("Multi query with {0} queries.",queries.Count);
				for (int i = 0; i < queries.Count; i++)
				{
					log.DebugFormat("Query #{0}: {1}",i,queries[i]);
				}
			}
			
			try
			{
				Before();
				
				if (cacheable)
				{
					return ListUsingQueryCache();
				}
				else
				{
					return ListIgnoreQueryCache();
				}
			}
			finally
			{
				After();
			}
		}

		public IMultiQuery SetFlushMode(FlushMode flushMode)
		{
			this.flushMode = flushMode;
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
			if (resultTransformer == null)
				return results;

			return resultTransformer.TransformList(results);
		}


		protected ArrayList DoList()
		{
			IDbCommand command = PrepareQueriesCommand();

			BindParameters(command);

			ArrayList results = new ArrayList();

			log.Info(command.CommandText);
			if (commandTimeout != RowSelection.NoValue)
				command.CommandTimeout = commandTimeout;
			IDataReader reader = session.Batcher.ExecuteReader(command);
			ArrayList[] hydratedObjects = new ArrayList[Translators.Count];
			ArrayList[] subselectResultKeys = new ArrayList[Translators.Count];
			bool[] createSubselects = new bool[Translators.Count];
			try
			{
				if(log.IsDebugEnabled)
					log.DebugFormat("Executing {0} queries", translators.Count);
				for (int i = 0; i < translators.Count; i++)
				{
					QueryTranslator translator = (QueryTranslator)Translators[i];
					QueryParameters parameter = (QueryParameters)Parameters[i];
					ArrayList tempResults = new ArrayList();
					int entitySpan = translator.EntityPersisters.Length;
					hydratedObjects[i] = entitySpan > 0 ? new ArrayList() : null;
					RowSelection selection = parameter.RowSelection;
					int maxRows = Loader.Loader.HasMaxRows(selection) ? selection.MaxRows : int.MaxValue;
					if (!dialect.SupportsLimitOffset || !Loader.Loader.UseLimit(selection, dialect))
					{
						translator.Advance(reader, selection);
					}

					LockMode[] lockModeArray = translator.GetLockModes(parameter.LockModes);
					EntityKey optionalObjectKey = Loader.Loader.GetOptionalObjectKey(parameter, session);

					createSubselects[i] = translator.IsSubselectLoadingEnabled;
					subselectResultKeys[i] = createSubselects[i] ? new ArrayList() : null;

					translator.HandleEmptyCollections(parameter.CollectionKeys, reader, session);
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

						object result =
							translator.GetRowFromResultSet(reader,
														   session,
														   parameter,
														   lockModeArray,
														   optionalObjectKey,
														   hydratedObjects[i],
														   keys,
														   false);
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
					
					if(log.IsDebugEnabled)
					{
						log.DebugFormat("Query {0} returned {1} results", i, tempResults.Count);
					}
					
					reader.NextResult();
				}
			
			}
			catch (Exception ex)
			{
				log.Error("Failed to execute multi query", ex);
				throw new HibernateException("Failed to execute multi query", ex);
			}
			finally
			{
				session.Batcher.CloseCommand(command, reader);
			}
			for (int i = 0; i < translators.Count; i++)
			{
				QueryTranslator translator = (QueryTranslator)translators[i];
				QueryParameters parameter = (QueryParameters)parameters[i];

				translator.InitializeEntitiesAndCollections(hydratedObjects[i], reader, session);

				if (createSubselects[i])
				{
					translator.CreateSubselects(subselectResultKeys[i], parameter, session);
				}
			}
			return results;
		}

		private IDbCommand PrepareQueriesCommand()
		{
			SqlType[] sqlTypes = (SqlType[])types.ToArray(typeof(SqlType));
			return session.Batcher.PrepareQueryCommand(CommandType.Text, SqlString, sqlTypes);
		}

		protected SqlString SqlString
		{
			get
			{
                if (sqlString==null)
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
				IQueryTranslator[] queryTranslators = session.GetQueries(query.BindParameterLists(queryParameters.NamedParameters), false);
				foreach (QueryTranslator translator in queryTranslators)
				{
					translators.Add(translator);
					parameters.Add(queryParameters);
					SqlCommandInfo commandInfo = translator.GetQueryStringAndTypes(session, queryParameters);
					sqlString = sqlString.Append(commandInfo.Text).Append(dialect.MultipleQueriesSeparator).Append(Environment.NewLine);
					types.AddRange(commandInfo.ParameterTypes);
				}
			}
		}

		private void BindParameters(IDbCommand command)
		{
			int colIndex = 0;

			colIndex = BindLimitParametersFirstIfNeccesary(command, colIndex);
			colIndex = BindQueryParameters(command, colIndex);

			BindLimitParametersLastIfNeccesary(command, colIndex);
		}

		private void BindLimitParametersLastIfNeccesary(IDbCommand command, int colIndex)
		{
			for (int i = 0; i < queries.Count; i++)
			{
				QueryTranslator translator = (QueryTranslator)Translators[i];
				QueryParameters parameter = (QueryParameters)parameters[i];
				RowSelection selection = parameter.RowSelection;
				if (Loader.Loader.UseLimit(selection, dialect) && !dialect.BindLimitParametersFirst)
				{
					colIndex += translator.BindLimitParameters(command, colIndex, selection, session);
				}
			}
		}

		private int BindQueryParameters(IDbCommand command, int colIndex)
		{
			for (int i = 0; i < queries.Count; i++)
			{
				QueryTranslator translator = (QueryTranslator)Translators[i];
				QueryParameters parameter = (QueryParameters)Parameters[i];
				colIndex += translator.BindPositionalParameters(command, parameter, colIndex, session);
				colIndex += translator.BindNamedParameters(command, parameter.NamedParameters, colIndex, session);
			}
			return colIndex;
		}

		private int BindLimitParametersFirstIfNeccesary(IDbCommand command, int colIndex)
		{
			for (int i = 0; i < queries.Count; i++)
			{
				QueryTranslator translator = (QueryTranslator)Translators[i];
				QueryParameters parameter = (QueryParameters)Parameters[i];
				RowSelection selection = parameter.RowSelection;
				if (Loader.Loader.UseLimit(selection, dialect) && dialect.BindLimitParametersFirst)
				{
					colIndex += translator.BindLimitParameters(command, colIndex, selection, session);
				}
			}
			return colIndex;
		}

		public override string ToString()
		{
			return "Multi Query: [" + SqlString + "]";
		}

		private class MultipleQueriesCacheAssembler : ICacheAssembler
		{
			private IList assemblersList;

			public MultipleQueriesCacheAssembler(IList assemblers)
			{
				assemblersList = assemblers;
			}

			public object Disassemble(object value, ISessionImplementor session)
			{
				IList srcList = (IList)value;
				ArrayList cacheable = new ArrayList();
				for (int i = 0; i < srcList.Count; i++)
				{
					ICacheAssembler[] assemblers = (ICacheAssembler[])assemblersList[i];
					IList itemList = (IList)srcList[i];
					ArrayList singleQueryCached = new ArrayList();
					foreach (object objToCache in itemList)
					{
						if (assemblers.Length == 1)
						{
							singleQueryCached.Add(assemblers[0].Disassemble(objToCache, session));
						}
						else
						{
							singleQueryCached.Add(TypeFactory.Disassemble((object[])objToCache, assemblers, session));
						}
					}
					cacheable.Add(singleQueryCached);
				}
				return cacheable;
			}

			public object Assemble(object cached, ISessionImplementor session, object owner)
			{
				IList srcList = (IList)cached;
				ArrayList result = new ArrayList();
				for (int i = 0; i < assemblersList.Count; i++)
				{
					ICacheAssembler[] assemblers = (ICacheAssembler[])assemblersList[i];
					IList queryFromCache = (IList)srcList[i];
					ArrayList queryResults = new ArrayList();
					foreach (object fromCache in queryFromCache)
					{
						if (assemblers.Length == 1)
						{
							queryResults.Add(assemblers[0].Assemble(fromCache, session, owner));
						}
						else
						{
							queryResults.Add(TypeFactory.Assemble((object[])fromCache, assemblers, session, owner));
						}
					}
					result.Add(queryResults);
				}
				return result;
			}
		}

		#region Implementation

		private IList ListIgnoreQueryCache()
		{
			return GetResultList(DoList());
		}

		private IList ListUsingQueryCache()
		{
			IQueryCache queryCache = session.Factory.GetQueryCache(cacheRegion);

			ISet filterKeys = FilterKey.CreateFilterKeys(session.EnabledFilters);

			ISet querySpaces = new HashedSet();
			ArrayList resultTypesList = new ArrayList();
			for (int i = 0; i < Translators.Count; i++)
			{
				QueryTranslator queryTranslator = (QueryTranslator)Translators[i];
				querySpaces.AddAll(queryTranslator.QuerySpaces);
				resultTypesList.Add(queryTranslator.ActualReturnTypes);
			}
			MultipleQueriesCacheAssembler assembler = new MultipleQueriesCacheAssembler(resultTypesList);

			QueryKey key = new QueryKey(session.Factory, SqlString, combinedParameters, filterKeys);

			IList result =
				GetResultFromQueryCache(session,
										combinedParameters,
										querySpaces,
										new ICacheAssembler[] { assembler },
										queryCache,
										key);

			if (result == null)
			{
				log.Debug("Cache miss for multi query");
				ArrayList list = DoList();
				queryCache.Put(key, new ICacheAssembler[] { assembler }, new object[] { list }, session);
				result = list;
			}

			return GetResultList(result);
		}

		private IList Translators
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
			combinedQueryParameters.NamedParameters = new Hashtable();
			ArrayList positionalParameterTypes = new ArrayList();
			ArrayList positionalParameterValues = new ArrayList();
			foreach (QueryParameters queryParameters in Parameters)
			{
				CopyNamedParametersDictionary(combinedQueryParameters.NamedParameters, queryParameters.NamedParameters);
				positionalParameterTypes.AddRange(queryParameters.PositionalParameterTypes);
				positionalParameterValues.AddRange(queryParameters.PositionalParameterValues);
			}
			combinedQueryParameters.PositionalParameterTypes = (IType[])positionalParameterTypes.ToArray(typeof(IType));
			combinedQueryParameters.PositionalParameterValues = (object[])positionalParameterValues.ToArray(typeof(object));
			return combinedQueryParameters;
		}

		private IList Parameters
		{
			get
			{
				if (sqlString == null)
					AggregateQueriesInformation();
				return parameters;
			}
		}

		private void CopyNamedParametersDictionary(IDictionary dest, IDictionary src)
		{
		    foreach (DictionaryEntry dictionaryEntry in src)
			{
				if (dest.Contains(dictionaryEntry.Key))
				{
				    if (IsParameterSafeToDuplicate(dictionaryEntry.Key.ToString()))
						continue;//we specify it for all the queries, so it is okay.
					
					throw new QueryException(
						string.Format(
							"The named parameter {0} was used in more than one query. Either give unique names to your parameters, or use the multi query SetParameter() methods to set the named parameter",
							dictionaryEntry.Key));
				}
				dest.Add(dictionaryEntry.Key, dictionaryEntry.Value);
			}
		}

	    private bool IsParameterSafeToDuplicate(string name)
	    {
            if(namedParametersThatAreSafeToDuplicate.Contains(name))
                return true;
	        Match match = parseParameterListOrignialName.Match(name);
            if(match!=null)
            {
                string originalName = match.Groups[1].Value;
                return namedParametersThatAreSafeToDuplicate.Contains(originalName);
            }
	        return false;
	    }

	    private static IList GetResultFromQueryCache(
			ISessionImplementor session,
			QueryParameters queryParameters,
			ISet querySpaces,
			ICacheAssembler[] resultTypes,
			IQueryCache queryCache,
			QueryKey key)
		{
			if (!queryParameters.ForceCacheRefresh)
			{
				IList list = queryCache.Get(key, resultTypes, querySpaces, session);
				//we had to wrap the query results in another list in order to save all
				//the queries in the same bucket, now we need to do it the other way around.
				if (list != null)
					list = (IList)list[0];
				return list;
			}
			return null;
		}

		#endregion
	}
}