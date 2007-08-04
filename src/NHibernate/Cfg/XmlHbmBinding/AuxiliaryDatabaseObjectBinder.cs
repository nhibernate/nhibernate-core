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
			string sqlCreateString = GetInnerText(SelectSingleNode(node, HbmConstants.nsCreate));
			string sqlDropString = GetInnerText(SelectSingleNode(node, HbmConstants.nsDrop));

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

				foreach (XmlNode parameterNode in definitionNode.ChildNodes)
				{
					string name = GetAttributeValue(parameterNode, "name");
					string text = GetInnerText(parameterNode) ?? "";

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
			foreach (XmlNode dialectScopeNode in SelectNodes(node, HbmConstants.nsDialectScope))
			{
				string dialectName = GetAttributeValue(dialectScopeNode, "name");
				auxDbObject.AddDialectScope(dialectName);
			}
		}
	}
}