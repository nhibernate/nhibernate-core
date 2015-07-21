using NHibernate.Cfg.MappingSchema;

namespace NHibernate.Mapping.ByCode
{
	public abstract class FetchKind
	{
		public static FetchKind Select = new SelectFetchMode();
		public static FetchKind Join = new JoinFetchMode();

		internal abstract HbmFetchMode ToHbm();
		internal abstract HbmJoinFetch ToHbmJoinFetch();

		#region Nested type: JoinFetchMode

		private class JoinFetchMode : FetchKind
		{
			internal override HbmFetchMode ToHbm()
			{
				return HbmFetchMode.Join;
			}

			internal override HbmJoinFetch ToHbmJoinFetch()
			{
				return HbmJoinFetch.Join;
			}
		}

		#endregion

		#region Nested type: SelectFetchMode

		private class SelectFetchMode : FetchKind
		{
			internal override HbmFetchMode ToHbm()
			{
				return HbmFetchMode.Select;
			}

			internal override HbmJoinFetch ToHbmJoinFetch()
			{
				return HbmJoinFetch.Select;
			}
		}

		#endregion
	}
}