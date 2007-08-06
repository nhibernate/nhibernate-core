using System.Xml;

using NHibernate.Mapping;

namespace NHibernate.Cfg.XmlHbmBinding
{
	public class JoinedSubclassBinder : ClassBinder
	{
		public JoinedSubclassBinder(Binder parent)
			: base(parent)
		{
		}

		public JoinedSubclassBinder(Mappings mappings, XmlNamespaceManager namespaceManager)
			: base(mappings, namespaceManager)
		{
		}

		public void BindEach(XmlNode parentNode, string xpath)
		{
			foreach (XmlNode node in SelectNodes(parentNode, xpath))
				Bind(node);
		}

		public void Bind(XmlNode node)
		{
			PersistentClass superModel = GetSuperclass(node);
			HandleJoinedSubclass(superModel, node);
		}
	}
}