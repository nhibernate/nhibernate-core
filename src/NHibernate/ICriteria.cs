using System;
using System.Collections;

using NHibernate.Expression;

namespace NHibernate 
{
	///<summary>
	///Criteria is a simplified API for retrieving entities
	///by composing Expression objects. This is a very
	///convenient approach for functionality like "search" screens
	///where there is a variable number of conditions to be placed
	///upon the result set.
	///
	///The Session is a factory for ICriteria. 
	///Expression instances are usually obtained via 
	///the factory methods on Expression. eg: 
	///<code>
	/// IList cats = session.CreateCriteria(typeof(Cat)) 
	///     .Add( Expression.Like("name", "Iz%") ) 
	///     .Add( Expression.Gt( "weight", minWeight ) ) 
	///     .AddOrder( Order.Asc("age") ) 
	///     .List(); 
	///</code>
	///In the current implementation, conditions may only be placed 
	///upon properties of the class being retrieved (and its 
	///components). Hibernate's query language is much more general
	///and should be used for non-simple cases.
	///
	///This is an experimental API
	///</summary>

	public interface ICriteria 
	{
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
		/// 
		/// </summary>
		int MaxResults {get; }
		
		/// <summary>
		/// 
		/// </summary>
		int FirstResult {get; }
		
		/// <summary>
		/// 
		/// </summary>
		int Timeout {get; }

		/// <summary>
		/// Add an Expression to constrain the results to be retrieved.
		/// </summary>
		/// <param name="expression"></param>
		/// <returns></returns>
		ICriteria Add(Expression.Expression expression);

		/// <summary>
		/// An an Order to the result set 
		/// </summary>
		ICriteria AddOrder(Order order); 


		/// <summary>
		/// Get the results
		/// </summary>
		/// <returns></returns>
		IList List();

		/// <summary>
		/// Contains all of the Expressions that were added as one 
		/// resulting expression.
		/// </summary>
		Expression.Expression Expression {get;}

		/// <summary>
		/// Provides an Enumerator to Iterate through the Expressions 
		/// that have been added
		/// </summary>
		/// <returns></returns>
		IEnumerator IterateExpressions();
	
		/// <summary>
		/// Provides an Enumerator to Iterate through the Order clauses
		/// that have been added.
		/// </summary>
		/// <returns></returns>
		IEnumerator IterateOrderings();
    
		/// <summary>
		/// The PersistentClass that is the entry point for the Criteria.
		/// </summary>
		System.Type PersistentClass { get; }

		/// <summary>
		/// Gets the association's fetching strategy.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		FetchMode GetFetchMode(string path);

		/// <summary>
		/// Specify an association fetching strategy.  Currently, only
		/// one-to-many and one-to-one associations are supported.
		/// </summary>
		/// <param name="associationPath">A dot seperated property path.</param>
		/// <param name="mode">The Fetch mode.</param>
		/// <returns></returns>
		ICriteria SetFetchMode(string associationPath, FetchMode mode);
	}
}