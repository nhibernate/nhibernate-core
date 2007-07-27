using System.Collections;
using System.Xml;

using NHibernate.Engine;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Cfg.XmlHbmBinding
{
	public class FilterDefBinder : Binder
	{
		public FilterDefBinder(Binder parent)
			: base(parent)
		{
		}

		public FilterDefBinder(Mappings mappings, XmlNamespaceManager namespaceManager)
			: base(mappings, namespaceManager)
		{
		}

		public override void Bind(XmlNode node)
		{
			string name = GetPropertyName(node);
			log.Debug("Parsing filter-def [" + name + "]");
			string defaultCondition = node.InnerText;

			if (defaultCondition == null || StringHelper.IsEmpty(defaultCondition.Trim()))
				if (node.Attributes != null)
				{
					XmlAttribute propertyNameNode = node.Attributes["condition"];
					defaultCondition = (propertyNameNode == null) ? null : propertyNameNode.Value;
				}

			Hashtable paramMappings = new Hashtable();

			foreach (XmlNode param in node.SelectNodes(HbmConstants.nsFilterParam, namespaceManager))
			{
				string paramName = GetPropertyName(param);
				string paramType = param.Attributes["type"].Value;
				log.Debug("adding filter parameter : " + paramName + " -> " + paramType);
				IType heuristicType = TypeFactory.HeuristicType(paramType);
				log.Debug("parameter heuristic type : " + heuristicType);
				paramMappings.Add(paramName, heuristicType);
			}

			log.Debug("Parsed filter-def [" + name + "]");
			FilterDefinition def = new FilterDefinition(name, defaultCondition, paramMappings);
			mappings.AddFilterDefinition(def);
		}
	}
}