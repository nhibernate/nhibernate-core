using System.Collections.Generic;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Engine;
using NHibernate.Mapping;
using NHibernate.Util;

namespace NHibernate.Cfg.XmlHbmBinding
{
	public class MappingRootBinder : Binder
	{
		private readonly Dialect.Dialect dialect;

		public MappingRootBinder(Mappings mappings, Dialect.Dialect dialect)
			: base(mappings)
		{
			this.dialect = dialect;
		}

		public void Bind(HbmMapping mappingSchema)
		{
			IDictionary<string, MetaAttribute> inheritedMetas = EmptyMeta;
			// get meta's from <hibernate-mapping>
			inheritedMetas = GetMetas(mappingSchema, inheritedMetas, true);

			SetMappingsProperties(mappingSchema);
			AddFilterDefinitions(mappingSchema);
			AddTypeDefs(mappingSchema);

			AddEntitiesMappings(mappingSchema, inheritedMetas);

			AddQueries(mappingSchema);
			AddSqlQueries(mappingSchema);
			AddImports(mappingSchema);
			AddAuxiliaryDatabaseObjects(mappingSchema);
			AddResultSetMappingDefinitions(mappingSchema);
		}

		private void AddEntitiesMappings(HbmMapping mappingSchema, IDictionary<string, MetaAttribute> inheritedMetas)
		{
			foreach (HbmClass rootClass in mappingSchema.RootClasses)
			{
				AddRootClasses(rootClass, inheritedMetas);
			}
			foreach (HbmSubclass subclass in mappingSchema.SubClasses)
			{
				AddSubclasses(subclass, inheritedMetas);
			}
			foreach (HbmJoinedSubclass joinedSubclass in mappingSchema.JoinedSubclasses)
			{
				AddJoinedSubclasses(joinedSubclass, inheritedMetas);
			}
			foreach (HbmUnionSubclass unionSubclass in mappingSchema.UnionSubclasses)
			{
				AddUnionSubclasses(unionSubclass, inheritedMetas);
			}
		}

		private void SetMappingsProperties(HbmMapping mappingSchema)
		{
			mappings.SchemaName = mappingSchema.schema ?? mappings.DefaultSchema;
			mappings.CatalogName = mappingSchema.catalog ?? mappings.DefaultCatalog;
			mappings.DefaultCascade = mappingSchema.defaultcascade;
			mappings.DefaultAccess = mappingSchema.defaultaccess;
			mappings.DefaultLazy = mappingSchema.defaultlazy;
			mappings.IsAutoImport = mappingSchema.autoimport;
			mappings.DefaultNamespace = mappingSchema.@namespace ?? mappings.DefaultNamespace;
			mappings.DefaultAssembly = mappingSchema.assembly ?? mappings.DefaultAssembly;
		}

		private void AddFilterDefinitions(HbmMapping mappingSchema)
		{
			foreach (HbmFilterDef filterDefSchema in mappingSchema.FilterDefinitions)
			{
				FilterDefinition definition = FilterDefinitionFactory.CreateFilterDefinition(filterDefSchema);
				mappings.AddFilterDefinition(definition);
			}
		}

		private void AddRootClasses(HbmClass rootClass, IDictionary<string, MetaAttribute> inheritedMetas)
		{
			var binder = new RootClassBinder(Mappings, dialect);

			binder.Bind(rootClass, inheritedMetas);
		}

		private void AddUnionSubclasses(HbmUnionSubclass unionSubclass,
		                                IDictionary<string, MetaAttribute> inheritedMetas)
		{
			var binder = new UnionSubclassBinder(Mappings, dialect);

			binder.Bind(unionSubclass, inheritedMetas);
		}

		private void AddJoinedSubclasses(HbmJoinedSubclass joinedSubclass,
		                                 IDictionary<string, MetaAttribute> inheritedMetas)
		{
			var binder = new JoinedSubclassBinder(Mappings, dialect);

			binder.Bind(joinedSubclass, inheritedMetas);
		}

		private void AddSubclasses(HbmSubclass subClass, IDictionary<string, MetaAttribute> inheritedMetas)
		{
			var binder = new SubclassBinder(this, dialect);

			binder.Bind(subClass, inheritedMetas);
		}

		private void AddQueries(HbmMapping mappingSchema)
		{
			var binder = new NamedQueryBinder(Mappings);
			System.Array.ForEach(mappingSchema.HqlQueries, binder.AddQuery);
		}

		private void AddSqlQueries(HbmMapping mappingSchema)
		{
			var binder = new NamedSQLQueryBinder(Mappings);
			System.Array.ForEach(mappingSchema.SqlQueries, binder.AddSqlQuery);
		}

		public void AddImports(HbmMapping mappingSchema)
		{
			foreach (HbmImport importSchema in mappingSchema.Imports)
			{
				string fullClassName = FullQualifiedClassName(importSchema.@class, mappings);
				string rename = importSchema.rename ?? StringHelper.GetClassname(fullClassName);

				log.DebugFormat("Import: {0} -> {1}", rename, fullClassName);
				mappings.AddImport(fullClassName, rename);
			}
		}

		public void AddTypeDefs(HbmMapping mappingSchema)
		{
			foreach (HbmTypedef typedef in mappingSchema.TypeDefinitions)
			{
				string typeClass = FullQualifiedClassName(typedef.@class, mappings);
				string typeName = typedef.name;
				IEnumerable<HbmParam> paramIter = typedef.param ?? new HbmParam[0];
				var parameters = new Dictionary<string, string>(5);
				foreach (HbmParam param in paramIter)
				{
					parameters.Add(param.name, param.GetText().Trim());
				}
				mappings.AddTypeDef(typeName, typeClass, parameters);
			}
		}

		private void AddAuxiliaryDatabaseObjects(HbmMapping mappingSchema)
		{
			foreach (HbmDatabaseObject objectSchema in mappingSchema.DatabaseObjects)
			{
				IAuxiliaryDatabaseObject dbObject = AuxiliaryDatabaseObjectFactory.Create(mappings, objectSchema);
				mappings.AddAuxiliaryDatabaseObject(dbObject);
			}
		}

		private void AddResultSetMappingDefinitions(HbmMapping mappingSchema)
		{
			var binder = new ResultSetMappingBinder(Mappings);

			foreach (HbmResultSet resultSetSchema in mappingSchema.ResultSets)
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