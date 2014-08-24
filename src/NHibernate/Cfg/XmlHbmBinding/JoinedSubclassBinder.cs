using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping;
using NHibernate.Persister.Entity;
using System.Collections.Generic;

namespace NHibernate.Cfg.XmlHbmBinding
{
	public class JoinedSubclassBinder : ClassBinder
	{
		public JoinedSubclassBinder(Mappings mappings, Dialect.Dialect dialect)
			: base(mappings, dialect)
		{
		}

		public JoinedSubclassBinder(ClassBinder parent)
			: base(parent)
		{
		}

		public void Bind(HbmJoinedSubclass joinedSubclassMapping, IDictionary<string, MetaAttribute> inheritedMetas)
		{
			PersistentClass superModel = GetSuperclass(joinedSubclassMapping.extends);
			HandleJoinedSubclass(superModel, joinedSubclassMapping, inheritedMetas);
		}

		public void HandleJoinedSubclass(PersistentClass model, HbmJoinedSubclass joinedSubclassMapping, IDictionary<string, MetaAttribute> inheritedMetas)
		{
			var subclass = new JoinedSubclass(model);

			BindClass(joinedSubclassMapping, subclass, inheritedMetas);
			inheritedMetas = GetMetas(joinedSubclassMapping, inheritedMetas, true); // get meta's from <joined-subclass>

			// joined subclass
			if (subclass.EntityPersisterClass == null)
				subclass.RootClazz.EntityPersisterClass = typeof(JoinedSubclassEntityPersister);

			//table + schema names
			string schema = joinedSubclassMapping.schema ?? mappings.SchemaName;
			string catalog = joinedSubclassMapping.catalog ?? mappings.CatalogName;

			Table mytable = mappings.AddTable(schema, catalog, GetClassTableName(subclass, joinedSubclassMapping.table), joinedSubclassMapping.Subselect, false, joinedSubclassMapping.schemaaction);
			((ITableOwner)subclass).Table = mytable;

			log.InfoFormat("Mapping joined-subclass: {0} -> {1}", subclass.EntityName, subclass.Table.Name);

			// KEY
			BindKey(subclass, joinedSubclassMapping.key, mytable);

			subclass.CreatePrimaryKey(dialect);

			if (!subclass.IsJoinedSubclass)
				throw new MappingException(
					"Cannot map joined-subclass " + subclass.EntityName + " to table " +
						subclass.Table.Name + ", the same table as its base class.");

			subclass.CreateForeignKey();
			// CHECK
			mytable.AddCheckConstraint(joinedSubclassMapping.check);

			// properties
			new PropertiesBinder(mappings, subclass, dialect).Bind(joinedSubclassMapping.Properties, inheritedMetas);

			BindJoinedSubclasses(joinedSubclassMapping.JoinedSubclasses, subclass, inheritedMetas);

			model.AddSubclass(subclass);
			mappings.AddClass(subclass);
		}

		private void BindKey(JoinedSubclass subclass, HbmKey keyMapping, Table mytable)
		{
			// TODO : property-ref ?? 
			SimpleValue key = new DependantValue(mytable, subclass.Identifier);
			subclass.Key = key;
			key.IsCascadeDeleteEnabled = keyMapping.ondelete == HbmOndelete.Cascade;
			key.ForeignKeyName = keyMapping.foreignkey;

			new ValuePropertyBinder(key, Mappings).BindSimpleValue(keyMapping, subclass.EntityName, false);
		}
	}
}