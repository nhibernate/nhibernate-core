
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using NHibernate.Criterion;
using NHibernate.SqlCommand;

namespace NHibernate
{

	/// <summary>
	/// QueryOver&lt;T&gt; is an API for retrieving entities by composing
	/// <see cref="Criterion.Expression" /> objects expressed using Lambda expression syntax.
	/// </summary>
	/// <remarks>
	/// <code>
	/// IList&lt;Cat&gt; cats = session.QueryOver&lt;Cat&gt;()
	/// 	.Where( c =&gt; c.Name == "Tigger" )
	///		.And( c =&gt; c.Weight > minWeight ) )
	///		.List();
	/// </code>
	/// </remarks>
	public interface IQueryOver<T>
	{

		/// <summary>
		/// Access the underlying ICriteria
		/// </summary>
		ICriteria UnderlyingCriteria { get; }

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
		/// Add projection expressed as a lambda expression
		/// </summary>
		/// <param name="projections">Lambda expressions</param>
		/// <returns>criteria instance</returns>
		IQueryOver<T> Select(params Expression<Func<T, object>>[] projections);

		/// <summary>
		/// Add order expressed as a lambda expression
		/// </summary>
		/// <param name="path">Lambda expression</param>
		/// <param name="orderDelegate">Order delegate (direction)</param>
		/// <returns>criteria instance</returns>
		IQueryOver<T> OrderBy(Expression<Func<T, object>> path, Func<string, Order> orderDelegate);

		/// <summary>
		/// Add order expressed as a lambda expression
		/// </summary>
		/// <param name="path">Lambda expression</param>
		/// <param name="orderDelegate">Order delegate (direction)</param>
		/// <returns>criteria instance</returns>
		IQueryOver<T> OrderBy(Expression<Func<object>> path, Func<string, Order> orderDelegate);

		/// <summary>
		/// Add order expressed as a lambda expression
		/// </summary>
		/// <param name="path">Lambda expression</param>
		/// <param name="orderDelegate">Order delegate (direction)</param>
		/// <returns>criteria instance</returns>
		IQueryOver<T> ThenBy(Expression<Func<T, object>> path, Func<string, Order> orderDelegate);

		/// <summary>
		/// Add order expressed as a lambda expression
		/// </summary>
		/// <param name="path">Lambda expression</param>
		/// <param name="orderDelegate">Order delegate (direction)</param>
		/// <returns>criteria instance</returns>
		IQueryOver<T> ThenBy(Expression<Func<object>> path, Func<string, Order> orderDelegate);

		/// <summary>
		/// Set the first result to be retrieved
		/// </summary>
		/// <param name="firstResult"></param>
		IQueryOver<T> Skip(int firstResult);

		/// <summary>
		/// Set a limit upon the number of objects to be retrieved
		/// </summary>
		/// <param name="maxResults"></param>
		IQueryOver<T> Take(int maxResults);

		/// <summary>
		/// Enable caching of this query result set
		/// </summary>
		IQueryOver<T> Cacheable();

		/// <summary>
		/// Creates a new NHibernate.ICriteria&lt;T&gt;, "rooted" at the associated entity
		/// </summary>
		/// <typeparam name="U">Type of sub-criteria</typeparam>
		/// <param name="path">Lambda expression returning association path</param>
		/// <returns>The created "sub criteria"</returns>
		IQueryOver<U> JoinQueryOver<U>(Expression<Func<T, U>> path);

		/// <summary>
		/// Creates a new NHibernate.ICriteria&lt;T&gt;, "rooted" at the associated entity
		/// </summary>
		/// <typeparam name="U">Type of sub-criteria</typeparam>
		/// <param name="path">Lambda expression returning association path</param>
		/// <param name="alias">Lambda expression returning alias reference</param>
		/// <returns>The created "sub criteria"</returns>
		IQueryOver<U> JoinQueryOver<U>(Expression<Func<T, U>> path, Expression<Func<U>> alias);

		/// <summary>
		/// Creates a new NHibernate.ICriteria&lt;T&gt;, "rooted" at the associated entity
		/// </summary>
		/// <typeparam name="U">Type of sub-criteria</typeparam>
		/// <param name="path">Lambda expression returning association path</param>
		/// <param name="joinType">Type of join</param>
		/// <returns>The created "sub criteria"</returns>
		IQueryOver<U> JoinQueryOver<U>(Expression<Func<T, U>> path, JoinType joinType);

