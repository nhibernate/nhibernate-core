using System;
using System.Collections.Generic;
using NHibernate.Engine.Query.Sql;
using NHibernate.Hql;
using NHibernate.Linq;
using NHibernate.Util;

namespace NHibernate.Engine.Query
{
	/// <summary> Acts as a cache for compiled query plans, as well as query-parameter metadata. </summary>
	[Serializable]
	public class QueryPlanCache
	{
		private static readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof(QueryPlanCache));

		private readonly ISessionFactoryImplementor factory;

		// simple cache of param metadata based on query string.  Ideally, the
		// original "user-supplied query" string should be used to retrieve this
		// metadata (i.e., not the para-list-expanded query string) to avoid
		// unnecessary cache entries.
		// Used solely for caching param metadata for native-sql queries, see
		// getSQLParameterMetadata() for a discussion as to why...
		private readonly SimpleMRUCache sqlParamMetadataCache = new SimpleMRUCache();

		// the cache of the actual plans...
		private readonly SoftLimitMRUCache planCache = new SoftLimitMRUCache(128);

		public QueryPlanCache(ISessionFactoryImplementor factory)
		{
			this.factory = factory;
		}

		public ParameterMetadata GetSQLParameterMetadata(string query)
		{
			var metadata = (ParameterMetadata)sqlParamMetadataCache[query];
			if (metadata == null)
			{
				// for native-sql queries, the param metadata is determined outside
				// any relation to a query plan, because query plan creation and/or
				// retrieval for a native-sql query depends on all of the return
				// types having been set, which might not be the case up-front when
				// param metadata would be most useful
				metadata = BuildNativeSQLParameterMetadata(query);
				sqlParamMetadataCache.Put(query, metadata);
			}
			return metadata;
		}

		[Obsolete("Please use overload with IQueryExpression")]
		public IQueryPlan GetHQLQueryPlan(string queryString, bool shallow, IDictionary<string, IFilter> enabledFilters)
		{
			return GetHQLQueryPlan(queryString.ToQueryExpression(), shallow, enabledFilters);
		}

		public IQueryExpressionPlan GetHQLQueryPlan(IQueryExpression queryExpression, bool shallow, IDictionary<string, IFilter> enabledFilters)
		{
			var key = new HQLQueryPlanKey(queryExpression, shallow, enabledFilters);
			var plan = (QueryExpressionPlan)planCache[key];

			if (plan == null)
			{
				if (log.IsDebugEnabled)
				{
					log.Debug("unable to locate HQL query plan in cache; generating (" + queryExpression.Key + ")");
				}
				plan = new QueryExpressionPlan(queryExpression, shallow, enabledFilters, factory);
				planCache.Put(key, plan);
			}
			else
			{
				if (log.IsDebugEnabled)
				{
					log.Debug("located HQL query plan in cache (" + queryExpression.Key + ")");
				}
				var planExpression = plan.QueryExpression as NhLinqExpression;
				var expression = queryExpression as NhLinqExpression;
				if (planExpression != null && expression != null)
				{
					//NH-3413
					//Here we have to use original expression.
					//In most cases NH do not translate expression in second time, but 
					// for cases when we have list parameters in query, like @p1.Contains(...),
					// it does, and then it uses parameters from first try. 
					//TODO: cache only required parts of QueryExpression

					//NH-3436
					// We have to return new instance plan with it's own query expression
					// because other treads can override queryexpression of current plan during execution of query if we will use cached instance of plan 
					expression.CopyExpressionTranslation(planExpression);
					plan = plan.Copy(expression);
				}
			}

			return plan;
		}

		public FilterQueryPlan GetFilterQueryPlan(string filterString, string collectionRole, bool shallow, IDictionary<string, IFilter> enabledFilters)
		{
			var key = new FilterQueryPlanKey(filterString, collectionRole, shallow, enabledFilters);
			var plan = (FilterQueryPlan) planCache[key];

			if (plan == null)
			{
				if (log.IsDebugEnabled)
				{
					log.Debug("unable to locate collection-filter query plan in cache; generating (" + collectionRole + " : "
							  + filterString + ")");
				}
				plan = new FilterQueryPlan(filterString.ToQueryExpression(), collectionRole, shallow, enabledFilters, factory);
				planCache.Put(key, plan);
			}
			else
			{
				if (log.IsDebugEnabled)
				{
					log.Debug("located collection-filter query plan in cache (" + collectionRole + " : " + filterString + ")");
				}
			}

			return plan;
		}

		public NativeSQLQueryPlan GetNativeSQLQueryPlan(NativeSQLQuerySpecification spec)
		{
			var plan = (NativeSQLQueryPlan)planCache[spec];

			if (plan == null)
			{
				if (log.IsDebugEnabled)
				{
					log.Debug("unable to locate native-sql query plan in cache; generating (" + spec.QueryString + ")");
				}
				plan = new NativeSQLQueryPlan(spec, factory);
				planCache.Put(spec, plan);
			}
			else
			{
				if (log.IsDebugEnabled)
				{
					log.Debug("located native-sql query plan in cache (" + spec.QueryString + ")");
				}
			}

			return plan;
		}

