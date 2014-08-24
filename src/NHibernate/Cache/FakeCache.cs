namespace NHibernate.Cache
{
	/// <summary>
	/// Used by <see cref="NoCacheProvider"/>
	/// </summary>
	public class FakeCache : ICache
	{
		public FakeCache(string regionName)
		{
			RegionName = regionName;
		}

		public object Get(object key)
		{
			return null;
		}

		public void Put(object key, object value)
		{
		}

		public void Remove(object key)
		{
		}

		public void Clear()
		{
		}

		public void Destroy()
		{
		}

		public void Lock(object key)
		{
		}

		public void Unlock(object key)
		{
		}

		public long NextTimestamp()
		{
			return Timestamper.Next();
		}

		public int Timeout { get; private set; }
		public string RegionName { get; private set; }
	}
}