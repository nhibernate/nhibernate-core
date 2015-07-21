namespace NHibernate.Cfg
{
	/// <summary>
	/// Values for class-cache and collection-cache strategy.
	/// </summary>
	public enum EntityCacheUsage
	{
		/// <summary>Xml value: read-only</summary>
		Readonly,
		/// <summary>Xml value: read-write</summary>
		ReadWrite,
		/// <summary>Xml value: nonstrict-read-write</summary>
		NonStrictReadWrite,
		/// <summary>Xml value: transactional</summary>
		Transactional
	}

	/// <summary>
	/// Helper to parse <see cref="EntityCacheUsage"/> to and from XML string value.
	/// </summary>
	public static class EntityCacheUsageParser
	{
		private const string ReadOnlyXmlValue = "read-only";
		private const string ReadWriteXmlValue = "read-write";
		private const string NonstrictReadWriteXmlValue = "nonstrict-read-write";
		private const string TransactionalXmlValue = "transactional";

		/// <summary>
		/// Convert a <see cref="EntityCacheUsage"/> in its xml expected value.
		/// </summary>
		/// <param name="value">The <see cref="EntityCacheUsage"/> to convert.</param>
		/// <returns>The <see cref="EntityCacheUsage"/>.</returns>
		public static string ToString(EntityCacheUsage value)
		{
			switch (value)
			{
				case EntityCacheUsage.Readonly:
					return ReadOnlyXmlValue;
				case EntityCacheUsage.ReadWrite:
					return ReadWriteXmlValue;
				case EntityCacheUsage.NonStrictReadWrite:
					return NonstrictReadWriteXmlValue;
				case EntityCacheUsage.Transactional:
					return TransactionalXmlValue;
				default:
					return string.Empty;
			}
		}

		/// <summary>
		/// Convert a string to <see cref="EntityCacheUsage"/>.
		/// </summary>
		/// <param name="value">The string that represent <see cref="EntityCacheUsage"/>.</param>
		/// <returns>
		/// The <paramref name="value"/> converted to <see cref="EntityCacheUsage"/>.
		/// </returns>
		/// <exception cref="HibernateConfigException">If the values is invalid.</exception>
		/// <remarks>
		/// See <see cref="EntityCacheUsage"/> for allowed values.
		/// </remarks>
		public static EntityCacheUsage Parse(string value)
		{
			switch (value)
			{
				case ReadOnlyXmlValue:
					return EntityCacheUsage.Readonly;
				case ReadWriteXmlValue:
					return EntityCacheUsage.ReadWrite;
				case NonstrictReadWriteXmlValue:
					return EntityCacheUsage.NonStrictReadWrite;
				case TransactionalXmlValue:
					return EntityCacheUsage.Transactional;
				default:
					throw new HibernateConfigException(string.Format("Invalid EntityCacheUsage value:{0}", value));
			}
		}
	}
}