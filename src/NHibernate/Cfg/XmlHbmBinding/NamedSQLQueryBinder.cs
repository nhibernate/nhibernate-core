using System.Collections;
using System.Xml;

using NHibernate.Engine;

namespace NHibernate.Cfg.XmlHbmBinding
{
	public class NamedSQLQueryBinder : QueryBinder
	{
		public NamedSQLQueryBinder(Mappings mappings, XmlNamespaceManager namespaceManager)
			: base(mappings, namespaceManager)
		{
		}

		public NamedSQLQueryBinder(Binder parent)
			: base(parent)
		{
		}

		public override void Bind(XmlNode node)
		{
			mappings.AddSecondPass(new DelegatingSecondPass(delegate { SecondPassBind(node); }));
		}

		private void SecondPassBind(XmlNode node)
		{
			string queryName = GetAttributeValue(node, "name");
			string queryText = GetInnerText(node);

			bool cacheable = "true".Equals(GetAttributeValue(node, "cacheable"));
			string region = GetAttributeValue(node, "cache-region");
			int timeout = int.Parse(GetAttributeValue(node, "timeout") ?? "-1");
			int fetchSize = int.Parse(GetAttributeValue(node, "fetch-size") ?? "-1");
			bool readOnly = "true".Equals(GetAttributeValue(node, "read-only") ?? "true");
			string comment = GetAttributeValue(node, "comment");
			FlushMode flushMode = GetFlushMode(GetAttributeValue(node, "flush-mode"));
			bool callable = "true".Equals(GetAttributeValue(node, "callable"));
			string resultSetRef = GetAttributeValue(node, "resultset-ref");

			IDictionary parameterTypes = GetParameterTypes(node);
			IList synchronizedTables = GetSynchronizedTables(node);

			if (!string.IsNullOrEmpty(resultSetRef))
			{
				// TODO: check there is no actual definition elemnents when a ref is defined

				NamedSQLQueryDefinition namedQuery = new NamedSQLQueryDefinition(queryText, resultSetRef,
					synchronizedTables, cacheable, region, timeout, fetchSize, flushMode, readOnly, comment,
					parameterTypes, callable);

				LogDebug("Named SQL query: {0} -> {1}", queryName, namedQuery.QueryString);
				mappings.AddSQLQuery(queryName, namedQuery);
			}
			else
			{
				ResultSetMappingDefinition definition =
					ResultSetMappingBinder.BuildResultSetMappingDefinition(node, null, mappings);

				NamedSQLQueryDefinition namedQuery = new NamedSQLQueryDefinition(queryText,
					definition.GetQueryReturns(),
					synchronizedTables, cacheable, region, timeout, fetchSize, flushMode, readOnly, comment,
					parameterTypes, callable);

				LogDebug("Named SQL query: {0} -> {1}", queryName, namedQuery.QueryString);
				mappings.AddSQLQuery(queryName, namedQuery);
			}
		}

		private IList GetSynchronizedTables(XmlNode node)
		{
			IList synchronizedTables = new ArrayList();

			foreach (XmlNode synchronizeNode in SelectNodes(node, HbmConstants.nsSynchronize))
			{
				string table = GetAttributeValue(synchronizeNode, "table");
				synchronizedTables.Add(table);
			}

			return synchronizedTables;
		}
	}
}