using System.Xml;

using NHibernate.Mapping;

namespace NHibernate.Cfg.XmlHbmBinding
{
	public class SubclassBinder : ClassBinder
	{
		public SubclassBinder(Binder parent)
			: base(parent)
		{
		}

		public SubclassBinder(Mappings mappings, XmlNamespaceManager namespaceManager)
			: base(mappings, namespaceManager)
		{
		}

		public override void Bind(XmlNode node)
		{
			PersistentClass superModel = GetSuperclass(mappings, node);
			HandleSubclass(superModel, mappings, node);
		}
	}
}