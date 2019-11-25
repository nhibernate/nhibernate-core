namespace NHibernate
{
	// 6.0 TODO: merge into IQueryOver<TRoot>
	public static class QueryOverExtensions
	{
		/// <summary>
		/// Set a timeout for the underlying ADO.NET query.
		/// </summary>
		/// <param name="queryOver">The query on which to set the timeout.</param>
		/// <param name="timeout">The timeout in seconds.</param>
		/// <returns><see langword="this" /> (for method chaining).</returns>
		public static TQueryOver SetTimeout<TQueryOver>(this TQueryOver queryOver, int timeout) where TQueryOver: IQueryOver
		{
			queryOver.RootCriteria.SetTimeout(timeout);
			return queryOver;
		}
		
		/// <summary>
		/// Set a fetch size for the underlying ADO query.
		/// </summary>
		/// <param name="queryOver">The query on which to set the timeout.</param>
		/// <param name="fetchSize">The fetch size.</param>
		/// <returns><see langword="this" /> (for method chaining).</returns>
		public static TQueryOver SetFetchSize<TQueryOver>(this TQueryOver queryOver, int fetchSize) where TQueryOver: IQueryOver
		{
			queryOver.RootCriteria.SetFetchSize(fetchSize);
			return queryOver;
		}

		/// <summary>
		/// Add a comment to the generated SQL.
		/// </summary>
		/// <param name="queryOver">The query on which to set the timeout.</param>
		/// <param name="comment">A human-readable string.</param>
		/// <returns><see langword="this" /> (for method chaining).</returns>
		public static TQueryOver SetComment<TQueryOver>(this TQueryOver queryOver, string comment) where TQueryOver: IQueryOver
		{
			queryOver.RootCriteria.SetComment(comment);
			return queryOver;
		}
	}
}
