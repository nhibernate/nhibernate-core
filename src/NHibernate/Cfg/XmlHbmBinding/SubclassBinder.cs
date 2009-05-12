using System.Xml;

using NHibernate.Mapping;
using NHibernate.Persister.Entity;

namespace NHibernate.Cfg.XmlHbmBinding
{
	public class SubclassBinder : ClassBinder
	{
		public SubclassBinder(Binder parent, XmlNamespaceManager namespaceManager, Dialect.Dialect dialect)
			: base(parent, namespaceManager, dialect)
		{
		}

		public SubclassBinder(ClassBinder parent)
			: base(parent)
		{
		}

		public void Bind(XmlNode node)
		{
			PersistentClass superModel = GetSuperclass(node);
			HandleSubclass(superModel, node);
		}

		public void HandleSubclass(PersistentClass model, XmlNode subnode)
		{
			Subclass subclass = new SingleTableSubclass(model);

			BindClass(subnode, null, subclass, EmptyMeta);

			if (subclass.EntityPersisterClass == null)
				subclass.RootClazz.EntityPersisterClass = typeof(SingleTableEntityPersister);

			log.InfoFormat("Mapping subclass: {0} -> {1}", subclass.EntityName, subclass.Table.Name);

			// properties
			PropertiesFromXML(subnode, subclass, EmptyMeta);

			model.AddSubclass(subclass);
			mappings.AddClass(subclass);
		}

	}
}