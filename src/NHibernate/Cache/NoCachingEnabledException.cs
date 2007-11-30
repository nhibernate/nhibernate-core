using System;
using Environment = NHibernate.Cfg.Environment;

namespace NHibernate.Cache
{
	[Serializable]
	public class NoCachingEnabledException : CacheException
	{
		private const string ExceptionMessage = "Second-level cache is enabled, but no cache provider was selected. " +
			"Please use " + Environment.CacheProvider + " to specify a cache provider such as SysCacheProvider";

		public NoCachingEnabledException()
			: base(ExceptionMessage)
		{
		}
	}
}