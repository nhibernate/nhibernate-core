using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using NHibernate.Cache;

namespace NHibernate.Test.NHSpecificTest.NH2898
{
	public partial class BinaryFormatterCache : ICache
	{
		private readonly IDictionary _hashtable = new Hashtable();
		private readonly string _regionName;

		public BinaryFormatterCache(string regionName)
		{
			_regionName = regionName;
		}

		public object Get(object key)
		{
			var entry = _hashtable[key] as byte[];
			if (entry == null)
				return null;

			var fmt = new BinaryFormatter();
			using (var stream = new MemoryStream(entry))
			{
				return fmt.Deserialize(stream);
			}
		}

		public void Put(object key, object value)
		{
			var fmt = new BinaryFormatter();
			using (var stream = new MemoryStream())
			{
				fmt.Serialize(stream, value);
				_hashtable[key] = stream.ToArray();
			}
		}

		public void Remove(object key)
		{
			_hashtable.Remove(key);
		}

		public void Clear()
		{
			_hashtable.Clear();
		}

		public void Destroy()
		{
		}

		public void Lock(object key)
		{
			// local cache, so we use synchronization
		}

		public void Unlock(object key)
		{
			// local cache, so we use synchronization
		}

		public long NextTimestamp()
		{
			return Timestamper.Next();
		}

		public int Timeout
		{
			get { return Timestamper.OneMs*60000; }
		}

		public string RegionName
		{
			get { return _regionName; }
		}
	}
}