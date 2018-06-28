using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate.Cache;

namespace NHibernate.Test.CacheTest.Caches
{
	public class BatchableCacheProvider : ICacheProvider
	{
		#region ICacheProvider Members

		public ICache BuildCache(string regionName, IDictionary<string, string> properties)
		{
			return new BatchableCache(regionName);
		}

		public long NextTimestamp()
		{
			return Timestamper.Next();
		}

		public void Start(IDictionary<string, string> properties)
		{
		}

		public void Stop()
		{
		}

		#endregion
	}
}
