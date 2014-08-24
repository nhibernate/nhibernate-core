using System.Collections.Generic;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping;
using NHibernate.Persister.Entity;

namespace NHibernate.Cfg.XmlHbmBinding
{
	public class UnionSubclassBinder : ClassBinder
	{
		public UnionSubclassBinder(Mappings mappings, Dialect.Dialect dialect)
			: base(mappings, dialect)
		{
		}

		public UnionSubclassBinder(ClassBinder parent)
			: base(parent)
		{
		}

		public void Bind(HbmUnionSubclass unionSubclassMapping, IDictionary<string, MetaAttribute> inheritedMetas)
		{
			PersistentClass superModel = GetSuperclass(unionSubclassMapping.extends);
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
				                              unionSubclass.IsAbstract.GetValueOrDefault(), unionSubclassMapping.Subselect, denormalizedSuperTable);
			((ITableOwner)unionSubclass).Table = mytable;

			log.InfoFormat("Mapping union-subclass: {0} -> {1}", unionSubclass.EntityName, unionSubclass.Table.Name);

			// properties
			new PropertiesBinder(mappings, unionSubclass, dialect).Bind(unionSubclassMapping.Properties, inheritedMetas);
			BindUnionSubclasses(unionSubclassMapping.UnionSubclasses, unionSubclass, inheritedMetas);

			model.AddSubclass(unionSubclass);
			mappings.AddClass(unionSubclass);
		}
	}
}