using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

using log4net;
using NHibernate.Mapping;
using NHibernate.Type;
using NHibernate.Util;
using NHibernate.Cfg.MappingSchema;

namespace NHibernate.Cfg.XmlHbmBinding
{
	public abstract class Binder
	{
		/// <summary>The XML Namespace for the nhibernate-mapping</summary>
		public const string MappingSchemaXMLNS = "urn:nhibernate-mapping-2.2";

		protected static readonly ILog log = LogManager.GetLogger(typeof (Binder));

		protected static readonly IDictionary<string, MetaAttribute> EmptyMeta =
			new CollectionHelper.EmptyMapClass<string, MetaAttribute>();

		protected readonly Mappings mappings;

		protected Binder(Mappings mappings)
		{
			if (mappings == null)
				throw new ArgumentNullException("mappings");

			this.mappings = mappings;
		}

		public Mappings Mappings
		{
			get { return mappings; }
		}

		/// <summary>
		/// Converts a partial class name into a fully qualified one
		/// </summary>
		/// <param name="className"></param>
		/// <param name="mappings"></param>
		/// <returns></returns>
		protected static string FullQualifiedClassName(string className, Mappings mappings)
		{
			if (className == null)
				return null;

			return TypeNameParser.Parse(className, mappings.DefaultNamespace, mappings.DefaultAssembly)
				.ToString();
		}

		/// <summary>
		/// Converts a partial class name into a fully one
		/// </summary>
		/// <param name="className"></param>
		/// <param name="mappings"></param>
		/// <returns>The class FullName (without the assembly)</returns>
		/// <remarks>
		/// The FullName is equivalent to the default entity-name
		/// </remarks>
		protected static string FullClassName(string className, Mappings mappings)
		{
			if (className == null)
				return null;

			return TypeNameParser.Parse(className, mappings.DefaultNamespace, mappings.DefaultAssembly).Type;
		}

