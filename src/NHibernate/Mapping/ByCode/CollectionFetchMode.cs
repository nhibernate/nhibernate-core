using NHibernate.Cfg.MappingSchema;

namespace NHibernate.Mapping.ByCode
{
	public abstract class CollectionFetchMode
	{
		public static CollectionFetchMode Select = new SelectFetchMode();
		public static CollectionFetchMode Join = new JoinFetchMode();
		public static CollectionFetchMode Subselect = new SubselectFetchMode();

		internal abstract HbmCollectionFetchMode ToHbm();

		#region Nested type: JoinFetchMode

		private class JoinFetchMode : CollectionFetchMode
		{
			internal override HbmCollectionFetchMode ToHbm()
			{
				return HbmCollectionFetchMode.Join;
			}
		}

		#endregion

		#region Nested type: SelectFetchMode

		private class SelectFetchMode : CollectionFetchMode
		{
			internal override HbmCollectionFetchMode ToHbm()
			{
				return HbmCollectionFetchMode.Select;
			}
		}

		#endregion

		#region Nested type: SubselectFetchMode

		private class SubselectFetchMode : CollectionFetchMode
		{
			internal override HbmCollectionFetchMode ToHbm()
			{
				return HbmCollectionFetchMode.Subselect;
			}
		}

		#endregion
	}
}