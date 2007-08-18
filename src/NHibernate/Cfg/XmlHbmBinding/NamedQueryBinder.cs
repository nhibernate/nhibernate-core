using System.Collections;
using System.Xml;

using NHibernate.Engine;

namespace NHibernate.Cfg.XmlHbmBinding
{
	public class NamedQueryBinder : QueryBinder
	{
		public NamedQueryBinder(XmlBinder parent)
			: base(parent)
		{
		}

		public NamedQueryBinder(Mappings mappings, XmlNamespaceManager namespaceManager)
			: base(mappings, namespaceManager)
		{
		}

		public void BindEach(XmlNode parentNode, string xpath)
		{
			foreach (XmlNode node in SelectNodes(parentNode, xpath))
				Bind(node);
		}

		public void Bind(XmlNode node)
		{
			string queryName = GetAttributeValue(node, "name");
			string queryText = GetInnerText(node);

			log.DebugFormat("Named query: {0} -> {1}", queryName, queryText);

			bool cacheable = "true".Equals(GetAttributeValue(node, "cacheable"));
			string region = GetAttributeValue(node, "cache-region");
			int timeout = int.Parse(GetAttributeValue(node, "timeout") ?? "-1");
			int fetchSize = int.Parse(GetAttributeValue(node, "fetch-size") ?? "-1");
			bool readOnly = "true".Equals(GetAttributeValue(node, "read-only") ?? "true");
			string comment = GetAttributeValue(node, "comment");
			FlushMode flushMode = GetFlushMode(GetAttributeValue(node, "flush-mode"));

			IDictionary parameterTypes = GetParameterTypes(node);

			NamedQueryDefinition namedQuery = new NamedQueryDefinition(queryText, cacheable, region, timeout,
				fetchSize, flushMode, readOnly, comment, parameterTypes);

			mappings.AddQuery(queryName, namedQuery);
		}
	}
}