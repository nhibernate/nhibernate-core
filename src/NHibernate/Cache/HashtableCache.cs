using System.Collections;

namespace NHibernate.Cache
{
	/// <summary>
	/// A simple <see cref="Hashtable" />-based cache
	/// </summary>
	public partial class HashtableCache : CacheBase
	{
		private IDictionary hashtable = new Hashtable();
		private readonly string regionName;

		public HashtableCache(string regionName)
		{
			this.regionName = regionName;
		}

		/// <inheritdoc />
		public override object Get(object key)
		{
			return hashtable[key];
		}

		/// <inheritdoc />
		public override void Put(object key, object value)
		{
			hashtable[key] = value;
		}

		/// <inheritdoc />
		public override void Remove(object key)
		{
			hashtable.Remove(key);
		}

		/// <inheritdoc />
		public override void Clear()
		{
			hashtable.Clear();
		}

		/// <inheritdoc />
		public override void Destroy()
		{
		}

		/// <inheritdoc />
		public override object Lock(object key)
		{
			// local cache, so we use synchronization
			return null;
		}

		/// <inheritdoc />
		public override void Unlock(object key, object lockValue)
		{
			// local cache, so we use synchronization
		}

		/// <inheritdoc />
		public override long NextTimestamp()
		{
			return Timestamper.Next();
		}

		/// <inheritdoc />
		public override int Timeout
		{
			get { return Timestamper.OneMs * 60000; // ie. 60 seconds
			}
		}

		/// <inheritdoc />
		public override string RegionName
		{
			get { return regionName; }
		}

		/// <inheritdoc />
		public override bool PreferMultipleGet => false;
	}
}
