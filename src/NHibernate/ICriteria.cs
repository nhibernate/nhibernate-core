using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NHibernate.Transform;

namespace NHibernate
{
	/// <summary>
	/// Criteria is a simplified API for retrieving entities by composing
	/// <see cref="Expression" /> objects.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Using criteria is a very convenient approach for functionality like "search" screens
	/// where there is a variable number of conditions to be placed upon the result set.
	/// </para>
	/// <para>
	/// The Session is a factory for ICriteria. Expression instances are usually obtained via 
	/// the factory methods on <see cref="Expression" />. eg:
	/// </para>
	/// <code>
	/// IList cats = session.CreateCriteria(typeof(Cat)) 
	///     .Add( Expression.Like("name", "Iz%") ) 
	///     .Add( Expression.Gt( "weight", minWeight ) ) 
	///     .AddOrder( Order.Asc("age") ) 
	///     .List(); 
	/// </code>
	/// You may navigate associations using <see cref="CreateAlias(string,string)" /> or <see cref="CreateCriteria(string)" />.
	/// <code>
	/// IList cats = session.CreateCriteria(typeof(Cat))
	///		.CreateCriteria("kittens")
	///			.Add( Expression.like("name", "Iz%") )
	///			.List();
	///	</code>
	/// <para>
	/// You may specify projection and aggregation using <tt>Projection</tt>
	/// instances obtained via the factory methods on <tt>Projections</tt>.
	/// <code>
	/// IList cats = session.CreateCriteria(typeof(Cat))
	/// .setProjection( Projections.ProjectionList()
	/// .Add( Projections.RowCount() )
	/// .Add( Projections.Avg("weight") )
	/// .Add( Projections.Max("weight") )
	/// .Add( Projections.Min("weight") )
	/// .Add( Projections.GroupProperty("color") )
	/// )
	/// .AddOrder( Order.Asc("color") )
	/// .List();	
	///	</code>
	/// </para>
	/// </remarks>
	public interface ICriteria : ICloneable
	{
		/// <summary>
		/// Get the alias of the entity encapsulated by this criteria instance.
		/// </summary>
		/// <value>The alias for the encapsulated entity.</value>
		string Alias { get; }

		/// <summary>
		/// Used to specify that the query results will be a projection (scalar in
		/// nature).  Implicitly specifies the projection result transformer.
		/// </summary>
		/// <param name="projection">The projection representing the overall "shape" of the
		/// query results.</param>
		/// <returns>This instance (for method chaining)</returns>
		/// <remarks>
		/// <para>
		/// The individual components contained within the given <see cref="IProjection"/>
		/// determines the overall "shape" of the query result.
		/// </para>
		/// </remarks>
		ICriteria SetProjection(params IProjection[] projection);

		/// <summary>
		/// Add an Expression to constrain the results to be retrieved.
		/// </summary>
		/// <param name="expression"></param>
		/// <returns></returns>
		ICriteria Add(ICriterion expression);

		/// <summary>
		/// An an Order to the result set 
		/// </summary>
		/// <param name="order"></param>
		ICriteria AddOrder(Order order);

		/// <summary>
		/// Specify an association fetching strategy.  Currently, only
		/// one-to-many and one-to-one associations are supported.
		/// </summary>
		/// <param name="associationPath">A dot seperated property path.</param>
		/// <param name="mode">The Fetch mode.</param>
		/// <returns></returns>
		ICriteria SetFetchMode(string associationPath, FetchMode mode);

		/// <summary>
		/// Set the lock mode of the current entity
		/// </summary>
		/// <param name="lockMode">the lock mode</param>
		/// <returns></returns>
		ICriteria SetLockMode(LockMode lockMode);

		/// <summary>
		/// Set the lock mode of the aliased entity
		/// </summary>
		/// <param name="alias">an alias</param>
		/// <param name="lockMode">the lock mode</param>
		/// <returns></returns>
		ICriteria SetLockMode(string alias, LockMode lockMode);

