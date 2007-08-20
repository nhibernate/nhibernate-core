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
			new ClassIdBinder(this).BindId(classSchema.Id, rootClass, table);

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
	}
}