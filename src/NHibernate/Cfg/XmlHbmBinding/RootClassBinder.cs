using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping;

namespace NHibernate.Cfg.XmlHbmBinding
{
	public class RootClassBinder : ClassBinder
	{
		public RootClassBinder(Mappings mappings, Dialect.Dialect dialect)
			: base(mappings, dialect)
		{
		}

		public void Bind(HbmClass classSchema, IDictionary<string, MetaAttribute> inheritedMetas)
		{
			var rootClass = new RootClass();
			BindClass(classSchema, rootClass, inheritedMetas);
			// OPTIMISTIC LOCK MODE
			rootClass.OptimisticLockMode = classSchema.optimisticlock.ToOptimisticLock();

			inheritedMetas = GetMetas(classSchema, inheritedMetas, true); // get meta's from <class>

			//TABLENAME
			string schema = classSchema.schema ?? mappings.SchemaName;
			string catalog = classSchema.catalog ?? mappings.CatalogName;
			string tableName = GetClassTableName(rootClass, classSchema);
			if (string.IsNullOrEmpty(tableName))
			{
				throw new MappingException(
					string.Format(
						"Could not determine the name of the table for entity '{0}'; remove the 'table' attribute or assign a value to it.",
						rootClass.EntityName));
			}

			Table table = mappings.AddTable(schema, catalog, tableName, classSchema.Subselect, rootClass.IsAbstract.GetValueOrDefault(), classSchema.schemaaction);
			((ITableOwner) rootClass).Table = table;

			log.InfoFormat("Mapping class: {0} -> {1}", rootClass.EntityName, rootClass.Table.Name);

			rootClass.IsMutable = classSchema.mutable;
			rootClass.Where = classSchema.where ?? rootClass.Where;

			if (classSchema.check != null)
				table.AddCheckConstraint(classSchema.check);

			rootClass.IsExplicitPolymorphism = classSchema.polymorphism == HbmPolymorphismType.Explicit;

			BindCache(classSchema.cache, rootClass);
			new ClassIdBinder(this).BindId(classSchema.Id, rootClass, table);
			new ClassCompositeIdBinder(this).BindCompositeId(classSchema.CompositeId, rootClass);
			new ClassDiscriminatorBinder(rootClass, Mappings).BindDiscriminator(classSchema.discriminator, table);
			BindTimestamp(classSchema.Timestamp, rootClass, table, inheritedMetas);
			BindVersion(classSchema.Version, rootClass, table, inheritedMetas);

			rootClass.CreatePrimaryKey(dialect);
			BindNaturalId(classSchema.naturalid, rootClass, inheritedMetas);
			new PropertiesBinder(mappings, rootClass, dialect).Bind(classSchema.Properties, inheritedMetas);

			BindJoins(classSchema.Joins, rootClass, inheritedMetas);
			BindSubclasses(classSchema.Subclasses, rootClass, inheritedMetas);
			BindJoinedSubclasses(classSchema.JoinedSubclasses, rootClass, inheritedMetas);
			BindUnionSubclasses(classSchema.UnionSubclasses, rootClass, inheritedMetas);

			new FiltersBinder(rootClass, Mappings).Bind(classSchema.filter);

			mappings.AddClass(rootClass);
		}

		private void BindNaturalId(HbmNaturalId naturalid, PersistentClass rootClass, IDictionary<string, MetaAttribute> inheritedMetas)
		{
			if (naturalid == null)
			{
				return;
			}
			//by default, natural-ids are "immutable" (constant)
			var propBinder = new PropertiesBinder(mappings, rootClass, dialect);
			var uk = new UniqueKey { Name = "_UniqueKey", Table = rootClass.Table };
			propBinder.Bind(naturalid.Properties, inheritedMetas, property =>
				{
					if (!naturalid.mutable)
						property.IsUpdateable = false;
					property.IsNaturalIdentifier = true;

					uk.AddColumns(property.ColumnIterator.OfType<Column>());
				});

			rootClass.Table.AddUniqueKey(uk);	
		}

		private string GetClassTableName(PersistentClass model, HbmClass classSchema)
		{
			if (classSchema.table == null)
				return mappings.NamingStrategy.ClassToTableName(model.EntityName);
			else
				return mappings.NamingStrategy.TableName(classSchema.table.Trim());
		}

		private void BindTimestamp(HbmTimestamp timestampSchema, PersistentClass rootClass, Table table, IDictionary<string, MetaAttribute> inheritedMetas)
		{
			if (timestampSchema == null)
				return;

			string propertyName = timestampSchema.name;
			var simpleValue = new SimpleValue(table);
			new ColumnsBinder(simpleValue, Mappings).Bind(timestampSchema.Columns, false,
			                                              () =>
			                                              new HbmColumn
			                                              	{name = mappings.NamingStrategy.PropertyToColumnName(propertyName)});

			if (!simpleValue.IsTypeSpecified)
			{
				switch (timestampSchema.source)
				{
					case HbmTimestampSource.Vm:
						simpleValue.TypeName = NHibernateUtil.Timestamp.Name; 
						break;
					case HbmTimestampSource.Db:
						simpleValue.TypeName = NHibernateUtil.DbTimestamp.Name; 
						break;
					default:
						simpleValue.TypeName = NHibernateUtil.Timestamp.Name;
						break;
				}
			}

			var property = new Property(simpleValue);
			BindProperty(timestampSchema, property, inheritedMetas);

			// for version properties marked as being generated, make sure they are "always"
			// generated; "insert" is invalid. This is dis-allowed by the schema, but just to make
			// sure...

			if (property.Generation == PropertyGeneration.Insert)
				throw new MappingException("'generated' attribute cannot be 'insert' for versioning property");
			simpleValue.NullValue = timestampSchema.unsavedvalue == HbmTimestampUnsavedvalue.Null ? null : "undefined";
			rootClass.Version = property;
			rootClass.AddProperty(property);
		}

