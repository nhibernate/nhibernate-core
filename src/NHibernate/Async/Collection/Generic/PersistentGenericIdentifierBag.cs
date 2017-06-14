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
using System.Diagnostics;
using System.Linq;
using NHibernate.DebugHelpers;
using NHibernate.Engine;
using NHibernate.Id;
using NHibernate.Loader;
using NHibernate.Persister.Collection;
using NHibernate.Type;

namespace NHibernate.Collection.Generic
{
	using System.Threading.Tasks;
	using System.Threading;
	/// <content>
	/// Contains generated async methods
	/// </content>
	public partial class PersistentIdentifierBag<T> : AbstractPersistentCollection, IList<T>, IList
	{

		/// <summary>
		/// Initializes this Bag from the cached values.
		/// </summary>
		/// <param name="persister">The CollectionPersister to use to reassemble the PersistentIdentifierBag.</param>
		/// <param name="disassembled">The disassembled PersistentIdentifierBag.</param>
		/// <param name="owner">The owner object.</param>
		/// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
		public override async Task InitializeFromCacheAsync(ICollectionPersister persister, object disassembled, object owner, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			object[] array = (object[])disassembled;
			int size = array.Length;
			BeforeInitialize(persister, size);
			for (int i = 0; i < size; i += 2)
			{
				_identifiers[i / 2] = await (persister.IdentifierType.AssembleAsync(array[i], Session, owner, cancellationToken)).ConfigureAwait(false);
				_values.Add((T) await (persister.ElementType.AssembleAsync(array[i + 1], Session, owner, cancellationToken)).ConfigureAwait(false));
			}
		}

		public override async Task<object> ReadFromAsync(DbDataReader reader, ICollectionPersister persister, ICollectionAliases descriptor, object owner, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			object element = await (persister.ReadElementAsync(reader, owner, descriptor.SuffixedElementAliases, Session, cancellationToken)).ConfigureAwait(false);
			object id = await (persister.ReadIdentifierAsync(reader, descriptor.SuffixedIdentifierAlias, Session, cancellationToken)).ConfigureAwait(false);

			// eliminate duplication if loaded in a cartesian product
			if (!_identifiers.ContainsValue(id))
			{
				_identifiers[_values.Count] = id;
				_values.Add((T) element);
			}
			return element;
		}

		public override Task<ICollection> GetOrphansAsync(object snapshot, string entityName, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<ICollection>(cancellationToken);
			}
			try
			{
				var sn = (ISet<SnapshotElement>)GetSnapshot();
				return GetOrphansAsync(sn.Select(x => x.Value).ToArray(), (ICollection) _values, entityName, Session, cancellationToken);
			}
			catch (Exception ex)
			{
				return Task.FromException<ICollection>(ex);
			}
		}

		public override async Task PreInsertAsync(ICollectionPersister persister, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if ((persister.IdentifierGenerator as IPostInsertIdentifierGenerator) != null)
			{
				// NH Different behavior (NH specific) : if we are using IdentityGenerator the PreInsert have no effect
				return;
			}
			try
			{
				int i = 0;
				foreach (object entry in _values)
				{
					int loc = i++;
					if (!_identifiers.ContainsKey(loc)) // TODO: native ids
					{
						object id = await (persister.IdentifierGenerator.GenerateAsync(Session, entry, cancellationToken)).ConfigureAwait(false);
						_identifiers[loc] = id;
					}
				}
			}
			catch (Exception sqle)
			{
				throw new ADOException("Could not generate idbag row id.", sqle);
			}
		}
	}
}