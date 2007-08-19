using System.Xml;

using NHibernate.Cfg.MappingSchema;
using NHibernate.Engine;
using NHibernate.Mapping;
using NHibernate.Util;

namespace NHibernate.Cfg.XmlHbmBinding
{
	public class MappingRootBinder : XmlBinder
	{
		private readonly Dialect.Dialect dialect;

		public MappingRootBinder(Mappings mappings, XmlNamespaceManager namespaceManager,
			Dialect.Dialect dialect)
			: base(mappings, namespaceManager)
		{
			this.dialect = dialect;
		}

		public void Bind(XmlNode node)
		{
			HbmMapping mappingSchema = Deserialize<HbmMapping>(node);

			SetMappingsProperties(mappingSchema);
			AddFilterDefinitions(mappingSchema);

			new RootClassBinder(this, dialect).BindEach(node, HbmConstants.nsClass);
			new SubclassBinder(this, dialect).BindEach(node, HbmConstants.nsSubclass);
			new JoinedSubclassBinder(this, dialect).BindEach(node, HbmConstants.nsJoinedSubclass);
			new NamedQueryBinder(this).BindEach(node, HbmConstants.nsQuery);

			AddNamedSqlQueries(mappingSchema);
			AddImports(mappingSchema);
			AddAuxiliaryDatabaseObjects(mappingSchema);
			AddResultSetMappingDefinitions(mappingSchema);
		}

		private void SetMappingsProperties(HbmMapping mappingSchema)
		{
			mappings.SchemaName = mappingSchema.schema;
			mappings.DefaultCascade = GetXmlEnumAttribute(mappingSchema.defaultcascade);
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

		private void AddNamedSqlQueries(HbmMapping mappingSchema)
		{
			foreach (object item in mappingSchema.Items1 ?? new object[0])
			{
				HbmSqlQuery querySchema = item as HbmSqlQuery;

				if (querySchema != null)
					new NamedSQLQueryBinder(this).AddNamedSqlQuery(querySchema);
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
				IAuxiliaryDatabaseObject dbObject = AuxiliaryDatabaseObjectFactory.Create(objectSchema);
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