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

		public override void Bind(XmlNode node)
		{
			string className = FullClassName(node.Attributes["class"].Value, mappings);
			XmlAttribute renameNode = node.Attributes["rename"];
			string rename = (renameNode == null) ? StringHelper.GetClassname(className) : renameNode.Value;
			log.Debug("Import: " + rename + " -> " + className);
			mappings.AddImport(className, rename);
		}
	}
}