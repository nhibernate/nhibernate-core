using System.Collections;
using System.Xml;

using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping;
using NHibernate.Type;
using NHibernate.Util;

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
			string tableName = GetClassTableName(rootClass, classSchema);

			Table table = mappings.AddTable(schema, tableName);
			((ITableOwner) rootClass).Table = table;

			log.InfoFormat("Mapping class: {0} -> {1}", rootClass.Name, rootClass.Table.Name);

			rootClass.IsMutable = classSchema.mutable;
			rootClass.Where = classSchema.where ?? rootClass.Where;

			if (classSchema.check != null)
				table.AddCheckConstraint(classSchema.check);

			rootClass.IsExplicitPolymorphism = classSchema.polymorphism == HbmPolymorphismType.Explicit;

			BindChildren(node, rootClass, table, classSchema);

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

		private void BindChildren(XmlNode node, RootClass rootClass, Table table, HbmClass classSchema)
		{
			BindCache(classSchema, rootClass);
			BindId(rootClass, table, classSchema.Id);

			foreach (XmlNode childNode in node.ChildNodes)
				if (IsInNHibernateNamespace(childNode))
					switch (childNode.LocalName)
					{
						case "composite-id":
							BindCompositeIdNode(childNode, rootClass);
							break;

						case "version":
							BindVersionNode(childNode, rootClass, table, NHibernateUtil.Int32);
							break;

						case "timestamp":
							BindVersionNode(childNode, rootClass, table, NHibernateUtil.Timestamp);
							break;

						case "discriminator":
							BindDiscriminatorNode(childNode, rootClass, table);
							break;
					}
		}

		private void BindCompositeIdNode(XmlNode compositeIdNode, PersistentClass rootClass)
		{
			string propertyName = GetAttributeValue(compositeIdNode, "name");
			Component compositeId = new Component(rootClass);
			rootClass.Identifier = compositeId;

			if (propertyName == null)
			{
				BindComponent(compositeIdNode, compositeId, null, rootClass.Name, "id", false);
				rootClass.HasEmbeddedIdentifier = compositeId.IsEmbedded;
			}
			else
			{
				System.Type reflectedClass = GetPropertyType(compositeIdNode, rootClass.MappedClass,
					propertyName);

				BindComponent(compositeIdNode, compositeId, reflectedClass, rootClass.Name, propertyName,
					false);

				Mapping.Property prop = new Mapping.Property(compositeId);
				BindProperty(compositeIdNode, prop);
				rootClass.IdentifierProperty = prop;
			}

			MakeIdentifier(compositeIdNode, compositeId);

			System.Type compIdClass = compositeId.ComponentClass;
			if (!ReflectHelper.OverridesEquals(compIdClass))
				throw new MappingException(
					"composite-id class must override Equals(): " + compIdClass.FullName
					);

			if (!ReflectHelper.OverridesGetHashCode(compIdClass))
				throw new MappingException(
					"composite-id class must override GetHashCode(): " + compIdClass.FullName
					);

			// Serializability check not ported
		}

		private void BindVersionNode(XmlNode versionNode, PersistentClass rootClass, Table table,
			IType versioningPropertyType)
		{
			string propertyName = GetAttributeValue(versionNode, "name");
			SimpleValue simpleValue = new SimpleValue(table);
			BindSimpleValue(versionNode, simpleValue, false, propertyName);

			if (simpleValue.Type == null)
				simpleValue.Type = simpleValue.Type ?? versioningPropertyType;

			Mapping.Property property = new Mapping.Property(simpleValue);
			BindProperty(versionNode, property);

			// for version properties marked as being generated, make sure they are "always"
			// generated; "insert" is invalid. This is dis-allowed by the schema, but just to make
			// sure...

			if (property.Generation == PropertyGeneration.Insert)
				throw new MappingException("'generated' attribute cannot be 'insert' for versioning property");

			MakeVersion(versionNode, simpleValue);
			rootClass.Version = property;
			rootClass.AddProperty(property);
		}

		private void BindDiscriminatorNode(XmlNode discriminatorNode, PersistentClass rootClass, Table table)
		{
			//DISCRIMINATOR
			SimpleValue discrim = new SimpleValue(table);
			rootClass.Discriminator = discrim;
			BindSimpleValue(discriminatorNode, discrim, false, RootClass.DefaultDiscriminatorColumnName);
			if (discrim.Type == null)
			{
				discrim.Type = NHibernateUtil.String;
				foreach (Column col in discrim.ColumnCollection)
				{
					col.Type = NHibernateUtil.String;
					break;
				}
			}
			rootClass.IsPolymorphic = true;
			if (discriminatorNode.Attributes["force"] != null &&
				"true".Equals(discriminatorNode.Attributes["force"].Value))
				rootClass.IsForceDiscriminator = true;
			if (discriminatorNode.Attributes["insert"] != null &&
				"false".Equals(discriminatorNode.Attributes["insert"].Value))
				rootClass.IsDiscriminatorInsertable = false;
		}

		public static void MakeVersion(XmlNode node, SimpleValue model)
		{
			// VERSION UNSAVED-VALUE
			model.NullValue = GetAttributeValue(node, "unsaved-value");
		}

		private static string GetAttributeValue(XmlNode node, string attributeName)
		{
			if (node != null && node.Attributes != null)
			{
				XmlAttribute attribute = node.Attributes[attributeName];
				return attribute == null ? null : attribute.Value;
			}
			else
				return null;
		}

		private static bool IsInNHibernateNamespace(XmlNode node)
		{
			return node.NamespaceURI == Configuration.MappingSchemaXMLNS;
		}

		private static void BindCache(HbmClass classSchema, RootClass rootClass)
		{
			if (classSchema.Cache != null)
			{
				rootClass.CacheConcurrencyStrategy = GetXmlEnumAttribute(classSchema.Cache.usage);
				rootClass.CacheRegionName = classSchema.Cache.region;
			}
		}

		#region BindId

		private void BindId(PersistentClass rootClass, Table table, HbmId idSchema)
		{
			if (idSchema == null)
				return;

			SimpleValue id = new SimpleValue(table);
			rootClass.Identifier = id;
			id.Type = GetType(idSchema);
			BindColumns(id, false, true, idSchema.name ?? RootClass.DefaultIdentifierColumnName, idSchema);

			if (idSchema.name != null)
			{
				id.SetTypeByReflection(rootClass.MappedClass, idSchema.name, idSchema.access ?? mappings.DefaultAccess);
				Mapping.Property prop = new Mapping.Property(id);
				BindProperty(prop, idSchema);
				rootClass.IdentifierProperty = prop;
			}
			else if (id.Type == null)
				throw new MappingException(string.Format("Must specify an identifier type: {0}.",
					rootClass.MappedClass.Name));

			if (id.Type.ReturnedClass.IsArray)
				throw new MappingException(
					"Illegal use of an array as an identifier (arrays don't reimplement equals).");

			BindGenerator(idSchema, id);
			id.Table.SetIdentifierValue(id);
			BindUnsavedValue(idSchema, id);
		}

		private static IType GetType(HbmId idSchema)
		{
			if (idSchema.type == null)
				return null; //we will have to use reflection

			IType type = TypeFactory.HeuristicType(idSchema.type, null);

			if (type == null)
				throw new MappingException("could not interpret type: " + idSchema.type);

			return type;
		}

		private static void BindUnsavedValue(HbmId idSchema, SimpleValue model)
		{
			if (idSchema.unsavedvalue != null)
				model.NullValue = idSchema.unsavedvalue;

			else if (model.IdentifierGeneratorStrategy == "assigned")
				// TODO: H3 has model.setNullValue("undefined") here, but
				// NH doesn't (yet) allow "undefined" for id unsaved-value,
				// so we use "null" here
				model.NullValue = "null";

			else
				model.NullValue = null;
		}

		private void BindGenerator(HbmId idSchema, SimpleValue model)
		{
			if (idSchema.generator != null)
			{
				if (idSchema.generator.@class == null)
					throw new MappingException("no class given for generator");

				model.IdentifierGeneratorStrategy = idSchema.generator.@class;

				IDictionary parms = new Hashtable();

				if (mappings.SchemaName != null)
					parms.Add("schema", dialect.QuoteForSchemaName(mappings.SchemaName));

				parms.Add("target_table", model.Table.GetQuotedName(dialect));

				foreach (Column col in model.ColumnCollection)
				{
					parms.Add("target_column", col);
					break;
				}

				foreach (HbmParam paramSchema in idSchema.generator.param ?? new HbmParam[0])
					parms.Add(paramSchema.name, paramSchema.GetText());

				model.IdentifierGeneratorProperties = parms;
			}
		}

		private void BindProperty(Mapping.Property property, HbmId idSchema)
		{
			property.Name = idSchema.name;

			if (property.Value.Type == null)
				throw new MappingException("could not determine a property type for: " + property.Name);

			property.PropertyAccessorName = idSchema.access ?? mappings.DefaultAccess;
			property.Cascade = mappings.DefaultCascade;
			property.IsUpdateable = true;
			property.IsInsertable = true;
			property.IsOptimisticLocked = true;
			property.Generation = PropertyGeneration.Never;

			if (log.IsDebugEnabled)
			{
				string msg = "Mapped property: " + property.Name;
				string columns = Columns(property.Value);
				if (columns.Length > 0)
					msg += " -> " + columns;
				if (property.Type != null)
					msg += ", type: " + property.Type.Name;
				log.Debug(msg);
			}

			property.MetaAttributes = GetMetas(idSchema);
		}

		private static IDictionary GetMetas(HbmId idSchema)
		{
			IDictionary map = new Hashtable();

			foreach (HbmMeta metaSchema in idSchema.meta ?? new HbmMeta[0])
			{
				MetaAttribute meta = (MetaAttribute) map[metaSchema.attribute];

				if (meta == null)
				{
					meta = new MetaAttribute();
					map[metaSchema.attribute] = meta;
				}

				meta.AddValue(metaSchema.GetText());
			}

			return map;
		}

		private void BindColumns(SimpleValue model, bool isNullable, bool autoColumn, string propertyPath, HbmId idSchema)
		{
			Table table = model.Table;
			//COLUMN(S)
			if (idSchema.column1 == null)
			{
				int count = 0;

				foreach (HbmColumn columnSchema in idSchema.column ?? new HbmColumn[0])
				{
					Column col = new Column(model.Type, count++);
					BindColumn(columnSchema, col, isNullable);

					string name = columnSchema.name;
					col.Name = mappings.NamingStrategy.ColumnName(name);
					if (table != null)
						table.AddColumn(col);
					//table=null -> an association, fill it in later
					model.AddColumn(col);

					//column index
					BindIndex(columnSchema.index, table, col);
					BindUniqueKey(columnSchema.uniquekey, table, col);
				}
			}
			else
			{
				Column col = new Column(model.Type, 0);
				BindColumn(idSchema, col, isNullable);
				col.Name = mappings.NamingStrategy.ColumnName(idSchema.column1);
				if (table != null)
					table.AddColumn(col);
				model.AddColumn(col);
				//column group index (although can serve as a separate column index)
				BindIndex(null, table, col);
				BindUniqueKey(null, table, col);
			}

			if (autoColumn && model.ColumnSpan == 0)
			{
				Column col = new Column(model.Type, 0);
				BindColumn(idSchema, col, isNullable);
				col.Name = mappings.NamingStrategy.PropertyToColumnName(propertyPath);
				model.Table.AddColumn(col);
				model.AddColumn(col);
				//column group index (although can serve as a separate column index)
				BindIndex(null, table, col);
				BindUniqueKey(null, table, col);
			}
		}

		private static void BindColumn(HbmColumn columnSchema, Column column, bool isNullable)
		{
			if (columnSchema.length != null)
				column.Length = int.Parse(columnSchema.length);

			column.IsNullable = columnSchema.notnullSpecified ? columnSchema.notnull : isNullable;
			column.IsUnique = columnSchema.uniqueSpecified && columnSchema.unique;
			column.CheckConstraint = columnSchema.check ?? string.Empty;
			column.SqlType = columnSchema.sqltype;
		}

		private static void BindIndex(string indexAttribute, Table table, Column column)
		{
			if (indexAttribute != null && table != null)
			{
				StringTokenizer tokens = new StringTokenizer(indexAttribute, ", ");
				foreach (string token in tokens)
					table.GetIndex(token).AddColumn(column);
			}
		}

		private static void BindUniqueKey(string uniqueKeyAttribute, Table table, Column column)
		{
			if (uniqueKeyAttribute != null && table != null)
			{
				StringTokenizer tokens = new StringTokenizer(uniqueKeyAttribute, ", ");
				foreach (string token in tokens)
					table.GetUniqueKey(token).AddColumn(column);
			}
		}

		private static void BindColumn(HbmId idSchema, Column column, bool isNullable)
		{
			if (idSchema.length != null)
				column.Length = int.Parse(idSchema.length);

			column.IsNullable = isNullable;
			column.IsUnique = false;
			column.CheckConstraint = string.Empty;
			column.SqlType = null;
		}

		#endregion
	}
}