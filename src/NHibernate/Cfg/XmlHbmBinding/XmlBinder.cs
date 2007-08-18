using System.Collections;
using System.Xml;

using NHibernate.Type;

namespace NHibernate.Cfg.XmlHbmBinding
{
	public class XmlBinder : Binder
	{
		protected readonly XmlNamespaceManager namespaceManager;

		public XmlBinder(XmlBinder parent)
			: base(parent)
		{
			namespaceManager = parent.namespaceManager;
		}

		public XmlBinder(Mappings mappings, XmlNamespaceManager namespaceManager)
			: base(mappings)
		{
			this.namespaceManager = namespaceManager;
		}

		protected IType GetTypeFromXML(XmlNode node)
		{
			IType type;

			IDictionary parameters = null;

			XmlAttribute typeAttribute = node.Attributes["type"];
			if (typeAttribute == null)
				typeAttribute = node.Attributes["id-type"]; //for an any
			string typeName;
			if (typeAttribute != null)
				typeName = typeAttribute.Value;
			else
			{
				XmlNode typeNode = node.SelectSingleNode(HbmConstants.nsType, namespaceManager);
				if (typeNode == null) //we will have to use reflection
					return null;
				XmlAttribute nameAttribute = typeNode.Attributes["name"]; //we know it exists because the schema validate it
				typeName = nameAttribute.Value;
				parameters = new Hashtable();
				foreach (XmlNode childNode in typeNode.ChildNodes)
					parameters.Add(childNode.Attributes["name"].Value,
						childNode.InnerText.Trim());
			}
			type = TypeFactory.HeuristicType(typeName, parameters);
			if (type == null)
				throw new MappingException("could not interpret type: " + typeAttribute.Value);
			return type;
		}

		protected XmlNodeList SelectNodes(XmlNode node, string xpath)
		{
			return node.SelectNodes(xpath, namespaceManager);
		}

		protected XmlNode SelectSingleNode(XmlNode node, string xpath)
		{
			return node.SelectSingleNode(xpath, namespaceManager);
		}

		protected static string GetAttributeValue(XmlNode node, string attributeName)
		{
			if (node != null && node.Attributes != null)
			{
				XmlAttribute attribute = node.Attributes[attributeName];
				return attribute == null ? null : attribute.Value;
			}
			else
				return null;
		}

		protected static string GetInnerText(XmlNode node)
		{
			if (node == null || node.InnerText == null || node.InnerText.Trim().Length == 0)
				return null;
			else
				return node.InnerText.Trim();
		}

		protected static bool IsInNHibernateNamespace(XmlNode node)
		{
			return node.NamespaceURI == Configuration.MappingSchemaXMLNS;
		}

		protected static string GetEntityName(XmlNode elem, Mappings model)
		{
			//string entityName = XmlHelper.GetAttributeValue(elem, "entity-name");
			//return entityName == null ? GetClassName( elem.Attributes[ "class" ], model ) : entityName;
			return GetClassName(elem.Attributes["class"], model);
		}

		private static string GetClassName(XmlAttribute att, Mappings model)
		{
			if (att == null)
				return null;
			return GetClassName(att.Value, model);
		}

		protected static string GetPropertyName(XmlNode node)
		{
			if (node.Attributes != null)
			{
				XmlAttribute propertyNameNode = node.Attributes["name"];
				return (propertyNameNode == null) ? null : propertyNameNode.Value;
			}
			return null;
		}
	}
}