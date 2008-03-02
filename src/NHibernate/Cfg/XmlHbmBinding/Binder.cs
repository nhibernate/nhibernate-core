using System;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

using log4net;

using NHibernate.Util;

namespace NHibernate.Cfg.XmlHbmBinding
{
	public abstract class Binder
	{
		protected static readonly ILog log = LogManager.GetLogger(typeof (Binder));
		protected readonly Mappings mappings;

		protected Binder(Binder parent)
		{
			if (parent == null)
				throw new ArgumentNullException("parent");

			mappings = parent.mappings;
		}

		protected Binder(Mappings mappings)
		{
			if (mappings == null)
				throw new ArgumentNullException("mappings");

			this.mappings = mappings;
		}

		/// <summary>
		/// Converts a partial class name into a fully qualified one
		/// </summary>
		/// <param name="className"></param>
		/// <param name="mappings"></param>
		/// <returns></returns>
		protected static string FullClassName(string className, Mappings mappings)
		{
			if (className == null)
				return null;

			return TypeNameParser.Parse(className, mappings.DefaultNamespace, mappings.DefaultAssembly)
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
	}
}