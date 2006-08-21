using System;
using System.Xml;

namespace NHibernate.Util
{
	public class XmlHelper
	{
		public static string GetAttributeValue(XmlNode node, string attributeName)
		{
			XmlAttribute attribute = node.Attributes[attributeName];
			if (attribute == null)
			{
				return null;
			}
			return attribute.Value;
		}
	}
}
