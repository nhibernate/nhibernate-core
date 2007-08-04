using System;
using System.Xml;

using log4net;

namespace NHibernate.Cfg.XmlHbmBinding
{
	// TODO: Refactor HbmBinder methods and eventually eliminate it
	// TODO: Move methods shared by Binder inheritors into this class
	public abstract class Binder : HbmBinder
	{
		private static readonly ILog log = LogManager.GetLogger(typeof (Binder));
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

		public abstract void Bind(XmlNode node);

		public void BindEach(XmlNode parentNode, string xpath)
		{
			foreach (XmlNode node in SelectNodes(parentNode, xpath))
				Bind(node);
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
	}
}