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
			XmlNode definitionNode = SelectSingleNode(node, HbmConstants.nsDefinition);

			if (definitionNode == null)
			{
				IAuxiliaryDatabaseObject simpleObject = CreateSimpleObject(node);
				AddDialectScopes(node, simpleObject);
				mappings.AddAuxiliaryDatabaseObject(simpleObject);
			}
			else
			{
				IAuxiliaryDatabaseObject customObject = CreateCustomObject(definitionNode);
				AddDialectScopes(node, customObject);
				mappings.AddAuxiliaryDatabaseObject(customObject);
			}
		}

		private IAuxiliaryDatabaseObject CreateSimpleObject(XmlNode node)
		{
			XmlNode createNode = SelectSingleNode(node, HbmConstants.nsCreate);
			string sqlCreateString = createNode == null ? null : createNode.InnerText.Trim();

			XmlNode dropNode = SelectSingleNode(node, HbmConstants.nsDrop);
			string sqlDropString = dropNode == null ? null : dropNode.InnerText.Trim();

			return new SimpleAuxiliaryDatabaseObject(sqlCreateString, sqlDropString);
		}

		private static IAuxiliaryDatabaseObject CreateCustomObject(XmlNode definitionNode)
		{
			string className = GetAttributeValue(definitionNode, "class");

			try
			{
				System.Type type = ReflectHelper.ClassForName(className);
				IAuxiliaryDatabaseObject auxDbObject = (IAuxiliaryDatabaseObject) Activator.CreateInstance(type);

				Hashtable parameters = new Hashtable();

				foreach (XmlNode childNode in definitionNode.ChildNodes)
				{
					string name = childNode.Attributes["name"].Value;
					string text = childNode.InnerText.Trim();

					parameters.Add(name, text);
				}

				auxDbObject.SetParameterValues(parameters);
				return auxDbObject;
			}
			catch (TypeLoadException exception)
			{
				throw new MappingException(string.Format(
					"Could not locate custom database object class [{0}].", className), exception);
			}
			catch (Exception exception)
			{
				throw new MappingException(string.Format(
					"Could not instantiate custom database object class [{0}].", className), exception);
			}
		}

		private void AddDialectScopes(XmlNode node, IAuxiliaryDatabaseObject auxDbObject)
		{
			foreach (XmlNode dialectScope in SelectNodes(node, HbmConstants.nsDialectScope))
			{
				string dialectName = GetAttributeValue(dialectScope, "name");
				auxDbObject.AddDialectScope(dialectName);
			}
		}
	}
}