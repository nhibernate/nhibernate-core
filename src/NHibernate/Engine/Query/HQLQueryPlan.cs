using System;
using System.Collections;
using System.Collections.Generic;
using Iesi.Collections;
using Iesi.Collections.Generic;
using log4net;
using NHibernate.Event;
using NHibernate.Hql;
using NHibernate.Hql.Ast.ANTLR;
using NHibernate.Impl;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Engine.Query
{
    public interface IQueryPlan
    {
        ParameterMetadata ParameterMetadata { get; }
        ISet<string> QuerySpaces { get; }
        IQueryTranslator[] Translators { get; }
        ReturnMetadata ReturnMetadata { get; }
        void PerformList(QueryParameters queryParameters, ISessionImplementor statelessSessionImpl, IList results);
        int PerformExecuteUpdate(QueryParameters queryParameters, ISessionImplementor statelessSessionImpl);
        IEnumerable<T> PerformIterate<T>(QueryParameters queryParameters, IEventSource session);
        IEnumerable PerformIterate(QueryParameters queryParameters, IEventSource session);
    }

    public interface IQueryExpressionPlan : IQueryPlan
    {
        IQueryExpression QueryExpression { get; }
    }

	/// <summary> Defines a query execution plan for an HQL query (or filter). </summary>
	[Serializable]
	public class HQLQueryPlan : IQueryPlan
	{
		protected static readonly ILog Log = LogManager.GetLogger(typeof(HQLQueryPlan));

		private readonly string _sourceQuery;

        protected HQLQueryPlan(string sourceQuery)
        {
            _sourceQuery = sourceQuery;
        }

		public ISet<string> QuerySpaces
		{
		    get;
		    protected set;
		}

		public ParameterMetadata ParameterMetadata
		{
            get;
            protected set;
        }

		public ReturnMetadata ReturnMetadata
		{
            get;
            protected set;
        }

		public string[] SqlStrings
		{
            get;
            protected set;
        }

		public IQueryTranslator[] Translators
		{
            get;
            protected set;
        }

		public void PerformList(QueryParameters queryParameters, ISessionImplementor session, IList results)
		{
			if (Log.IsDebugEnabled)
			{
				Log.Debug("find: " + _sourceQuery);
				queryParameters.LogParameters(session.Factory);
			}

			bool hasLimit = queryParameters.RowSelection != null && queryParameters.RowSelection.DefinesLimits;
			bool needsLimit = hasLimit && Translators.Length > 1;
			QueryParameters queryParametersToUse;
			if (needsLimit)
			{
				Log.Warn("firstResult/maxResults specified on polymorphic query; applying in memory!");
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
			for (int i = 0; i < Translators.Length; i++)
			{
				IList tmp = Translators[i].List(session, queryParametersToUse);
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
			if (Log.IsDebugEnabled)
			{
				Log.Debug("enumerable: " + _sourceQuery);
				queryParameters.LogParameters(session.Factory);
			}
			if (Translators.Length == 0)
			{
				result = CollectionHelper.EmptyEnumerable;
			}
			else
			{
				results = null;
				bool many = Translators.Length > 1;
				if (many)
				{
					results = new IEnumerable[Translators.Length];
				}

				result = null;
				for (int i = 0; i < Translators.Length; i++)
				{
					result = Translators[i].GetEnumerable(queryParameters, session);
					if (many)
						results[i] = result;
				}
				isMany = many;
			}
		}

		public int PerformExecuteUpdate(QueryParameters queryParameters, ISessionImplementor session)
		{
			if (Log.IsDebugEnabled)
			{
				Log.Debug("executeUpdate: " + _sourceQuery);
				queryParameters.LogParameters(session.Factory);
			}
			if (Translators.Length != 1)
			{
				Log.Warn("manipulation query [" + _sourceQuery + "] resulted in [" + Translators.Length + "] split queries");
			}
			int result = 0;
			for (int i = 0; i < Translators.Length; i++)
			{
				result += Translators[i].ExecuteUpdate(queryParameters, session);
			}
			return result;
		}

        protected void BuildSqlStringsAndQuerySpaces()
        {
            var combinedQuerySpaces = new HashedSet<string>();
            var sqlStringList = new List<string>();

            foreach (var translator in Translators)
            {
                foreach (var qs in translator.QuerySpaces)
                {
                    combinedQuerySpaces.Add(qs);
                }

                sqlStringList.AddRange(translator.CollectSqlStrings);
            }

            SqlStrings = sqlStringList.ToArray();
            QuerySpaces = combinedQuerySpaces;
        }
    }

    [Serializable]
    public class HQLStringQueryPlan : HQLQueryPlan
    {
	    public HQLStringQueryPlan(string hql, bool shallow, 
			IDictionary<string, IFilter> enabledFilters, ISessionFactoryImplementor factory)
			: this(hql, (string) null, shallow, enabledFilters, factory)
		{
		}

   		protected internal HQLStringQueryPlan(string hql, string collectionRole, bool shallow,
			IDictionary<string, IFilter> enabledFilters, ISessionFactoryImplementor factory)
            :base(hql)
		{
            Translators = factory.Settings.QueryTranslatorFactory.CreateQueryTranslators(hql, collectionRole, shallow, enabledFilters, factory);

		    BuildSqlStringsAndQuerySpaces();

            if (Translators.Length == 0)
			{
				ParameterMetadata = new ParameterMetadata(null, null);
				ReturnMetadata = null;
			}
			else
			{
				ParameterMetadata = BuildParameterMetadata(Translators[0].GetParameterTranslations(), hql);
				if (Translators[0].IsManipulationStatement)
				{
					ReturnMetadata = null;
				}
				else
				{
                    if (Translators.Length > 1)
					{
						int returns = Translators[0].ReturnTypes.Length;
						ReturnMetadata = new ReturnMetadata(Translators[0].ReturnAliases, new IType[returns]);
					}
					else
					{
						ReturnMetadata = new ReturnMetadata(Translators[0].ReturnAliases, Translators[0].ReturnTypes);
					}
				}
			}
		}

        private static ParameterMetadata BuildParameterMetadata(IParameterTranslations parameterTranslations, string hql)
        {
            long start = DateTime.Now.Ticks;
            ParamLocationRecognizer recognizer = ParamLocationRecognizer.ParseLocations(hql);
            long end = DateTime.Now.Ticks;
            if (Log.IsDebugEnabled)
            {
                Log.Debug("HQL param location recognition took " + (end - start) + " mills (" + hql + ")");
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
    }

    [Serializable]
    public class HQLLinqQueryPlan : HQLQueryPlan, IQueryExpressionPlan
    {
        public IQueryExpression QueryExpression
        {
            get;
            protected set;
        }

        public HQLLinqQueryPlan(string expressionStr, IQueryExpression queryExpression, bool shallow,
            IDictionary<string, IFilter> enabledFilters, ISessionFactoryImplementor factory)
            : this(expressionStr, queryExpression, null, shallow, enabledFilters, factory)
        {
        }

    	protected internal HQLLinqQueryPlan(string expressionStr, IQueryExpression queryExpression, string collectionRole, bool shallow,
                                            IDictionary<string, IFilter> enabledFilters, ISessionFactoryImplementor factory)
            : base (expressionStr)
        {
            QueryExpression = queryExpression;

            IQueryTranslatorFactory2 qtFactory = new ASTQueryTranslatorFactory();

            Translators = qtFactory.CreateQueryTranslators(expressionStr, queryExpression, collectionRole, shallow, enabledFilters, factory);

            BuildSqlStringsAndQuerySpaces();

            if (Translators.Length == 0)
            {
                ParameterMetadata = new ParameterMetadata(null, null);
                ReturnMetadata = null;
            }
            else
            {
                var parameterTranslations = Translators[0].GetParameterTranslations();

                var namedParamDescriptorMap = new Dictionary<string, NamedParameterDescriptor>();
                foreach (NamedParameterDescriptor entry in queryExpression.ParameterDescriptors)
                {
                    namedParamDescriptorMap[entry.Name] =
                        new NamedParameterDescriptor(entry.Name, parameterTranslations.GetNamedParameterExpectedType(entry.Name),
                                                     entry.SourceLocations, entry.JpaStyle);
                }

                ParameterMetadata = new ParameterMetadata(new OrdinalParameterDescriptor[0], namedParamDescriptorMap);

                if (Translators[0].IsManipulationStatement)
                {
                    ReturnMetadata = null;
                }
                else
                {
                    if (Translators.Length > 1)
                    {
                        int returns = Translators[0].ReturnTypes.Length;
                        ReturnMetadata = new ReturnMetadata(Translators[0].ReturnAliases, new IType[returns]);
                    }
                    else
                    {
                        ReturnMetadata = new ReturnMetadata(Translators[0].ReturnAliases, Translators[0].ReturnTypes);
                    }
                }
            }
        }
    }
}
