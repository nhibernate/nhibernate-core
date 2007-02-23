using System.Collections;

namespace NHibernate.Cache
{
	/// <summary>
	/// Cache Provider plugin for NHibernate that is configured by using
	/// <c>hibernate.cache.provider_class="NHibernate.Cache.HashtableCacheProvider"</c>
	/// </summary>
	public class HashtableCacheProvider : ICacheProvider
	{
		#region ICacheProvider Members

		/// <summary>
		/// 
		/// </summary>
		/// <param name="regionName"></param>
		/// <param name="properties"></param>
		/// <returns></returns>
		public ICache BuildCache(string regionName, IDictionary properties)
		{
			return new HashtableCache(regionName);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public long NextTimestamp()
		{
			return Timestamper.Next();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="properties"></param>
		public void Start(IDictionary properties)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		public void Stop()
		{
		}

		#endregion
	}
}