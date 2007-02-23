#region License

//
//  MemCache - A cache provider for NHibernate using the .NET client
//  (http://sourceforge.net/projects/memcacheddotnet) for memcached,
//  which is located at http://www.danga.com/memcached/.
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
// CLOVER:OFF
//

#endregion

using System;
using System.Collections;
using System.Configuration;
using System.Xml;

using log4net;

namespace NHibernate.Caches.MemCache
{
	/// <summary>
	/// config file provider
	/// </summary>
	public class MemCacheSectionHandler : IConfigurationSectionHandler
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof(MemCacheSectionHandler));

		/// <summary>
		/// parse the config section
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="configContext"></param>
		/// <param name="section"></param>
		/// <returns>an array of <see cref="MemCacheConfig"/> objects</returns>
		public object Create(object parent, object configContext, XmlNode section)
		{
			ArrayList configs = new ArrayList();
			if (section != null)
			{
				XmlNodeList nodes = section.SelectNodes("memcached");
				foreach (XmlNode node in nodes)
				{
					XmlAttribute h = node.Attributes["host"];
					XmlAttribute p = node.Attributes["port"];
					XmlAttribute w = node.Attributes["weight"];
					if (h == null || p == null)
					{
						if (_log.IsWarnEnabled)
						{
							_log.Warn("incomplete node found - each memcached element must have a 'host' and a 'port' attribute.");
						}
						continue;
					}
					string host = h.Value;
					int port = ((p.Value == null || p.Value.Length < 1) ? 0 : Convert.ToInt32(p.Value));
					int weight = ((w == null || w.Value == null || w.Value.Length < 1) ? 0 : Convert.ToInt32(w.Value));
					configs.Add(new MemCacheConfig(host, port, weight));
				}
			}
			return configs.ToArray(typeof(MemCacheConfig));
		}
	}
}