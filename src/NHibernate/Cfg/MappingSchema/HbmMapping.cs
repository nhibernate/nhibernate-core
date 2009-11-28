using System.Linq;

namespace NHibernate.Cfg.MappingSchema
{
	partial class HbmMapping : AbstractDecoratable
	{
		public HbmDatabaseObject[] ListDatabaseObjects()
		{
			return databaseobject ?? new HbmDatabaseObject[0];
		}

		public HbmFilterDef[] ListFilterDefs()
		{
			return filterdef ?? new HbmFilterDef[0];
		}

		protected override HbmMeta[] GetMetadataField()
		{
			return meta;
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