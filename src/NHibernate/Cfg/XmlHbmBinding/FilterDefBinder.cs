using System.Collections;
using System.Xml;

using NHibernate.Engine;
using NHibernate.Type;

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

			log.Debug(string.Format("Parsing filter-def [{0}]", name));
			
			string defaultCondition = GetInnerText(node) ?? GetAttributeValue(node, "condition");
			Hashtable parameterTypes = GetFilterParameterTypes(node);
			
			log.Debug(string.Format("Parsed filter-def [{0}]", name));
			
			FilterDefinition def = new FilterDefinition(name, defaultCondition, parameterTypes);
			mappings.AddFilterDefinition(def);
		}

		private Hashtable GetFilterParameterTypes(XmlNode node)
		{
			Hashtable paramMappings = new Hashtable();

			foreach (XmlNode param in SelectNodes(node, HbmConstants.nsFilterParam))
			{
				string paramName = GetAttributeValue(param, "name");
				string paramType = GetAttributeValue(param, "type");

				log.Debug(string.Format("adding filter parameter : {0} -> {1}", paramName, paramType));
			
				IType heuristicType = TypeFactory.HeuristicType(paramType);
				
				log.Debug(string.Format("parameter heuristic type : {0}", heuristicType));

				paramMappings.Add(paramName, heuristicType);
			}

			return paramMappings;
		}
	}
}