using System.Xml;

using NHibernate.Mapping;

namespace NHibernate.Cfg.XmlHbmBinding
{
	public class SubclassBinder : ClassBinder
	{
		public SubclassBinder(Mappings mappings, XmlNamespaceManager namespaceManager, Dialect.Dialect dialect)
			: base(mappings, namespaceManager, dialect)
		{
		}

		public SubclassBinder(Binder parent, Dialect.Dialect dialect)
			: base(parent, dialect)
		{
		}

		public SubclassBinder(ClassBinder parent)
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
			PersistentClass superModel = GetSuperclass(node);
			HandleSubclass(superModel, node);
		}
	}
}