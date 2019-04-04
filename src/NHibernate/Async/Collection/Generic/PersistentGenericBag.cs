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
using System.Linq.Expressions;
using NHibernate.DebugHelpers;
using NHibernate.Engine;
using NHibernate.Linq;
using NHibernate.Loader;
using NHibernate.Persister.Collection;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Collection.Generic
{
	using System.Threading.Tasks;
	using System.Threading;
	public partial class PersistentGenericBag<T> : AbstractPersistentCollection, IList<T>, IList, IQueryable<T>
	{

		public override async Task<object> DisassembleAsync(ICollectionPersister persister, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			var length = _gbag.Count;
			var result = new object[length];

			for (var i = 0; i < length; i++)
			{
				result[i] = await (persister.ElementType.DisassembleAsync(_gbag[i], Session, null, cancellationToken)).ConfigureAwait(false);
			}

			return result;
		}

		public override Task<bool> EqualsSnapshotAsync(ICollectionPersister persister, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<bool>(cancellationToken);
			}
			try
			{
				return Task.FromResult<bool>(EqualsSnapshot(persister));
			}
			catch (Exception ex)
			{
				return Task.FromException<bool>(ex);
			}
		}

		public override Task<IEnumerable> GetDeletesAsync(ICollectionPersister persister, bool indexIsFormula, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<IEnumerable>(cancellationToken);
			}
			try
			{
				return Task.FromResult<IEnumerable>(GetDeletes(persister, indexIsFormula));
			}
			catch (Exception ex)
			{
				return Task.FromException<IEnumerable>(ex);
			}
		}

		public override Task<ICollection> GetOrphansAsync(object snapshot, string entityName, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<ICollection>(cancellationToken);
			}
			try
			{
				return Task.FromResult<ICollection>(GetOrphans(snapshot, entityName));
			}
			catch (Exception ex)
			{
				return Task.FromException<ICollection>(ex);
			}
		}

		/// <summary>
		/// Initializes this PersistentBag from the cached values.
		/// </summary>
		/// <param name="persister">The CollectionPersister to use to reassemble the PersistentBag.</param>
		/// <param name="disassembled">The disassembled PersistentBag.</param>
		/// <param name="owner">The owner object.</param>
		/// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
		public override async Task InitializeFromCacheAsync(ICollectionPersister persister, object disassembled, object owner, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			var array = (object[]) disassembled;
			var size = array.Length;
			BeforeInitialize(persister, size);
			for (var i = 0; i < size; i++)
			{
				var element = await (persister.ElementType.AssembleAsync(array[i], Session, owner, cancellationToken)).ConfigureAwait(false);
				if (element != null)
				{
					_gbag.Add((T) element);
				}
			}
		}

		public override Task<bool> NeedsInsertingAsync(object entry, int i, IType elemType, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<bool>(cancellationToken);
			}
			try
			{
				return Task.FromResult<bool>(NeedsInserting(entry, i, elemType));
			}
			catch (Exception ex)
			{
				return Task.FromException<bool>(ex);
			}
		}

		public override Task<bool> NeedsUpdatingAsync(object entry, int i, IType elemType, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<bool>(cancellationToken);
			}
			try
			{
				return Task.FromResult<bool>(NeedsUpdating(entry, i, elemType));
			}
			catch (Exception ex)
			{
				return Task.FromException<bool>(ex);
			}
		}

		public override async Task<object> ReadFromAsync(DbDataReader reader, ICollectionPersister role, ICollectionAliases descriptor, object owner, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			// note that if we load this collection from a cartesian product
			// the multiplicity would be broken ... so use an idbag instead
			var element = await (role.ReadElementAsync(reader, owner, descriptor.SuffixedElementAliases, Session, cancellationToken)).ConfigureAwait(false);
			// NH Different behavior : we don't check for null
			// The NH-750 test show how checking for null we are ignoring the not-found tag and
			// the DB may have some records ignored by NH. This issue may need some more deep consideration.
			//if (element != null)
			_gbag.Add((T) element);
			return element;
		}
	}
}
