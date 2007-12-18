using System.Collections;
using System.Collections.Generic;
using System.Xml;

using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Cfg.XmlHbmBinding
{
	public class ClassIdBinder : ClassBinder
	{
		public ClassIdBinder(ClassBinder parent)
			: base(parent)
		{
		}

		public ClassIdBinder(Binder parent, XmlNamespaceManager namespaceManager, Dialect.Dialect dialect)
			: base(parent, namespaceManager, dialect)
		{
		}

		public void BindId(HbmId idSchema, PersistentClass rootClass, Table table)
		{
			if (idSchema != null)
			{
				SimpleValue id = CreateIdentifier(idSchema, rootClass, table);

				AddColumns(idSchema, id);
				CreateIdentifierProperty(idSchema, rootClass, id);
				VerifiyIdTypeIsValid(id, rootClass.MappedClass.Name);
				BindGenerator(idSchema, id);
				id.Table.SetIdentifierValue(id);
				BindUnsavedValue(idSchema, id);
			}
		}

		private static SimpleValue CreateIdentifier(HbmId idSchema, PersistentClass rootClass, Table table)
		{
			SimpleValue iv = new SimpleValue(table);
			iv.TypeName = idSchema.type;
			rootClass.Identifier = iv;

			return iv;
		}

		private void AddColumns(HbmId idSchema, SimpleValue id)
		{
			if (idSchema.column1 != null)
				AddColumnFromAttribute(idSchema, id);
			else
				AddColumnsFromList(idSchema, id);

			if (id.ColumnSpan == 0)
				AddDefaultColumn(idSchema, id);
		}

		private void CreateIdentifierProperty(HbmId idSchema, PersistentClass rootClass, SimpleValue id)
		{
			if (idSchema.name != null)
			{
				string access = idSchema.access ?? mappings.DefaultAccess;
				id.SetTypeUsingReflection(rootClass.MappedClass.AssemblyQualifiedName, idSchema.name, access);

				Mapping.Property property = new Mapping.Property(id);
				property.Name = idSchema.name;

				if (property.Value.Type == null)
					throw new MappingException("could not determine a property type for: " + property.Name);

				property.PropertyAccessorName = idSchema.access ?? mappings.DefaultAccess;
				property.Cascade = mappings.DefaultCascade;
				property.IsUpdateable = true;
				property.IsInsertable = true;
				property.IsOptimisticLocked = true;
				property.Generation = PropertyGeneration.Never;
				property.MetaAttributes = GetMetas(idSchema);

				rootClass.IdentifierProperty = property;

				LogMappedProperty(property);
			}
		}

		private static void VerifiyIdTypeIsValid(IValue id, string className)
		{
			if (id.Type == null)
				throw new MappingException(string.Format("Must specify an identifier type: {0}.", className));

			if (id.Type.ReturnedClass.IsArray)
				throw new MappingException(
					"Illegal use of an array as an identifier (arrays don't reimplement equals).");
		}

		private void BindGenerator(HbmId idSchema, SimpleValue id)
		{
			if (idSchema.generator != null)
			{
				if (idSchema.generator.@class == null)
					throw new MappingException("no class given for generator");

				id.IdentifierGeneratorStrategy = idSchema.generator.@class;
				id.IdentifierGeneratorProperties = GetGeneratorProperties(idSchema, id);
			}
		}

		private IDictionary<string,string> GetGeneratorProperties(HbmId idSchema, IValue id)
		{
			Dictionary<string, string> results = new Dictionary<string, string>();

			if (id.Table.Schema != null)
				results.Add(Id.PersistentIdGeneratorParmsNames.Schema, id.Table.Schema);
			else if (mappings.SchemaName != null)
				results.Add(Id.PersistentIdGeneratorParmsNames.Schema, dialect.QuoteForSchemaName(mappings.SchemaName));

			foreach (HbmParam paramSchema in idSchema.generator.param ?? new HbmParam[0])
				results.Add(paramSchema.name, paramSchema.GetText());

			return results;
		}

		private static void BindUnsavedValue(HbmId idSchema, SimpleValue id)
		{
			if (idSchema.unsavedvalue != null)
				id.NullValue = idSchema.unsavedvalue;

			else if (id.IdentifierGeneratorStrategy == "assigned")
				// TODO: H3 has id.setNullValue("undefined") here, but NH doesn't (yet) allow "undefined"
				// for id unsaved-value, so we use "null" here
				id.NullValue = "null";

			else
				id.NullValue = null;
		}

		private void AddColumnFromAttribute(HbmId idSchema, SimpleValue id)
		{
			Column column = CreateColumn(idSchema);
			column.Value = id;
			column.Name = mappings.NamingStrategy.ColumnName(idSchema.column1);

			if (id.Table != null)
				id.Table.AddColumn(column);

			id.AddColumn(column);
		}

		private void AddColumnsFromList(HbmId idSchema, SimpleValue id)
		{
			int count = 0;

			foreach (HbmColumn columnSchema in idSchema.column ?? new HbmColumn[0])
			{
				Column column = CreateColumn(columnSchema, id, count++);
				column.Name = mappings.NamingStrategy.ColumnName(columnSchema.name);

				if (id.Table != null)
					id.Table.AddColumn(column);
				//table=null -> an association, fill it in later

				id.AddColumn(column);

				if (columnSchema.index != null && id.Table != null)
				{
					StringTokenizer tokens = new StringTokenizer(columnSchema.index, ", ");
					foreach (string token in tokens)
						id.Table.GetOrCreateIndex(token).AddColumn(column);
				}

				if (columnSchema.uniquekey != null && id.Table != null)
				{
					StringTokenizer tokens = new StringTokenizer(columnSchema.uniquekey, ", ");
					foreach (string token in tokens)
						id.Table.GetOrCreateUniqueKey(token).AddColumn(column);
				}
			}
		}

		private void AddDefaultColumn(HbmId idSchema, SimpleValue id)
		{
			Column column = CreateColumn(idSchema);
			column.Value = id;
			string propertyName = idSchema.name ?? RootClass.DefaultIdentifierColumnName;
			column.Name = mappings.NamingStrategy.PropertyToColumnName(propertyName);

			id.Table.AddColumn(column);
			id.AddColumn(column);
		}

		private static IDictionary<string, MetaAttribute> GetMetas(HbmId idSchema)
		{
			Dictionary<string, MetaAttribute> map = new Dictionary<string, MetaAttribute>();

			foreach (HbmMeta metaSchema in idSchema.meta ?? new HbmMeta[0])
			{
				MetaAttribute meta;
				if (!map.TryGetValue(metaSchema.attribute, out meta))
				{
					meta = new MetaAttribute(metaSchema.attribute);
					map[metaSchema.attribute] = meta;
				}

				meta.AddValue(metaSchema.GetText());
			}

			return map;
		}

		private static Column CreateColumn(HbmId idSchema)
		{
			Column column = new Column();

			if (idSchema.length != null)
				column.Length = int.Parse(idSchema.length);

			column.IsNullable = false;
			column.IsUnique = false;
			column.CheckConstraint = string.Empty;
			column.SqlType = null;

			return column;
		}

		private static Column CreateColumn(HbmColumn columnSchema, IValue type, int index)
		{
			Column column = new Column();
			column.Value = type;
			column.TypeIndex = index;

			if (columnSchema.length != null)
				column.Length = int.Parse(columnSchema.length);

			column.IsNullable = columnSchema.notnullSpecified ? columnSchema.notnull : false;
			column.IsUnique = columnSchema.uniqueSpecified && columnSchema.unique;
			column.CheckConstraint = columnSchema.check ?? string.Empty;
			column.SqlType = columnSchema.sqltype;

			return column;
		}
	}
}