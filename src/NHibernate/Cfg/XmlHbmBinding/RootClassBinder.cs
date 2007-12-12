using System;
using System.Collections.Generic;
using System.Xml;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping;
using NHibernate.Type;

namespace NHibernate.Cfg.XmlHbmBinding
{
	public class RootClassBinder : ClassBinder
	{
		public RootClassBinder(Binder parent, XmlNamespaceManager namespaceManager, Dialect.Dialect dialect)
			: base(parent, namespaceManager, dialect)
		{
		}

		public void Bind(XmlNode node, HbmClass classSchema)
		{
			RootClass rootClass = new RootClass();
			BindClass(node, rootClass);

			//TABLENAME
			string schema = classSchema.schema ?? mappings.SchemaName;
			string catalog = mappings.CatalogName; //string catalog = classSchema.catalog ?? mappings.CatalogName;
			string tableName = GetClassTableName(rootClass, classSchema);

			Table table = mappings.AddTable(schema, catalog, tableName, null, rootClass.IsAbstract.GetValueOrDefault());
			((ITableOwner) rootClass).Table = table;

			log.InfoFormat("Mapping class: {0} -> {1}", rootClass.Name, rootClass.Table.Name);

			rootClass.IsMutable = classSchema.mutable;
			rootClass.Where = classSchema.where ?? rootClass.Where;

			if (classSchema.check != null)
				table.AddCheckConstraint(classSchema.check);

			rootClass.IsExplicitPolymorphism = classSchema.polymorphism == HbmPolymorphismType.Explicit;

			BindCache(classSchema.Cache, rootClass);
			new ClassIdBinder(this).BindId(classSchema.Id, rootClass, table);
			new ClassCompositeIdBinder(this).BindCompositeId(classSchema.CompositeId, rootClass);
			new ClassDiscriminatorBinder(this).BindDiscriminator(classSchema.discriminator, rootClass, table);
			BindTimestamp(classSchema.Timestamp, rootClass, table);
			BindVersion(classSchema.Version, rootClass, table);

			rootClass.CreatePrimaryKey(dialect);
			PropertiesFromXML(node, rootClass);
			mappings.AddClass(rootClass);
		}

		private string GetClassTableName(PersistentClass model, HbmClass classSchema)
		{
			if (classSchema.table == null)
				return mappings.NamingStrategy.ClassToTableName(model.Name);
			else
				return mappings.NamingStrategy.TableName(classSchema.table);
		}

		private void BindTimestamp(HbmTimestamp timestampSchema, PersistentClass rootClass, Table table)
		{
			if (timestampSchema == null)
				return;

			IType versioningPropertyType = NHibernateUtil.Timestamp;

			string propertyName = timestampSchema.name;
			SimpleValue simpleValue = new SimpleValue(table);

			simpleValue.Type = null;
			BindColumns(timestampSchema, simpleValue, propertyName);

			if (simpleValue.Type == null)
				simpleValue.Type = simpleValue.Type ?? versioningPropertyType;

			Mapping.Property property = new Mapping.Property(simpleValue);
			BindProperty(timestampSchema, property);

			// for version properties marked as being generated, make sure they are "always"
			// generated; "insert" is invalid. This is dis-allowed by the schema, but just to make
			// sure...

			if (property.Generation == PropertyGeneration.Insert)
				throw new MappingException("'generated' attribute cannot be 'insert' for versioning property");

			simpleValue.NullValue = timestampSchema.unsavedvalue;
			rootClass.Version = property;
			rootClass.AddProperty(property);
		}

		private void BindColumns(HbmTimestamp timestampSchema, SimpleValue model, string propertyPath)
		{
			Table table = model.Table;

			if (timestampSchema.column != null)
			{
				Column col = new Column(model.Type, 0);
				BindColumn(col, false);
				col.Name = mappings.NamingStrategy.ColumnName(timestampSchema.column);

				if (table != null)
					table.AddColumn(col);

				model.AddColumn(col);
			}

			if (model.ColumnSpan == 0)
			{
				Column col = new Column(model.Type, 0);
				BindColumn(col, false);
				col.Name = mappings.NamingStrategy.PropertyToColumnName(propertyPath);
				model.Table.AddColumn(col);
				model.AddColumn(col);
			}
		}

		private static void BindColumn(Column column, bool isNullable)
		{
			column.IsNullable = isNullable;
			column.IsUnique = false;
			column.CheckConstraint = string.Empty;
			column.SqlType = null;
		}

		private void BindProperty(HbmTimestamp timestampSchema, Mapping.Property property)
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

			property.MetaAttributes = new Dictionary<string, MetaAttribute>();

			LogMappedProperty(property);
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

		private void BindVersion(HbmVersion versionSchema, PersistentClass rootClass, Table table)
		{
			IType versioningPropertyType = NHibernateUtil.Int32;

			if (versionSchema == null)
				return;

			string propertyName = versionSchema.name;
			SimpleValue simpleValue = new SimpleValue(table);

			simpleValue.Type = GetType(versionSchema);
			BindColumns(versionSchema, simpleValue, false, propertyName);

			if (simpleValue.Type == null)
				simpleValue.Type = simpleValue.Type ?? versioningPropertyType;

			Mapping.Property property = new Mapping.Property(simpleValue);
			BindProperty(versionSchema, property);

			// for version properties marked as being generated, make sure they are "always"
			// generated; "insert" is invalid. This is dis-allowed by the schema, but just to make
			// sure...

			if (property.Generation == PropertyGeneration.Insert)
				throw new MappingException("'generated' attribute cannot be 'insert' for versioning property");

			simpleValue.NullValue = versionSchema.unsavedvalue;
			rootClass.Version = property;
			rootClass.AddProperty(property);
		}

		private static IType GetType(HbmVersion versionSchema)
		{
			if (versionSchema.type == null)
				return null;

			IType type = TypeFactory.HeuristicType(versionSchema.type, null);

			if (type == null)
				throw new MappingException("could not interpret type: " + versionSchema.type);

			return type;
		}

		private void BindColumns(HbmVersion versionSchema, SimpleValue model, bool isNullable, string propertyPath)
		{
			Table table = model.Table;

			if (versionSchema.column != null)
			{
				Column col = new Column(model.Type, 0);
				BindColumn(col, isNullable);
				col.Name = mappings.NamingStrategy.ColumnName(versionSchema.column);

				if (table != null)
					table.AddColumn(col);

				model.AddColumn(col);
			}

			if (model.ColumnSpan == 0)
			{
				Column col = new Column(model.Type, 0);
				BindColumn(col, isNullable);
				col.Name = mappings.NamingStrategy.PropertyToColumnName(propertyPath);
				model.Table.AddColumn(col);
				model.AddColumn(col);
			}
		}

		private void BindProperty(HbmVersion versionSchema, Mapping.Property property)
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

			property.MetaAttributes = new Dictionary<string, MetaAttribute>();

			LogMappedProperty(property);
		}

		public static void MakeVersion(XmlNode node, SimpleValue model)
		{
			if (node != null && node.Attributes != null)
			{
				XmlAttribute attribute = node.Attributes["unsaved-value"];
				model.NullValue = attribute == null ? null : attribute.Value;
			}
			else
				model.NullValue = null;
		}

		private static void BindCache(HbmCacheType cacheSchema, RootClass rootClass)
		{
			if (cacheSchema != null)
			{
				rootClass.CacheConcurrencyStrategy = GetXmlEnumAttribute(cacheSchema.usage);
				rootClass.CacheRegionName = cacheSchema.region;
			}
		}
	}
}