using System;
using System.Collections;
using System.Xml;

using NHibernate.Mapping;
using NHibernate.Util;

namespace NHibernate.Cfg.XmlHbmBinding
{
	public class AuxiliaryDatabaseObjectBinder : Binder
	{
		public AuxiliaryDatabaseObjectBinder(Mappings mappings, XmlNamespaceManager namespaceManager)
			: base(mappings, namespaceManager)
		{
		}

		public AuxiliaryDatabaseObjectBinder(Binder parent)
			: base(parent)
		{
		}

		public override void Bind(XmlNode node)
		{
			IAuxiliaryDatabaseObject auxDbObject;
			XmlNode definitionNode = node.SelectSingleNode(HbmConstants.nsDefinition, namespaceManager);

			if (definitionNode != null)
			{
				string className = XmlHelper.GetAttributeValue(definitionNode, "class");

				try
				{
					auxDbObject = (IAuxiliaryDatabaseObject) Activator.CreateInstance(ReflectHelper.ClassForName(className));
					Hashtable parameters = new Hashtable();
					foreach (XmlNode childNode in definitionNode.ChildNodes)
						parameters.Add(childNode.Attributes["name"].Value, childNode.InnerText.Trim());
					auxDbObject.SetParameterValues(parameters);
				}
				catch (TypeLoadException e)
				{
					throw new MappingException(
						"could not locate custom database object class [" +
							className + "]", e
						);
				}
				catch (Exception t)
				{
					throw new MappingException(
						"could not instantiate custom database object class [" +
							className + "]", t
						);
				}
			}
			else
				auxDbObject = new SimpleAuxiliaryDatabaseObject(
					XmlHelper.ElementTextTrim(node, HbmConstants.nsCreate, namespaceManager),
					XmlHelper.ElementTextTrim(node, HbmConstants.nsDrop, namespaceManager)
					);

			foreach (XmlNode dialectScoping in node.SelectNodes(HbmConstants.nsDialectScope, namespaceManager))
				auxDbObject.AddDialectScope(XmlHelper.GetAttributeValue(dialectScoping, "name"));

			mappings.AddAuxiliaryDatabaseObject(auxDbObject);
		}
	}
}