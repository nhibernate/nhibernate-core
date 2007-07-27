using System.Xml;

using NHibernate.Engine;
using NHibernate.Util;

namespace NHibernate.Cfg.XmlHbmBinding
{
	public class NamedQueryBinder : Binder
	{
		public NamedQueryBinder(Binder parent)
			: base(parent)
		{
		}

		public NamedQueryBinder(Mappings mappings, XmlNamespaceManager namespaceManager)
			: base(mappings, namespaceManager)
		{
		}

		public override void Bind(XmlNode node)
		{
			string path = null;

			string queryName = node.Attributes["name"].Value;
			if (path != null)
			{
				queryName = path + '.' + queryName;
			}
			string query = node.InnerText;
			log.Debug("Named query: " + queryName + " -> " + query);

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

			NamedQueryDefinition namedQuery = new NamedQueryDefinition(
				query,
				cacheable,
				region,
				timeout,
				fetchSize,
				GetFlushMode(XmlHelper.GetAttributeValue(node, "flush-mode")),
				//GetCacheMode(cacheMode),
				readOnly,
				comment,
				GetParameterTypes(node)
				);

			mappings.AddQuery(queryName, namedQuery);
		}

	}
}