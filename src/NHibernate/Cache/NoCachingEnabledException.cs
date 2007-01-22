using System;
using Environment=NHibernate.Cfg.Environment;

namespace NHibernate.Cache
{
	[Serializable]
	public class NoCachingEnabledException : CacheException
	{
		private const string ExceptionMessage = "Second-level cache is not enabled for usage ["
			+ Environment.UseSecondLevelCache + " | " + Environment.UseQueryCache + "]";

		public NoCachingEnabledException() : base(ExceptionMessage)
		{
		}
	}
}
