using NHibernate.Cfg.MappingSchema;

namespace NHibernate.Mapping.ByCode
{
	public abstract class VersionGeneration
	{
		public static VersionGeneration Never = new NeverGeneration();
		public static VersionGeneration Always = new AlwaysGeneration();

		public abstract HbmVersionGeneration ToHbm();

		#region Nested type: AlwaysGeneration

		private class AlwaysGeneration : VersionGeneration
		{
			public override HbmVersionGeneration ToHbm()
			{
				return HbmVersionGeneration.Always;
			}
		}

		#endregion

		#region Nested type: NeverGeneration

		private class NeverGeneration : VersionGeneration
		{
			public override HbmVersionGeneration ToHbm()
			{
				return HbmVersionGeneration.Never;
			}
		}

		#endregion
	}
}