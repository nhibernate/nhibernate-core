using System.Xml;

using NHibernate.Mapping;

namespace NHibernate.Cfg.XmlHbmBinding
{
	public class SubclassBinder : ClassBinder
	{
		public SubclassBinder(Binder parent, XmlNamespaceManager namespaceManager, Dialect.Dialect dialect)
			: base(parent, namespaceManager, dialect)
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