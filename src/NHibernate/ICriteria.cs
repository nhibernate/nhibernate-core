using System;
using System.Collections;

namespace NHibernate {

	///<summary>
	///Criteria is a simplified API for retrieving objects
	///by composing Expression objects. This is a very
	///convenient approach for functionality like "search" screens
	///where there is a variable number of conditions to be placed
	///upon the result set.<br>
	///
	///In the current implementation, conditions may only be placed 
	///upon properties of the class being retrieved (and its 
	///components). Hibernate's query language is much more general
	///and should be used for non-simple cases.
	///
	///This is an experimental API
	///</summary>

	public interface ICriteria {
		
		/// <summary>
		/// Set a limit upon the number of objects to be retrieved
		/// </summary>
		ICriteria SetMaxResults(int maxResults);

		/// <summary>
		/// Set the first result to be retrieved
		/// </summary>
		ICriteria SetFirstResult(int firstResult);

		/// <summary>
		/// Set a timeout for the underlying ADO.NET query
		/// </summary>
		/// <param name="timeout"></param>
		/// <returns></returns>
		ICriteria SetTimeout(int timeout);

		/// <summary>
		/// Add an Expression to constrain the results to be retrieved.
		/// </summary>
		/// <param name="expression"></param>
		/// <returns></returns>
		ICriteria Add(Expression.Expression expression);

		/// <summary>
		/// Get the results
		/// </summary>
		/// <returns></returns>
		IList List();
	}
}