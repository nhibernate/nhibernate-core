using System.Xml;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping;
using NHibernate.Persister.Entity;
using System.Collections.Generic;

namespace NHibernate.Cfg.XmlHbmBinding
{
	public class JoinedSubclassBinder : ClassBinder
	{
		public JoinedSubclassBinder(Binder parent, XmlNamespaceManager namespaceManager, Dialect.Dialect dialect)
			: base(parent, namespaceManager, dialect)
		{
		}

		public JoinedSubclassBinder(ClassBinder parent)
			: base(parent)
		{
		}

		public void Bind(XmlNode node, HbmJoinedSubclass joinedSubclassMapping, IDictionary<string, MetaAttribute> inheritedMetas)
		{
			PersistentClass superModel = GetSuperclass(node);
			HandleJoinedSubclass(superModel, node, joinedSubclassMapping, inheritedMetas);
		}

		public void HandleJoinedSubclass(PersistentClass model, XmlNode subnode, HbmJoinedSubclass joinedSubclassMapping, IDictionary<string, MetaAttribute> inheritedMetas)
		{
			JoinedSubclass subclass = new JoinedSubclass(model);

			BindClass(joinedSubclassMapping, subclass, inheritedMetas);
			inheritedMetas = GetMetas(joinedSubclassMapping, inheritedMetas, true); // get meta's from <joined-subclass>

			// joined subclass
			if (subclass.EntityPersisterClass == null)
				subclass.RootClazz.EntityPersisterClass = typeof(JoinedSubclassEntityPersister);

			//table + schema names
			string schema = joinedSubclassMapping.schema ?? mappings.SchemaName;
			string catalog = joinedSubclassMapping.catalog ?? mappings.CatalogName;

			// TODO: very strange, the schema does not support it
			XmlAttribute actionNode = subnode.Attributes["schema-action"];
			string action = actionNode == null ? "all" : actionNode.Value;

			Table mytable = mappings.AddTable(schema, catalog, GetClassTableName(subclass, joinedSubclassMapping.table), null, false, action);
			((ITableOwner)subclass).Table = mytable;

			log.InfoFormat("Mapping joined-subclass: {0} -> {1}", subclass.EntityName, subclass.Table.Name);

			// KEY
			XmlNode keyNode = subnode.SelectSingleNode(HbmConstants.nsKey, namespaceManager);
			SimpleValue key = new DependantValue(mytable, subclass.Identifier);
			subclass.Key = key;
			if (keyNode.Attributes["on-delete"] != null)
				key.IsCascadeDeleteEnabled = "cascade".Equals(keyNode.Attributes["on-delete"].Value);
			BindSimpleValue(keyNode, key, false, subclass.EntityName);

			subclass.CreatePrimaryKey(dialect);

			if (!subclass.IsJoinedSubclass)
				throw new MappingException(
					"Cannot map joined-subclass " + subclass.EntityName + " to table " +
						subclass.Table.Name + ", the same table as its base class.");

			subclass.CreateForeignKey();
			// CHECK
			mytable.AddCheckConstraint(joinedSubclassMapping.check);

			// properties
			PropertiesFromXML(subnode, subclass, inheritedMetas);

			model.AddSubclass(subclass);
			mappings.AddClass(subclass);
		}

	}
}