using System.Xml;

using NHibernate.Engine;

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
			mappings.AddSecondPass(delegate { SecondPassBind(node); });
		}

		private void SecondPassBind(XmlNode node)
		{
			ResultSetMappingDefinition definition =
				ResultSetMappingBinder.BuildResultSetMappingDefinition(node, null, mappings);

			mappings.AddResultSetMapping(definition);
		}
	}
}