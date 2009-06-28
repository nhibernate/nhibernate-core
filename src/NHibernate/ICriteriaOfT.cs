
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace NHibernate
{

	/// <summary>
	/// Criteria&lt;T&gt; is an API for retrieving entities by composing
	/// <see cref="Criterion.Expression" /> objects expressed using Lambda expression syntax.
	/// </summary>
	/// <remarks>
	/// <code>
	/// IList&lt;Cat&gt cats = session.QueryOver&lt;Cat&gt;()
	/// 	.Add( c =&gt; c.Name == "Tigger" )
	///		.Add( c =&gt; c.Weight > minWeight ) )
	///		.List();
	/// </code>
	/// </remarks>
	public interface ICriteria<T>
	{

		/// <summary>
		/// Add criterion expressed as a lambda expression
		/// </summary>
		/// <param name="expression">Lambda expression</param>
		/// <returns>criteria instance</returns>
		ICriteria<T> And(Expression<Func<T, bool>> expression);

		/// <summary>
		/// Add criterion expressed as a lambda expression
		/// </summary>
		/// <param name="expression">Lambda expression</param>
		/// <returns>criteria instance</returns>
		ICriteria<T> And(Expression<Func<bool>> expression);

		/// <summary>
		/// Identical semantics to Add() to allow more readable queries
		/// </summary>
		/// <param name="expression">Lambda expression</param>
		/// <returns>criteria instance</returns>
		ICriteria<T> Where(Expression<Func<T, bool>> expression);

		/// <summary>
		/// Identical semantics to Add() to allow more readable queries
		/// </summary>
		/// <param name="expression">Lambda expression</param>
		/// <returns>criteria instance</returns>
		ICriteria<T> Where(Expression<Func<bool>> expression);

		/// <summary>
		/// Creates a new NHibernate.ICriteria&lt;T&gt;, "rooted" at the associated entity
		/// </summary>
		/// <typeparam name="U">Type of sub-criteria</typeparam>
		/// <param name="expression">Lambda expression returning association path</param>
		/// <returns>The created "sub criteria"</returns>
		ICriteria<U> Join<U>(Expression<Func<T, U>> expression);

		/// <summary>
		/// Creates a new NHibernate.ICriteria&lt;T&gt;, "rooted" at the associated entity
		/// specifying a collection for the join.
		/// </summary>
		/// <typeparam name="U">Type of sub-criteria (type of the collection)</typeparam>
		/// <param name="expression">Lambda expression returning association path</param>
		/// <returns>The created "sub criteria"</returns>
		ICriteria<U> Join<U>(Expression<Func<T, IEnumerable<U>>> expression);

		/// <summary>
		/// Get the results of the root type and fill the <see cref="IList&lt;T&gt;"/>
		/// </summary>
		/// <param name="results">The list filled with the results.</param>
		IList<T> List();

	}

}
