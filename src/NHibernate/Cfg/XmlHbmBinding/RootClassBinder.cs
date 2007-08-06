using System.Xml;

using NHibernate.Mapping;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Cfg.XmlHbmBinding
{
	public class RootClassBinder : ClassBinder
	{
		public RootClassBinder(Mappings mappings, XmlNamespaceManager namespaceManager, Dialect.Dialect dialect)
			: base(mappings, namespaceManager, dialect)
		{
		}

		public RootClassBinder(Binder parent, Dialect.Dialect dialect)
			: base(parent, dialect)
		{
		}

		public RootClassBinder(ClassBinder parent)
			: base(parent)
		{
		}

		public void BindEach(XmlNode parentNode, string xpath)
		{
			foreach (XmlNode node in SelectNodes(parentNode, xpath))
				Bind(node);
		}

		public void Bind(XmlNode node)
		{
			RootClass rootClass = new RootClass();
			BindClass(node, rootClass);

			//TABLENAME
			string schema = GetAttributeValue(node, "schema") ?? mappings.SchemaName;
			string tableName = GetClassTableName(rootClass, node);

			Table table = mappings.AddTable(schema, tableName);
			((ITableOwner) rootClass).Table = table;

			LogInfo("Mapping class: {0} -> {1}", rootClass.Name, rootClass.Table.Name);

			//MUTABLE
			rootClass.IsMutable = "true".Equals(GetAttributeValue(node, "mutable") ?? "true");

			//WHERE
			rootClass.Where = GetAttributeValue(node, "where") ?? rootClass.Where;

			//CHECK
			string check = GetAttributeValue(node, "check");
			if (check != null)
				table.AddCheckConstraint(check);

			//POLYMORPHISM
			rootClass.IsExplicitPolymorphism = "explicit".Equals(GetAttributeValue(node, "polymorphism"));

			foreach (XmlNode childNode in node.ChildNodes)
				if (IsInNHibernateNamespace(childNode))
					BindChildNode(childNode, rootClass, table);

			rootClass.CreatePrimaryKey(dialect);
			PropertiesFromXML(node, rootClass);
			mappings.AddClass(rootClass);
		}

		private void BindChildNode(XmlNode childNode, RootClass rootClass, Table table)
		{
			switch (childNode.LocalName)
			{
				case "id":
					BindIdNode(childNode, rootClass, table);
					break;

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

				case "jcs-cache":
				case "cache":
					BindCacheNode(childNode, rootClass);
					break;
			}
		}

		private void BindIdNode(XmlNode idNode, PersistentClass rootClass, Table table)
		{
			string propertyName = GetAttributeValue(idNode, "name");
			SimpleValue id = new SimpleValue(table);
			rootClass.Identifier = id;

			if (propertyName == null)
			{
				BindSimpleValue(idNode, id, false, RootClass.DefaultIdentifierColumnName);

				if (id.Type == null)
					throw new MappingException(string.Format(
						"Must specify an identifier type: {0}.", rootClass.MappedClass.Name));
			}
			else
			{
				BindSimpleValue(idNode, id, false, propertyName);

				id.SetTypeByReflection(rootClass.MappedClass, propertyName, PropertyAccess(idNode));
				Mapping.Property prop = new Mapping.Property(id);
				BindProperty(idNode, prop);
				rootClass.IdentifierProperty = prop;
			}

			if (id.Type.ReturnedClass.IsArray)
				throw new MappingException(
					"Illegal use of an array as an identifier (arrays don't reimplement equals).");

			MakeIdentifier(idNode, id);
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

		private static void BindCacheNode(XmlNode cacheNode, RootClass rootClass)
		{
			XmlAttribute usageNode = cacheNode.Attributes["usage"];
			rootClass.CacheConcurrencyStrategy = (usageNode != null) ? usageNode.Value : null;
			XmlAttribute regionNode = cacheNode.Attributes["region"];
			rootClass.CacheRegionName = (regionNode != null) ? regionNode.Value : null;
		}

		public static void MakeVersion(XmlNode node, SimpleValue model)
		{
			// VERSION UNSAVED-VALUE
			model.NullValue = GetAttributeValue(node, "unsaved-value");
		}
	}
}