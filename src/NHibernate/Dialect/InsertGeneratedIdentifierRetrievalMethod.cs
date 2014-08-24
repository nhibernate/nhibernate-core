namespace NHibernate.Dialect
{
	public enum InsertGeneratedIdentifierRetrievalMethod
	{
		/// <summary>
		/// Use a parameter with ParameterDirection.Output
		/// </summary>
		OutputParameter,

		/// <summary>
		/// Use a parameter with ParameterDirection.ReturnValue
		/// </summary>
		ReturnValueParameter,

		// <summary>
		// Get the result from the statment as if it were a query, using ExecuteScalar() or ExecuteDataReader().
		// </summary>
		// QueryResult
	}
}