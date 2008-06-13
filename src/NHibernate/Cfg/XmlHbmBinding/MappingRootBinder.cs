using System.Xml;

using NHibernate.Cfg.MappingSchema;
using NHibernate.Engine;
using NHibernate.Mapping;
using NHibernate.Util;

namespace NHibernate.Cfg.XmlHbmBinding
{
	public class MappingRootBinder : Binder
	{
		private readonly XmlNamespaceManager namespaceManager;
		private readonly Dialect.Dialect dialect;

		public MappingRootBinder(Mappings mappings, XmlNamespaceManager namespaceManager,
			Dialect.Dialect dialect)
			: base(mappings)
		{
			this.namespaceManager = namespaceManager;
			this.dialect = dialect;
		}

		public void Bind(XmlNode node)
		{
			HbmMapping mappingSchema = Deserialize<HbmMapping>(node);

			SetMappingsProperties(mappingSchema);
			AddFilterDefinitions(mappingSchema);

			AddRootClasses(node);
			AddSubclasses(node);
			AddJoinedSubclasses(node);
			AddUnionSubclasses(node);

			AddQueries(mappingSchema);
			AddSqlQueries(mappingSchema);
			AddImports(mappingSchema);
			AddAuxiliaryDatabaseObjects(mappingSchema);
			AddResultSetMappingDefinitions(mappingSchema);
		}

		private void SetMappingsProperties(HbmMapping mappingSchema)
		{
			mappings.SchemaName = mappingSchema.schema;
			mappings.DefaultCascade = mappingSchema.defaultcascade;
			mappings.DefaultAccess = mappingSchema.defaultaccess;
			mappings.DefaultLazy = mappingSchema.defaultlazy;
			mappings.IsAutoImport = mappingSchema.autoimport;
			mappings.DefaultNamespace = mappingSchema.@namespace ?? mappings.DefaultNamespace;
			mappings.DefaultAssembly = mappingSchema.assembly ?? mappings.DefaultAssembly;
		}

		private void AddFilterDefinitions(HbmMapping mappingSchema)
		{
			foreach (HbmFilterDef filterDefSchema in mappingSchema.ListFilterDefs())
			{
				FilterDefinition definition = FilterDefinitionFactory.CreateFilterDefinition(filterDefSchema);
				mappings.AddFilterDefinition(definition);
			}
		}

		private void AddRootClasses(XmlNode parentNode)
		{
			RootClassBinder binder = new RootClassBinder(this, namespaceManager, dialect);

			foreach (XmlNode node in parentNode.SelectNodes(HbmConstants.nsClass, namespaceManager))
				binder.Bind(node, Deserialize<HbmClass>(node));
		}

		private void AddUnionSubclasses(XmlNode parentNode)
		{
			UnionSubclassBinder binder = new UnionSubclassBinder(this, namespaceManager, dialect);

			foreach (XmlNode node in parentNode.SelectNodes(HbmConstants.nsUnionSubclass, namespaceManager))
				binder.Bind(node);
		}

		private void AddJoinedSubclasses(XmlNode parentNode)
		{
			JoinedSubclassBinder binder = new JoinedSubclassBinder(this, namespaceManager, dialect);

			foreach (XmlNode node in parentNode.SelectNodes(HbmConstants.nsJoinedSubclass, namespaceManager))
				binder.Bind(node);
		}

		private void AddSubclasses(XmlNode parentNode)
		{
			SubclassBinder binder = new SubclassBinder(this, namespaceManager, dialect);

			foreach (XmlNode node in parentNode.SelectNodes(HbmConstants.nsSubclass, namespaceManager))
				binder.Bind(node);
		}

		private void AddQueries(HbmMapping mappingSchema)
		{
			NamedQueryBinder binder = new NamedQueryBinder(this);

			foreach (object item in mappingSchema.Items1 ?? new object[0])
			{
				HbmQuery querySchema = item as HbmQuery;

				if (querySchema != null)
					binder.AddQuery(querySchema);
			}
		}

		private void AddSqlQueries(HbmMapping mappingSchema)
		{
			NamedSQLQueryBinder binder = new NamedSQLQueryBinder(this);

			foreach (object item in mappingSchema.Items1 ?? new object[0])
			{
				HbmSqlQuery sqlQuerySchema = item as HbmSqlQuery;

				if (sqlQuerySchema != null)
					binder.AddSqlQuery(sqlQuerySchema);
			}
		}

		public void AddImports(HbmMapping mappingSchema)
		{
			foreach (HbmImport importSchema in mappingSchema.import ?? new HbmImport[0])
			{
				string fullClassName = FullClassName(importSchema.@class, mappings);
				string rename = importSchema.rename ?? StringHelper.GetClassname(fullClassName);

				log.DebugFormat("Import: {0} -> {1}", rename, fullClassName);
				mappings.AddImport(fullClassName, rename);
			}
		}

		private void AddAuxiliaryDatabaseObjects(HbmMapping mappingSchema)
		{
			foreach (HbmDatabaseObject objectSchema in mappingSchema.ListDatabaseObjects())
			{
				IAuxiliaryDatabaseObject dbObject = AuxiliaryDatabaseObjectFactory.Create(mappings, objectSchema);
				mappings.AddAuxiliaryDatabaseObject(dbObject);
			}
		}

		private void AddResultSetMappingDefinitions(HbmMapping mappingSchema)
		{
			ResultSetMappingBinder binder = new ResultSetMappingBinder(this);

			foreach (HbmResultSet resultSetSchema in mappingSchema.resultset ?? new HbmResultSet[0])
			{
				// Do not inline this variable or the anonymous method will not work correctly.
				HbmResultSet tempResultSetSchema = resultSetSchema;

				mappings.AddSecondPass(delegate
					{
						ResultSetMappingDefinition definition = binder.Create(tempResultSetSchema);
						mappings.AddResultSetMapping(definition);
					});
			}
		}
	}
}
