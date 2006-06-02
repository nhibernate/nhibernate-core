using System.Collections;
using System.Configuration;
using System.Xml;

namespace NHibernate.Caches.SysCache
{
	/// <summary>
	/// Config file provider
	/// </summary>
	public class SysCacheSectionHandler : IConfigurationSectionHandler
	{
		/// <summary>
		/// parse the config section
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="configContext"></param>
		/// <param name="section"></param>
		/// <returns>an array of CacheConfig objects</returns>
		public object Create( object parent, object configContext, XmlNode section )
		{
			ArrayList caches = new ArrayList();
			XmlNodeList nodes = section.SelectNodes( "cache" );
			foreach (XmlNode node in nodes )
			{
				string region = null;
				string relativeExpiration = null;
				string staticExpiration = null;
				string priority = "3";
				XmlAttribute r = node.Attributes[ "region" ];
				XmlAttribute re = node.Attributes[ "relativeExpiration" ];
				XmlAttribute ae = node.Attributes[ "staticExpiration" ];
				XmlAttribute p = node.Attributes[ "priority" ];
				if( r != null )
				{
					region = r.Value;
				}
				if( re != null && ae == null )
				{
					relativeExpiration = re.Value;
				}
				else if( ae != null && re == null )
				{
					staticExpiration = ae.Value;
				}
				if( p != null )
				{
					priority = p.Value;
				}
				if( region != null && ( relativeExpiration != null || staticExpiration != null ) )
				{
					caches.Add( new CacheConfig( region, relativeExpiration, staticExpiration, priority ) );
				}
			}
			return ( CacheConfig[] ) caches.ToArray( typeof( CacheConfig ) );
		}
	}
}
