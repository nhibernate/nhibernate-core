namespace NHibernate.Linq
{
	/// <summary>
	/// Expose NH queryable options.
	/// </summary>
	public class NhQueryableOptions 
#pragma warning disable 618
		: IQueryableOptions
#pragma warning restore 618
	{
		protected bool? Cacheable { get; private set; }
		protected CacheMode? CacheMode { get; private set; }
		protected string CacheRegion { get; private set; }
		protected int? Timeout { get; private set; }
		protected bool? ReadOnly { get; private set; }
		protected string Comment { get; private set; }

#pragma warning disable 618
		/// <inheritdoc />
		IQueryableOptions IQueryableOptions.SetCacheable(bool cacheable) => SetCacheable(cacheable);

		/// <inheritdoc />
		IQueryableOptions IQueryableOptions.SetCacheMode(CacheMode cacheMode) => SetCacheMode(cacheMode);

		/// <inheritdoc />
		IQueryableOptions IQueryableOptions.SetCacheRegion(string cacheRegion) => SetCacheRegion(cacheRegion);

		/// <inheritdoc />
		IQueryableOptions IQueryableOptions.SetTimeout(int timeout) => SetTimeout(timeout);
#pragma warning restore 618

		/// <summary>
		/// Enable caching of this query result set.
		/// </summary>
		/// <param name="cacheable">Should the query results be cacheable?</param>
		/// <returns><see langword="this"/> (for method chaining).</returns>
		public NhQueryableOptions SetCacheable(bool cacheable)
		{
			Cacheable = cacheable;
			return this;
		}

		/// <summary>
		/// Override the current session cache mode, just for this query.
		/// </summary>
		/// <param name="cacheMode">The cache mode to use.</param>
		/// <returns><see langword="this"/> (for method chaining).</returns>
		public NhQueryableOptions SetCacheMode(CacheMode cacheMode)
		{
			CacheMode = cacheMode;
			return this;
		}

		/// <summary>
		/// Set the name of the cache region.
		/// </summary>
		/// <param name="cacheRegion">The name of a query cache region, or <see langword="null" />
		/// for the default query cache</param>
		/// <returns><see langword="this"/> (for method chaining).</returns>
		public NhQueryableOptions SetCacheRegion(string cacheRegion)
		{
			CacheRegion = cacheRegion;
			return this;
		}

		/// <summary>
		/// Set the timeout for the underlying ADO query.
		/// </summary>
		/// <param name="timeout">The timeout in seconds.</param>
		/// <returns><see langword="this"/> (for method chaining).</returns>
		public NhQueryableOptions SetTimeout(int timeout)
		{
			Timeout = timeout;
			return this;
		}

		/// <summary>
		/// Set the read-only mode for entities (and proxies) loaded by this query. This setting 
		/// overrides the default setting for the session (see <see cref="ISession.DefaultReadOnly" />).
		/// </summary>
		/// <remarks>
		/// <para>
		/// Read-only entities can be modified, but changes are not persisted. They are not 
		/// dirty-checked and snapshots of persistent state are not maintained.
		/// </para>
		/// <para>
		/// When a proxy is initialized, the loaded entity will have the same read-only setting 
		/// as the uninitialized proxy, regardless of the session's current setting.
		/// </para>
		/// <para>
		/// The read-only setting has no impact on entities or proxies returned by the criteria
		/// that existed in the session before the criteria was executed.
		/// </para>
		/// </remarks>
		/// <param name="readOnly">
		/// If <c>true</c>, entities (and proxies) loaded by the query will be read-only.
		/// </param>
		/// <returns><c>this</c> (for method chaining)</returns>
		public NhQueryableOptions SetReadOnly(bool readOnly)
		{
			ReadOnly = readOnly;
			return this;
		}
		
		/// <summary> 
		/// Set a comment that will be prepended before the generated SQL.
		/// </summary>
		/// <param name="comment">The comment to prepend.</param>
		/// <returns><see langword="this"/> (for method chaining).</returns>
		public NhQueryableOptions SetComment(string comment)
		{
			Comment = comment;
			return this;
		}

		protected internal NhQueryableOptions Clone()
		{
			return new NhQueryableOptions
			{
				Cacheable = Cacheable,
				CacheMode = CacheMode,
				CacheRegion = CacheRegion,
				Timeout = Timeout,
				ReadOnly = ReadOnly,
				Comment = Comment
			};
		}

		protected internal void Apply(IQuery query)
		{
			if (Timeout.HasValue)
				query.SetTimeout(Timeout.Value);

			if (Cacheable.HasValue)
				query.SetCacheable(Cacheable.Value);

			if (CacheMode.HasValue)
				query.SetCacheMode(CacheMode.Value);

			if (CacheRegion != null)
				query.SetCacheRegion(CacheRegion);

			if (ReadOnly.HasValue)
				query.SetReadOnly(ReadOnly.Value);
			
			if (!string.IsNullOrEmpty(Comment))
				query.SetComment(Comment);
		}
	}
}
