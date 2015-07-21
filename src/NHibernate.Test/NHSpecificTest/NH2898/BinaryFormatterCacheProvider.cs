using System.Collections.Generic;
using NHibernate.Cache;

namespace NHibernate.Test.NHSpecificTest.NH2898
{
	public class BinaryFormatterCacheProvider : ICacheProvider
	{
		#region ICacheProvider Members

		public ICache BuildCache(string regionName, IDictionary<string, string> properties)
		{
			return new BinaryFormatterCache(regionName);
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