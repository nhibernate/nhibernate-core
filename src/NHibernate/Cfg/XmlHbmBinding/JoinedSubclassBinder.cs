using System.Xml;

using NHibernate.Mapping;

namespace NHibernate.Cfg.XmlHbmBinding
{
	public class JoinedSubclassBinder : ClassBinder
	{
		public JoinedSubclassBinder(Mappings mappings, XmlNamespaceManager namespaceManager, Dialect.Dialect dialect)
			: base(mappings, namespaceManager, dialect)
		{
		}

		public JoinedSubclassBinder(Binder parent, Dialect.Dialect dialect)
			: base(parent, dialect)
		{
		}

		public JoinedSubclassBinder(ClassBinder parent)
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
			HandleJoinedSubclass(superModel, node);
		}
	}
}