namespace NHibernate.Cache
{
	/// <summary>
	/// Used by <see cref="NoCacheProvider"/>
	/// </summary>
	public partial class FakeCache : CacheBase
	{
		public FakeCache(string regionName)
		{
			RegionName = regionName;
		}

		public override bool PreferMultipleGet => false;

		public override object Get(object key)
		{
			return null;
		}

		public override void Put(object key, object value)
		{
		}

		public override void Remove(object key)
		{
		}

		public override void Clear()
		{
		}

		public override void Destroy()
		{
		}

		public override object Lock(object key)
		{
			return null;
		}

		public override void Unlock(object key, object lockValue)
		{
		}

		public override long NextTimestamp()
		{
			return Timestamper.Next();
		}

		public override int Timeout => 0;
		public override string RegionName { get; }
	}
}