		protected static bool NeedQualifiedClassName(string className)
		{
			if(string.IsNullOrEmpty(className))
			{
				return false;
			}
			if(className.IndexOf('.') > 0)
			{
				return false;
			}
			if(TypeFactory.Basic(className) != null)
			{
				return false;
			}

			return true;
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
		/// by calling <see cref="FullQualifiedClassName" />.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="mappings"></param>
		/// <param name="errorMessage"></param>
		/// <returns></returns>
		protected static System.Type ClassForNameChecked(string name, Mappings mappings, string errorMessage)
		{
			return ClassForFullNameChecked(FullQualifiedClassName(name, mappings), errorMessage);
		}

		protected static string GetClassName(string unqualifiedName, Mappings mappings)
		{
			return ClassForNameChecked(unqualifiedName, mappings, "unknown class {0}").FullName;
			//return TypeNameParser.Parse(unqualifiedName, mappings.DefaultNamespace, mappings.DefaultAssembly).ToString();
		}

		protected static string GetQualifiedClassName(string unqualifiedName, Mappings mappings)
		{
			return ClassForNameChecked(unqualifiedName, mappings, "unknown class {0}").AssemblyQualifiedName;
		}

		protected static T Deserialize<T>(XmlNode node)
		{
			using (StringReader reader = new StringReader(node.OuterXml))
				return (T) new XmlSerializer(typeof (T)).Deserialize(reader);
		}

		protected static XmlNode Serialize<T>(T hbmElement)
		{
			// TODO : this method is only for TEMPORAL usage; should be removed after refactorize all binders
			var xmlTypeMapping = typeof (T);
			return Serialize(xmlTypeMapping, hbmElement);
		}

		protected static XmlNode Serialize(System.Type xmlTypeMapping, object hbmElement)
		{
			// TODO : this method is only for TEMPORAL usage; should be removed after refactorize all binders
			var serializer = new XmlSerializer(xmlTypeMapping);
			using (var memStream = new MemoryStream(2000))
			using (var xmlWriter = XmlWriter.Create(memStream))
			{
				serializer.Serialize(xmlWriter, hbmElement);
				memStream.Position = 0;
				using (XmlReader reader = XmlReader.Create(memStream))
				{
					var hbmDocument = new XmlDocument();
					hbmDocument.Load(reader);
					// note that the prefix has absolutely nothing to do with what the user
					// selects as their prefix in the document.  It is the prefix we use to 
					// build the XPath and the nsmgr takes care of translating our prefix into
					// the user defined prefix...
					var namespaceManager = new XmlNamespaceManager(hbmDocument.NameTable);
					namespaceManager.AddNamespace(HbmConstants.nsPrefix, MappingSchemaXMLNS);

					return hbmDocument.DocumentElement;
				}
			}
		}

		protected static string GetXmlEnumAttribute(Enum cascadeStyle)
		{
			MemberInfo[] memberInfo = cascadeStyle.GetType().GetMember(cascadeStyle.ToString());

			if (memberInfo != null && memberInfo.Length == 1)
			{
				object[] customAttributes = memberInfo[0].GetCustomAttributes(typeof(XmlEnumAttribute), false);

				if (customAttributes.Length == 1)
					return ((XmlEnumAttribute)customAttributes[0]).Name;
			}

			return null;
		}

		protected static bool IsTrue(string xmlNodeValue)
		{
			return "true".Equals(xmlNodeValue) || "1".Equals(xmlNodeValue);
		}

		protected static bool IsFalse(string xmlNodeValue)
		{
			return "false".Equals(xmlNodeValue) || "0".Equals(xmlNodeValue);
		}

		protected static string GetAttributeValue(XmlNode node, string attributeName)
		{
			XmlAttribute att = node.Attributes[attributeName];
			return att != null ? att.Value : null;
		}

		public static IDictionary<string, MetaAttribute> GetMetas(IDecoratable decoratable, IDictionary<string, MetaAttribute> inheritedMeta)
		{
			return GetMetas(decoratable, inheritedMeta, false);
		}

		public static IDictionary<string, MetaAttribute> GetMetas(IDecoratable decoratable, IDictionary<string, MetaAttribute> inheritedMeta, bool onlyInheritable)
		{
			if(decoratable == null)
			{
				return EmptyMeta;
			}
			var map = new Dictionary<string, MetaAttribute>(inheritedMeta);

			IDictionary<string, MetaAttribute> metaAttributes = onlyInheritable
			                                                    	? decoratable.InheritableMetaData
			                                                    	: decoratable.MappedMetaData;

			foreach (var metaAttribute in metaAttributes)
			{
				string name = metaAttribute.Key;

				MetaAttribute meta;
				MetaAttribute inheritedAttribute;

				map.TryGetValue(name, out meta);
				inheritedMeta.TryGetValue(name, out inheritedAttribute);

				if (meta == null)
				{
					meta = new MetaAttribute(name);
					map[name] = meta;
				}
				else if (meta == inheritedAttribute)
				{
					// overriding inherited meta attribute.
					meta = new MetaAttribute(name);
					map[name] = meta;
				}
				meta.AddValues(metaAttribute.Value.Values);
			}
			return map;
		}

		public static IDictionary<string, MetaAttribute> GetMetas(XmlNodeList nodes, IDictionary<string, MetaAttribute> inheritedMeta)
		{
			return GetMetas(nodes, inheritedMeta, false);
		}

		public static IDictionary<string, MetaAttribute> GetMetas(XmlNodeList nodes, IDictionary<string, MetaAttribute> inheritedMeta, bool onlyInheritable)
		{
			var map = new Dictionary<string, MetaAttribute>(inheritedMeta);
			foreach (XmlNode metaNode in nodes)
			{
				if(metaNode.Name != "meta")
				{
					continue;
				}
				var inheritableValue = GetAttributeValue(metaNode, "inherit");
				bool inheritable = inheritableValue != null ? IsTrue(inheritableValue) : true;
				if (onlyInheritable & !inheritable)
				{
					continue;
				}
				string name = GetAttributeValue(metaNode, "attribute");

				MetaAttribute meta;
				MetaAttribute inheritedAttribute;
				map.TryGetValue(name, out meta);
				inheritedMeta.TryGetValue(name, out inheritedAttribute);
				if (meta == null)
				{
					meta = new MetaAttribute(name);
					map[name] = meta;
				}
				else if (meta == inheritedAttribute)
				{
					meta = new MetaAttribute(name);
					map[name] = meta;
				}
				meta.AddValue(metaNode.InnerText);
			}
			return map;
		}

	}
}