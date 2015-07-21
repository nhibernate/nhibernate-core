using NHibernate.Cfg.MappingSchema;

namespace NHibernate.Mapping.ByCode
{
	public abstract class CacheUsage
	{
		public static CacheUsage ReadOnly = new ReadOnlyUsage();
		public static CacheUsage ReadWrite = new ReadWriteUsage();
		public static CacheUsage NonstrictReadWrite = new NonstrictReadWriteUsage();
		public static CacheUsage Transactional = new TransactionalUsage();

		internal abstract HbmCacheUsage ToHbm();

		#region Nested type: NonstrictReadWriteUsage

		private class NonstrictReadWriteUsage : CacheUsage
		{
			internal override HbmCacheUsage ToHbm()
			{
				return HbmCacheUsage.NonstrictReadWrite;
			}
		}

		#endregion

		#region Nested type: ReadOnlyUsage

		private class ReadOnlyUsage : CacheUsage
		{
			internal override HbmCacheUsage ToHbm()
			{
				return HbmCacheUsage.ReadOnly;
			}
		}

		#endregion

		#region Nested type: ReadWriteUsage

		private class ReadWriteUsage : CacheUsage
		{
			internal override HbmCacheUsage ToHbm()
			{
				return HbmCacheUsage.ReadWrite;
			}
		}

		#endregion

		#region Nested type: TransactionalUsage

		private class TransactionalUsage : CacheUsage
		{
			internal override HbmCacheUsage ToHbm()
			{
				return HbmCacheUsage.Transactional;
			}
		}

		#endregion
	}
}