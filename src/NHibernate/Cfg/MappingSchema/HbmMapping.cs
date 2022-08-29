using System;
using System.Linq;

namespace NHibernate.Cfg.MappingSchema
{
	partial class HbmMapping : AbstractDecoratable
	{
		public HbmDatabaseObject[] DatabaseObjects
		{
			get { return databaseobject ?? Array.Empty<HbmDatabaseObject>(); }
		}

		public HbmFilterDef[] FilterDefinitions
		{
			get { return filterdef ?? Array.Empty<HbmFilterDef>(); }
		}

		public HbmResultSet[] ResultSets
		{
			get { return resultset ?? Array.Empty<HbmResultSet>(); }
		}

		public HbmTypedef[] TypeDefinitions
		{
			get { return typedef ?? Array.Empty<HbmTypedef>(); }
		}

		public HbmImport[] Imports
		{
			get { return import ?? Array.Empty<HbmImport>(); }
		}

		protected override HbmMeta[] Metadatas
		{
			get { return meta ?? Array.Empty<HbmMeta>(); }
		}

		public HbmClass[] RootClasses
		{
			get { return Items != null ? Items.OfType<HbmClass>().ToArray() : Array.Empty<HbmClass>(); }
		}

		public HbmSubclass[] SubClasses
		{
			get { return Items != null ? Items.OfType<HbmSubclass>().ToArray() : Array.Empty<HbmSubclass>(); }
		}

		public HbmJoinedSubclass[] JoinedSubclasses
		{
			get { return Items != null ? Items.OfType<HbmJoinedSubclass>().ToArray() : Array.Empty<HbmJoinedSubclass>(); }
		}

		public HbmUnionSubclass[] UnionSubclasses
		{
			get { return Items != null ? Items.OfType<HbmUnionSubclass>().ToArray() : Array.Empty<HbmUnionSubclass>(); }
		}

		public HbmQuery[] HqlQueries
		{
			get { return Items1 != null ? Items1.OfType<HbmQuery>().ToArray() : Array.Empty<HbmQuery>(); }
		}

		public HbmSqlQuery[] SqlQueries
		{
			get { return Items1 != null ? Items1.OfType<HbmSqlQuery>().ToArray() : Array.Empty<HbmSqlQuery>(); }
		}
	}
}
