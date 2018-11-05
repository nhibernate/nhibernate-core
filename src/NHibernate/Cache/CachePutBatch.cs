using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NHibernate.Engine;

namespace NHibernate.Cache
{
	/// <summary>
	/// A batch for batching the <see cref="ICacheConcurrencyStrategy.Put"/> operation.
	/// </summary>
	internal partial class CachePutBatch : AbstractCacheBatch<CachePutData>
	{
		public CachePutBatch(ISessionImplementor session, ICacheConcurrencyStrategy cacheConcurrencyStrategy) : base(session, cacheConcurrencyStrategy)
		{
		}

		protected override void Execute(CachePutData[] data)
		{
			var length = data.Length;
			var keys = new CacheKey[length];
			var values = new object[length];
			var versions = new object[length];
			var versionComparers = new IComparer[length];
			var minimalPuts = new bool[length];

			for (int i = 0; i < length; i++)
			{
				var item = data[i];
				keys[i] = item.Key;
				values[i] = item.Value;
				versions[i] = item.Version;
				versionComparers[i] = item.VersionComparer;
				minimalPuts[i] = item.MinimalPut;
			}

			var factory = Session.Factory;
			var cacheStrategy = CacheConcurrencyStrategy;
			var puts = cacheStrategy.PutMany(keys, values, Session.Timestamp, versions, versionComparers, minimalPuts);

			if (factory.Statistics.IsStatisticsEnabled && puts.Any(o => o))
			{
				factory.StatisticsImplementor.SecondLevelCachePut(cacheStrategy.RegionName);
			}
		}
	}
}
