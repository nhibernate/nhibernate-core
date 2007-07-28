using System;
using System.Xml;

using log4net;

using NHibernate.Util;

namespace NHibernate.Cfg.XmlHbmBinding
{
	// TODO: Refactor HbmBinder methods and eventually eliminate it
	// TODO: Move methods shared by Binder inheritors into this class
	public abstract class Binder : HbmBinder
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

		public abstract void Bind(XmlNode node);

		public void BindEach(XmlNode parentNode, string xpath)
		{
			foreach (XmlNode node in parentNode.SelectNodes(xpath, namespaceManager))
				Bind(node);
		}

		protected static string GetAttributeValue(XmlNode node, string attributeName)
		{
			return XmlHelper.GetAttributeValue(node, attributeName);
		}

		protected XmlNode SelectSingleNode(XmlNode node, string xpath)
		{
			return node.SelectSingleNode(xpath, namespaceManager);
		}

		protected XmlNodeList SelectNodes(XmlNode node, string xpath)
		{
			return node.SelectNodes(xpath, namespaceManager);
		}
	}
}