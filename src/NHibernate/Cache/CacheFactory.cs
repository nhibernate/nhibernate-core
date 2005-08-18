using System.Xml;
using log4net;

namespace NHibernate.Cache
{
	/// <summary>
	/// Factory class for creating an <see cref="ICacheConcurrencyStrategy"/>.
	/// </summary>
	public sealed class CacheFactory
	{
		private static readonly ILog log = LogManager.GetLogger( typeof( CacheFactory ) );

		private CacheFactory()
		{
			// not publically creatable
		}

		/// <summary></summary>
		public const string ReadOnly = "read-only";
		/// <summary></summary>
		public const string ReadWrite = "read-write";
		/// <summary></summary>
		public const string NonstrictReadWrite = "nonstrict-read-write";
		/// <summary></summary>
		/// <remarks>
		/// No providers implement transactional caching currently,
		/// it was ported from Hibernate just for the sake of completeness.
		/// </remarks>
		public const string Transactional = "transactional";

		/// <summary>
		/// Creates an <see cref="ICacheConcurrencyStrategy"/> from the parameters.
		/// </summary>
		/// <param name="node">The <see cref="XmlNode"/> that contains the attribute <c>usage</c>.</param>
		/// <param name="name">The name of the class the strategy is being created for.</param>
		/// <param name="mutable"><c>true</c> if the object being stored in the cache is mutable.</param>
		/// <returns>An <see cref="ICacheConcurrencyStrategy"/> to use for this object in the <see cref="ICache"/>.</returns>
		public static ICacheConcurrencyStrategy CreateCache( XmlNode node, string name, bool mutable )
		{
			return CacheFactory.CreateCache( node.Attributes[ "usage" ].Value, name, mutable );
		}

		/// <summary>
		/// Creates an <see cref="ICacheConcurrencyStrategy"/> from the parameters.
		/// </summary>
		/// <param name="usage">The name of the strategy that <see cref="ICacheProvider"/> should use for the class.</param>
		/// <param name="name">The name of the class the strategy is being created for.</param>
		/// <param name="mutable"><c>true</c> if the object being stored in the cache is mutable.</param>
		/// <returns>An <see cref="ICacheConcurrencyStrategy"/> to use for this object in the <see cref="ICache"/>.</returns>
		// was private in h2.1
		public static ICacheConcurrencyStrategy CreateCache( string usage, string name, bool mutable )
		{
			if( log.IsDebugEnabled )
			{
				log.Debug( string.Format( "cache for: {0} usage strategy: {1}", name, usage ) );
			}

			ICacheConcurrencyStrategy ccs = null;
			switch( usage )
			{
				case CacheFactory.ReadOnly:
					if( mutable )
					{
						log.Warn( "read-only cache configured for mutable: " + name );
					}
					ccs = new ReadOnlyCache();
					break;
				case CacheFactory.ReadWrite:
					ccs = new ReadWriteCache();
					break;
				case CacheFactory.NonstrictReadWrite:
					ccs = new NonstrictReadWriteCache();
					break;
				//case CacheFactory.Transactional:
				//	ccs = new TransactionalCache();
				//	break;
				default:
					throw new MappingException( "cache usage attribute should be read-write, read-only, nonstrict-read-write, or transactional" );
			}

			return ccs;
		}
	}
}