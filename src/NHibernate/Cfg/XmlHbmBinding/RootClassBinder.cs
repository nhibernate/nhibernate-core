using System.Xml;

using NHibernate.Mapping;
using NHibernate.Util;

namespace NHibernate.Cfg.XmlHbmBinding
{
	public class RootClassBinder : ClassBinder
	{
		public RootClassBinder(Binder parent)
			: base(parent)
		{
		}

		public RootClassBinder(Mappings mappings, XmlNamespaceManager namespaceManager)
			: base(mappings, namespaceManager)
		{
		}

		public override void Bind(XmlNode node)
		{
			RootClass rootClass = new RootClass();

			BindClass(node, rootClass, mappings);

			//TABLENAME
			XmlAttribute schemaNode = node.Attributes["schema"];
			string schema = schemaNode == null ? mappings.SchemaName : schemaNode.Value;
			Table table = mappings.AddTable(schema, GetClassTableName(rootClass, node, mappings));
			((ITableOwner) rootClass).Table = table;

			log.Info("Mapping class: " + rootClass.Name + " -> " + rootClass.Table.Name);

			//MUTABLE
			XmlAttribute mutableNode = node.Attributes["mutable"];
			rootClass.IsMutable = (mutableNode == null) || mutableNode.Value.Equals("true");

			//WHERE
			XmlAttribute whereNode = node.Attributes["where"];
			if (whereNode != null)
				rootClass.Where = whereNode.Value;

			//CHECK
			XmlAttribute checkNode = node.Attributes["check"];
			if (checkNode != null)
				table.AddCheckConstraint(checkNode.Value);

			//POLYMORPHISM
			XmlAttribute polyNode = node.Attributes["polymorphism"];
			rootClass.IsExplicitPolymorphism = (polyNode != null) && polyNode.Value.Equals("explicit");

			foreach (XmlNode subnode in node.ChildNodes)
			{
				string name = subnode.LocalName; //Name;
				string propertyName = GetPropertyName(subnode);

				//I am only concerned with elements that are from the nhibernate namespace
				if (subnode.NamespaceURI != Configuration.MappingSchemaXMLNS)
					continue;

				switch (name)
				{
					case "id":
						SimpleValue id = new SimpleValue(table);
						rootClass.Identifier = id;

						if (propertyName == null)
						{
							BindSimpleValue(subnode, id, false, RootClass.DefaultIdentifierColumnName, mappings);
							if (id.Type == null)
								throw new MappingException("must specify an identifier type: " + rootClass.MappedClass.Name);
							//model.IdentifierProperty = null;
						}
						else
						{
							BindSimpleValue(subnode, id, false, propertyName, mappings);
							id.SetTypeByReflection(rootClass.MappedClass, propertyName, PropertyAccess(subnode, mappings));
							Mapping.Property prop = new Mapping.Property(id);
							BindProperty(subnode, prop, mappings);
							rootClass.IdentifierProperty = prop;
						}

						if (id.Type.ReturnedClass.IsArray)
							throw new MappingException("illegal use of an array as an identifier (arrays don't reimplement equals)");

						MakeIdentifier(subnode, id, mappings);
						break;

					case "composite-id":
						Component compId = new Component(rootClass);
						rootClass.Identifier = compId;
						if (propertyName == null)
						{
							BindComponent(subnode, compId, null, rootClass.Name, "id", false, mappings);
							rootClass.HasEmbeddedIdentifier = compId.IsEmbedded;
							//model.IdentifierProperty = null;
						}
						else
						{
							System.Type reflectedClass = GetPropertyType(subnode, mappings, rootClass.MappedClass, propertyName);
							BindComponent(subnode, compId, reflectedClass, rootClass.Name, propertyName, false, mappings);
							Mapping.Property prop = new Mapping.Property(compId);
							BindProperty(subnode, prop, mappings);
							rootClass.IdentifierProperty = prop;
						}
						MakeIdentifier(subnode, compId, mappings);

						System.Type compIdClass = compId.ComponentClass;
						if (!ReflectHelper.OverridesEquals(compIdClass))
							throw new MappingException(
								"composite-id class must override Equals(): " + compIdClass.FullName
								);

						if (!ReflectHelper.OverridesGetHashCode(compIdClass))
							throw new MappingException(
								"composite-id class must override GetHashCode(): " + compIdClass.FullName
								);

						// Serializability check not ported
						break;

					case "version":
					case "timestamp":
						//VERSION / TIMESTAMP
						BindVersioningProperty(table, subnode, mappings, name, rootClass);
						break;

					case "discriminator":
						//DISCRIMINATOR
						SimpleValue discrim = new SimpleValue(table);
						rootClass.Discriminator = discrim;
						BindSimpleValue(subnode, discrim, false, RootClass.DefaultDiscriminatorColumnName, mappings);
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
						if (subnode.Attributes["force"] != null && "true".Equals(subnode.Attributes["force"].Value))
							rootClass.IsForceDiscriminator = true;
						if (subnode.Attributes["insert"] != null && "false".Equals(subnode.Attributes["insert"].Value))
							rootClass.IsDiscriminatorInsertable = false;
						break;

					case "jcs-cache":
					case "cache":
						XmlAttribute usageNode = subnode.Attributes["usage"];
						rootClass.CacheConcurrencyStrategy = (usageNode != null) ? usageNode.Value : null;
						XmlAttribute regionNode = subnode.Attributes["region"];
						rootClass.CacheRegionName = (regionNode != null) ? regionNode.Value : null;

						break;
				}
			}

			rootClass.CreatePrimaryKey(dialect);

			PropertiesFromXML(node, rootClass, mappings);
			mappings.AddClass(rootClass);
		}

		protected static void BindVersioningProperty(Table table, XmlNode subnode, Mappings mappings, string name, RootClass entity)
		{
			string propertyName = subnode.Attributes["name"].Value;
			SimpleValue val = new SimpleValue(table);
			BindSimpleValue(subnode, val, false, propertyName, mappings);
			if (val.Type == null)
			{
				val.Type = (("version".Equals(name)) ? NHibernateUtil.Int32 : NHibernateUtil.Timestamp);
			}
			Mapping.Property prop = new Mapping.Property(val);
			BindProperty(subnode, prop, mappings);
			// for version properties marked as being generated, make sure they are "always"
			// generated; "insert" is invalid. This is dis-allowed by the schema, but just to make
			// sure...
			if (prop.Generation == PropertyGeneration.Insert)
			{
				throw new MappingException("'generated' attribute cannot be 'insert' for versioning property");
			}
			MakeVersion(subnode, val);
			entity.Version = prop;
			entity.AddProperty(prop);
		}
	}
}