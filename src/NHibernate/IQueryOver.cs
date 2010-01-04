
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using NHibernate.Criterion;
using NHibernate.Criterion.Lambda;
using NHibernate.SqlCommand;

namespace NHibernate
{

	/// <summary>
	/// QueryOver&lt;TRoot&gt; is an API for retrieving entities by composing
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
	public interface IQueryOver<TRoot>
	{
		/// <summary>
		/// Access the underlying ICriteria
		/// </summary>
		ICriteria UnderlyingCriteria { get; }

		/// <summary>
		/// Get the results of the root type and fill the <see cref="IList&lt;T&gt;"/>
		/// </summary>
		/// <returns>The list filled with the results.</returns>
		IList<TRoot> List();

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
		TRoot SingleOrDefault();

		/// <summary>
		/// Override type of <see cref="SingleOrDefault()" />.
		/// </summary>
		U SingleOrDefault<U>();

		/// <summary>
		/// Get a enumerable that when enumerated will execute
		/// a batch of queries in a single database roundtrip
		/// </summary>
		IEnumerable<TRoot> Future();

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
		IFutureValue<TRoot> FutureValue();

		/// <summary>
		/// Get an IFutureValue instance, whose value can be retrieved through
		/// its Value property. The query is not executed until the Value property
		/// is retrieved, which will execute other Future queries as well in a
		/// single roundtrip
		/// </summary>
		IFutureValue<U> FutureValue<U>();

	}

	/// <summary>
	/// QueryOver&lt;TRoot,TSubType&gt; is an API for retrieving entities by composing
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
	public interface IQueryOver<TRoot,TSubType> : IQueryOver<TRoot>
	{

		/// <summary>
		/// Add criterion expressed as a lambda expression
		/// </summary>
		/// <param name="expression">Lambda expression</param>
		/// <returns>criteria instance</returns>
		IQueryOver<TRoot,TSubType> And(Expression<Func<TSubType, bool>> expression);

		/// <summary>
		/// Add criterion expressed as a lambda expression
		/// </summary>
		/// <param name="expression">Lambda expression</param>
		/// <returns>criteria instance</returns>
		IQueryOver<TRoot,TSubType> And(Expression<Func<bool>> expression);

		/// <summary>
		/// Add arbitrary ICriterion (e.g., to allow protected member access)
		/// </summary>
		IQueryOver<TRoot,TSubType> And(ICriterion expression);

		/// <summary>
		/// Add negation of criterion expressed as a lambda expression
		/// </summary>
		/// <param name="expression">Lambda expression</param>
		/// <returns>criteria instance</returns>
		IQueryOver<TRoot,TSubType> AndNot(Expression<Func<TSubType, bool>> expression);

		/// <summary>
		/// Add negation of criterion expressed as a lambda expression
		/// </summary>
		/// <param name="expression">Lambda expression</param>
		/// <returns>criteria instance</returns>
		IQueryOver<TRoot,TSubType> AndNot(Expression<Func<bool>> expression);

		/// <summary>
		/// Add restriction to a property
		/// </summary>
		/// <param name="expression">Lambda expression containing path to property</param>
		/// <returns>criteria instance</returns>
		IQueryOverRestrictionBuilder<TRoot,TSubType> AndRestrictionOn(Expression<Func<TSubType, object>> expression);

		/// <summary>
		/// Add restriction to a property
		/// </summary>
		/// <param name="expression">Lambda expression containing path to property</param>
		/// <returns>criteria instance</returns>
		IQueryOverRestrictionBuilder<TRoot,TSubType> AndRestrictionOn(Expression<Func<object>> expression);

		/// <summary>
		/// Identical semantics to And() to allow more readable queries
		/// </summary>
		/// <param name="expression">Lambda expression</param>
		/// <returns>criteria instance</returns>
		IQueryOver<TRoot,TSubType> Where(Expression<Func<TSubType, bool>> expression);

		/// <summary>
		/// Identical semantics to And() to allow more readable queries
		/// </summary>
		/// <param name="expression">Lambda expression</param>
		/// <returns>criteria instance</returns>
		IQueryOver<TRoot,TSubType> Where(Expression<Func<bool>> expression);

