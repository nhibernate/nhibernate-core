#region License

//
//  SysCache - A cache provider for NHibernate using System.Web.Caching.Cache.
//
//  This library is free software; you can redistribute it and/or
//  modify it under the terms of the GNU Lesser General Public
//  License as published by the Free Software Foundation; either
//  version 2.1 of the License, or (at your option) any later version.
//
//  This library is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//  Lesser General Public License for more details.
//
//  You should have received a copy of the GNU Lesser General Public
//  License along with this library; if not, write to the Free Software
//  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//

#endregion

using System;
using System.Collections;
using System.Configuration;
using System.Text;

using log4net;

using NHibernate.Cache;

namespace NHibernate.Caches.SysCache
{
	/// <summary>
	/// Cache provider using the System.Web.Caching classes
	/// </summary>
	public class SysCacheProvider : ICacheProvider
	{
		private static readonly ILog log;
		private static Hashtable caches;

		static SysCacheProvider()
		{
			log = LogManager.GetLogger(typeof(SysCacheProvider));
			caches = new Hashtable();

			CacheConfig[] list = ConfigurationSettings.GetConfig("syscache") as CacheConfig[];
			if (list != null)
			{
				foreach (CacheConfig cache in list)
				{
					caches.Add(cache.Region, new SysCache(cache.Region, cache.Properties));
				}
			}
		}

		/// <summary>
		/// build a new SysCache
		/// </summary>
		/// <param name="regionName"></param>
		/// <param name="properties"></param>
		/// <returns></returns>
		[CLSCompliant(false)]
		public ICache BuildCache(string regionName, IDictionary properties)
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
				properties = new Hashtable();
			}
			if (log.IsDebugEnabled)
			{
				StringBuilder sb = new StringBuilder();
				foreach (DictionaryEntry de in properties)
				{
					sb.Append("name=");
					sb.Append(de.Key.ToString());
					sb.Append("&value=");
					sb.Append(de.Value.ToString());
					sb.Append(";");
				}
				log.Debug("building cache with region: " + regionName + ", properties: " + sb.ToString());
			}
			return new SysCache(regionName, properties);
		}

		/// <summary></summary>
		/// <returns></returns>
		public long NextTimestamp()
		{
			return Timestamper.Next();
		}

		/// <summary></summary>
		/// <param name="properties"></param>
		public void Start(IDictionary properties)
		{
		}

		/// <summary></summary>
		public void Stop()
		{
		}
	}
}