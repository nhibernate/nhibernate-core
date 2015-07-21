using NHibernate.Engine;

namespace NHibernate.Param
{
	/// <summary>
	/// Additional information for potential paging parameters in HQL/LINQ
	/// </summary>
	public interface IPageableParameterSpecification : IExplicitParameterSpecification
	{
		/// <summary>
		/// Notifies the parameter that it is a 'skip' parameter, and should calculate its value using the dialect settings
		/// </summary>
		void IsSkipParameter();

		/// <summary>
		/// Notifies the parameter that it is a 'take' parameter, and should calculate its value using the dialect settings
		/// and the value of the supplied skipParameter.
		/// </summary>
		/// <param name="skipParameter">The associated skip parameter (null if there is none).</param>
		void IsTakeParameterWithSkipParameter(IPageableParameterSpecification skipParameter);

		/// <summary>
		/// Retrieve the skip/offset value for the query
		/// </summary>
		/// <param name="queryParameters">The parameters for the query</param>
		/// <returns>The paging skip/offset value</returns>
		int GetSkipValue(QueryParameters queryParameters);
	}
}