using System;
using System.Collections.Generic;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping;
using NHibernate.Persister.Entity;

namespace NHibernate.Cfg.XmlHbmBinding
{
	public class SubclassBinder : ClassBinder
	{
		public SubclassBinder(Binder parent)
			: base(parent.Mappings)
		{
		}

		public SubclassBinder(Mappings mappings)
			: base(mappings)
		{
		}

		//Since v5.2
		[Obsolete("Please use constructor without a dialect parameter.")]
		public SubclassBinder(Binder parent, Dialect.Dialect dialect)
			: base(parent.Mappings, dialect)
		{
		}

		//Since v5.2
		[Obsolete("Please use constructor that accepts mappings parameter instead.")]
		public SubclassBinder(ClassBinder parent)
			: base(parent)
		{
		}

		public void Bind(HbmSubclass subClassMapping, IDictionary<string, MetaAttribute> inheritedMetas)
		{
			PersistentClass superModel = GetSuperclass(subClassMapping.extends);
			HandleSubclass(superModel, subClassMapping, inheritedMetas);
		}

		public void HandleSubclass(PersistentClass model, HbmSubclass subClassMapping, IDictionary<string, MetaAttribute> inheritedMetas)
		{
			Subclass subclass = new SingleTableSubclass(model);

			BindClass(subClassMapping, subclass, inheritedMetas);
			inheritedMetas = GetMetas(subClassMapping, inheritedMetas, true); // get meta's from <subclass>

			if (subclass.EntityPersisterClass == null)
				subclass.RootClazz.EntityPersisterClass = typeof(SingleTableEntityPersister);

			log.Info("Mapping subclass: {0} -> {1}", subclass.EntityName, subclass.Table.Name);

			// properties
			new PropertiesBinder(mappings, subclass).Bind(subClassMapping.Properties, inheritedMetas);
			BindJoins(subClassMapping.Joins, subclass, inheritedMetas);
			BindSubclasses(subClassMapping.Subclasses, subclass, inheritedMetas);

			model.AddSubclass(subclass);
			mappings.AddClass(subclass);

            new FiltersBinder(model, Mappings).Bind(subClassMapping.filter);
        }

	}
}