		/// <summary>
		/// Add arbitrary ICriterion (e.g., to allow protected member access)
		/// </summary>
		IQueryOver<TRoot,TSubType> Where(ICriterion expression);

		/// <summary>
		/// Identical semantics to AndNot() to allow more readable queries
		/// </summary>
		/// <param name="expression">Lambda expression</param>
		/// <returns>criteria instance</returns>
		IQueryOver<TRoot,TSubType> WhereNot(Expression<Func<TSubType, bool>> expression);

		/// <summary>
		/// Identical semantics to AndNot() to allow more readable queries
		/// </summary>
		/// <param name="expression">Lambda expression</param>
		/// <returns>criteria instance</returns>
		IQueryOver<TRoot,TSubType> WhereNot(Expression<Func<bool>> expression);

		/// <summary>
		/// Identical semantics to AndRestrictionOn() to allow more readable queries
		/// </summary>
		/// <param name="expression">Lambda expression</param>
		/// <returns>criteria instance</returns>
		IQueryOverRestrictionBuilder<TRoot,TSubType> WhereRestrictionOn(Expression<Func<TSubType, object>> expression);

		/// <summary>
		/// Identical semantics to AndRestrictionOn() to allow more readable queries
		/// </summary>
		/// <param name="expression">Lambda expression</param>
		/// <returns>criteria instance</returns>
		IQueryOverRestrictionBuilder<TRoot,TSubType> WhereRestrictionOn(Expression<Func<object>> expression);

		/// <summary>
		/// Add projection expressed as a lambda expression
		/// </summary>
		/// <param name="projections">Lambda expressions</param>
		/// <returns>criteria instance</returns>
		IQueryOver<TRoot,TSubType> Select(params Expression<Func<TRoot, object>>[] projections);

		/// <summary>
		/// Add arbitrary IProjections to query
		/// </summary>
		IQueryOver<TRoot,TSubType> Select(params IProjection[] projections);

		/// <summary>
		/// Create a list of projections inline
		/// </summary>
		QueryOverProjectionBuilder<IQueryOver<TRoot,TSubType>, TRoot, TSubType> SelectList { get; }

		/// <summary>
		/// Add order expressed as a lambda expression
		/// </summary>
		/// <param name="path">Lambda expression</param>
		/// <returns>criteria instance</returns>
		IQueryOverOrderBuilder<TRoot,TSubType> OrderBy(Expression<Func<TSubType, object>> path);

		/// <summary>
		/// Add order expressed as a lambda expression
		/// </summary>
		/// <param name="path">Lambda expression</param>
		/// <returns>criteria instance</returns>
		IQueryOverOrderBuilder<TRoot,TSubType> OrderBy(Expression<Func<object>> path);

		/// <summary>
		/// Add order expressed as a lambda expression
		/// </summary>
		/// <param name="path">Lambda expression</param>
		/// <returns>criteria instance</returns>
		IQueryOverOrderBuilder<TRoot,TSubType> ThenBy(Expression<Func<TSubType, object>> path);

		/// <summary>
		/// Add order expressed as a lambda expression
		/// </summary>
		/// <param name="path">Lambda expression</param>
		/// <returns>criteria instance</returns>
		IQueryOverOrderBuilder<TRoot,TSubType> ThenBy(Expression<Func<object>> path);

		/// <summary>
		/// Set the first result to be retrieved
		/// </summary>
		/// <param name="firstResult"></param>
		IQueryOver<TRoot,TSubType> Skip(int firstResult);

		/// <summary>
		/// Set a limit upon the number of objects to be retrieved
		/// </summary>
		/// <param name="maxResults"></param>
		IQueryOver<TRoot,TSubType> Take(int maxResults);

		/// <summary>
		/// Enable caching of this query result set
		/// </summary>
		IQueryOver<TRoot,TSubType> Cacheable();

		/// <summary> Override the cache mode for this particular query. </summary>
		/// <param name="cacheMode">The cache mode to use. </param>
		/// <returns> this (for method chaining) </returns>
		IQueryOver<TRoot,TSubType> CacheMode(CacheMode cacheMode);

