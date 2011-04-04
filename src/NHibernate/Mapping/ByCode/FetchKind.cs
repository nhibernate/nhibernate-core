using NHibernate.Cfg.MappingSchema;

namespace NHibernate.Mapping.ByCode
{
	public abstract class FetchKind
	{
		public static FetchKind Select = new SelectFetchMode();
		public static FetchKind Join = new JoinFetchMode();

		public abstract HbmFetchMode ToHbm();
		public abstract HbmJoinFetch ToHbmJoinFetch();

		#region Nested type: JoinFetchMode

		private class JoinFetchMode : FetchKind
		{
			public override HbmFetchMode ToHbm()
			{
				return HbmFetchMode.Join;
			}

			public override HbmJoinFetch ToHbmJoinFetch()
			{
				return HbmJoinFetch.Join;
			}
		}

		#endregion

		#region Nested type: SelectFetchMode

		private class SelectFetchMode : FetchKind
		{
			public override HbmFetchMode ToHbm()
			{
				return HbmFetchMode.Select;
			}

			public override HbmJoinFetch ToHbmJoinFetch()
			{
				return HbmJoinFetch.Select;
			}
		}

		#endregion
	}
}