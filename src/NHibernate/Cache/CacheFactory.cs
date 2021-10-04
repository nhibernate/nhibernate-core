using System;
using System.Collections.Generic;
using NHibernate.Cfg;
using NHibernate.Util;

namespace NHibernate.Cache
{
	/// <summary>
	/// Factory class for creating an <see cref="ICacheConcurrencyStrategy"/>.
	/// </summary>
	public static class CacheFactory
	{
		private static readonly INHibernateLogger log = NHibernateLogger.For(typeof(CacheFactory));

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
		// Since v5.3
		[Obsolete("Please use overload with a CacheBase parameter.")]
		public static ICacheConcurrencyStrategy CreateCache(
			string usage,
			string name,
			bool mutable,
			Settings settings,
			IDictionary<string, string> properties)
		{
			if (usage == null || !settings.IsSecondLevelCacheEnabled) return null;

			var cache = BuildCacheBase(name, settings, properties);

			var ccs = CreateCache(usage, cache, settings);

			if (mutable && usage == ReadOnly)
				log.Warn("read-only cache configured for mutable: {0}", name);

			return ccs;
		}

		/// <summary>
		/// Creates an <see cref="ICacheConcurrencyStrategy"/> from the parameters.
		/// </summary>
		/// <param name="usage">The name of the strategy that <see cref="ICacheProvider"/> should use for the class.</param>
		/// <param name="cache">The <see cref="CacheBase"/> used for this strategy.</param>
		/// <returns>An <see cref="ICacheConcurrencyStrategy"/> to use for this object in the <see cref="ICache"/>.</returns>
		// TODO: Since v5.4
		//[Obsolete("Please use overload with a CacheBase and Settings parameters.")]
		public static ICacheConcurrencyStrategy CreateCache(string usage, CacheBase cache)
		{
			return CreateCache(usage, cache, null);
		}

		/// <summary>
		/// Creates an <see cref="ICacheConcurrencyStrategy"/> from the parameters.
		/// </summary>
		/// <param name="usage">The name of the strategy that <see cref="ICacheProvider"/> should use for the class.</param>
		/// <param name="cache">The <see cref="CacheBase"/> used for this strategy.</param>
		/// <param name="settings">NHibernate settings</param>
		/// <returns>An <see cref="ICacheConcurrencyStrategy"/> to use for this object in the <see cref="ICache"/>.</returns>
		public static ICacheConcurrencyStrategy CreateCache(string usage, CacheBase cache, Settings settings)
		{
			if (log.IsDebugEnabled())
				log.Debug("cache for: {0} usage strategy: {1}", cache.RegionName, usage);

			ICacheConcurrencyStrategy ccs;
			switch (usage)
			{
				case ReadOnly:
					ccs = new ReadOnlyCache();
					break;
				case ReadWrite:
					ccs = new ReadWriteCache(settings == null ? new AsyncReaderWriterLock() : settings.CacheReadWriteReadWriteLockFactory.Create());
					break;
				case NonstrictReadWrite:
					ccs = new NonstrictReadWriteCache();
					break;
				//case CacheFactory.Transactional:
				//	ccs = new TransactionalCache();
				//	break;
				default:
					throw new MappingException(
						"cache usage attribute should be read-write, read-only or nonstrict-read-write");
			}

			ccs.Cache = cache;

			return ccs;
		}

		internal static CacheBase BuildCacheBase(string name, Settings settings, IDictionary<string, string> properties)
		{
			try
			{
				return settings.CacheProvider.BuildCache(name, properties).AsCacheBase();
			}
			catch (CacheException e)
			{
				throw new HibernateException("Could not instantiate cache implementation", e);
			}
		}
	}
}