		/// <summary>
		/// Set the name of the cache region.
		/// </summary>
		/// <param name="cacheRegion">the name of a query cache region, or <see langword="null" />
		/// for the default query cache</param>
		IQueryOver<TRoot,TSubType> CacheRegion(string cacheRegion);

		/// <summary>
		/// Add a subquery expression
		/// </summary>
		IQueryOverSubqueryBuilder<TRoot,TSubType> WithSubquery { get; }

		/// <summary>
		/// Specify an association fetching strategy.  Currently, only
		/// one-to-many and one-to-one associations are supported.
		/// </summary>
		/// <param name="path">A lambda expression path (e.g., ChildList[0].Granchildren[0].Pets).</param>
		/// <returns></returns>
		IQueryOverFetchBuilder<TRoot,TSubType> Fetch(Expression<Func<TRoot, object>> path);

		/// <summary>
		/// Set the lock mode of the current entity
		/// </summary>
		IQueryOverLockBuilder<TRoot,TSubType> Lock();

		/// <summary>
		/// Set the lock mode of the aliased entity
		/// </summary>
		IQueryOverLockBuilder<TRoot,TSubType> Lock(Expression<Func<object>> alias);

		/// <summary>
		/// Creates a new NHibernate.IQueryOver&lt;TRoot, U&gt;, "rooted" at the associated entity
		/// </summary>
		/// <typeparam name="U">Type of sub-criteria</typeparam>
		/// <param name="path">Lambda expression returning association path</param>
		/// <returns>The created "sub criteria"</returns>
		IQueryOver<TRoot,U> JoinQueryOver<U>(Expression<Func<TSubType, U>> path);

		/// <summary>
		/// Creates a new NHibernate.IQueryOver&lt;TRoot, U&gt;, "rooted" at the associated entity
		/// </summary>
		/// <typeparam name="U">Type of sub-criteria</typeparam>
		/// <param name="path">Lambda expression returning association path</param>
		/// <returns>The created "sub criteria"</returns>
		IQueryOver<TRoot,U> JoinQueryOver<U>(Expression<Func<U>> path);

		/// <summary>
		/// Creates a new NHibernate.IQueryOver&lt;TRoot, U&gt;, "rooted" at the associated entity
		/// </summary>
		/// <typeparam name="U">Type of sub-criteria</typeparam>
		/// <param name="path">Lambda expression returning association path</param>
		/// <param name="alias">Lambda expression returning alias reference</param>
		/// <returns>The created "sub criteria"</returns>
		IQueryOver<TRoot,U> JoinQueryOver<U>(Expression<Func<TSubType, U>> path, Expression<Func<U>> alias);

		/// <summary>
		/// Creates a new NHibernate.IQueryOver&lt;TRoot, U&gt;, "rooted" at the associated entity
		/// </summary>
		/// <typeparam name="U">Type of sub-criteria</typeparam>
		/// <param name="path">Lambda expression returning association path</param>
		/// <param name="alias">Lambda expression returning alias reference</param>
		/// <returns>The created "sub criteria"</returns>
		IQueryOver<TRoot,U> JoinQueryOver<U>(Expression<Func<U>> path, Expression<Func<U>> alias);

		/// <summary>
		/// Creates a new NHibernate.IQueryOver&lt;TRoot, U&gt;, "rooted" at the associated entity
		/// </summary>
		/// <typeparam name="U">Type of sub-criteria</typeparam>
		/// <param name="path">Lambda expression returning association path</param>
		/// <param name="joinType">Type of join</param>
		/// <returns>The created "sub criteria"</returns>
		IQueryOver<TRoot,U> JoinQueryOver<U>(Expression<Func<TSubType, U>> path, JoinType joinType);

		/// <summary>
		/// Creates a new NHibernate.IQueryOver&lt;TRoot, U&gt;, "rooted" at the associated entity
		/// </summary>
		/// <typeparam name="U">Type of sub-criteria</typeparam>
		/// <param name="path">Lambda expression returning association path</param>
		/// <param name="joinType">Type of join</param>
		/// <returns>The created "sub criteria"</returns>
		IQueryOver<TRoot,U> JoinQueryOver<U>(Expression<Func<U>> path, JoinType joinType);

