using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate.Collection;
using NHibernate.Event;
using NHibernate.Persister.Entity;
using NHibernate.Proxy;
using NHibernate.Type;

namespace NHibernate.Engine
{
	/// <summary>
	/// A session action that may be cascaded from parent entity to its children
	/// </summary>
	public abstract partial class CascadingAction
	{
		private static readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof(CascadingAction));

		#region The CascadingAction contract

		/// <summary> Cascade the action to the child object. </summary>
		/// <param name="session">The session within which the cascade is occurring. </param>
		/// <param name="child">The child to which cascading should be performed. </param>
		/// <param name="entityName">The child's entity name </param>
		/// <param name="anything">Typically some form of cascade-local cache which is specific to each CascadingAction type </param>
		/// <param name="isCascadeDeleteEnabled">Are cascading deletes enabled. </param>
		public abstract void Cascade(IEventSource session, object child, string entityName, object anything, bool isCascadeDeleteEnabled);

		/// <summary> 
		/// Given a collection, get an iterator of the children upon which the
		/// current cascading action should be visited. 
		/// </summary>
		/// <param name="session">The session within which the cascade is occurring. </param>
		/// <param name="collectionType">The mapping type of the collection. </param>
		/// <param name="collection">The collection instance. </param>
		/// <returns> The children iterator. </returns>
		public abstract IEnumerable GetCascadableChildrenIterator(IEventSource session, CollectionType collectionType, object collection);

		/// <summary> Does this action potentially extrapolate to orphan deletes? </summary>
		/// <returns> True if this action can lead to deletions of orphans. </returns>
		public abstract bool DeleteOrphans { get;}


		/// <summary> Does the specified cascading action require verification of no cascade validity? </summary>
		/// <returns> True if this action requires no-cascade verification; false otherwise. </returns>
		public virtual bool RequiresNoCascadeChecking
		{
			get { return false; }
		}

		/// <summary> 
		/// Called (in the case of <see cref="RequiresNoCascadeChecking"/> returning true) to validate
		/// that no cascade on the given property is considered a valid semantic. 
		/// </summary>
		/// <param name="session">The session within which the cascade is occurring. </param>
		/// <param name="child">The property value </param>
		/// <param name="parent">The property value owner </param>
		/// <param name="persister">The entity persister for the owner </param>
		/// <param name="propertyIndex">The index of the property within the owner. </param>
		public virtual void NoCascade(IEventSource session, object child, object parent, IEntityPersister persister, int propertyIndex)
		{
		}

		/// <summary> Should this action be performed (or noCascade consulted) in the case of lazy properties.</summary>
		public virtual bool PerformOnLazyProperty
		{
			get { return true; }
		}

		#endregion

		#region Static helper methods

		/// <summary> 
		/// Given a collection, get an iterator of all its children, loading them
		/// from the database if necessary. 
		/// </summary>
		/// <param name="session">The session within which the cascade is occurring. </param>
		/// <param name="collectionType">The mapping type of the collection. </param>
		/// <param name="collection">The collection instance. </param>
		/// <returns> The children iterator. </returns>
		private static IEnumerable GetAllElementsIterator(IEventSource session, CollectionType collectionType, object collection)
		{
			return collectionType.GetElementsIterator(collection, session);
		}

		/// <summary> 
		/// Iterate just the elements of the collection that are already there. Don't load
		/// any new elements from the database.
		/// </summary>
		public static IEnumerable GetLoadedElementsIterator(ISessionImplementor session, CollectionType collectionType, object collection)
		{
			if (CollectionIsInitialized(collection))
			{
				// handles arrays and newly instantiated collections
				return collectionType.GetElementsIterator(collection, session);
			}
			else
			{
				// does not handle arrays (that's ok, cos they can't be lazy)
				// or newly instantiated collections, so we can do the cast
				return ((IPersistentCollection)collection).QueuedAdditionIterator;
			}
		}

		private static bool CollectionIsInitialized(object collection)
		{
			IPersistentCollection pc = collection as IPersistentCollection;
			return pc == null || pc.WasInitialized;
		}

		#endregion

		#region The CascadingAction implementations

		/// <seealso cref="ISession.Delete(object)"/>
		public static readonly CascadingAction Delete = new DeleteCascadingAction();

		/// <seealso cref="ISession.Lock(object, LockMode)"/>
		public static readonly CascadingAction Lock = new LockCascadingAction();

		/// <seealso cref="ISession.Refresh(object)"/>
		public static readonly CascadingAction Refresh= new RefreshCascadingAction();

