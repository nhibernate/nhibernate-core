namespace NHibernate
{
	/// <summary>
	/// Represents fetching options for Criteria
	/// </summary>
	public enum SelectMode
	{
		/// <summary>
		/// Default to the setting configured in the mapping file.
		/// </summary>
		Undefined,

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
		JoinOnly,

		/// <summary>
		/// Skips fetching for fetch="join" association (no-op for lazy association).
		/// </summary>
		Skip,

		/// <summary>
		/// Fetch lazy property group.
		/// Provide path to lazy property and it will be fetched along with properties that belong to the same fetch group (lazy-group)
		/// Note: To fetch single property it must be mapped with unique fetch group (lazy-group)
		/// </summary>
		FetchLazyPropertyGroup,
	}
}
