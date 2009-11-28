
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using NHibernate.Criterion;
using NHibernate.Criterion.Lambda;
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

	/// <summary>
	/// QueryOver&lt;R,T&gt; is an API for retrieving entities by composing
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
	public interface IQueryOver<R,T> : IQueryOver<R>
	{

		/// <summary>
		/// Add criterion expressed as a lambda expression
		/// </summary>
		/// <param name="expression">Lambda expression</param>
		/// <returns>criteria instance</returns>
		IQueryOver<R,T> And(Expression<Func<T, bool>> expression);

		/// <summary>
		/// Add criterion expressed as a lambda expression
		/// </summary>
		/// <param name="expression">Lambda expression</param>
		/// <returns>criteria instance</returns>
		IQueryOver<R,T> And(Expression<Func<bool>> expression);

		/// <summary>
		/// Add arbitrary ICriterion (e.g., to allow protected member access)
		/// </summary>
		IQueryOver<R,T> And(ICriterion expression);

		/// <summary>
		/// Add negation of criterion expressed as a lambda expression
		/// </summary>
		/// <param name="expression">Lambda expression</param>
		/// <returns>criteria instance</returns>
		IQueryOver<R,T> AndNot(Expression<Func<T, bool>> expression);

		/// <summary>
		/// Add negation of criterion expressed as a lambda expression
		/// </summary>
		/// <param name="expression">Lambda expression</param>
		/// <returns>criteria instance</returns>
		IQueryOver<R,T> AndNot(Expression<Func<bool>> expression);

		/// <summary>
		/// Add restriction to a property
		/// </summary>
		/// <param name="expression">Lambda expression containing path to property</param>
		/// <returns>criteria instance</returns>
		IQueryOverRestrictionBuilder<R,T> AndRestrictionOn(Expression<Func<T, object>> expression);

		/// <summary>
		/// Add restriction to a property
		/// </summary>
		/// <param name="expression">Lambda expression containing path to property</param>
		/// <returns>criteria instance</returns>
		IQueryOverRestrictionBuilder<R,T> AndRestrictionOn(Expression<Func<object>> expression);

		/// <summary>
		/// Identical semantics to And() to allow more readable queries
		/// </summary>
		/// <param name="expression">Lambda expression</param>
		/// <returns>criteria instance</returns>
		IQueryOver<R,T> Where(Expression<Func<T, bool>> expression);

		/// <summary>
		/// Identical semantics to And() to allow more readable queries
		/// </summary>
		/// <param name="expression">Lambda expression</param>
		/// <returns>criteria instance</returns>
		IQueryOver<R,T> Where(Expression<Func<bool>> expression);

		/// <summary>
		/// Add arbitrary ICriterion (e.g., to allow protected member access)
		/// </summary>
		IQueryOver<R,T> Where(ICriterion expression);

		/// <summary>
		/// Identical semantics to AndNot() to allow more readable queries
		/// </summary>
		/// <param name="expression">Lambda expression</param>
		/// <returns>criteria instance</returns>
		IQueryOver<R,T> WhereNot(Expression<Func<T, bool>> expression);

		/// <summary>
		/// Identical semantics to AndNot() to allow more readable queries
		/// </summary>
		/// <param name="expression">Lambda expression</param>
		/// <returns>criteria instance</returns>
		IQueryOver<R,T> WhereNot(Expression<Func<bool>> expression);

		/// <summary>
		/// Identical semantics to AndRestrictionOn() to allow more readable queries
		/// </summary>
		/// <param name="expression">Lambda expression</param>
		/// <returns>criteria instance</returns>
		IQueryOverRestrictionBuilder<R,T> WhereRestrictionOn(Expression<Func<T, object>> expression);

		/// <summary>
		/// Identical semantics to AndRestrictionOn() to allow more readable queries
		/// </summary>
		/// <param name="expression">Lambda expression</param>
		/// <returns>criteria instance</returns>
		IQueryOverRestrictionBuilder<R,T> WhereRestrictionOn(Expression<Func<object>> expression);

		/// <summary>
		/// Add projection expressed as a lambda expression
		/// </summary>
		/// <param name="projections">Lambda expressions</param>
		/// <returns>criteria instance</returns>
		IQueryOver<R,T> Select(params Expression<Func<R, object>>[] projections);

		/// <summary>
		/// Add arbitrary IProjections to query
		/// </summary>
		IQueryOver<R,T> Select(params IProjection[] projections);

		/// <summary>
		/// Create a list of projections inline
		/// </summary>
		QueryOverProjectionBuilder<IQueryOver<R,T>, R, T> SelectList { get; }

		/// <summary>
		/// Add order expressed as a lambda expression
		/// </summary>
		/// <param name="path">Lambda expression</param>
		/// <returns>criteria instance</returns>
		IQueryOverOrderBuilder<R,T> OrderBy(Expression<Func<T, object>> path);

		/// <summary>
		/// Add order expressed as a lambda expression
		/// </summary>
		/// <param name="path">Lambda expression</param>
		/// <returns>criteria instance</returns>
		IQueryOverOrderBuilder<R,T> OrderBy(Expression<Func<object>> path);

		/// <summary>
		/// Add order expressed as a lambda expression
		/// </summary>
		/// <param name="path">Lambda expression</param>
		/// <returns>criteria instance</returns>
		IQueryOverOrderBuilder<R,T> ThenBy(Expression<Func<T, object>> path);

		/// <summary>
		/// Add order expressed as a lambda expression
		/// </summary>
		/// <param name="path">Lambda expression</param>
		/// <returns>criteria instance</returns>
		IQueryOverOrderBuilder<R,T> ThenBy(Expression<Func<object>> path);

		/// <summary>
		/// Set the first result to be retrieved
		/// </summary>
		/// <param name="firstResult"></param>
		IQueryOver<R,T> Skip(int firstResult);

		/// <summary>
		/// Set a limit upon the number of objects to be retrieved
		/// </summary>
		/// <param name="maxResults"></param>
		IQueryOver<R,T> Take(int maxResults);

		/// <summary>
		/// Enable caching of this query result set
		/// </summary>
		IQueryOver<R,T> Cacheable();

		/// <summary> Override the cache mode for this particular query. </summary>
		/// <param name="cacheMode">The cache mode to use. </param>
		/// <returns> this (for method chaining) </returns>
		IQueryOver<R,T> CacheMode(CacheMode cacheMode);

		/// <summary>
		/// Set the name of the cache region.
		/// </summary>
		/// <param name="cacheRegion">the name of a query cache region, or <see langword="null" />
		/// for the default query cache</param>
		IQueryOver<R,T> CacheRegion(string cacheRegion);

		/// <summary>
		/// Add a subquery expression
		/// </summary>
		IQueryOverSubqueryBuilder<R,T> WithSubquery { get; }

		/// <summary>
		/// Specify an association fetching strategy.  Currently, only
		/// one-to-many and one-to-one associations are supported.
		/// </summary>
		/// <param name="path">A lambda expression path (e.g., ChildList[0].Granchildren[0].Pets).</param>
		/// <returns></returns>
		IQueryOverFetchBuilder<R,T> Fetch(Expression<Func<R, object>> path);

		/// <summary>
		/// Set the lock mode of the current entity
		/// </summary>
		IQueryOverLockBuilder<R,T> Lock();

		/// <summary>
		/// Set the lock mode of the aliased entity
		/// </summary>
		IQueryOverLockBuilder<R,T> Lock(Expression<Func<object>> alias);

		/// <summary>
		/// Creates a new NHibernate.ICriteria&lt;U&gt;, "rooted" at the associated entity
		/// </summary>
		/// <typeparam name="U">Type of sub-criteria</typeparam>
		/// <param name="path">Lambda expression returning association path</param>
		/// <returns>The created "sub criteria"</returns>
		IQueryOver<R,U> JoinQueryOver<U>(Expression<Func<T, U>> path);

		/// <summary>
		/// Creates a new NHibernate.ICriteria&lt;U&gt;, "rooted" at the associated entity
		/// </summary>
		/// <typeparam name="U">Type of sub-criteria</typeparam>
		/// <param name="path">Lambda expression returning association path</param>
		/// <returns>The created "sub criteria"</returns>
		IQueryOver<R,U> JoinQueryOver<U>(Expression<Func<U>> path);

		/// <summary>
		/// Creates a new NHibernate.ICriteria&lt;U&gt;, "rooted" at the associated entity
		/// </summary>
		/// <typeparam name="U">Type of sub-criteria</typeparam>
		/// <param name="path">Lambda expression returning association path</param>
		/// <param name="alias">Lambda expression returning alias reference</param>
		/// <returns>The created "sub criteria"</returns>
		IQueryOver<R,U> JoinQueryOver<U>(Expression<Func<T, U>> path, Expression<Func<U>> alias);

		/// <summary>
		/// Creates a new NHibernate.ICriteria&lt;U&gt;, "rooted" at the associated entity
		/// </summary>
		/// <typeparam name="U">Type of sub-criteria</typeparam>
		/// <param name="path">Lambda expression returning association path</param>
		/// <param name="alias">Lambda expression returning alias reference</param>
		/// <returns>The created "sub criteria"</returns>
		IQueryOver<R,U> JoinQueryOver<U>(Expression<Func<U>> path, Expression<Func<U>> alias);

		/// <summary>
		/// Creates a new NHibernate.ICriteria&lt;U&gt;, "rooted" at the associated entity
		/// </summary>
		/// <typeparam name="U">Type of sub-criteria</typeparam>
		/// <param name="path">Lambda expression returning association path</param>
		/// <param name="joinType">Type of join</param>
		/// <returns>The created "sub criteria"</returns>
		IQueryOver<R,U> JoinQueryOver<U>(Expression<Func<T, U>> path, JoinType joinType);

		/// <summary>
		/// Creates a new NHibernate.ICriteria&lt;U&gt;, "rooted" at the associated entity
		/// </summary>
		/// <typeparam name="U">Type of sub-criteria</typeparam>
		/// <param name="path">Lambda expression returning association path</param>
		/// <param name="joinType">Type of join</param>
		/// <returns>The created "sub criteria"</returns>
		IQueryOver<R,U> JoinQueryOver<U>(Expression<Func<U>> path, JoinType joinType);

		/// <summary>
		/// Creates a new NHibernate.ICriteria&lt;U&gt;, "rooted" at the associated entity
		/// </summary>
		/// <typeparam name="U">Type of sub-criteria</typeparam>
		/// <param name="path">Lambda expression returning association path</param>
		/// <param name="alias">Lambda expression returning alias reference</param>
		/// <param name="joinType">Type of join</param>
		/// <returns>The created "sub criteria"</returns>
		IQueryOver<R,U> JoinQueryOver<U>(Expression<Func<T, U>> path, Expression<Func<U>> alias, JoinType joinType);

		/// <summary>
		/// Creates a new NHibernate.ICriteria&lt;U&gt;, "rooted" at the associated entity
		/// </summary>
		/// <typeparam name="U">Type of sub-criteria</typeparam>
		/// <param name="path">Lambda expression returning association path</param>
		/// <param name="alias">Lambda expression returning alias reference</param>
		/// <param name="joinType">Type of join</param>
		/// <returns>The created "sub criteria"</returns>
		IQueryOver<R,U> JoinQueryOver<U>(Expression<Func<U>> path, Expression<Func<U>> alias, JoinType joinType);

		/// <summary>
		/// Creates a new NHibernate.ICriteria&lt;U&gt;, "rooted" at the associated entity
		/// specifying a collection for the join.
		/// </summary>
		/// <typeparam name="U">Type of sub-criteria (type of the collection)</typeparam>
		/// <param name="path">Lambda expression returning association path</param>
		/// <returns>The created "sub criteria"</returns>
		IQueryOver<R,U> JoinQueryOver<U>(Expression<Func<T, IEnumerable<U>>> path);

		/// <summary>
		/// Creates a new NHibernate.ICriteria&lt;U&gt;, "rooted" at the associated entity
		/// specifying a collection for the join.
		/// </summary>
		/// <typeparam name="U">Type of sub-criteria (type of the collection)</typeparam>
		/// <param name="path">Lambda expression returning association path</param>
		/// <returns>The created "sub criteria"</returns>
		IQueryOver<R,U> JoinQueryOver<U>(Expression<Func<IEnumerable<U>>> path);

		/// <summary>
		/// Creates a new NHibernate.ICriteria&lt;U&gt;, "rooted" at the associated entity
		/// specifying a collection for the join.
		/// </summary>
		/// <typeparam name="U">Type of sub-criteria (type of the collection)</typeparam>
		/// <param name="path">Lambda expression returning association path</param>
		/// <param name="alias">Lambda expression returning alias reference</param>
		/// <returns>The created "sub criteria"</returns>
		IQueryOver<R,U> JoinQueryOver<U>(Expression<Func<T, IEnumerable<U>>> path, Expression<Func<U>> alias);

		/// <summary>
		/// Creates a new NHibernate.ICriteria&lt;U&gt;, "rooted" at the associated entity
		/// specifying a collection for the join.
		/// </summary>
		/// <typeparam name="U">Type of sub-criteria (type of the collection)</typeparam>
		/// <param name="path">Lambda expression returning association path</param>
		/// <param name="alias">Lambda expression returning alias reference</param>
		/// <returns>The created "sub criteria"</returns>
		IQueryOver<R,U> JoinQueryOver<U>(Expression<Func<IEnumerable<U>>> path, Expression<Func<U>> alias);

		/// <summary>
		/// Creates a new NHibernate.ICriteria&lt;U&gt;, "rooted" at the associated entity
		/// specifying a collection for the join.
		/// </summary>
		/// <typeparam name="U">Type of sub-criteria (type of the collection)</typeparam>
		/// <param name="path">Lambda expression returning association path</param>
		/// <param name="joinType">Type of join</param>
		/// <returns>The created "sub criteria"</returns>
		IQueryOver<R,U> JoinQueryOver<U>(Expression<Func<T, IEnumerable<U>>> path, JoinType joinType);

		/// <summary>
		/// Creates a new NHibernate.ICriteria&lt;U&gt;, "rooted" at the associated entity
		/// specifying a collection for the join.
		/// </summary>
		/// <typeparam name="U">Type of sub-criteria (type of the collection)</typeparam>
		/// <param name="path">Lambda expression returning association path</param>
		/// <param name="joinType">Type of join</param>
		/// <returns>The created "sub criteria"</returns>
		IQueryOver<R,U> JoinQueryOver<U>(Expression<Func<IEnumerable<U>>> path, JoinType joinType);

		/// <summary>
		/// Creates a new NHibernate.ICriteria&lt;U&gt;, "rooted" at the associated entity
		/// specifying a collection for the join.
		/// </summary>
		/// <typeparam name="U">Type of sub-criteria (type of the collection)</typeparam>
		/// <param name="path">Lambda expression returning association path</param>
		/// <param name="alias">Lambda expression returning alias reference</param>
		/// <param name="joinType">Type of join</param>
		/// <returns>The created "sub criteria"</returns>
		IQueryOver<R,U> JoinQueryOver<U>(Expression<Func<T, IEnumerable<U>>> path, Expression<Func<U>> alias, JoinType joinType);

		/// <summary>
		/// Creates a new NHibernate.ICriteria&lt;U&gt;, "rooted" at the associated entity
		/// specifying a collection for the join.
		/// </summary>
		/// <typeparam name="U">Type of sub-criteria (type of the collection)</typeparam>
		/// <param name="path">Lambda expression returning association path</param>
		/// <param name="alias">Lambda expression returning alias reference</param>
		/// <param name="joinType">Type of join</param>
		/// <returns>The created "sub criteria"</returns>
		IQueryOver<R,U> JoinQueryOver<U>(Expression<Func<IEnumerable<U>>> path, Expression<Func<U>> alias, JoinType joinType);

		/// <summary>
		/// Join an association, assigning an alias to the joined entity
		/// </summary>
		/// <param name="path">Lambda expression returning association path</param>
		/// <param name="alias">Lambda expression returning alias reference</param>
		/// <returns>criteria instance</returns>
		IQueryOver<R,T> JoinAlias(Expression<Func<T, object>> path, Expression<Func<object>> alias);

		/// <summary>
		/// Join an association, assigning an alias to the joined entity
		/// </summary>
		/// <param name="path">Lambda expression returning association path</param>
		/// <param name="alias">Lambda expression returning alias reference</param>
		/// <returns>criteria instance</returns>
		IQueryOver<R,T> JoinAlias(Expression<Func<object>> path, Expression<Func<object>> alias);

		/// <summary>
		/// Join an association, assigning an alias to the joined entity
		/// </summary>
		/// <param name="path">Lambda expression returning association path</param>
		/// <param name="alias">Lambda expression returning alias reference</param>
		/// <param name="joinType">Type of join</param>
		/// <returns>criteria instance</returns>
		IQueryOver<R,T> JoinAlias(Expression<Func<T, object>> path, Expression<Func<object>> alias, JoinType joinType);

		/// <summary>
		/// Join an association, assigning an alias to the joined entity
		/// </summary>
		/// <param name="path">Lambda expression returning association path</param>
		/// <param name="alias">Lambda expression returning alias reference</param>
		/// <param name="joinType">Type of join</param>
		/// <returns>criteria instance</returns>
		IQueryOver<R,T> JoinAlias(Expression<Func<object>> path, Expression<Func<object>> alias, JoinType joinType);

		IQueryOverJoinBuilder<R,T> Inner { get; }
		IQueryOverJoinBuilder<R,T> Left	{ get; }
		IQueryOverJoinBuilder<R,T> Right	{ get; }
		IQueryOverJoinBuilder<R,T> Full	{ get; }

	}

}
