using System.Collections.Generic;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Engine;
using NHibernate.Util;

namespace NHibernate.Cfg.XmlHbmBinding
{
	public class NamedSQLQueryBinder : Binder
	{
		public NamedSQLQueryBinder(Mappings mappings)
			: base(mappings)
		{
		}

		public NamedSQLQueryBinder(Binder parent)
			: base(parent)
		{
		}

		public void AddSqlQuery(HbmSqlQuery querySchema)
		{
			mappings.AddSecondPass(delegate
				{
					string queryName = querySchema.name;
					string queryText = querySchema.GetText();
					bool cacheable = querySchema.cacheableSpecified ? querySchema.cacheable: false;
					string region = querySchema.cacheregion;
					int timeout = querySchema.timeoutSpecified ? querySchema.timeout : RowSelection.NoValue;
					int fetchSize = querySchema.fetchsizeSpecified ? querySchema.fetchsize : -1;
					bool readOnly = querySchema.readonlySpecified ? querySchema.@readonly : false;
					string comment = null;
					bool callable = querySchema.callableSpecified ? querySchema.callable : false;
					string resultSetRef = querySchema.resultsetref;

					FlushMode flushMode = FlushModeConverter.GetFlushMode(querySchema);
					CacheMode? cacheMode = (querySchema.cachemodeSpecified)
												? CacheModeConverter.GetCacheMode(querySchema.cachemode)
												: null;

					IDictionary<string,string> parameterTypes = new LinkedHashMap<string,string>();
					IList<string> synchronizedTables = GetSynchronizedTables(querySchema);

					NamedSQLQueryDefinition namedQuery;

					if (string.IsNullOrEmpty(resultSetRef))
					{
						ResultSetMappingDefinition definition =
							new ResultSetMappingBinder(this).Create(querySchema);

						namedQuery = new NamedSQLQueryDefinition(queryText,
							definition.GetQueryReturns(), synchronizedTables, cacheable, region, timeout,
							fetchSize, flushMode, cacheMode, readOnly, comment, parameterTypes, callable);
					}
					else
						// TODO: check there is no actual definition elemnents when a ref is defined
						namedQuery = new NamedSQLQueryDefinition(queryText,
							resultSetRef, synchronizedTables, cacheable, region, timeout, fetchSize,
							flushMode, cacheMode, readOnly, comment, parameterTypes, callable);

					log.DebugFormat("Named SQL query: {0} -> {1}", queryName, namedQuery.QueryString);
					mappings.AddSQLQuery(queryName, namedQuery);
				});
		}

		private static IList<string> GetSynchronizedTables(HbmSqlQuery querySchema)
		{
			IList<string> synchronizedTables = new List<string>();

			foreach (object item in querySchema.Items ?? new object[0])
			{
				HbmSynchronize synchronizeSchema = item as HbmSynchronize;

				if (synchronizeSchema != null)
					synchronizedTables.Add(synchronizeSchema.table);
			}

			return synchronizedTables;
		}
	}
}