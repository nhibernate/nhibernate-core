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
			if (subclass.ClassPersisterClass == null)
				subclass.RootClazz.ClassPersisterClass = typeof(JoinedSubclassEntityPersister);

			//table + schema names
			XmlAttribute schemaNode = subnode.Attributes["schema"];
			string schema = schemaNode == null ? mappings.SchemaName : schemaNode.Value;
			Table mytable = mappings.AddTable(schema, GetClassTableName(subclass, subnode));
			((ITableOwner)subclass).Table = mytable;

			log.InfoFormat("Mapping joined-subclass: {0} -> {1}", subclass.Name, subclass.Table.Name);

			XmlNode keyNode = subnode.SelectSingleNode(HbmConstants.nsKey, namespaceManager);
			SimpleValue key = new DependentValue(mytable, subclass.Identifier);
			subclass.Key = key;
			BindSimpleValue(keyNode, key, false, subclass.Name);
			subclass.Key.Type = subclass.Identifier.Type;

			subclass.CreatePrimaryKey(dialect);

			if (!subclass.IsJoinedSubclass)
				throw new MappingException(
					"Cannot map joined-subclass " + subclass.Name + " to table " +
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