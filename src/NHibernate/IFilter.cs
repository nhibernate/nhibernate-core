using System.Collections;
using NHibernate.Engine;

namespace NHibernate
{
	/// <summary>
	/// Type definition of Filter.  Filter defines the user's view into enabled dynamic filters,
	/// allowing them to set filter parameter values.
	/// </summary>
	public interface IFilter
	{
		/// <summary>
		/// Get the name of this filter. 
		/// </summary>
		/// <returns>This filter's name.</returns>
		string Name { get; }

		/// <summary>
		/// Get the filter definition containing additional information about the
		/// filter (such as default-condition and expected parameter names/types).
		/// </summary>
		/// <returns>The filter definition</returns>
		FilterDefinition FilterDefinition { get; }

		/// <summary>
		/// Set the named parameter's value list for this filter. 
		/// </summary>
		/// <param name="name">The parameter's name.</param>
		/// <param name="value">The values to be applied.</param>
		/// <returns>This FilterImpl instance (for method chaining).</returns>
		IFilter SetParameter(string name, object value);

		/// <summary>
		/// Set the named parameter's value list for this filter.  Used
		/// in conjunction with IN-style filter criteria.        
		/// </summary>
		/// <param name="name">The parameter's name.</param>
		/// <param name="values">The values to be expanded into an SQL IN list.</param>
		/// <returns>This FilterImpl instance (for method chaining).</returns>
		IFilter SetParameterList(string name, ICollection values);

		/// <summary>
		/// Set the named parameter's value list for this filter.  Used
		/// in conjunction with IN-style filter criteria.        
		/// </summary>
		/// <param name="name">The parameter's name.</param>
		/// <param name="values">The values to be expanded into an SQL IN list.</param>
		/// <returns>This FilterImpl instance (for method chaining).</returns>
		IFilter SetParameterList(string name, object[] values);

		/// <summary>
		/// Perform validation of the filter state.  This is used to verify the
		/// state of the filter after its enablement and before its use.
		/// </summary>
		/// <returns></returns>
		void Validate();
	}
}
