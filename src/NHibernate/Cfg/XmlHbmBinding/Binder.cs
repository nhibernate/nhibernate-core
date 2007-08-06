using System;
using System.Collections;
using System.Xml;

using log4net;

using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Cfg.XmlHbmBinding
{
	public abstract class Binder
	{
		protected static readonly ILog log = LogManager.GetLogger(typeof (Binder));
		protected readonly Mappings mappings;
		protected readonly XmlNamespaceManager namespaceManager;

		protected Binder(Binder parent)
		{
			if (parent == null)
				throw new ArgumentNullException("parent");

			mappings = parent.mappings;
			namespaceManager = parent.namespaceManager;
		}

		protected Binder(Mappings mappings, XmlNamespaceManager namespaceManager)
		{
			if (mappings == null)
				throw new ArgumentNullException("mappings");

			this.mappings = mappings;
			this.namespaceManager = namespaceManager;
		}

		protected static void LogDebug(string format, params object[] args)
		{
			log.Debug(string.Format(format, args));
		}

		protected static void LogInfo(string format, params object[] args)
		{
			log.Info(string.Format(format, args));
		}

		#region XML helper methods

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

		#endregion

		protected static bool IsInNHibernateNamespace(XmlNode node)
		{
			return node.NamespaceURI == Configuration.MappingSchemaXMLNS;
		}

		protected IType GetTypeFromXML(XmlNode node)
		{
			IType type;

			IDictionary parameters = null;

			XmlAttribute typeAttribute = node.Attributes["type"];
			if (typeAttribute == null)
			{
				typeAttribute = node.Attributes["id-type"]; //for an any
			}
			string typeName;
			if (typeAttribute != null)
			{
				typeName = typeAttribute.Value;
			}
			else
			{
				XmlNode typeNode = node.SelectSingleNode(HbmConstants.nsType, namespaceManager);
				if (typeNode == null) //we will have to use reflection
					return null;
				XmlAttribute nameAttribute = typeNode.Attributes["name"]; //we know it exists because the schema validate it
				typeName = nameAttribute.Value;
				parameters = new Hashtable();
				foreach (XmlNode childNode in typeNode.ChildNodes)
				{
					parameters.Add(childNode.Attributes["name"].Value,
						childNode.InnerText.Trim());
				}
			}
			type = TypeFactory.HeuristicType(typeName, parameters);
			if (type == null)
			{
				throw new MappingException("could not interpret type: " + typeAttribute.Value);
			}
			return type;
		}

		protected static string GetEntityName(XmlNode elem, Mappings model)
		{
			//string entityName = XmlHelper.GetAttributeValue(elem, "entity-name");
			//return entityName == null ? GetClassName( elem.Attributes[ "class" ], model ) : entityName;
			return GetClassName(elem.Attributes["class"], model);
		}

		/// <summary>
		/// Converts a partial class name into a fully qualified one
		/// </summary>
		/// <param name="className"></param>
		/// <param name="mapping"></param>
		/// <returns></returns>
		protected static string FullClassName(string className, Mappings mapping)
		{
			if (className == null)
			{
				return null;
			}

			return TypeNameParser.Parse(className, mapping.DefaultNamespace, mapping.DefaultAssembly)
				.ToString();
		}

		/// <summary>
		/// Attempts to find a type by its full name. Throws a <see cref="MappingException" />
		/// using the provided <paramref name="errorMessage" /> in case of failure.
		/// </summary>
		/// <param name="fullName">name of the class to find</param>
		/// <param name="errorMessage">Error message to use for
		/// the <see cref="MappingException" /> in case of failure. Should contain
		/// the <c>{0}</c> formatting placeholder.</param>
		/// <returns>A <see cref="System.Type" /> instance.</returns>
		/// <exception cref="MappingException">
		/// Thrown when there is an error loading the class.
		/// </exception>
		protected static System.Type ClassForFullNameChecked(string fullName, string errorMessage)
		{
			try
			{
				return ReflectHelper.ClassForName(fullName);
			}
			catch (Exception e)
			{
				throw new MappingException(String.Format(errorMessage, fullName), e);
			}
		}

		/// <summary>
		/// Similar to <see cref="ClassForFullNameChecked" />, but handles short class names
		/// by calling <see cref="FullClassName" />.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="mappings"></param>
		/// <param name="errorMessage"></param>
		/// <returns></returns>
		protected static System.Type ClassForNameChecked(string name, Mappings mappings, string errorMessage)
		{
			return ClassForFullNameChecked(FullClassName(name, mappings), errorMessage);
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

		private static string GetClassName(XmlAttribute att, Mappings model)
		{
			if (att == null)
			{
				return null;
			}
			return GetClassName(att.Value, model);
		}

		private static string GetClassName(string unqualifiedName, Mappings model)
		{
			return ClassForNameChecked(unqualifiedName, model, "unknown class {0}").AssemblyQualifiedName;
			//return TypeNameParser.Parse(unqualifiedName, model.DefaultNamespace, model.DefaultAssembly).ToString();
		}

	}
}