
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace NHibernate
{

	/// <summary>
	/// QueryOver&lt;T&gt; is an API for retrieving entities by composing
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
	public interface IQueryOver<T>
	{

		/// <summary>
		/// Add criterion expressed as a lambda expression
		/// </summary>
		/// <param name="expression">Lambda expression</param>
		/// <returns>criteria instance</returns>
		IQueryOver<T> And(Expression<Func<T, bool>> expression);

		/// <summary>
		/// Add criterion expressed as a lambda expression
		/// </summary>
		/// <param name="expression">Lambda expression</param>
		/// <returns>criteria instance</returns>
		IQueryOver<T> And(Expression<Func<bool>> expression);

		/// <summary>
		/// Identical semantics to Add() to allow more readable queries
		/// </summary>
		/// <param name="expression">Lambda expression</param>
		/// <returns>criteria instance</returns>
		IQueryOver<T> Where(Expression<Func<T, bool>> expression);

		/// <summary>
		/// Identical semantics to Add() to allow more readable queries
		/// </summary>
		/// <param name="expression">Lambda expression</param>
		/// <returns>criteria instance</returns>
		IQueryOver<T> Where(Expression<Func<bool>> expression);

		/// <summary>
		/// Creates a new NHibernate.ICriteria&lt;T&gt;, "rooted" at the associated entity
		/// </summary>
		/// <typeparam name="U">Type of sub-criteria</typeparam>
		/// <param name="path">Lambda expression returning association path</param>
		/// <returns>The created "sub criteria"</returns>
		IQueryOver<U> JoinQueryOver<U>(Expression<Func<T, U>> path);

		/// <summary>
		/// Creates a new NHibernate.ICriteria&lt;T&gt;, "rooted" at the associated entity
		/// specifying a collection for the join.
		/// </summary>
		/// <typeparam name="U">Type of sub-criteria (type of the collection)</typeparam>
		/// <param name="path">Lambda expression returning association path</param>
		/// <returns>The created "sub criteria"</returns>
		IQueryOver<U> JoinQueryOver<U>(Expression<Func<T, IEnumerable<U>>> path);

		/// <summary>
		/// Join an association, assigning an alias to the joined entity
		/// </summary>
		/// <param name="path">Lambda expression returning association path</param>
		/// <param name="alias">Lambda expression returning alias reference</param>
		/// <returns>criteria instance</returns>
		IQueryOver<T> Join(Expression<Func<T, object>> path, Expression<Func<object>> alias);

		/// <summary>
		/// Get the results of the root type and fill the <see cref="IList&lt;T&gt;"/>
		/// </summary>
		/// <returns>The list filled with the results.</returns>
		IList<T> List();

	}

}
