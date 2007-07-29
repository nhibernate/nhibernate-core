using System.Collections;
using System.Xml;

using NHibernate.Util;

namespace NHibernate.Cfg.XmlHbmBinding
{
	public abstract class QueryBinder : Binder
	{
		public QueryBinder(Mappings mappings, XmlNamespaceManager namespaceManager)
			: base(mappings, namespaceManager)
		{
		}

		public QueryBinder(Binder parent)
			: base(parent)
		{
		}

		protected static FlushMode GetFlushMode(string flushModeText)
		{
			switch (flushModeText)
			{
				case null:
					return FlushMode.Unspecified;
				case "auto":
					return FlushMode.Auto;
				case "commit":
					return FlushMode.Commit;
				case "never":
					return FlushMode.Never;
				default:
					throw new MappingException("unknown flushmode " + flushModeText);
			}
		}

		protected IDictionary GetParameterTypes(XmlNode node)
		{
			IDictionary parameterTypes = new SequencedHashMap();

			foreach (XmlNode subNode in SelectNodes(node, HbmConstants.nsQueryParam))
			{
				string name = GetAttributeValue(subNode, "name");
				string type = GetAttributeValue(subNode, "type");

				parameterTypes[name] = type;
			}

			return parameterTypes;
		}
	}
}