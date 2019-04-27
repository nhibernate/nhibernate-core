﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Data.Common;
using NHibernate.Cache;
using NHibernate.Cache.Entry;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Id;
using NHibernate.Metadata;
using NHibernate.Persister.Entity;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Persister.Collection
{
	using System.Threading.Tasks;
	using System.Threading;
	public partial interface ICollectionPersister
		//TODO 6.0: Uncomment
		//,ICacheablePersister
	{

		/// <summary>
		/// Initialize the given collection with the given key
		/// </summary>
		/// <param name="key"></param>
		/// <param name="session"></param>
		/// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
		Task InitializeAsync(object key, ISessionImplementor session, CancellationToken cancellationToken);

		/// <summary>
		/// Read the key from a row of the <see cref="DbDataReader" />
		/// </summary>
		Task<object> ReadKeyAsync(DbDataReader rs, string[] keyAliases, ISessionImplementor session, CancellationToken cancellationToken);

		/// <summary>
		/// Read the element from a row of the <see cref="DbDataReader" />
		/// </summary>
		//TODO: the ReadElement should really be a parameterized TElement
		Task<object> ReadElementAsync(DbDataReader rs, object owner, string[] columnAliases, ISessionImplementor session, CancellationToken cancellationToken);

		/// <summary>
		/// Read the index from a row of the <see cref="DbDataReader" />
		/// </summary>
		//TODO: the ReadIndex should really be a parameterized TIndex
		Task<object> ReadIndexAsync(DbDataReader rs, string[] columnAliases, ISessionImplementor session, CancellationToken cancellationToken);

		/// <summary>
		/// Read the identifier from a row of the <see cref="DbDataReader" />
		/// </summary>
		//TODO: the ReadIdentifier should really be a parameterized TIdentifier
		Task<object> ReadIdentifierAsync(DbDataReader rs, string columnAlias, ISessionImplementor session, CancellationToken cancellationToken);

		/// <summary>
		/// Completely remove the persistent state of the collection
		/// </summary>
		/// <param name="id"></param>
		/// <param name="session"></param>
		/// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
		Task RemoveAsync(object id, ISessionImplementor session, CancellationToken cancellationToken);

		/// <summary>
		/// (Re)create the collection's persistent state
		/// </summary>
		/// <param name="collection"></param>
		/// <param name="key"></param>
		/// <param name="session"></param>
		/// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
		Task RecreateAsync(IPersistentCollection collection, object key, ISessionImplementor session, CancellationToken cancellationToken);

		/// <summary>
		/// Delete the persistent state of any elements that were removed from the collection
		/// </summary>
		/// <param name="collection"></param>
		/// <param name="key"></param>
		/// <param name="session"></param>
		/// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
		Task DeleteRowsAsync(IPersistentCollection collection, object key, ISessionImplementor session, CancellationToken cancellationToken);

		/// <summary>
		/// Update the persistent state of any elements that were modified
		/// </summary>
		/// <param name="collection"></param>
		/// <param name="key"></param>
		/// <param name="session"></param>
		/// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
		Task UpdateRowsAsync(IPersistentCollection collection, object key, ISessionImplementor session, CancellationToken cancellationToken);

		/// <summary>
		/// Insert the persistent state of any new collection elements
		/// </summary>
		/// <param name="collection"></param>
		/// <param name="key"></param>
		/// <param name="session"></param>
		/// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
		Task InsertRowsAsync(IPersistentCollection collection, object key, ISessionImplementor session, CancellationToken cancellationToken);
	}
}
