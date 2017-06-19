﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using NHibernate.AdoNet;
using NHibernate.Cache;
using NHibernate.Collection;
using NHibernate.Engine.Query.Sql;
using NHibernate.Event;
using NHibernate.Hql;
using NHibernate.Impl;
using NHibernate.Loader.Custom;
using NHibernate.Persister.Entity;
using NHibernate.Transaction;
using NHibernate.Type;

namespace NHibernate.Engine
{
	using System.Threading.Tasks;
	using System.Threading;
	/// <content>
	/// Contains generated async methods
	/// </content>
	public partial interface ISessionImplementor
	{

		/// <summary>
		/// Initialize the collection (if not already initialized)
		/// </summary>
		/// <param name="collection"></param>
		/// <param name="writing"></param>
		/// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
		Task InitializeCollectionAsync(IPersistentCollection collection, bool writing, CancellationToken cancellationToken);

		// NH-268
		/// <summary>
		/// Load an instance without checking if it was deleted. If it does not exist and isn't nullable, throw an exception.
		/// This method may create a new proxy or return an existing proxy.
		/// </summary>
		/// <param name="entityName">The entityName (or class full name) to load.</param>
		/// <param name="id">The identifier of the object in the database.</param>
		/// <param name="isNullable">Allow null instance</param>
		/// <param name="eager">When enabled, the object is eagerly fetched.</param>
		/// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
		/// <returns>
		/// A proxy of the object or an instance of the object if the <c>persistentClass</c> does not have a proxy.
		/// </returns>
		/// <exception cref="ObjectNotFoundException">No object could be found with that <c>id</c>.</exception>
		Task<object> InternalLoadAsync(string entityName, object id, bool eager, bool isNullable, CancellationToken cancellationToken);

		/// <summary>
		/// Load an instance immediately. Do not return a proxy.
		/// </summary>
		/// <param name="entityName"></param>
		/// <param name="id"></param>
		/// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
		/// <returns></returns>
		Task<object> ImmediateLoadAsync(string entityName, object id, CancellationToken cancellationToken);

		/// <summary>
		/// Execute a <c>List()</c> expression query
		/// </summary>
		/// <param name="queryExpression"></param>
		/// <param name="parameters"></param>
		/// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
		/// <returns></returns>
		Task<IList> ListAsync(IQueryExpression queryExpression, QueryParameters parameters, CancellationToken cancellationToken);

		Task ListAsync(IQueryExpression queryExpression, QueryParameters queryParameters, IList results, CancellationToken cancellationToken);

		/// <summary>
		/// Strongly-typed version of <see cref="List(IQueryExpression,QueryParameters)" />
		/// </summary>
		Task<IList<T>> ListAsync<T>(IQueryExpression queryExpression, QueryParameters queryParameters, CancellationToken cancellationToken);

		/// <summary>
		/// Strongly-typed version of <see cref="List(CriteriaImpl)" />
		/// </summary>
		Task<IList<T>> ListAsync<T>(CriteriaImpl criteria, CancellationToken cancellationToken);

		Task ListAsync(CriteriaImpl criteria, IList results, CancellationToken cancellationToken);

		Task<IList> ListAsync(CriteriaImpl criteria, CancellationToken cancellationToken);

		/// <summary>
		/// Execute an <c>Iterate()</c> query
		/// </summary>
		/// <param name="query"></param>
		/// <param name="parameters"></param>
		/// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
		/// <returns></returns>
		Task<IEnumerable> EnumerableAsync(IQueryExpression query, QueryParameters parameters, CancellationToken cancellationToken);

		/// <summary>
		/// Strongly-typed version of <see cref="Enumerable(IQueryExpression, QueryParameters)" />
		/// </summary>
		Task<IEnumerable<T>> EnumerableAsync<T>(IQueryExpression query, QueryParameters queryParameters, CancellationToken cancellationToken);

		/// <summary>
		/// Execute a filter
		/// </summary>
		Task<IList> ListFilterAsync(object collection, string filter, QueryParameters parameters, CancellationToken cancellationToken);

		/// <summary>
		/// Execute a filter (strongly-typed version).
		/// </summary>
		Task<IList<T>> ListFilterAsync<T>(object collection, string filter, QueryParameters parameters, CancellationToken cancellationToken);

		/// <summary>
		/// Collection from a filter
		/// </summary>
		Task<IEnumerable> EnumerableFilterAsync(object collection, string filter, QueryParameters parameters, CancellationToken cancellationToken);

		/// <summary>
		/// Strongly-typed version of <see cref="EnumerableFilter(object, string, QueryParameters)" />
		/// </summary>
		Task<IEnumerable<T>> EnumerableFilterAsync<T>(object collection, string filter, QueryParameters parameters, CancellationToken cancellationToken);

		/// <summary>
		/// Notify the session that the transaction completed, so we no longer own the old locks.
		/// (Also we should release cache softlocks). May be called multiple times during the transaction
		/// completion process.
		/// </summary>
		Task AfterTransactionCompletionAsync(bool successful, ITransaction tx);

		/// <summary>
		/// Execute an SQL Query
		/// </summary>
		Task<IList> ListAsync(NativeSQLQuerySpecification spec, QueryParameters queryParameters, CancellationToken cancellationToken);

		Task ListAsync(NativeSQLQuerySpecification spec, QueryParameters queryParameters, IList results, CancellationToken cancellationToken);

		/// <summary>
		/// Strongly-typed version of <see cref="List(NativeSQLQuerySpecification, QueryParameters)" />
		/// </summary>
		Task<IList<T>> ListAsync<T>(NativeSQLQuerySpecification spec, QueryParameters queryParameters, CancellationToken cancellationToken);

		/// <summary> Execute an SQL Query</summary>
		Task ListCustomQueryAsync(ICustomQuery customQuery, QueryParameters queryParameters, IList results, CancellationToken cancellationToken);

		Task<IList<T>> ListCustomQueryAsync<T>(ICustomQuery customQuery, QueryParameters queryParameters, CancellationToken cancellationToken);
		
		Task<IQueryTranslator[]> GetQueriesAsync(IQueryExpression query, bool scalar, CancellationToken cancellationToken); // NH specific for MultiQuery

		/// <summary> 
		/// Get the entity instance associated with the given <tt>Key</tt>,
		/// calling the Interceptor if necessary
		/// </summary>
		Task<object> GetEntityUsingInterceptorAsync(EntityKey key, CancellationToken cancellationToken);

		Task FlushAsync(CancellationToken cancellationToken);

		/// <summary> Execute a native SQL update or delete query</summary>
		Task<int> ExecuteNativeUpdateAsync(NativeSQLQuerySpecification specification, QueryParameters queryParameters, CancellationToken cancellationToken);

		/// <summary> Execute a HQL update or delete query</summary>
		Task<int> ExecuteUpdateAsync(IQueryExpression query, QueryParameters queryParameters, CancellationToken cancellationToken);
	}
}
