using System;
using System.Runtime.CompilerServices;

namespace NHibernate.Cache 
{
	/// <summary>
	/// Caches data that is never updated
	/// </summary>
	public class ReadOnlyCache : ICacheConcurrencyStrategy 
	{
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(ReadOnlyCache));
		private readonly ICache cache;


		public ReadOnlyCache(ICache cache) 
		{
			this.cache = cache;
		}

		#region ICacheConcurrencyStrategy Members

		[MethodImpl(MethodImplOptions.Synchronized)]
		public object Get(object key, long timestamp) 
		{
			object result = cache.Get(key);
			if ( result!=null && log.IsDebugEnabled) log.Debug("Cache hit: " + key);
			return result;
		}

		public void Lock(object key)
		{
			log.Error("Application attempted to edit read only item: " + key);
			throw new InvalidOperationException("Can't write to a readonly object");
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public bool Put(object key, object value, long timestamp) {
			if (log.IsDebugEnabled) log.Debug("Caching: " + key);
			cache.Put(key, value); 
			return true;
		}

		public void Release(object key) 
		{
			log.Error("Application attempted to edit read only item: " + key);
			throw new InvalidOperationException("Can't write to a readonly object");
		}

		public void Clear() 
		{
			cache.Clear();
		}

		public void Remove(object key) 
		{
			cache.Remove(key);
		}

		public void Destroy() 
		{
			try 
			{
				cache.Destroy();
			}
			catch(Exception e) 
			{
				log.Warn("Could not destroy cache", e);
			}
		}
		
		#endregion

	}
}
