using System;
using System.Diagnostics;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Event;
using NHibernate.Persister.Collection;

namespace NHibernate.Action
{
	[Serializable]
	public sealed partial class CollectionRemoveAction : CollectionAction
	{
		private readonly bool emptySnapshot;
		private readonly object affectedOwner;

		/// <summary> 
		/// Removes a persistent collection from its loaded owner. 
		/// </summary>
		/// <param name="collection">The collection to to remove; must be non-null </param>
		/// <param name="persister"> The collection's persister </param>
		/// <param name="id">The collection key </param>
		/// <param name="emptySnapshot">Indicates if the snapshot is empty </param>
		/// <param name="session">The session </param>
		/// <remarks>Use this constructor when the collection is non-null.</remarks>
		public CollectionRemoveAction(IPersistentCollection collection, ICollectionPersister persister, object id,
		                              bool emptySnapshot, ISessionImplementor session)
			: base(persister, collection, id, session)
		{
			if (collection == null)
			{
				throw new AssertionFailure("collection == null");
			}

			this.emptySnapshot = emptySnapshot;
			affectedOwner = session.PersistenceContext.GetLoadedCollectionOwnerOrNull(collection);
		}

		/// <summary> 
		/// Removes a persistent collection from a specified owner. 
		/// </summary>
		/// <param name="affectedOwner">The collection's owner; must be non-null </param>
		/// <param name="persister"> The collection's persister </param>
		/// <param name="id">The collection key </param>
		/// <param name="emptySnapshot">Indicates if the snapshot is empty </param>
		/// <param name="session">The session </param>
		/// <remarks> Use this constructor when the collection to be removed has not been loaded. </remarks>
		public CollectionRemoveAction(object affectedOwner, ICollectionPersister persister, object id, bool emptySnapshot,
		                              ISessionImplementor session) : base(persister, null, id, session)
		{
			if (affectedOwner == null)
			{
				throw new AssertionFailure("affectedOwner == null");
			}
			this.emptySnapshot = emptySnapshot;
			this.affectedOwner = affectedOwner;
		}

		public override void Execute()
		{
			bool statsEnabled = Session.Factory.Statistics.IsStatisticsEnabled;
			Stopwatch stopwatch = null;
			if (statsEnabled)
			{
				stopwatch = Stopwatch.StartNew();
			}

			PreRemove();

			if (!emptySnapshot)
			{
				Persister.Remove(Key, Session);
			}

			IPersistentCollection collection = Collection;
			if (collection != null)
			{
				Session.PersistenceContext.GetCollectionEntry(collection).AfterAction(collection);
			}

			Evict();

			PostRemove();

			if (statsEnabled)
			{
				stopwatch.Stop();
				Session.Factory.StatisticsImplementor.RemoveCollection(Persister.Role, stopwatch.Elapsed);
			}
		}

		private void PreRemove()
		{
			IPreCollectionRemoveEventListener[] preListeners = Session.Listeners.PreCollectionRemoveEventListeners;
			if (preListeners.Length > 0)
			{
				PreCollectionRemoveEvent preEvent = new PreCollectionRemoveEvent(Persister, Collection, (IEventSource) Session,
				                                                                 affectedOwner);
				for (int i = 0; i < preListeners.Length; i++)
				{
					preListeners[i].OnPreRemoveCollection(preEvent);
				}
			}
		}

		private void PostRemove()
		{
			IPostCollectionRemoveEventListener[] postListeners = Session.Listeners.PostCollectionRemoveEventListeners;
			if (postListeners.Length > 0)
			{
				PostCollectionRemoveEvent postEvent = new PostCollectionRemoveEvent(Persister, Collection, (IEventSource) Session,
				                                                                    affectedOwner);
				for (int i = 0; i < postListeners.Length; i++)
				{
					postListeners[i].OnPostRemoveCollection(postEvent);
				}
			}
		}

		public override int CompareTo(CollectionAction other)
		{
			return 0;
		}
	}
}