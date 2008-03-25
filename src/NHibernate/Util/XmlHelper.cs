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

		public static string ElementTextTrim(XmlNode node, string elementName, XmlNamespaceManager nsmgr)
		{
			XmlNode subNode = node.SelectSingleNode(elementName, nsmgr);
			if (subNode == null)
			{
				return null;
			}

			return subNode.InnerText.Trim();
		}
	}
}