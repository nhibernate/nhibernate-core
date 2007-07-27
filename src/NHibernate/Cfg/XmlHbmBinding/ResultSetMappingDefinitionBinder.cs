using System.Xml;

namespace NHibernate.Cfg.XmlHbmBinding
{
	public class ResultSetMappingDefinitionBinder : Binder
	{
		public ResultSetMappingDefinitionBinder(Mappings mappings, XmlNamespaceManager namespaceManager)
			: base(mappings, namespaceManager)
		{
		}

		public ResultSetMappingDefinitionBinder(Binder parent)
			: base(parent)
		{
		}

		public override void Bind(XmlNode node)
		{
			mappings.AddSecondPass(new ResultSetMappingSecondPass(node, null, mappings));
		}
	}
}