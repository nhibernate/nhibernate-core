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
		/// <param name="relativeExpiration"></param>
		/// <param name="staticExpiration"></param>
		/// <param name="priority"></param>
		public CacheConfig( string region, string relativeExpiration, string staticExpiration, string priority )
		{
			regionName = region;
			properties = new Hashtable();
			properties.Add( "relativeExpiration", relativeExpiration );
			properties.Add( "staticExpiration", staticExpiration );
			properties.Add( "priority", priority );
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