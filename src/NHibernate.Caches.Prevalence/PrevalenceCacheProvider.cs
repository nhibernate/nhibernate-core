using System;
using System.Collections;
using System.Text;
using log4net;
using NHibernate.Cache;

namespace NHibernate.Caches.Prevalence
{
	/// <summary>
	/// Cache provider using <a href="http://bbooprevalence.sourceforge.net/">Bamboo Prevalence</a>,
	/// a Prevayler implementation in .NET.
	/// </summary>
	public class PrevalenceCacheProvider : ICacheProvider
	{
		private static readonly ILog log = LogManager.GetLogger( typeof( PrevalenceCacheProvider ) );

		/// <summary>
		/// build and return a new cache implementation
		/// </summary>
		/// <param name="regionName"></param>
		/// <param name="properties"></param>
		/// <returns></returns>
		[CLSCompliant(false)]
		public ICache BuildCache( string regionName, IDictionary properties )
		{
			if( regionName == null )
			{
				regionName = "";
			}
			if( properties == null )
			{
				properties = new Hashtable();
			}
			if( log.IsDebugEnabled )
			{
				StringBuilder sb = new StringBuilder();
				foreach( DictionaryEntry de in properties )
				{
					sb.Append( "name=" );
					sb.Append( de.Key.ToString() );
					sb.Append( "&value=" );
					sb.Append( de.Value.ToString() );
					sb.Append( ";" );
				}
				log.Debug( "building cache with region: " + regionName + ", properties: " + sb.ToString() );
			}
			return new PrevalenceCache( regionName, properties );
		}

		/// <summary></summary>
		/// <returns></returns>
		public long NextTimestamp()
		{
			return Timestamper.Next();
		}

		/// <summary></summary>
		/// <param name="properties"></param>
		public void Start( IDictionary properties )
		{
		}

		/// <summary></summary>
		public void Stop()
		{
		}
	}
}