		/// <summary>
		/// Creates a new NHibernate.IQueryOver&lt;TRoot, U&gt;, "rooted" at the associated entity
		/// </summary>
		/// <typeparam name="U">Type of sub-criteria</typeparam>
		/// <param name="path">Lambda expression returning association path</param>
		/// <param name="alias">Lambda expression returning alias reference</param>
		/// <param name="joinType">Type of join</param>
		/// <returns>The created "sub criteria"</returns>
		IQueryOver<TRoot,U> JoinQueryOver<U>(Expression<Func<TSubType, U>> path, Expression<Func<U>> alias, JoinType joinType);

		/// <summary>
		/// Creates a new NHibernate.IQueryOver&lt;TRoot, U&gt;, "rooted" at the associated entity
		/// </summary>
		/// <typeparam name="U">Type of sub-criteria</typeparam>
		/// <param name="path">Lambda expression returning association path</param>
		/// <param name="alias">Lambda expression returning alias reference</param>
		/// <param name="joinType">Type of join</param>
		/// <returns>The created "sub criteria"</returns>
		IQueryOver<TRoot,U> JoinQueryOver<U>(Expression<Func<U>> path, Expression<Func<U>> alias, JoinType joinType);

		/// <summary>
		/// Creates a new NHibernate.IQueryOver&lt;TRoot, U&gt;, "rooted" at the associated entity
		/// specifying a collection for the join.
		/// </summary>
		/// <typeparam name="U">Type of sub-criteria (type of the collection)</typeparam>
		/// <param name="path">Lambda expression returning association path</param>
		/// <returns>The created "sub criteria"</returns>
		IQueryOver<TRoot,U> JoinQueryOver<U>(Expression<Func<TSubType, IEnumerable<U>>> path);

		/// <summary>
		/// Creates a new NHibernate.IQueryOver&lt;TRoot, U&gt;, "rooted" at the associated entity
		/// specifying a collection for the join.
		/// </summary>
		/// <typeparam name="U">Type of sub-criteria (type of the collection)</typeparam>
		/// <param name="path">Lambda expression returning association path</param>
		/// <returns>The created "sub criteria"</returns>
		IQueryOver<TRoot,U> JoinQueryOver<U>(Expression<Func<IEnumerable<U>>> path);

		/// <summary>
		/// Creates a new NHibernate.IQueryOver&lt;TRoot, U&gt;, "rooted" at the associated entity
		/// specifying a collection for the join.
		/// </summary>
		/// <typeparam name="U">Type of sub-criteria (type of the collection)</typeparam>
		/// <param name="path">Lambda expression returning association path</param>
		/// <param name="alias">Lambda expression returning alias reference</param>
		/// <returns>The created "sub criteria"</returns>
		IQueryOver<TRoot,U> JoinQueryOver<U>(Expression<Func<TSubType, IEnumerable<U>>> path, Expression<Func<U>> alias);

		/// <summary>
		/// Creates a new NHibernate.IQueryOver&lt;TRoot, U&gt;, "rooted" at the associated entity
		/// specifying a collection for the join.
		/// </summary>
		/// <typeparam name="U">Type of sub-criteria (type of the collection)</typeparam>
		/// <param name="path">Lambda expression returning association path</param>
		/// <param name="alias">Lambda expression returning alias reference</param>
		/// <returns>The created "sub criteria"</returns>
		IQueryOver<TRoot,U> JoinQueryOver<U>(Expression<Func<IEnumerable<U>>> path, Expression<Func<U>> alias);

		/// <summary>
		/// Creates a new NHibernate.IQueryOver&lt;TRoot, U&gt;, "rooted" at the associated entity
		/// specifying a collection for the join.
		/// </summary>
		/// <typeparam name="U">Type of sub-criteria (type of the collection)</typeparam>
		/// <param name="path">Lambda expression returning association path</param>
		/// <param name="joinType">Type of join</param>
		/// <returns>The created "sub criteria"</returns>
		IQueryOver<TRoot,U> JoinQueryOver<U>(Expression<Func<TSubType, IEnumerable<U>>> path, JoinType joinType);

