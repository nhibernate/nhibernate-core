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

		public override void Bind(XmlNode node)
		{
			PersistentClass superModel = GetSuperclass(mappings, node);
			HandleJoinedSubclass(superModel, mappings, node);
		}
	}
}