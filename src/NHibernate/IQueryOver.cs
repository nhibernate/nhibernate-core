
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
		/// Add arbitrary ICriterion (e.g., to allow protected member access)
		/// </summary>
		IQueryOver<T> And(ICriterion expression);

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
		/// Add arbitrary ICriterion (e.g., to allow protected member access)
		/// </summary>
		IQueryOver<T> Where(ICriterion expression);

		/// <summary>
		/// Add projection expressed as a lambda expression
		/// </summary>
		/// <param name="projections">Lambda expressions</param>
		/// <returns>criteria instance</returns>
		IQueryOver<T> Select(params Expression<Func<T, object>>[] projections);

		/// <summary>
		/// Add arbitrary IProjections to query
		/// </summary>
		IQueryOver<T> Select(params IProjection[] projections);

		/// <summary>
		/// Add order expressed as a lambda expression
		/// </summary>
		/// <param name="path">Lambda expression</param>
		/// <returns>criteria instance</returns>
		IQueryOverOrderBuilder<T> OrderBy(Expression<Func<T, object>> path);

		/// <summary>
		/// Add order expressed as a lambda expression
		/// </summary>
		/// <param name="path">Lambda expression</param>
		/// <returns>criteria instance</returns>
		IQueryOverOrderBuilder<T> OrderBy(Expression<Func<object>> path);

		/// <summary>
		/// Add order expressed as a lambda expression
		/// </summary>
		/// <param name="path">Lambda expression</param>
		/// <returns>criteria instance</returns>
		IQueryOverOrderBuilder<T> ThenBy(Expression<Func<T, object>> path);

		/// <summary>
		/// Add order expressed as a lambda expression
		/// </summary>
		/// <param name="path">Lambda expression</param>
		/// <returns>criteria instance</returns>
		IQueryOverOrderBuilder<T> ThenBy(Expression<Func<object>> path);

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

		/// <summary> Override the cache mode for this particular query. </summary>
		/// <param name="cacheMode">The cache mode to use. </param>
		/// <returns> this (for method chaining) </returns>
		IQueryOver<T> CacheMode(CacheMode cacheMode);

		/// <summary>
		/// Set the name of the cache region.
		/// </summary>
		/// <param name="cacheRegion">the name of a query cache region, or <see langword="null" />
		/// for the default query cache</param>
		IQueryOver<T> CacheRegion(string cacheRegion);

		/// <summary>
		/// Specify an association fetching strategy.  Currently, only
		/// one-to-many and one-to-one associations are supported.
		/// </summary>
		/// <param name="path">A lambda expression path (e.g., ChildList[0].Granchildren[0].Pets).</param>
		/// <returns></returns>
		IQueryOverFetchBuilder<T> Fetch(Expression<Func<T, object>> path);

		/// <summary>
		/// Set the lock mode of the current entity
		/// </summary>
		IQueryOverLockBuilder<T> Lock();

		/// <summary>
		/// Set the lock mode of the aliased entity
		/// </summary>
		IQueryOverLockBuilder<T> Lock(Expression<Func<object>> alias);

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

		/// <summary>
		/// Convenience method to return a single instance that matches
		/// the query, or null if the query returns no results.
		/// </summary>
		/// <returns>the single result or <see langword="null" /></returns>
		/// <exception cref="HibernateException">
		/// If there is more than one matching result
		/// </exception>
		T UniqueResult();

		/// <summary>
		/// Override type of <see cref="UniqueResult()" />.
		/// </summary>
		U UniqueResult<U>();

		/// <summary>
		/// Get a enumerable that when enumerated will execute
		/// a batch of queries in a single database roundtrip
		/// </summary>
		IEnumerable<T> Future();

		/// <summary>
		/// Get a enumerable that when enumerated will execute
		/// a batch of queries in a single database roundtrip
		/// </summary>
		IEnumerable<U> Future<U>();

		/// <summary>
		/// Get an IFutureValue instance, whose value can be retrieved through
		/// its Value property. The query is not executed until the Value property
		/// is retrieved, which will execute other Future queries as well in a
		/// single roundtrip
		/// </summary>
		IFutureValue<T> FutureValue();

		/// <summary>
		/// Get an IFutureValue instance, whose value can be retrieved through
		/// its Value property. The query is not executed until the Value property
		/// is retrieved, which will execute other Future queries as well in a
		/// single roundtrip
		/// </summary>
		IFutureValue<U> FutureValue<U>();

	}

}