		/// <summary>
		/// Join an association, assigning an alias to the joined entity
		/// </summary>
		/// <param name="associationPath"></param>
		/// <param name="alias"></param>
		/// <returns></returns>
		ICriteria CreateAlias(string associationPath, string alias);

		/// <summary>
		/// Join an association using the specified join-type, assigning an alias to the joined
		/// association
		/// </summary>
		/// <param name="associationPath"></param>
		/// <param name="alias"></param>
		/// <param name="joinType">The type of join to use.</param>
		/// <returns>this (for method chaining)</returns>
		ICriteria CreateAlias(string associationPath, string alias, JoinType joinType);

		/// <summary>
		/// Join an association using the specified join-type, assigning an alias to the joined
		/// association
		/// </summary>
		/// <param name="associationPath"></param>
		/// <param name="alias"></param>
		/// <param name="joinType">The type of join to use.</param>
		/// <param name="withClause"The criteria to be added to the join condition (ON clause)</param>
		/// <returns>this (for method chaining)</returns>
		ICriteria CreateAlias(string associationPath, string alias, JoinType joinType, ICriterion withClause);

		/// <summary>
		/// Create a new <see cref="ICriteria" />, "rooted" at the associated entity
		/// </summary>
		/// <param name="associationPath"></param>
		/// <returns></returns>
		ICriteria CreateCriteria(string associationPath);

		/// <summary>
		/// Create a new <see cref="ICriteria" />, "rooted" at the associated entity, 
		/// using the specified join type.
		/// </summary>
		/// <param name="associationPath">A dot-seperated property path</param>
		/// <param name="joinType">The type of join to use</param>
		/// <returns>The created "sub criteria"</returns>
		ICriteria CreateCriteria(string associationPath, JoinType joinType);

		/// <summary>
		/// Create a new <see cref="ICriteria" />, "rooted" at the associated entity,
		/// assigning the given alias
		/// </summary>
		/// <param name="associationPath"></param>
		/// <param name="alias"></param>
		/// <returns></returns>
		ICriteria CreateCriteria(string associationPath, string alias);

		/// <summary>
		/// Create a new <see cref="ICriteria" />, "rooted" at the associated entity,
		/// assigning the given alias and using the specified join type.
		/// </summary>
		/// <param name="associationPath">A dot-separated property path</param>
		/// <param name="alias">The alias to assign to the joined association (for later reference).</param>
		/// <param name="joinType">The type of join to use.</param>
		/// <returns>The created "sub criteria"</returns>
		ICriteria CreateCriteria(string associationPath, string alias, JoinType joinType);

		/// <summary>
		/// Create a new <see cref="ICriteria" />, "rooted" at the associated entity,
		/// assigning the given alias and using the specified join type.
		/// </summary>
		/// <param name="associationPath">A dot-separated property path</param>
		/// <param name="alias">The alias to assign to the joined association (for later reference).</param>
		/// <param name="joinType">The type of join to use.</param>
		/// <param name="withClause"The criteria to be added to the join condition (ON clause)</param>
		/// <returns>The created "sub criteria"</returns>
		ICriteria CreateCriteria(string associationPath, string alias, JoinType joinType, ICriterion withClause);

		/// <summary>
		/// Set a strategy for handling the query results. This determines the
		/// "shape" of the query result set.
		/// <seealso cref="CriteriaSpecification.RootEntity"/>
		/// <seealso cref="CriteriaSpecification.DistinctRootEntity"/>
		/// <seealso cref="CriteriaSpecification.AliasToEntityMap"/>
		/// </summary>
		/// <param name="resultTransformer"></param>
		/// <returns></returns>
		ICriteria SetResultTransformer(IResultTransformer resultTransformer);

		/// <summary>
		/// Set a limit upon the number of objects to be retrieved
		/// </summary>
		/// <param name="maxResults"></param>
		ICriteria SetMaxResults(int maxResults);

