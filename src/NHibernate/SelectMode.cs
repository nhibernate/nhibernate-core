namespace NHibernate
{
	public enum SelectMode
	{
		/// <summary>
		/// Default behavior (as if no select mode is specified).
		/// </summary>
		Default,

		/// <summary>
		/// Fetch the entity.
		/// </summary>
		Fetch,

		/// <summary>
		/// Fetch the entity and its lazy properties.
		/// </summary>
		FetchLazyProperties,

		/// <summary>
		/// Only identifier columns are added to select statement. Use it for fetching child objects for already loaded
		/// entities.
		/// Entities missing in session will be loaded (lazily if possible, otherwise with additional immediate loads).
		/// </summary>
		ChildFetch,

		/// <summary>
		/// Skips the entity from select statement but keeps joining it in the query.
		/// </summary>
		Skip,

		/// <summary>
		/// Equivalent of <see cref="FetchMode.Lazy"/> and <see cref="FetchMode.Select"/>.
		/// Skips fetching for eagerly mapped association (no-op for lazy association).
		/// </summary>
		SkipJoin,
	}
}
