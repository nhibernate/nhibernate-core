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

		public void BindEach(XmlNode parentNode, string xpath)
		{
			foreach (XmlNode node in SelectNodes(parentNode, xpath))
				Bind(node);
		}

		public void Bind(XmlNode node)
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
			string customTypeName = GetAttributeValue(definitionNode, "class");

			try
			{
				System.Type customType = ReflectHelper.ClassForName(customTypeName);
				
				IAuxiliaryDatabaseObject customObject =
					(IAuxiliaryDatabaseObject) Activator.CreateInstance(customType);

				// TODO: Move Build/Set parameters statements up into Bind method? Do they need to be in the
				// try/catch block?
				IDictionary parameters = BuildParameterCollection(definitionNode);
				customObject.SetParameterValues(parameters);

				return customObject;
			}
			catch (TypeLoadException exception)
			{
				throw new MappingException(string.Format(
					"Could not locate custom database object class [{0}].", customTypeName), exception);
			}
			catch (Exception exception)
			{
				throw new MappingException(string.Format(
					"Could not instantiate custom database object class [{0}].", customTypeName), exception);
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

		private static IDictionary BuildParameterCollection(XmlNode definitionNode)
		{
			// TODO: Change type to Dictionary<string, string>? Would this have any side effects due to its
			// different behavior when a requested key is not in the dictionary?

			Hashtable parameters = new Hashtable();

			foreach (XmlNode parameterNode in definitionNode.ChildNodes)
			{
				string name = GetAttributeValue(parameterNode, "name");
				string text = GetInnerText(parameterNode) ?? "";

				parameters.Add(name, text);
			}

			return parameters;
		}
	}
}