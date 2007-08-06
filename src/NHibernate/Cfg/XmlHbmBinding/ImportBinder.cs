using System.Xml;

using NHibernate.Util;

namespace NHibernate.Cfg.XmlHbmBinding
{
	public class ImportBinder : Binder
	{
		public ImportBinder(Mappings mappings, XmlNamespaceManager namespaceManager)
			: base(mappings, namespaceManager)
		{
		}

		public ImportBinder(Binder parent)
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
			string className = GetAttributeValue(node, "class");
			string fullClassName = FullClassName(className, mappings);
			string rename = GetAttributeValue(node, "rename") ?? StringHelper.GetClassname(fullClassName);

			LogDebug("Import: {0} -> {1}", rename, fullClassName);

			mappings.AddImport(fullClassName, rename);
		}
	}
}