		/// <seealso cref="ISession.Evict(object)"/>
		public static readonly CascadingAction Evict= new EvictCascadingAction();

		/// <seealso cref="ISession.SaveOrUpdate(object)"/>
		public static readonly CascadingAction SaveUpdate= new SaveUpdateCascadingAction();

		/// <seealso cref="ISession.Merge(object)"/>
		public static readonly CascadingAction Merge= new MergeCascadingAction();
        
		/// <seealso cref="ISession.Persist(object)"/>
		public static readonly CascadingAction Persist= new PersistCascadingAction();

		/// <summary> Execute persist during flush time </summary>
		/// <seealso cref="ISession.Persist(object)"/>
		public static readonly CascadingAction PersistOnFlush= new PersistOnFlushCascadingAction();

		/// <seealso cref="ISession.Replicate(object, ReplicationMode)"/>
		public static readonly CascadingAction Replicate= new ReplicateCascadingAction();

		#endregion

		private partial class DeleteCascadingAction : CascadingAction
		{
			public override void Cascade(IEventSource session, object child, string entityName, object anything, bool isCascadeDeleteEnabled)
			{
				if (log.IsDebugEnabled)
				{
					log.Debug("cascading to delete: " + entityName);
				}
				session.Delete(entityName, child, isCascadeDeleteEnabled, (ISet<object>)anything);
			}

			public override IEnumerable GetCascadableChildrenIterator(IEventSource session, CollectionType collectionType, object collection)
			{
				// delete does cascade to uninitialized collections
				return GetAllElementsIterator(session, collectionType, collection);
			}
			public override bool DeleteOrphans
			{
				// orphans should be deleted during delete
				get { return true; }
			}
		}

		private partial class LockCascadingAction : CascadingAction
		{
			public override void Cascade(IEventSource session, object child, string entityName, object anything, bool isCascadeDeleteEnabled)
			{
				if (log.IsDebugEnabled)
				{
					log.Debug("cascading to lock: " + entityName);
				}
				session.Lock(entityName, child, LockMode.None);
			}
			public override IEnumerable GetCascadableChildrenIterator(IEventSource session, CollectionType collectionType, object collection)
			{
				// lock doesn't cascade to uninitialized collections
				return GetLoadedElementsIterator(session, collectionType, collection);
			}
			public override bool DeleteOrphans
			{
				//TODO: should orphans really be deleted during lock???
				get { return false; }
			}
		}

		private partial class RefreshCascadingAction : CascadingAction
		{
			public override void Cascade(IEventSource session, object child, string entityName, object anything, bool isCascadeDeleteEnabled)
			{
				if (log.IsDebugEnabled)
				{
					log.Debug("cascading to refresh: " + entityName);
				}
				session.Refresh(child, (IDictionary)anything);
			}
			public override IEnumerable GetCascadableChildrenIterator(IEventSource session, CollectionType collectionType, object collection)
			{
				// refresh doesn't cascade to uninitialized collections
				return GetLoadedElementsIterator(session, collectionType, collection);
			}
			public override bool DeleteOrphans
			{
				get { return false; }
			}
		}

		private partial class EvictCascadingAction : CascadingAction
		{
			public override void Cascade(IEventSource session, object child, string entityName, object anything, bool isCascadeDeleteEnabled)
			{
				if (log.IsDebugEnabled)
				{
					log.Debug("cascading to evict: " + entityName);
				}
				session.Evict(child);
			}
			public override IEnumerable GetCascadableChildrenIterator(IEventSource session, CollectionType collectionType, object collection)
			{
				// evicts don't cascade to uninitialized collections
				return GetLoadedElementsIterator(session, collectionType, collection);
			}
			public override bool DeleteOrphans
			{
				get { return false; }
			}
			public override bool PerformOnLazyProperty
			{
				get { return false; }
			}
		}

		private partial class SaveUpdateCascadingAction : CascadingAction
		{
			public override void Cascade(IEventSource session, object child, string entityName, object anything, bool isCascadeDeleteEnabled)
			{
				if (log.IsDebugEnabled)
				{
					log.Debug("cascading to saveOrUpdate: " + entityName);
				}
				session.SaveOrUpdate(entityName, child);
			}
			public override IEnumerable GetCascadableChildrenIterator(IEventSource session, CollectionType collectionType, object collection)
			{
				// saves / updates don't cascade to uninitialized collections
				return GetLoadedElementsIterator(session, collectionType, collection);
			}
			public override bool DeleteOrphans
			{
				// orphans should be deleted during save/update
				get { return true; }
			}
			public override bool PerformOnLazyProperty
			{
				get { return false; }
			}
		}

