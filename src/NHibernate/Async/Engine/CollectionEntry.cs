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
using NHibernate.Action;
using NHibernate.Collection;
using NHibernate.Impl;
using NHibernate.Persister.Collection;

namespace NHibernate.Engine
{
	using System.Threading.Tasks;
	using System.Threading;
	public partial class CollectionEntry
	{

		/// <summary> 
		/// Determine if the collection is "really" dirty, by checking dirtiness
		/// of the collection elements, if necessary
		/// </summary>
		private async Task DirtyAsync(IPersistentCollection collection, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			// if the collection is initialized and it was previously persistent
			// initialize the dirty flag
			bool forceDirty = collection.WasInitialized && !collection.IsDirty && LoadedPersister != null
							  && LoadedPersister.IsMutable
							  && (collection.IsDirectlyAccessible || LoadedPersister.ElementType.IsMutable)
							  && !await (collection.EqualsSnapshotAsync(LoadedPersister, cancellationToken)).ConfigureAwait(false);

			if (forceDirty)
			{
				collection.Dirty();
			}
		}

		/// <summary>
		/// Prepares this CollectionEntry for the Flush process.
		/// </summary>
		/// <param name="collection">The <see cref="IPersistentCollection"/> that this CollectionEntry will be responsible for flushing.</param>
		/// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
		public async Task PreFlushAsync(IPersistentCollection collection, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			bool nonMutableChange = collection.IsDirty && LoadedPersister != null && !LoadedPersister.IsMutable;
			if (nonMutableChange)
			{
				throw new HibernateException("changed an immutable collection instance: " + MessageHelper.InfoString(LoadedPersister.Role, LoadedKey));
			}
			await (DirtyAsync(collection, cancellationToken)).ConfigureAwait(false);

			if (log.IsDebugEnabled() && collection.IsDirty && loadedPersister != null)
			{
				log.Debug("Collection dirty: {0}", MessageHelper.CollectionInfoString(loadedPersister, loadedKey));
			}

			// reset all of these values so any previous flush status 
			// information is cleared from this CollectionEntry
			doupdate = false;
			doremove = false;
			dorecreate = false;
			reached = false;
			processed = false;
		}

		public async Task AfterActionAsync(IPersistentCollection collection, ISessionImplementor session, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			loadedKey = CurrentKey;
			if (loadedKey is DelayedPostInsertIdentifier && CurrentPersister != null)
			{
				// Resolve the actual key
				loadedKey = await (CurrentPersister.CollectionType.GetKeyOfOwnerAsync(collection.Owner, session, cancellationToken)).ConfigureAwait(false);
			}

			SetLoadedPersister(CurrentPersister);

			if (collection.WasInitialized && (IsDoremove || IsDorecreate || IsDoupdate))
			{
				//re-snapshot
				snapshot = loadedPersister == null || !loadedPersister.IsMutable ? null : collection.GetSnapshot(loadedPersister);
			}

			collection.PostAction();
		}

		public Task<ICollection> GetOrphansAsync(string entityName, IPersistentCollection collection, CancellationToken cancellationToken)
		{
			if (snapshot == null)
			{
				throw new AssertionFailure("no collection snapshot for orphan delete");
			}
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<ICollection>(cancellationToken);
			}
			return collection.GetOrphansAsync(snapshot, entityName, cancellationToken);
		}
	}
}
