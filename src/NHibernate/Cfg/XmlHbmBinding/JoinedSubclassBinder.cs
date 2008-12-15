using System.Xml;

using NHibernate.Mapping;
using NHibernate.Persister.Entity;

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

		public void Bind(XmlNode node)
		{
			PersistentClass superModel = GetSuperclass(node);
			HandleJoinedSubclass(superModel, node);
		}

		public void HandleJoinedSubclass(PersistentClass model, XmlNode subnode)
		{
			JoinedSubclass subclass = new JoinedSubclass(model);

			BindClass(subnode, subclass);

			// joined subclass
			if (subclass.EntityPersisterClass == null)
				subclass.RootClazz.EntityPersisterClass = typeof(JoinedSubclassEntityPersister);

			//table + schema names
			XmlAttribute schemaNode = subnode.Attributes["schema"];
			string schema = schemaNode == null ? mappings.SchemaName : schemaNode.Value;
			XmlAttribute catalogNode = subnode.Attributes["catalog"];
			string catalog = catalogNode == null ? mappings.CatalogName : catalogNode.Value;

			XmlAttribute actionNode = subnode.Attributes["schema-action"];
			string action = actionNode == null ? "all" : actionNode.Value;
            
			Table mytable = mappings.AddTable(schema, catalog, GetClassTableName(subclass, subnode), null, false, action);
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
			XmlAttribute chNode = subnode.Attributes["check"];
			if (chNode != null)
				mytable.AddCheckConstraint(chNode.Value);

			// properties
			PropertiesFromXML(subnode, subclass);

			model.AddSubclass(subclass);
			mappings.AddClass(subclass);
		}

	}
}