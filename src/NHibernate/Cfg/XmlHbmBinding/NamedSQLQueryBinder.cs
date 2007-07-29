using System.Collections;
using System.Xml;

using NHibernate.Engine;
using NHibernate.Util;

namespace NHibernate.Cfg.XmlHbmBinding
{
	public class NamedSQLQueryBinder : Binder
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
			DelegatingSecondPass secondPass = new DelegatingSecondPass(delegate { SecondPassBind(node); });
			mappings.AddSecondPass(secondPass);
		}

		private void SecondPassBind(XmlNode node)
		{
			string queryName = node.Attributes["name"].Value;
			bool cacheable = "true".Equals(XmlHelper.GetAttributeValue(node, "cacheable"));
			string region = XmlHelper.GetAttributeValue(node, "cache-region");
			XmlAttribute tAtt = node.Attributes["timeout"];
			int timeout = tAtt == null ? -1 : int.Parse(tAtt.Value);
			XmlAttribute fsAtt = node.Attributes["fetch-size"];
			int fetchSize = fsAtt == null ? -1 : int.Parse(fsAtt.Value);
			XmlAttribute roAttr = node.Attributes["read-only"];
			bool readOnly = roAttr != null && "true".Equals(roAttr.Value);
			XmlAttribute cacheModeAtt = node.Attributes["cache-mode"];
			string cacheMode = cacheModeAtt == null ? null : cacheModeAtt.Value;
			XmlAttribute cmAtt = node.Attributes["comment"];
			string comment = cmAtt == null ? null : cmAtt.Value;

			IList synchronizedTables = new ArrayList();

			foreach (XmlNode item in node.SelectNodes(HbmConstants.nsSynchronize, namespaceManager))
				synchronizedTables.Add(XmlHelper.GetAttributeValue(item, "table"));
			bool callable = "true".Equals(XmlHelper.GetAttributeValue(node, "callable"));

			NamedSQLQueryDefinition namedQuery;
			XmlAttribute @ref = node.Attributes["resultset-ref"];
			string resultSetRef = @ref == null ? null : @ref.Value;
			if (StringHelper.IsNotEmpty(resultSetRef))
				namedQuery = new NamedSQLQueryDefinition(
					node.InnerText,
					resultSetRef,
					synchronizedTables,
					cacheable,
					region,
					timeout,
					fetchSize,
					GetFlushMode(XmlHelper.GetAttributeValue(node, "flush-mode")),
					//HbmBinder.GetCacheMode(cacheMode),
					readOnly,
					comment,
					GetParameterTypes(node),
					callable
					);
				//TODO check there is no actual definition elemnents when a ref is defined
			else
			{
				ResultSetMappingDefinition definition =
					ResultSetMappingBinder.BuildResultSetMappingDefinition(node, null, mappings);
				namedQuery = new NamedSQLQueryDefinition(
					node.InnerText,
					definition.GetQueryReturns(),
					synchronizedTables,
					cacheable,
					region,
					timeout,
					fetchSize,
					GetFlushMode(XmlHelper.GetAttributeValue(node, "flush-mode")),
					readOnly,
					comment,
					GetParameterTypes(node),
					callable
					);
			}

			log.Debug("Named SQL query: " + queryName + " -> " + namedQuery.QueryString);
			mappings.AddSQLQuery(queryName, namedQuery);
		}
	}
}