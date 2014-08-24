using NHibernate.Cfg.MappingSchema;

namespace NHibernate.Mapping.ByCode
{
	public abstract class NotFoundMode
	{
		public static NotFoundMode Ignore = new IgnoreNotFoundMode();
		public static NotFoundMode Exception = new ExceptionNotFoundMode();

		public abstract HbmNotFoundMode ToHbm();

		#region Nested type: ExceptionNotFoundMode

		private class ExceptionNotFoundMode : NotFoundMode
		{
			#region Overrides of NotFoundMode

			public override HbmNotFoundMode ToHbm()
			{
				return HbmNotFoundMode.Exception;
			}

			#endregion
		}

		#endregion

		#region Nested type: IgnoreNotFoundMode

		private class IgnoreNotFoundMode : NotFoundMode
		{
			#region Overrides of NotFoundMode

			public override HbmNotFoundMode ToHbm()
			{
				return HbmNotFoundMode.Ignore;
			}

			#endregion
		}

		#endregion
	}
}