		/// <summary>
		/// Set the first result to be retrieved
		/// </summary>
		/// <param name="firstResult"></param>
		ICriteria SetFirstResult(int firstResult);

		/// <summary> Set a fetch size for the underlying ADO query. </summary>
		/// <param name="fetchSize">the fetch size </param>
		/// <returns> this (for method chaining) </returns>
		ICriteria SetFetchSize(int fetchSize);

		/// <summary>
		/// Set a timeout for the underlying ADO.NET query
		/// </summary>
		/// <param name="timeout"></param>
		/// <returns></returns>
		ICriteria SetTimeout(int timeout);

		/// <summary>
		/// Enable caching of this query result set
		/// </summary>
		/// <param name="cacheable"></param>
		/// <returns></returns>
		ICriteria SetCacheable(bool cacheable);

		/// <summary>
		/// Set the name of the cache region.
		/// </summary>
		/// <param name="cacheRegion">the name of a query cache region, or <see langword="null" />
		/// for the default query cache</param>
		/// <returns></returns>
		ICriteria SetCacheRegion(string cacheRegion);

		/// <summary> Add a comment to the generated SQL. </summary>
		/// <param name="comment">a human-readable string </param>
		/// <returns> this (for method chaining) </returns>
		ICriteria SetComment(string comment);
		
		/// <summary> Override the flush mode for this particular query. </summary>
		/// <param name="flushMode">The flush mode to use. </param>
		/// <returns> this (for method chaining) </returns>
		ICriteria SetFlushMode(FlushMode flushMode);

		/// <summary> Override the cache mode for this particular query. </summary>
		/// <param name="cacheMode">The cache mode to use. </param>
		/// <returns> this (for method chaining) </returns>
		ICriteria SetCacheMode(CacheMode cacheMode);

		/// <summary>
		/// Get the results
		/// </summary>
		/// <returns></returns>
		IList List();

		/// <summary>
		/// Convenience method to return a single instance that matches
		/// the query, or null if the query returns no results.
		/// </summary>
		/// <returns>the single result or <see langword="null" /></returns>
		/// <exception cref="HibernateException">
		/// If there is more than one matching result
		/// </exception>
		object UniqueResult();

        /// <summary>
        /// Get a enumerable that when enumerated will execute
        /// a batch of queries in a single database roundtrip
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
	    IEnumerable<T> Future<T>();

		/// <summary>
		/// Get an IFutureValue instance, whose value can be retrieved through
		/// its Value property. The query is not executed until the Value property
		/// is retrieved, which will execute other Future queries as well in a
		/// single roundtrip
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		IFutureValue<T> FutureValue<T>();

		#region NHibernate specific

		/// <summary>
		/// Get the results and fill the <see cref="IList"/>
		/// </summary>
		/// <param name="results">The list to fill with the results.</param>
		void List(IList results);

		/// <summary>
		/// Strongly-typed version of <see cref="List()" />.
		/// </summary>
		IList<T> List<T>();

		/// <summary>
		/// Strongly-typed version of <see cref="UniqueResult()" />.
		/// </summary>
		T UniqueResult<T>();

		/// <summary>
		/// Clear all orders from criteria.
		/// </summary>
		void ClearOrders();

		/// <summary>
		/// Allows to get a sub criteria by path.
		/// Will return null if the criteria does not exists.
		/// </summary>
		/// <param name="path">The path.</param>
		ICriteria GetCriteriaByPath(string path);

		/// <summary>
		/// Alows to get a sub criteria by alias.
		/// Will return null if the criteria does not exists
		/// </summary>
		/// <param name="alias">The alias.</param>
		/// <returns></returns>
		ICriteria GetCriteriaByAlias(string alias);

		/// <summary>
		/// Gets the root entity type if available, throws otherwise
		/// </summary>
		/// <remarks>
		/// This is an NHibernate specific method, used by several dependent 
		/// frameworks for advance integration with NHibernate.
		/// </remarks>
		System.Type GetRootEntityTypeIfAvailable();

		#endregion
	}
}
