using System;

namespace NHibernate.Cache {
	/// <summary>
	/// Caches data that is never updated
	/// </summary>
	public class ReadOnlyCache : ICacheConcurrencyStrategy {
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(ReadOnlyCache));
		private ICache cache;


		public ReadOnlyCache(ICache cache) {
			this.cache = cache;
		}

		public object Get(object key, long timestamp) {
			lock(this) {
				object result = cache[key];
				if ( result!=null && log.IsDebugEnabled) log.Debug("Cache hit: " + key);
				return result;
			}
		}

		public void Lock(object key) {
			log.Error("Application attempted to edit read only item: " + key);
			throw new InvalidOperationException("Can't write to a readonly object");
		}

		public bool Put(object key, object value, long timestamp) {
			lock(this) {
				if (log.IsDebugEnabled) log.Debug("Caching: " + key);
				cache[key] = value;
				return true;
			}
		}

		public void Release(object key) {
			log.Error("Application attempted to edit read only item: " + key);
			throw new InvalidOperationException("Can't write to a readonly object");
		}
	}
}
