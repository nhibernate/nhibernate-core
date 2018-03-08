namespace NHibernate.Loader
{
	public enum SelectMode
	{
		/// <summary>
		/// Default behavior (as if no select mode applied).
		/// </summary>
		Default,

		/// <summary>
		/// Fetch entity.
		/// </summary>
		Fetch,
		
		/// <summary>
		/// Fetch lazy properties.
		/// </summary>
		FetchLazyProperties,

		/// <summary>
		/// Only Identifier columns are added to select statement.
		/// Use it for fetching child objects for already loaded entities.
		/// Note: Missing entities in session will be returned as null
		/// </summary>
		ChildFetch,

		/// <summary>
		/// Skips entity from select statement but keeps join in query
		/// </summary>
		Skip,
	}
}
