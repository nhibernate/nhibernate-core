using System.Collections.Generic;
using System.Xml;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping;
using NHibernate.Persister.Entity;

namespace NHibernate.Cfg.XmlHbmBinding
{
	public class UnionSubclassBinder : ClassBinder
	{
		public UnionSubclassBinder(Mappings mappings, XmlNamespaceManager namespaceManager, Dialect.Dialect dialect)
			: base(mappings, namespaceManager, dialect)
		{
		}

		public UnionSubclassBinder(ClassBinder parent)
			: base(parent)
		{
		}

		public void Bind(XmlNode node, HbmUnionSubclass unionSubclassMapping, IDictionary<string, MetaAttribute> inheritedMetas)
		{
			PersistentClass superModel = GetSuperclass(node);
			HandleUnionSubclass(superModel, unionSubclassMapping, inheritedMetas);
		}

		public void HandleUnionSubclass(PersistentClass model, HbmUnionSubclass unionSubclassMapping, IDictionary<string, MetaAttribute> inheritedMetas)
		{
			var unionSubclass = new UnionSubclass(model);

			BindClass(unionSubclassMapping, unionSubclass, inheritedMetas);
			inheritedMetas = GetMetas(unionSubclassMapping, inheritedMetas, true); // get meta's from <union-subclass>

			// union subclass
			if (unionSubclass.EntityPersisterClass == null)
				unionSubclass.RootClazz.EntityPersisterClass = typeof(UnionSubclassEntityPersister);

			//table + schema names
			string schema = unionSubclassMapping.schema ?? mappings.SchemaName;
			string catalog = unionSubclassMapping.catalog ?? mappings.CatalogName;

			Table denormalizedSuperTable = unionSubclass.Superclass.Table;
			Table mytable =
				mappings.AddDenormalizedTable(schema, catalog, GetClassTableName(unionSubclass, unionSubclassMapping.table),
				                              unionSubclass.IsAbstract.GetValueOrDefault(), null, denormalizedSuperTable);
			((ITableOwner)unionSubclass).Table = mytable;

			log.InfoFormat("Mapping union-subclass: {0} -> {1}", unionSubclass.EntityName, unionSubclass.Table.Name);

			// properties
			new PropertiesBinder(mappings, unionSubclass, namespaceManager, dialect).Bind(unionSubclassMapping.Properties, inheritedMetas);
			BindUnionSubclasses(unionSubclassMapping.UnionSubclasses, unionSubclass, inheritedMetas);

			model.AddSubclass(unionSubclass);
			mappings.AddClass(unionSubclass);
		}
	}
}