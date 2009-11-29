using System.Collections.Generic;
using System.Xml;
using NHibernate.Cfg.MappingSchema;
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

		public void Bind(XmlNode node, HbmSubclass subClassMapping, IDictionary<string, MetaAttribute> inheritedMetas)
		{
			PersistentClass superModel = GetSuperclass(node);
			HandleSubclass(superModel, node, subClassMapping, inheritedMetas);
		}

		public void HandleSubclass(PersistentClass model, XmlNode subnode, HbmSubclass subClassMapping, IDictionary<string, MetaAttribute> inheritedMetas)
		{
			Subclass subclass = new SingleTableSubclass(model);

			BindClass(subClassMapping, subclass, inheritedMetas);
			inheritedMetas = GetMetas(subClassMapping, inheritedMetas, true); // get meta's from <subclass>

			if (subclass.EntityPersisterClass == null)
				subclass.RootClazz.EntityPersisterClass = typeof(SingleTableEntityPersister);

			log.InfoFormat("Mapping subclass: {0} -> {1}", subclass.EntityName, subclass.Table.Name);

			// properties
			PropertiesFromXML(subnode, subclass, inheritedMetas);

			model.AddSubclass(subclass);
			mappings.AddClass(subclass);
		}

	}
}