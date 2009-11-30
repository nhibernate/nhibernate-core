using NHibernate.Engine;

namespace NHibernate.Cfg.MappingSchema
{
	public static class HbmExtensions
	{
		public static Versioning.OptimisticLock ToOptimisticLock(this HbmOptimisticLockMode hbmOptimisticLockMode)
		{
			switch (hbmOptimisticLockMode)
			{
				case HbmOptimisticLockMode.None:
					return Versioning.OptimisticLock.None;
				case HbmOptimisticLockMode.Version:
					return Versioning.OptimisticLock.Version;
				case HbmOptimisticLockMode.Dirty:
					return Versioning.OptimisticLock.Dirty;
				case HbmOptimisticLockMode.All:
					return Versioning.OptimisticLock.All;
				default:
					return Versioning.OptimisticLock.Version;
			}
		}
	}
}