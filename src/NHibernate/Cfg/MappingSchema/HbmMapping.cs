using System.Linq;

namespace NHibernate.Cfg.MappingSchema
{
	partial class HbmMapping : AbstractDecoratable
	{
		public HbmDatabaseObject[] DatabaseObjects
		{
			get { return databaseobject ?? new HbmDatabaseObject[0]; }
		}

		public HbmFilterDef[] FilterDefinitions
		{
			get { return filterdef ?? new HbmFilterDef[0]; }
		}

		public HbmResultSet[] ResultSets
		{
			get { return resultset ?? new HbmResultSet[0]; }
		}

		public HbmTypedef[] TypeDefinitions
		{
			get { return typedef ?? new HbmTypedef[0]; }
		}

		public HbmImport[] Imports
		{
			get { return import ?? new HbmImport[0]; }
		}

		protected override HbmMeta[] Metadatas
		{
			get { return meta ?? new HbmMeta[0]; }
		}

		public HbmClass[] RootClasses
		{
			get { return Items != null ? Items.OfType<HbmClass>().ToArray():new HbmClass[0]; }
		}

		public HbmSubclass[] SubClasses
		{
			get { return Items != null ? Items.OfType<HbmSubclass>().ToArray(): new HbmSubclass[0]; }
		}

		public HbmJoinedSubclass[] JoinedSubclasses
		{
			get { return Items != null ? Items.OfType<HbmJoinedSubclass>().ToArray(): new HbmJoinedSubclass[0]; }
		}

		public HbmUnionSubclass[] UnionSubclasses
		{
			get { return Items != null ? Items.OfType<HbmUnionSubclass>().ToArray(): new HbmUnionSubclass[0]; }
		}

		public HbmQuery[] HqlQueries
		{
			get { return Items1 != null ? Items1.OfType<HbmQuery>().ToArray() : new HbmQuery[0]; }
		}

		public HbmSqlQuery[] SqlQueries
		{
			get { return Items1 != null ? Items1.OfType<HbmSqlQuery>().ToArray() : new HbmSqlQuery[0]; }
		}
	}
}