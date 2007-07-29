using System.Xml;

namespace NHibernate.Cfg.XmlHbmBinding
{
	public class NamedSQLQueryBinder : Binder
	{
		public NamedSQLQueryBinder(Mappings mappings, XmlNamespaceManager namespaceManager)
			: base(mappings, namespaceManager)
		{
		}

		public NamedSQLQueryBinder(Binder parent)
			: base(parent)
		{
		}

		public override void Bind(XmlNode node)
		{
			NamedSQLQuerySecondPass secondPass = new NamedSQLQuerySecondPass(node, null, mappings);
			mappings.AddSecondPass(secondPass);
		}
	}
}