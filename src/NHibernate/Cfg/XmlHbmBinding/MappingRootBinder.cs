using System.Collections.Generic;
using System.Xml;
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
				AddRootClasses(Serialize(rootClass), rootClass, inheritedMetas);
			}
			foreach (HbmSubclass subclass in mappingSchema.SubClasses)
			{
				AddSubclasses(Serialize(subclass), subclass, inheritedMetas);
			}
			foreach (HbmJoinedSubclass joinedSubclass in mappingSchema.JoinedSubclasses)
			{
				AddJoinedSubclasses(Serialize(joinedSubclass), joinedSubclass, inheritedMetas);
			}
			foreach (HbmUnionSubclass unionSubclass in mappingSchema.UnionSubclasses)
			{
				AddUnionSubclasses(Serialize(unionSubclass), unionSubclass, inheritedMetas);
			}
		}

		private void SetMappingsProperties(HbmMapping mappingSchema)
		{
			mappings.SchemaName = mappingSchema.schema;
			mappings.CatalogName = mappingSchema.catalog;
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

		private void AddRootClasses(XmlNode parentNode, HbmClass rootClass, IDictionary<string, MetaAttribute> inheritedMetas)
		{
			var binder = new RootClassBinder(this, GetNamespaceManager(parentNode), dialect);

			binder.Bind(parentNode, rootClass, inheritedMetas);
		}

		private XmlNamespaceManager GetNamespaceManager(XmlNode parentNode)
		{
			// note that the prefix has absolutely nothing to do with what the user
			// selects as their prefix in the document.  It is the prefix we use to 
			// build the XPath and the nsmgr takes care of translating our prefix into
			// the user defined prefix...
			var namespaceManager = new XmlNamespaceManager(parentNode.OwnerDocument.NameTable);
			namespaceManager.AddNamespace(HbmConstants.nsPrefix, MappingSchemaXMLNS);
			return namespaceManager;
		}

		private void AddUnionSubclasses(XmlNode parentNode, HbmUnionSubclass unionSubclass,
		                                IDictionary<string, MetaAttribute> inheritedMetas)
		{
			var binder = new UnionSubclassBinder(this, GetNamespaceManager(parentNode), dialect);

			binder.Bind(parentNode, unionSubclass, inheritedMetas);
		}

		private void AddJoinedSubclasses(XmlNode parentNode, HbmJoinedSubclass joinedSubclass,
		                                 IDictionary<string, MetaAttribute> inheritedMetas)
		{
			var binder = new JoinedSubclassBinder(this, GetNamespaceManager(parentNode), dialect);

			binder.Bind(parentNode, joinedSubclass, inheritedMetas);
		}

		private void AddSubclasses(XmlNode parentNode, HbmSubclass subClass, IDictionary<string, MetaAttribute> inheritedMetas)
		{
			var binder = new SubclassBinder(this, GetNamespaceManager(parentNode), dialect);

			binder.Bind(parentNode, subClass, inheritedMetas);
		}

		private void AddQueries(HbmMapping mappingSchema)
		{
			var binder = new NamedQueryBinder(this);
			System.Array.ForEach(mappingSchema.HqlQueries, binder.AddQuery);
		}

		private void AddSqlQueries(HbmMapping mappingSchema)
		{
			var binder = new NamedSQLQueryBinder(this);
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
			var binder = new ResultSetMappingBinder(this);

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