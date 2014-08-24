using System;
using System.Collections;
using System.Text;
using log4net;
using NHibernate.Cache;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH720
{
	public class FooCacheProvider : ICacheProvider
	{
		private static readonly ILog log;
		private static Hashtable caches;

		static FooCacheProvider()
		{
			log = LogManager.GetLogger(typeof(FooCacheProvider));
			caches = new Hashtable();
		}

		public static ICache BuildCacheStatic(string regionName, IDictionary<string,string> properties)
		{
			if (regionName != null && caches[regionName] != null)
			{
				return caches[regionName] as ICache;
			}

			if (regionName == null)
			{
				regionName = "";
			}
			if (properties == null)
			{
				properties = new Dictionary<string,string>();
			}
			if (log.IsDebugEnabled)
			{
				StringBuilder sb = new StringBuilder();
				foreach (KeyValuePair<string, string> de in properties)
				{
					sb.Append("name=");
					sb.Append(de.Key);
					sb.Append("&value=");
					sb.Append(de.Value);
					sb.Append(";");
				}
				log.Debug("building cache with region: " + regionName + ", properties: " + sb.ToString());
			}
			FooCache cache = new FooCache(regionName, properties);
			caches.Add(regionName, cache);
			return cache;
		}

		public ICache BuildCache(string regionName, IDictionary<string, string> properties)
		{
			return BuildCacheStatic(regionName, properties);
		}

		public static bool RegionExists(string regionName)
		{
			return caches.ContainsKey(regionName);
		}

		public static int RegionCount
		{
			get { return caches.Count; }
		}

		/// <summary></summary>
		/// <returns></returns>
		public long NextTimestamp()
		{
			return Timestamper.Next();
		}

		/// <summary></summary>
		/// <param name="properties"></param>
		public void Start(IDictionary<string, string> properties)
		{
		}

		/// <summary></summary>
		public void Stop()
		{
		}
	}

	public class FooCache : ICache
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(FooCache));
		private string _region;
		private TimeSpan _expiration;
		public static readonly TimeSpan DefaultExpiration = TimeSpan.FromSeconds(300);

		public FooCache(string region, IDictionary<string, string> properties)
		{
			_region = region;

			Configure(properties);
		}

		public string RegionName
		{
			get { return _region; }
		}

		public TimeSpan Expiration
		{
			get { return _expiration; }
		}

		private void Configure(IDictionary<string, string> props)
		{
			if (props == null)
			{
				if (log.IsDebugEnabled)
				{
					log.Debug("configuring cache with default values");
				}
				_expiration = DefaultExpiration;
			}
			else
			{
				if (props.ContainsKey("expiration"))
				{
					try
					{
						int seconds = Convert.ToInt32(props["expiration"]);
						_expiration = TimeSpan.FromSeconds(seconds);
						if (log.IsDebugEnabled)
						{
							log.Debug("new expiration value: " + seconds.ToString());
						}
					}
					catch (Exception ex)
					{
						if (log.IsWarnEnabled)
						{
							log.Warn("error parsing expiration value");
						}
						throw new ArgumentException("could not parse exception as a number of seconds", "expiration", ex);
					}
				}
				else
				{
					if (log.IsDebugEnabled)
					{
						log.Debug("no expiration value given, using defaults");
					}
					_expiration = DefaultExpiration;
				}
			}
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
			Clear();
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

		public int Timeout
		{
			get { return Timestamper.OneMs * 60000; } // 60 seconds
		}
	}

	public class A
	{
		private int id;
		private string foo;
		private IList<B> bees;

		protected A()
		{
		}

		public A(int id, string foo)
		{
			this.id = id;
			this.foo = foo;
			bees = new List<B>();
		}

		public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual string Foo
		{
			get { return foo; }
			set { foo = value; }
		}

		public virtual IList<B> Bees
		{
			get { return bees; }
		}
	}

	public class B
	{
		private int id;
		private A a;
		private string foo;

		protected B()
		{
		}

		public B(int id, A a, string foo)
		{
			this.id = id;
			this.a = a;
			this.foo = foo;
		}

		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		public A A
		{
			get { return a; }
		}

		public string Foo
		{
			get { return foo; }
			set { foo = value; }
		}
	}
}