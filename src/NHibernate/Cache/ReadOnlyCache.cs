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
		
		private object lockObject = new object();
		private ICache _cache;

		public ReadOnlyCache() 
		{
		}

		#region ICacheConcurrencyStrategy Members

		public object Get(object key, long timestamp) 
		{
			lock( lockObject ) 
			{
				if( log.IsDebugEnabled ) 
				{
					log.Debug( "Cache lookup: " + key );
				}

				object result = _cache.Get( key );
				if ( result!=null && log.IsDebugEnabled ) 
				{
					log.Debug( "Cache hit" );
				}
				else if( log.IsDebugEnabled )
				{
					log.Debug( "Cache miss" );
				}
				return result;
			}
		}

		public void Lock(object key)
		{
			log.Error("Application attempted to edit read only item: " + key);
			throw new InvalidOperationException("Can't write to a readonly object");
		}

		public bool Put(object key, object value, long timestamp) {
			
			lock( lockObject ) 
			{
				if (log.IsDebugEnabled) 
				{
					log.Debug("Caching: " + key);
				}
				_cache.Put(key, value); 
				return true;
			}
		}

		public void Release(object key) 
		{
			log.Error("Application attempted to edit read only item: " + key);
			throw new InvalidOperationException("Can't write to a readonly object");
		}

		public void Clear() 
		{
			_cache.Clear();
		}

		public void Remove(object key) 
		{
			_cache.Remove(key);
		}

		public void Destroy() 
		{
			try 
			{
				_cache.Destroy();
			}
			catch(Exception e) 
			{
				log.Warn("Could not destroy cache", e);
			}
		}

		public ICache Cache 
		{
			get { return _cache; }
			set { _cache = value; }
		}
		
		#endregion

	}
}
