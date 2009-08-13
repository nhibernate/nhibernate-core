using System;
using System.Collections;
using System.Collections.Generic;
using Iesi.Collections;
using Iesi.Collections.Generic;
using log4net;
using NHibernate.Event;
using NHibernate.Hql;
using NHibernate.Hql.Ast.ANTLR;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Engine.Query
{
	/// <summary> Defines a query execution plan for an HQL query (or filter). </summary>
	[Serializable]
	public class HQLQueryPlan
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(HQLQueryPlan));

		private readonly string sourceQuery;
		private readonly IQueryTranslator[] translators;
		private readonly string[] sqlStrings;

		private readonly ParameterMetadata parameterMetadata;
		private readonly ReturnMetadata returnMetadata;
		private readonly HashedSet<string> querySpaces;

		private readonly HashedSet<string> enabledFilterNames;
		private readonly bool shallow;

		public HQLQueryPlan(string hql, bool shallow, 
			IDictionary<string, IFilter> enabledFilters, ISessionFactoryImplementor factory)
			: this(hql, (string) null, shallow, enabledFilters, factory)
		{
		}

        public HQLQueryPlan(string expressionStr, IQueryExpression queryExpression, bool shallow,
            IDictionary<string, IFilter> enabledFilters, ISessionFactoryImplementor factory)
            : this(expressionStr, queryExpression, null, shallow, enabledFilters, factory)
        {
        }

		protected internal HQLQueryPlan(string hql, string collectionRole, bool shallow,
			IDictionary<string, IFilter> enabledFilters, ISessionFactoryImplementor factory)
		{
			sourceQuery = hql;
			this.shallow = shallow;

			enabledFilterNames = new HashedSet<string>(enabledFilters.Keys);

			HashedSet<string> combinedQuerySpaces = new HashedSet<string>();
			string[] concreteQueryStrings = QuerySplitter.ConcreteQueries(hql, factory);
			int length = concreteQueryStrings.Length;
			translators = new IQueryTranslator[length];
			List<string> sqlStringList = new List<string>();
			for (int i = 0; i < length; i++)
			{
				if (collectionRole == null)
				{
					translators[i] =
						factory.Settings.QueryTranslatorFactory.CreateQueryTranslator(hql, concreteQueryStrings[i], enabledFilters,
						                                                              factory);
					translators[i].Compile(factory.Settings.QuerySubstitutions, shallow);
				}
				else
				{
					translators[i] =
						factory.Settings.QueryTranslatorFactory.CreateFilterTranslator(hql, concreteQueryStrings[i], enabledFilters,
						                                                               factory);
					((IFilterTranslator)translators[i]).Compile(collectionRole, factory.Settings.QuerySubstitutions, shallow);
				}
				foreach (string qs in translators[i].QuerySpaces)
				{
					combinedQuerySpaces.Add(qs);	
				}
				sqlStringList.AddRange(translators[i].CollectSqlStrings);
			}

			sqlStrings = sqlStringList.ToArray();
			querySpaces = combinedQuerySpaces;

			if (length == 0)
			{
				parameterMetadata = new ParameterMetadata(null, null);
				returnMetadata = null;
			}
			else
			{
				parameterMetadata = BuildParameterMetadata(translators[0].GetParameterTranslations(), hql);
				if (translators[0].IsManipulationStatement)
				{
					returnMetadata = null;
				}
				else
				{
					if (length > 1)
					{
						int returns = translators[0].ReturnTypes.Length;
						returnMetadata = new ReturnMetadata(translators[0].ReturnAliases, new IType[returns]);
					}
					else
					{
						returnMetadata = new ReturnMetadata(translators[0].ReturnAliases, translators[0].ReturnTypes);
					}
				}
			}
		}

        protected internal HQLQueryPlan(string expressionStr, IQueryExpression queryExpression, string collectionRole, bool shallow,
                                    IDictionary<string, IFilter> enabledFilters, ISessionFactoryImplementor factory)
        {
            sourceQuery = expressionStr;
            this.shallow = shallow;

            enabledFilterNames = new HashedSet<string>(enabledFilters.Keys);

            // TODO - no support for polymorphism here - done during Expression -> AST translation?
            // TODO - polymorphism approach used in method above also sucks.  Could be done in AST much more cleanly?  Look at this...
            IQueryTranslatorFactory2 qtFactory = new ASTQueryTranslatorFactory();

            IQueryTranslator translator = qtFactory.CreateQueryTranslator(expressionStr, queryExpression, enabledFilters,
                                                                          factory);

            translator.Compile(factory.Settings.QuerySubstitutions, shallow);

            translators = new[] { translator };

            sqlStrings = new List<string>(translator.CollectSqlStrings).ToArray();

            querySpaces = new HashedSet<string>(translator.QuerySpaces);

            // TODO - need to build parameterMetadata.  Current function no good, since is parses the HQL.  Might need to walk the AST here,
            // probably inside the QueryTranslator.  That's probably a better place for the parsing to be anyway; possibly worth moving for classic as well...
            //parameterMetadata = BuildParameterMetadata(translator.GetParameterTranslations(), hql);
            parameterMetadata = new ParameterMetadata(new OrdinalParameterDescriptor[0], new Dictionary<string, NamedParameterDescriptor>());

            returnMetadata = new ReturnMetadata(translator.ReturnAliases, translator.ReturnTypes);
        }

		public string SourceQuery
		{
			get { return sourceQuery; }
		}

		public ISet<string> QuerySpaces
		{
			get { return querySpaces; }
		}

		public ParameterMetadata ParameterMetadata
		{
			get { return parameterMetadata; }
		}

		public ReturnMetadata ReturnMetadata
		{
			get { return returnMetadata; }
		}

		public ISet EnabledFilterNames
		{
			get { return enabledFilterNames; }
		}

		public string[] SqlStrings
		{
			get { return sqlStrings; }
		}

		public ISet UtilizedFilterNames
		{
			get
			{
				// TODO : add this info to the translator and aggregate it here...
				return null;
			}
		}

		public bool Shallow
		{
			get { return shallow; }
		}

		public IQueryTranslator[] Translators
		{
			get
			{
				IQueryTranslator[] copy = new IQueryTranslator[translators.Length];
				Array.Copy(translators, 0, copy, 0, copy.Length);
				return copy;
			}
		}

		private static ParameterMetadata BuildParameterMetadata(IParameterTranslations parameterTranslations, string hql)
		{
			long start = DateTime.Now.Ticks;
			ParamLocationRecognizer recognizer = ParamLocationRecognizer.ParseLocations(hql);
			long end = DateTime.Now.Ticks;
			if (log.IsDebugEnabled)
			{
				log.Debug("HQL param location recognition took " + (end - start) + " mills (" + hql + ")");
			}

			int ordinalParamCount = parameterTranslations.OrdinalParameterCount;
			int[] locations = recognizer.OrdinalParameterLocationList.ToArray();
			if (parameterTranslations.SupportsOrdinalParameterMetadata && locations.Length != ordinalParamCount)
			{
				throw new HibernateException("ordinal parameter mismatch");
			}
			ordinalParamCount = locations.Length;
			OrdinalParameterDescriptor[] ordinalParamDescriptors = new OrdinalParameterDescriptor[ordinalParamCount];
			for (int i = 1; i <= ordinalParamCount; i++)
			{
				ordinalParamDescriptors[i - 1] =
					new OrdinalParameterDescriptor(i,
					                               parameterTranslations.SupportsOrdinalParameterMetadata
					                               	? parameterTranslations.GetOrdinalParameterExpectedType(i)
					                               	: null, locations[i - 1]);
			}

			Dictionary<string, NamedParameterDescriptor> namedParamDescriptorMap = new Dictionary<string, NamedParameterDescriptor>();
			foreach (KeyValuePair<string, ParamLocationRecognizer.NamedParameterDescription> entry in recognizer.NamedParameterDescriptionMap)
			{
				string name = entry.Key;
				ParamLocationRecognizer.NamedParameterDescription description = entry.Value;
				namedParamDescriptorMap[name] =
					new NamedParameterDescriptor(name, parameterTranslations.GetNamedParameterExpectedType(name),
					                             description.BuildPositionsArray(), description.JpaStyle);

			}
			return new ParameterMetadata(ordinalParamDescriptors, namedParamDescriptorMap);
		}

		public void PerformList(QueryParameters queryParameters, ISessionImplementor session, IList results)
		{
			if (log.IsDebugEnabled)
			{
				log.Debug("find: " + SourceQuery);
				queryParameters.LogParameters(session.Factory);
			}
			bool hasLimit = queryParameters.RowSelection != null && queryParameters.RowSelection.DefinesLimits;
			bool needsLimit = hasLimit && translators.Length > 1;
			QueryParameters queryParametersToUse;
			if (needsLimit)
			{
				log.Warn("firstResult/maxResults specified on polymorphic query; applying in memory!");
				RowSelection selection = new RowSelection();
				selection.FetchSize = queryParameters.RowSelection.FetchSize;
				selection.Timeout = queryParameters.RowSelection.Timeout;
				queryParametersToUse = queryParameters.CreateCopyUsing(selection);
			}
			else
			{
				queryParametersToUse = queryParameters;
			}

			IList combinedResults = results ?? new List<object>();
			IdentitySet distinction = new IdentitySet();
			int includedCount = -1;
			for (int i = 0; i < translators.Length; i++)
			{
				IList tmp = translators[i].List(session, queryParametersToUse);
				if (needsLimit)
				{
					// NOTE : firstRow is zero-based
					int first = queryParameters.RowSelection.FirstRow == RowSelection.NoValue
												? 0
												: queryParameters.RowSelection.FirstRow;

					int max = queryParameters.RowSelection.MaxRows == RowSelection.NoValue
											? RowSelection.NoValue
											: queryParameters.RowSelection.MaxRows;

					int size = tmp.Count;
					for (int x = 0; x < size; x++)
					{
						object result = tmp[x];
						if (distinction.Add(result))
						{
							continue;
						}
						includedCount++;
						if (includedCount < first)
						{
							continue;
						}
						combinedResults.Add(result);
						if (max >= 0 && includedCount > max)
						{
							// break the outer loop !!!
							return;
						}
					}
				}
				else
					ArrayHelper.AddAll(combinedResults, tmp);
			}
		}

		public IEnumerable PerformIterate(QueryParameters queryParameters, IEventSource session)
		{
			bool? many;
			IEnumerable[] results;
			IEnumerable result;

			DoIterate(queryParameters, session, out many, out results, out result);

			return (many.HasValue && many.Value) ? new JoinedEnumerable(results) : result;
		}

		public IEnumerable<T> PerformIterate<T>(QueryParameters queryParameters, IEventSource session)
		{
			return new SafetyEnumerable<T>(PerformIterate(queryParameters, session));
		}

		private void DoIterate(QueryParameters queryParameters, IEventSource session, out bool? isMany,
			out IEnumerable[] results, out IEnumerable result)
		{
			isMany = null;
			results = null;
			if (log.IsDebugEnabled)
			{
				log.Debug("enumerable: " + SourceQuery);
				queryParameters.LogParameters(session.Factory);
			}
			if (translators.Length == 0)
			{
				result = CollectionHelper.EmptyEnumerable;
			}
			else
			{
				results = null;
				bool many = translators.Length > 1;
				if (many)
				{
					results = new IEnumerable[translators.Length];
				}

				result = null;
				for (int i = 0; i < translators.Length; i++)
				{
					result = translators[i].GetEnumerable(queryParameters, session);
					if (many)
						results[i] = result;
				}
				isMany = many;
			}
		}

		public int PerformExecuteUpdate(QueryParameters queryParameters, ISessionImplementor session)
		{
			if (log.IsDebugEnabled)
			{
				log.Debug("executeUpdate: " + SourceQuery);
				queryParameters.LogParameters(session.Factory);
			}
			if (translators.Length != 1)
			{
				log.Warn("manipulation query [" + SourceQuery + "] resulted in [" + translators.Length + "] split queries");
			}
			int result = 0;
			for (int i = 0; i < translators.Length; i++)
			{
				result += translators[i].ExecuteUpdate(queryParameters, session);
			}
			return result;
		}

	}
}
