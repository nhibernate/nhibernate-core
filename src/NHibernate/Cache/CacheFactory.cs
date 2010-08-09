
using NHibernate.Cfg;
using System.Collections.Generic;

namespace NHibernate.Cache
{
	/// <summary>
	/// Factory class for creating an <see cref="ICacheConcurrencyStrategy"/>.
	/// </summary>
	public static class CacheFactory
	{
		private static readonly ILogger log = LoggerProvider.LoggerFor(typeof(CacheFactory));

		public const string ReadOnly = "read-only";
		public const string ReadWrite = "read-write";
		public const string NonstrictReadWrite = "nonstrict-read-write";

		/// <remarks>
		/// No providers implement transactional caching currently,
		/// it was ported from Hibernate just for the sake of completeness.
		/// </remarks>
		public const string Transactional = "transactional";

		/// <summary>
		/// Creates an <see cref="ICacheConcurrencyStrategy"/> from the parameters.
		/// </summary>
		/// <param name="usage">The name of the strategy that <see cref="ICacheProvider"/> should use for the class.</param>
		/// <param name="name">The name of the class the strategy is being created for.</param>
		/// <param name="mutable"><see langword="true" /> if the object being stored in the cache is mutable.</param>
		/// <param name="settings">Used to retrieve the global cache region prefix.</param>
		/// <param name="properties">Properties the cache provider can use to configure the cache.</param>
		/// <returns>An <see cref="ICacheConcurrencyStrategy"/> to use for this object in the <see cref="ICache"/>.</returns>
		public static ICacheConcurrencyStrategy CreateCache(string usage, string name, bool mutable, Settings settings,
		                                                    IDictionary<string,string> properties)
		{
			if (usage == null || !settings.IsSecondLevelCacheEnabled) return null; //no cache

			string prefix = settings.CacheRegionPrefix;
			if (prefix != null) name = prefix + '.' + name;

			if (log.IsDebugEnabled)
			{
				log.Debug(string.Format("cache for: {0} usage strategy: {1}", name, usage));
			}

			ICacheConcurrencyStrategy ccs;
			switch (usage)
			{
				case ReadOnly:
					if (mutable)
					{
						log.Warn("read-only cache configured for mutable: " + name);
					}
					ccs = new ReadOnlyCache();
					break;
				case ReadWrite:
					ccs = new ReadWriteCache();
					break;
				case NonstrictReadWrite:
					ccs = new NonstrictReadWriteCache();
					break;
					//case CacheFactory.Transactional:
					//	ccs = new TransactionalCache();
					//	break;
				default:
					throw new MappingException(
						"cache usage attribute should be read-write, read-only, nonstrict-read-write, or transactional");
			}

			ICache impl;
			try
			{
				impl = settings.CacheProvider.BuildCache(name, properties);
			}
			catch (CacheException e)
			{
				throw new HibernateException("Could not instantiate cache implementation", e);
			}
			ccs.Cache = impl;

			return ccs;
		}
	}
}