		private partial class MergeCascadingAction : CascadingAction
		{
			public override void Cascade(IEventSource session, object child, string entityName, object anything, bool isCascadeDeleteEnabled)
			{
				if (log.IsDebugEnabled)
				{
					log.Debug("cascading to merge: " + entityName);
				}
				session.Merge(entityName, child, (IDictionary)anything);
			}
			public override IEnumerable GetCascadableChildrenIterator(IEventSource session, CollectionType collectionType, object collection)
			{
				// merges don't cascade to uninitialized collections
				//TODO: perhaps this does need to cascade after all....
				return GetLoadedElementsIterator(session, collectionType, collection);
			}
			public override bool DeleteOrphans
			{
				// orphans should not be deleted during merge??
				get { return false; }
			}
		}
        
		private partial class PersistCascadingAction : CascadingAction
		{
			public override void Cascade(IEventSource session, object child, string entityName, object anything, bool isCascadeDeleteEnabled)
			{
				if (log.IsDebugEnabled)
				{
					log.Debug("cascading to persist: " + entityName);
				}
				session.Persist(entityName, child, (IDictionary)anything);
			}
			public override IEnumerable GetCascadableChildrenIterator(IEventSource session, CollectionType collectionType, object collection)
			{
				// persists don't cascade to uninitialized collections
				return GetLoadedElementsIterator(session, collectionType, collection);
			}
			public override bool DeleteOrphans
			{
				get { return false; }
			}
			public override bool PerformOnLazyProperty
			{
				get { return false; }
			}
		}

		private partial class PersistOnFlushCascadingAction : CascadingAction
		{
			public override void Cascade(IEventSource session, object child, string entityName, object anything, bool isCascadeDeleteEnabled)
			{
				if (log.IsDebugEnabled)
				{
					log.Debug("cascading to persistOnFlush: " + entityName);
				}
				session.PersistOnFlush(entityName, child, (IDictionary)anything);
			}
			public override IEnumerable GetCascadableChildrenIterator(IEventSource session, CollectionType collectionType, object collection)
			{
				// persists don't cascade to uninitialized collections
				return GetLoadedElementsIterator(session, collectionType, collection);
			}
			public override bool DeleteOrphans
			{
				get { return true; }
			}
			public override bool RequiresNoCascadeChecking
			{
				get { return true; }
			}

			public override void NoCascade(IEventSource session, object child, object parent, IEntityPersister persister, int propertyIndex)
			{
				if (child == null)
				{
					return;
				}
				IType type = persister.PropertyTypes[propertyIndex];
				if (type.IsEntityType)
				{
					string childEntityName = ((EntityType)type).GetAssociatedEntityName(session.Factory);

					if (!IsInManagedState(child, session) && !child.IsProxy() && ForeignKeys.IsTransientSlow(childEntityName, child, session))
					{
						string parentEntiytName = persister.EntityName;
						string propertyName = persister.PropertyNames[propertyIndex];
						throw new TransientObjectException(
							string.Format(
								"object references an unsaved transient instance - save the transient instance before flushing or set cascade action for the property to something that would make it autosave: {0}.{1} -> {2}",
								parentEntiytName, propertyName, childEntityName));
					}
				}
			}
			public override bool PerformOnLazyProperty
			{
				get { return false; }
			}

			private bool IsInManagedState(object child, IEventSource session)
			{
				EntityEntry entry = session.PersistenceContext.GetEntry(child);
				return entry != null && (entry.Status == Status.Loaded || entry.Status == Status.ReadOnly);
			}
		}

		private partial class ReplicateCascadingAction : CascadingAction
		{
			public override void Cascade(IEventSource session, object child, string entityName, object anything, bool isCascadeDeleteEnabled)
			{
				if (log.IsDebugEnabled)
				{
					log.Debug("cascading to replicate: " + entityName);
				}
				session.Replicate(entityName, child, (ReplicationMode)anything);
			}
			public override IEnumerable GetCascadableChildrenIterator(IEventSource session, CollectionType collectionType, object collection)
			{
				// replicate does cascade to uninitialized collections
				return GetLoadedElementsIterator(session, collectionType, collection);
			}
			public override bool DeleteOrphans
			{
				get { return false; }
			}
		}
	}
}
