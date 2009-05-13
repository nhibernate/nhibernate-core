using System.Collections.Generic;
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

		public void Bind(XmlNode node, IDictionary<string, MetaAttribute> inheritedMetas)
		{
			PersistentClass superModel = GetSuperclass(node);
			HandleSubclass(superModel, node, inheritedMetas);
		}

		public void HandleSubclass(PersistentClass model, XmlNode subnode, IDictionary<string, MetaAttribute> inheritedMetas)
		{
			Subclass subclass = new SingleTableSubclass(model);

			BindClass(subnode, null, subclass, inheritedMetas);

			inheritedMetas = GetMetas(subnode.SelectNodes(HbmConstants.nsMeta, namespaceManager), inheritedMetas, true); // get meta's from <subclass>

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