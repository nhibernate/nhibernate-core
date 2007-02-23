using System.Collections;

namespace NHibernate.Caches.SysCache
{
	/// <summary>
	/// Config properties
	/// </summary>
	public class CacheConfig
	{
		private string regionName;
		private Hashtable properties;

		/// <summary>
		/// build a configuration
		/// </summary>
		/// <param name="region"></param>
		/// <param name="expiration"></param>
		/// <param name="priority"></param>
		public CacheConfig(string region, string expiration, string priority)
		{
			regionName = region;
			properties = new Hashtable();
			properties.Add("expiration", expiration);
			properties.Add("priority", priority);
		}

		/// <summary></summary>
		public string Region
		{
			get { return regionName; }
		}

		/// <summary></summary>
		public IDictionary Properties
		{
			get { return properties; }
		}
	}
}