		private ParameterMetadata BuildNativeSQLParameterMetadata(string sqlString)
		{
			ParamLocationRecognizer recognizer = ParamLocationRecognizer.ParseLocations(sqlString);

			var ordinalDescriptors = new OrdinalParameterDescriptor[recognizer.OrdinalParameterLocationList.Count];
			for (int i = 0; i < recognizer.OrdinalParameterLocationList.Count; i++)
			{
				int position = recognizer.OrdinalParameterLocationList[i];
				ordinalDescriptors[i] = new OrdinalParameterDescriptor(i, null);
			}

			IDictionary<string, NamedParameterDescriptor> namedParamDescriptorMap = new Dictionary<string, NamedParameterDescriptor>();

			foreach (KeyValuePair<string, ParamLocationRecognizer.NamedParameterDescription> entry in recognizer.NamedParameterDescriptionMap)
			{
				string name = entry.Key;
				ParamLocationRecognizer.NamedParameterDescription description = entry.Value;
				namedParamDescriptorMap[name] =
					new NamedParameterDescriptor(name, null, description.JpaStyle);				
			}

			return new ParameterMetadata(ordinalDescriptors, namedParamDescriptorMap);
		}

		[Serializable]
		private class HQLQueryPlanKey : IEquatable<HQLQueryPlanKey>
		{
			private readonly string query;
			private readonly bool shallow;
			private readonly HashSet<string> filterNames;
			private readonly int hashCode;
			private readonly System.Type queryTypeDiscriminator;

			public HQLQueryPlanKey(string query, bool shallow, IDictionary<string, IFilter> enabledFilters)
				: this(typeof(object), query, shallow, enabledFilters)
			{
			}

			public HQLQueryPlanKey(IQueryExpression queryExpression, bool shallow, IDictionary<string, IFilter> enabledFilters)
				: this(queryExpression.GetType(), queryExpression.Key, shallow, enabledFilters)
			{
			}

			protected HQLQueryPlanKey(System.Type queryTypeDiscriminator, string query, bool shallow, IDictionary<string, IFilter> enabledFilters)
			{
				this.queryTypeDiscriminator = queryTypeDiscriminator;
				this.query = query;
				this.shallow = shallow;

				if (enabledFilters == null || (enabledFilters.Count == 0))
				{
					filterNames = new HashSet<string>();
				}
				else
				{
					filterNames = new HashSet<string>(enabledFilters.Keys);
				}

				unchecked
				{
					var hash = query.GetHashCode();
					hash = 29*hash + (shallow ? 1 : 0);
					hash = 29*hash + CollectionHelper.GetHashCode(filterNames);
					hash = 29*hash + queryTypeDiscriminator.GetHashCode();
					hashCode = hash;
				}
			}

			public override bool Equals(object obj)
			{
				return this == obj || Equals(obj as HQLQueryPlanKey);
			}

			public bool Equals(HQLQueryPlanKey that)
			{
				if (that == null)
				{
					return false;
				}

				if (shallow != that.shallow)
				{
					return false;
				}

				if (!filterNames.SetEquals(that.filterNames))
				{
					return false;
				}

				if (!query.Equals(that.query))
				{
					return false;
				}

				if (queryTypeDiscriminator != that.queryTypeDiscriminator)
				{
					return false;
				}

				return true;
			}

			public override int GetHashCode()
			{
				return hashCode;
			}
		}

		[Serializable]
		private class FilterQueryPlanKey
		{
			private readonly string query;
			private readonly string collectionRole;
			private readonly bool shallow;
			private readonly HashSet<string> filterNames;
			private readonly int hashCode;

			public FilterQueryPlanKey(string query, string collectionRole, bool shallow, IDictionary<string, IFilter> enabledFilters)
			{
				this.query = query;
				this.collectionRole = collectionRole;
				this.shallow = shallow;

				if (enabledFilters == null || (enabledFilters.Count == 0))
				{
					filterNames = new HashSet<string>();
				}
				else
				{
					filterNames = new HashSet<string>(enabledFilters.Keys);
				}

				int hash = query.GetHashCode();
				hash = 29 * hash + collectionRole.GetHashCode();
				hash = 29 * hash + (shallow ? 1 : 0);
				hash = 29 * hash + CollectionHelper.GetHashCode(filterNames);
				hashCode = hash;
			}

			public override bool Equals(object obj)
			{
				return this == obj || Equals(obj as FilterQueryPlanKey);
			}

			public bool Equals(FilterQueryPlanKey that)
			{
				if (that == null)
				{
					return false;
				}
				if (shallow != that.shallow)
				{
					return false;
				}
				if (!filterNames.SetEquals(that.filterNames))
				{
					return false;
				}
				if (!query.Equals(that.query))
				{
					return false;
				}
				if (!collectionRole.Equals(that.collectionRole))
				{
					return false;
				}

				return true;
			}

			public override int GetHashCode()
			{
				return hashCode;
			}
		}
	}
}
