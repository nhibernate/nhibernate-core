using System;
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

		public void AddSqlQuery(HbmSqlQuery querySchema)
		{
			mappings.AddSecondPass(delegate
				{
					string queryName = querySchema.name;
					string queryText = querySchema.GetText();
					bool cacheable = querySchema.cacheable;
					string region = querySchema.cacheregion;
					int timeout = string.IsNullOrEmpty(querySchema.timeout) ? RowSelection.NoValue : int.Parse(querySchema.timeout);
					int fetchSize = querySchema.fetchsizeSpecified ? querySchema.fetchsize : -1;
					bool readOnly = querySchema.readonlySpecified ? querySchema.@readonly : false;
					string comment = null;
					bool callable = querySchema.callable;
					string resultSetRef = querySchema.resultsetref;

					FlushMode flushMode = FlushModeConverter.GetFlushMode(querySchema);
					CacheMode? cacheMode = (querySchema.cachemodeSpecified)
												? querySchema.cachemode.ToCacheMode()
												: null;

					var parameterTypes = new LinkedHashMap<string, string>();
					var synchronizedTables = GetSynchronizedTables(querySchema);

					NamedSQLQueryDefinition namedQuery;

					if (string.IsNullOrEmpty(resultSetRef))
					{
						ResultSetMappingDefinition definition =
							new ResultSetMappingBinder(Mappings).Create(querySchema);

						namedQuery = new NamedSQLQueryDefinition(queryText,
							definition.GetQueryReturns(), synchronizedTables, cacheable, region, timeout,
							fetchSize, flushMode, cacheMode, readOnly, comment, parameterTypes, callable);
					}
					else
						// TODO: check there is no actual definition elemnents when a ref is defined
						namedQuery = new NamedSQLQueryDefinition(queryText,
							resultSetRef, synchronizedTables, cacheable, region, timeout, fetchSize,
							flushMode, cacheMode, readOnly, comment, parameterTypes, callable);

					log.Debug("Named SQL query: {0} -> {1}", queryName, namedQuery.QueryString);
					mappings.AddSQLQuery(queryName, namedQuery);
				});
		}

		private static List<string> GetSynchronizedTables(HbmSqlQuery querySchema)
		{
			var synchronizedTables = new List<string>();

			foreach (object item in querySchema.Items ?? Array.Empty<object>())
			{
				if (item is HbmSynchronize synchronizeSchema)
					synchronizedTables.Add(synchronizeSchema.table);
			}

			return synchronizedTables;
		}
	}
}
