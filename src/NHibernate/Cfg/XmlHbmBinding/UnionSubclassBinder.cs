using System.Collections.Generic;
using System.Xml;
using NHibernate.Mapping;
using NHibernate.Persister.Entity;

namespace NHibernate.Cfg.XmlHbmBinding
{
	public class UnionSubclassBinder : ClassBinder
	{
		public UnionSubclassBinder(Binder parent, XmlNamespaceManager namespaceManager, Dialect.Dialect dialect)
			: base(parent, namespaceManager, dialect)
		{
		}

		public UnionSubclassBinder(ClassBinder parent)
			: base(parent)
		{
		}

		public void Bind(XmlNode node, IDictionary<string, MetaAttribute> inheritedMetas)
		{
			PersistentClass superModel = GetSuperclass(node);
			HandleUnionSubclass(superModel, node, inheritedMetas);
		}

		public void HandleUnionSubclass(PersistentClass model, XmlNode subnode, IDictionary<string, MetaAttribute> inheritedMetas)
		{
			var unionSubclass = new UnionSubclass(model);

			BindClass(subnode, null, unionSubclass, inheritedMetas);
			inheritedMetas = GetMetas(subnode.SelectNodes(HbmConstants.nsMeta, namespaceManager), inheritedMetas, true); // get meta's from <union-subclass>

			// union subclass
			if (unionSubclass.EntityPersisterClass == null)
				unionSubclass.RootClazz.EntityPersisterClass = typeof(UnionSubclassEntityPersister);

			//table + schema names
			XmlAttribute schemaNode = subnode.Attributes["schema"];
			string schema = schemaNode == null ? mappings.SchemaName : schemaNode.Value;
			XmlAttribute catalogNode = subnode.Attributes["catalog"];
			string catalog = catalogNode == null ? mappings.CatalogName : catalogNode.Value;

			Table denormalizedSuperTable = unionSubclass.Superclass.Table;
			Table mytable =
				mappings.AddDenormalizedTable(schema, catalog, GetClassTableName(unionSubclass, subnode),
				                              unionSubclass.IsAbstract.GetValueOrDefault(), null, denormalizedSuperTable);
			((ITableOwner)unionSubclass).Table = mytable;

			log.InfoFormat("Mapping union-subclass: {0} -> {1}", unionSubclass.EntityName, unionSubclass.Table.Name);

			// properties
			PropertiesFromXML(subnode, unionSubclass, inheritedMetas);

			model.AddSubclass(unionSubclass);
			mappings.AddClass(unionSubclass);
		}
	}
}