		private void BindProperty(HbmTimestamp timestampSchema, Property property, IDictionary<string, MetaAttribute> inheritedMetas)
		{
			property.Name = timestampSchema.name;

			if (property.Value.Type == null)
				throw new MappingException("could not determine a property type for: " + property.Name);

			property.PropertyAccessorName = timestampSchema.access ?? mappings.DefaultAccess;
			property.Cascade = mappings.DefaultCascade;
			property.IsUpdateable = true;
			property.IsInsertable = true;
			property.IsOptimisticLocked = true;
			property.Generation = Convert(timestampSchema.generated);

			if (property.Generation == PropertyGeneration.Always ||
				property.Generation == PropertyGeneration.Insert)
			{
				// generated properties can *never* be insertable...
				if (property.IsInsertable)
					// insertable simply because that is the user did not specify
					// anything; just override it
					property.IsInsertable = false;

				// properties generated on update can never be updateable...
				if (property.IsUpdateable && property.Generation == PropertyGeneration.Always)
					// updateable only because the user did not specify 
					// anything; just override it
					property.IsUpdateable = false;
			}

			property.MetaAttributes = GetMetas(timestampSchema, inheritedMetas);

			property.LogMapped(log);
		}

		private static PropertyGeneration Convert(HbmVersionGeneration versionGeneration)
		{
			switch (versionGeneration)
			{
				case HbmVersionGeneration.Never:
					return PropertyGeneration.Never;

				case HbmVersionGeneration.Always:
					return PropertyGeneration.Always;

				default:
					throw new ArgumentOutOfRangeException("versionGeneration");
			}
		}

		private void BindVersion(HbmVersion versionSchema, PersistentClass rootClass, Table table, IDictionary<string, MetaAttribute> inheritedMetas)
		{
			if (versionSchema == null)
				return;

			string propertyName = versionSchema.name;
			var simpleValue = new SimpleValue(table);
			new TypeBinder(simpleValue, Mappings).Bind(versionSchema.type);
			new ColumnsBinder(simpleValue, Mappings).Bind(versionSchema.Columns, false,
			                                              () =>
			                                              new HbmColumn
			                                              	{name = mappings.NamingStrategy.PropertyToColumnName(propertyName)});

			if (!simpleValue.IsTypeSpecified)
				simpleValue.TypeName = NHibernateUtil.Int32.Name;

			var property = new Property(simpleValue);
			BindProperty(versionSchema, property, inheritedMetas);

			// for version properties marked as being generated, make sure they are "always"
			// generated; "insert" is invalid. This is dis-allowed by the schema, but just to make
			// sure...

			if (property.Generation == PropertyGeneration.Insert)
				throw new MappingException("'generated' attribute cannot be 'insert' for versioning property");

			simpleValue.NullValue = versionSchema.unsavedvalue;
			rootClass.Version = property;
			rootClass.AddProperty(property);
		}

		private void BindProperty(HbmVersion versionSchema, Property property, IDictionary<string, MetaAttribute> inheritedMetas)
		{
			property.Name = versionSchema.name;

			if (property.Value.Type == null)
				throw new MappingException("could not determine a property type for: " + property.Name);

			property.PropertyAccessorName = versionSchema.access ?? mappings.DefaultAccess;
			property.Cascade = mappings.DefaultCascade;
			property.IsUpdateable = true;
			property.IsInsertable = true;
			property.IsOptimisticLocked = true;
			property.Generation = Convert(versionSchema.generated);

			if (property.Generation == PropertyGeneration.Always
				|| property.Generation == PropertyGeneration.Insert)
			{
				// generated properties can *never* be insertable...
				if (property.IsInsertable)
					// insertable simply because that is the user did not specify
					// anything; just override it
					property.IsInsertable = false;

				// properties generated on update can never be updateable...
				if (property.IsUpdateable && property.Generation == PropertyGeneration.Always)
					// updateable only because the user did not specify 
					// anything; just override it
					property.IsUpdateable = false;
			}

			property.MetaAttributes = GetMetas(versionSchema, inheritedMetas);

			property.LogMapped(log);
		}

		private static void BindCache(HbmCache cacheSchema, RootClass rootClass)
		{
			if (cacheSchema != null)
			{
				rootClass.CacheConcurrencyStrategy = cacheSchema.usage.ToCacheConcurrencyStrategy();
				rootClass.CacheRegionName = cacheSchema.region;
			}
		}
	}
}