		/// <summary>
		/// Creates a new NHibernate.ICriteria&lt;T&gt;, "rooted" at the associated entity
		/// </summary>
		/// <typeparam name="U">Type of sub-criteria</typeparam>
		/// <param name="path">Lambda expression returning association path</param>
		/// <param name="alias">Lambda expression returning alias reference</param>
		/// <param name="joinType">Type of join</param>
		/// <returns>The created "sub criteria"</returns>
		IQueryOver<U> JoinQueryOver<U>(Expression<Func<T, U>> path, Expression<Func<U>> alias, JoinType joinType);

		/// <summary>
		/// Creates a new NHibernate.ICriteria&lt;T&gt;, "rooted" at the associated entity
		/// specifying a collection for the join.
		/// </summary>
		/// <typeparam name="U">Type of sub-criteria (type of the collection)</typeparam>
		/// <param name="path">Lambda expression returning association path</param>
		/// <returns>The created "sub criteria"</returns>
		IQueryOver<U> JoinQueryOver<U>(Expression<Func<T, IEnumerable<U>>> path);

		/// <summary>
		/// Creates a new NHibernate.ICriteria&lt;T&gt;, "rooted" at the associated entity
		/// specifying a collection for the join.
		/// </summary>
		/// <typeparam name="U">Type of sub-criteria (type of the collection)</typeparam>
		/// <param name="path">Lambda expression returning association path</param>
		/// <param name="alias">Lambda expression returning alias reference</param>
		/// <returns>The created "sub criteria"</returns>
		IQueryOver<U> JoinQueryOver<U>(Expression<Func<T, IEnumerable<U>>> path, Expression<Func<U>> alias);

		/// <summary>
		/// Creates a new NHibernate.ICriteria&lt;T&gt;, "rooted" at the associated entity
		/// specifying a collection for the join.
		/// </summary>
		/// <typeparam name="U">Type of sub-criteria (type of the collection)</typeparam>
		/// <param name="path">Lambda expression returning association path</param>
		/// <param name="joinType">Type of join</param>
		/// <returns>The created "sub criteria"</returns>
		IQueryOver<U> JoinQueryOver<U>(Expression<Func<T, IEnumerable<U>>> path, JoinType joinType);

		/// <summary>
		/// Creates a new NHibernate.ICriteria&lt;T&gt;, "rooted" at the associated entity
		/// specifying a collection for the join.
		/// </summary>
		/// <typeparam name="U">Type of sub-criteria (type of the collection)</typeparam>
		/// <param name="path">Lambda expression returning association path</param>
		/// <param name="alias">Lambda expression returning alias reference</param>
		/// <param name="joinType">Type of join</param>
		/// <returns>The created "sub criteria"</returns>
		IQueryOver<U> JoinQueryOver<U>(Expression<Func<T, IEnumerable<U>>> path, Expression<Func<U>> alias, JoinType joinType);

		/// <summary>
		/// Join an association, assigning an alias to the joined entity
		/// </summary>
		/// <param name="path">Lambda expression returning association path</param>
		/// <param name="alias">Lambda expression returning alias reference</param>
		/// <returns>criteria instance</returns>
		IQueryOver<T> Join(Expression<Func<T, object>> path, Expression<Func<object>> alias);

		/// <summary>
		/// Join an association, assigning an alias to the joined entity
		/// </summary>
		/// <param name="path">Lambda expression returning association path</param>
		/// <param name="alias">Lambda expression returning alias reference</param>
		/// <param name="joinType">Type of join</param>
		/// <returns>criteria instance</returns>
		IQueryOver<T> Join(Expression<Func<T, object>> path, Expression<Func<object>> alias, JoinType joinType);

		IQueryOverJoinBuilder<T> Inner	{ get; }
		IQueryOverJoinBuilder<T> Left	{ get; }
		IQueryOverJoinBuilder<T> Right	{ get; }
		IQueryOverJoinBuilder<T> Full	{ get; }

		/// <summary>
		/// Get the results of the root type and fill the <see cref="IList&lt;T&gt;"/>
		/// </summary>
		/// <returns>The list filled with the results.</returns>
		IList<T> List();

		/// <summary>
		/// Get the results of the root type and fill the <see cref="IList&lt;T&gt;"/>
		/// </summary>
		/// <returns>The list filled with the results.</returns>
		IList<U> List<U>();

	}

}
