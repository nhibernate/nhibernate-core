using System;
using System.Xml;

namespace NHibernate.Cache
{
	/// <summary>
	/// Summary description for CacheFactory.
	/// </summary>
	public class CacheFactory
	{
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger( typeof(CacheFactory) );

		private CacheFactory()
		{
			// not publically creatable
		}

		public const string ReadOnly = "read-only";
		public const string ReadWrite = "read-write";
		public const string NonstrictReadWrite = "nonstrict-read-write";

		public static ICacheConcurrencyStrategy CreateCache(XmlNode node, string name, bool mutable) 
		{
			return CacheFactory.CreateCache( node.Attributes["usage"].Value, name, mutable );
		}
		
		// was private in h2.1
		public static ICacheConcurrencyStrategy CreateCache(string usage, string name, bool mutable) 
		{
			if( log.IsDebugEnabled ) 
			{
				log.Debug( "cache for: " + name + "usage strategy: " + usage );
			}
		
			ICacheConcurrencyStrategy ccs = null;
			switch( usage ) 
			{
				case CacheFactory.ReadOnly :
					if( mutable ) 
					{
						log.Warn( "read-only cache configured for mutable: " + name );
					}
					ccs = new ReadOnlyCache();
					break;
				case CacheFactory.ReadWrite :
					ccs = new ReadWriteCache();
					break;
				case CacheFactory.NonstrictReadWrite :
					ccs = new NonstrictReadWriteCache();
					break;
				default :
					throw new MappingException( "cache usage attribute should be read-write, read-only, nonstrict-read-write, or transactional" );
			}

			return ccs;
		
		}

	}
}
