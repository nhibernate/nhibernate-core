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

		public void BindEach(XmlNode parentNode, string xpath)
		{
			foreach (XmlNode node in SelectNodes(parentNode, xpath))
				Bind(node);
		}

		public void Bind(XmlNode node)
		{
			mappings.AddSecondPass(delegate { SecondPassBind(node); });
		}

		private void SecondPassBind(XmlNode node)
		{
			ResultSetMappingDefinition definition =
				new ResultSetMappingBinder(this).BuildResultSetMappingDefinition(node, null);

			mappings.AddResultSetMapping(definition);
		}
	}
}