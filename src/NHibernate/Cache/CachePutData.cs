using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NHibernate.Persister.Collection;
using NHibernate.Persister.Entity;

namespace NHibernate.Cache
{
	/// <summary>
	/// The data used to put a value to the 2nd level cache.
	/// </summary>
	internal class CachePutData
	{
		public CachePutData(CacheKey key, object value, object version, IComparer versionComparer, bool minimalPut)
		{
			Key = key;
			Value = value;
			Version = version;
			VersionComparer = versionComparer;
			MinimalPut = minimalPut;
		}

		public CacheKey Key { get; }

		public object Value { get; }

		public object Version { get; }

		public IComparer VersionComparer { get; }

		public bool MinimalPut { get; }
	}
}