		/// <summary>
		/// Creates a new NHibernate.IQueryOver&lt;TRoot, U&gt;, "rooted" at the associated entity
		/// specifying a collection for the join.
		/// </summary>
		/// <typeparam name="U">Type of sub-criteria (type of the collection)</typeparam>
		/// <param name="path">Lambda expression returning association path</param>
		/// <param name="joinType">Type of join</param>
		/// <returns>The created "sub criteria"</returns>
		IQueryOver<TRoot,U> JoinQueryOver<U>(Expression<Func<IEnumerable<U>>> path, JoinType joinType);

		/// <summary>
		/// Creates a new NHibernate.IQueryOver&lt;TRoot, U&gt;, "rooted" at the associated entity
		/// specifying a collection for the join.
		/// </summary>
		/// <typeparam name="U">Type of sub-criteria (type of the collection)</typeparam>
		/// <param name="path">Lambda expression returning association path</param>
		/// <param name="alias">Lambda expression returning alias reference</param>
		/// <param name="joinType">Type of join</param>
		/// <returns>The created "sub criteria"</returns>
		IQueryOver<TRoot,U> JoinQueryOver<U>(Expression<Func<TSubType, IEnumerable<U>>> path, Expression<Func<U>> alias, JoinType joinType);

		/// <summary>
		/// Creates a new NHibernate.IQueryOver&lt;TRoot, U&gt;, "rooted" at the associated entity
		/// specifying a collection for the join.
		/// </summary>
		/// <typeparam name="U">Type of sub-criteria (type of the collection)</typeparam>
		/// <param name="path">Lambda expression returning association path</param>
		/// <param name="alias">Lambda expression returning alias reference</param>
		/// <param name="joinType">Type of join</param>
		/// <returns>The created "sub criteria"</returns>
		IQueryOver<TRoot,U> JoinQueryOver<U>(Expression<Func<IEnumerable<U>>> path, Expression<Func<U>> alias, JoinType joinType);

		/// <summary>
		/// Join an association, assigning an alias to the joined entity
		/// </summary>
		/// <param name="path">Lambda expression returning association path</param>
		/// <param name="alias">Lambda expression returning alias reference</param>
		/// <returns>criteria instance</returns>
		IQueryOver<TRoot,TSubType> JoinAlias(Expression<Func<TSubType, object>> path, Expression<Func<object>> alias);

		/// <summary>
		/// Join an association, assigning an alias to the joined entity
		/// </summary>
		/// <param name="path">Lambda expression returning association path</param>
		/// <param name="alias">Lambda expression returning alias reference</param>
		/// <returns>criteria instance</returns>
		IQueryOver<TRoot,TSubType> JoinAlias(Expression<Func<object>> path, Expression<Func<object>> alias);

		/// <summary>
		/// Join an association, assigning an alias to the joined entity
		/// </summary>
		/// <param name="path">Lambda expression returning association path</param>
		/// <param name="alias">Lambda expression returning alias reference</param>
		/// <param name="joinType">Type of join</param>
		/// <returns>criteria instance</returns>
		IQueryOver<TRoot,TSubType> JoinAlias(Expression<Func<TSubType, object>> path, Expression<Func<object>> alias, JoinType joinType);

		/// <summary>
		/// Join an association, assigning an alias to the joined entity
		/// </summary>
		/// <param name="path">Lambda expression returning association path</param>
		/// <param name="alias">Lambda expression returning alias reference</param>
		/// <param name="joinType">Type of join</param>
		/// <returns>criteria instance</returns>
		IQueryOver<TRoot,TSubType> JoinAlias(Expression<Func<object>> path, Expression<Func<object>> alias, JoinType joinType);

		IQueryOverJoinBuilder<TRoot,TSubType> Inner { get; }
		IQueryOverJoinBuilder<TRoot,TSubType> Left	{ get; }
		IQueryOverJoinBuilder<TRoot,TSubType> Right	{ get; }
		IQueryOverJoinBuilder<TRoot,TSubType> Full	{ get; }

	}

}
