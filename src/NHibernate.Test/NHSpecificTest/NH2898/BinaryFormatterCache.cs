using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using NHibernate.Cache;
#if !NETFX
using NHibernate.Util;
#endif

namespace NHibernate.Test.NHSpecificTest.NH2898
{
	public partial class BinaryFormatterCache : CacheBase
	{
		private readonly IDictionary _hashtable = new Hashtable();
		private readonly string _regionName;

		public BinaryFormatterCache(string regionName)
		{
			_regionName = regionName;
		}

		public override bool PreferMultipleGet => false;

		public override object Get(object key)
		{
			var entry = _hashtable[key] as byte[];
			if (entry == null)
				return null;

			var fmt = new BinaryFormatter
			{
#if !NETFX
				SurrogateSelector = new SerializationHelper.SurrogateSelector()	
#endif
			};
			using (var stream = new MemoryStream(entry))
			{
				return fmt.Deserialize(stream);
			}
		}

		public override void Put(object key, object value)
		{
			var fmt = new BinaryFormatter
			{
#if !NETFX
				SurrogateSelector = new SerializationHelper.SurrogateSelector()	
#endif
			};
			using (var stream = new MemoryStream())
			{
				fmt.Serialize(stream, value);
				_hashtable[key] = stream.ToArray();
			}
		}

		public override void Remove(object key)
		{
			_hashtable.Remove(key);
		}

		public override void Clear()
		{
			_hashtable.Clear();
		}

		public override void Destroy()
		{
		}

		public override object Lock(object key)
		{
			// local cache, so we use synchronization
			return null;
		}

		public override void Unlock(object key, object lockValue)
		{
			// local cache, so we use synchronization
		}

		public override long NextTimestamp()
		{
			return Timestamper.Next();
		}

		public override int Timeout
		{
			get { return Timestamper.OneMs*60000; }
		}

		public override string RegionName
		{
			get { return _regionName; }
		}
	}
}
