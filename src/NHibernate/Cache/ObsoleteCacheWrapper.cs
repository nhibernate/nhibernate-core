namespace NHibernate.Cache
{
	// 6.0 TODO: remove this class
	internal partial class ObsoleteCacheWrapper : CacheBase
	{
#pragma warning disable 618
		private ICache _cache;
#pragma warning restore 618

#pragma warning disable 618
		internal ObsoleteCacheWrapper(ICache cache)
#pragma warning restore 618
		{
			_cache = cache;
		}

		public override long NextTimestamp()
		{
			return _cache.NextTimestamp();
		}

		public override int Timeout => _cache.Timeout;

		public override string RegionName => _cache.RegionName;

		public override bool PreferMultipleGet => false;

		public override object Get(object key)
		{
			return _cache.Get(key);
		}

		public override void Put(object key, object value)
		{
			_cache.Put(key, value);
		}

		public override void Remove(object key)
		{
			_cache.Remove(key);
		}

		public override void Clear()
		{
			_cache.Clear();
		}

		public override void Destroy()
		{
			_cache.Destroy();
		}

		public override object Lock(object key)
		{
			_cache.Lock(key);
			return null;
		}

		public override void Unlock(object key, object lockValue)
		{
			_cache.Unlock(key);
		}
	}
}
