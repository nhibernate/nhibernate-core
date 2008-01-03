using System.Collections;
using log4net;
using NHibernate.Collection;
using NHibernate.Event;
using NHibernate.Persister.Collection;
using NHibernate.Persister.Entity;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Engine
{
	/// <summary>
	/// The types of children to cascade to
	/// </summary>
	public enum CascadePoint
	{
		/// <summary>
		/// A cascade point that occurs just after the insertion of the parent
		/// entity and just before deletion
		/// </summary>
		CascadeAfterInsertBeforeDelete = 1,

		/// <summary>
		/// A cascade point that occurs just before the insertion of the parent entity
		/// and just after deletion
		/// </summary>
		CascadeBeforeInsertAfterDelete = 2,

		/// <summary>
		/// A cascade point that occurs just after the insertion of the parent entity
		/// and just before deletion, inside a collection
		/// </summary>
		CascadeAfterInsertBeforeDeleteViaCollection = 3,

		/// <summary> A cascade point that occurs just before the session is flushed</summary>
		CascadeBeforeFlush = 0,

		/// <summary>
		/// A cascade point that occurs just after the update of the parent entity
		/// </summary>
		CascadeOnUpdate = 0,

		/// <summary>
		/// A cascade point that occurs just after eviction of the parent entity from the
		/// session cache
		/// </summary>
		CascadeOnEvict = 0, //-1

		/// <summary>
		/// A cascade point that occurs just after locking a transient parent entity into the session cache
		/// </summary>
		CascadeOnLock = 0,

		/// <summary>
		/// A cascade point that occurs just after copying from a transient parent entity into the object in the session cache
		/// </summary>
		CascadeOnCopy = 0,

		/// <summary> 
		/// A cascade point that occurs just after locking a transient parent entity into the
		/// session cache
		/// </summary>
		CascadeBeforeRefresh = 0
	}

	/// <summary>
	/// Summary description for Cascades.
	/// </summary>
	public sealed class Cascades
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(Cascades));

		private Cascades()
		{
			// should not be initialized	
		}

		/// <summary>
		/// A session action that may be cascaded from parent entity to its children
		/// </summary>
		public abstract class CascadingAction
		{
			/// <summary>
			/// Cascade the action to the child object
			/// </summary>
			public abstract void Cascade(IEventSource eventSource, object child, object anything);

			/// <summary>
			/// The children to whom we should cascade.
			/// </summary>
			public abstract IEnumerable CascadableChildrenIterator(CollectionType collectionType, object collection);

			/// <summary>
			/// Do we need to handle orphan delete for this action?
			/// </summary>
			public abstract bool DeleteOrphans();

			/// <summary></summary>
			public static CascadingAction ActionDelete = new ActionDeleteClass();

			private class ActionDeleteClass : CascadingAction
			{
				public override void Cascade(IEventSource eventSource, object child, object anything)
				{
					log.Debug("cascading to delete()");
					if (eventSource.IsSaved(child))
					{
						eventSource.Delete(child);
					}
				}

				public override IEnumerable CascadableChildrenIterator(CollectionType collectionType, object collection)
				{
					return GetAllElementsIterator(collectionType, collection);
				}

				public override bool DeleteOrphans()
				{
					return true;
				}
			}

			/// <summary></summary>
			public static CascadingAction ActionLock = new ActionLockClass();

			private class ActionLockClass : CascadingAction
			{
				public override void Cascade(IEventSource eventSource, object child, object anything)
				{
					log.Debug("cascading to lock()");
					eventSource.Lock(child, (LockMode) anything);
				}

				public override IEnumerable CascadableChildrenIterator(CollectionType collectionType, object collection)
				{
					return GetLoadedElementsIterator(collectionType, collection);
				}

				public override bool DeleteOrphans()
				{
					return false;
				}
			}

			/// <summary></summary>
			public static CascadingAction ActionEvict = new ActionEvictClass();

			private class ActionEvictClass : CascadingAction
			{
				public override void Cascade(IEventSource eventSource, object child, object anything)
				{
					log.Debug("cascading to evict()");
					eventSource.Evict(child);
				}

				public override IEnumerable CascadableChildrenIterator(CollectionType collectionType, object collection)
				{
					return GetLoadedElementsIterator(collectionType, collection);
				}

				public override bool DeleteOrphans()
				{
					return false;
				}
			}

			/// <summary></summary>
			public static CascadingAction ActionSaveUpdate = new ActionSaveUpdateClass();

			private class ActionSaveUpdateClass : CascadingAction
			{
				public override void Cascade(IEventSource eventSource, object child, object anything)
				{
					log.Debug("cascading to SaveOrUpdate()");
					eventSource.SaveOrUpdate(child);
				}

				public override IEnumerable CascadableChildrenIterator(CollectionType collectionType, object collection)
				{
					return GetLoadedElementsIterator(collectionType, collection);
				}

				public override bool DeleteOrphans()
				{
					return true;
				}
			}

			/// <summary></summary>
			public static CascadingAction ActionCopy = new ActionCopyClass();

			private class ActionCopyClass : CascadingAction
			{
				public override void Cascade(IEventSource eventSource, object child, object anything)
				{
					log.Debug("cascading to Copy()");
					eventSource.Copy(child, (IDictionary)anything);
				}

				public override IEnumerable CascadableChildrenIterator(CollectionType collectionType, object collection)
				{
					// saves / updates don't cascade to uninitialized collections
					return GetLoadedElementsIterator(collectionType, collection);
				}

				public override bool DeleteOrphans()
				{
					// orphans should not be deleted during copy???
					return false;
				}
			}

			/// <summary></summary>
			public static CascadingAction ActionReplicate = new ActionReplicateClass();

			private class ActionReplicateClass : CascadingAction
			{
				public override void Cascade(IEventSource eventSource, object child, object anything)
				{
					log.Debug("cascading to Replicate()");
					eventSource.Replicate(child, (ReplicationMode)anything);
				}

				public override IEnumerable CascadableChildrenIterator(CollectionType collectionType, object collection)
				{
					return GetLoadedElementsIterator(collectionType, collection);
				}

				public override bool DeleteOrphans()
				{
					return false; // I suppose?
				}
			}

			public static CascadingAction ActionPersist = new ActionPersistClass();
			private class ActionPersistClass : CascadingAction
			{
				public override void Cascade(IEventSource eventSource, object child, object anything)
				{
					if (log.IsDebugEnabled)
					{
						//log.Debug("cascading to persist: " + entityName);
						log.Debug("cascading to persist");
					}
					eventSource.Persist(null, child, (IDictionary)anything); // todo-events session.persist(entityName, child, (System.Collections.IDictionary) anything);
				}

				public override IEnumerable CascadableChildrenIterator(CollectionType collectionType, object collection)
				{
					return GetLoadedElementsIterator(collectionType, collection);
				}

				public override bool DeleteOrphans()
				{
					return false;
				}
			}

			public static CascadingAction ActionPersistOnFlush = new ActionPersistOnFlushClass();
			private class ActionPersistOnFlushClass : CascadingAction
			{
				public override void Cascade(IEventSource eventSource, object child, object anything)
				{
					if (log.IsDebugEnabled)
					{
						//log.Debug("cascading to persistOnFlush: " + entityName);
						log.Debug("cascading to persistOnFlush");
					}
					eventSource.PersistOnFlush(null, child, (IDictionary)anything); // todo-events session.persistOnFlush(entityName, child, (System.Collections.IDictionary) anything);
				}

				public override IEnumerable CascadableChildrenIterator(CollectionType collectionType, object collection)
				{
					return GetLoadedElementsIterator(collectionType, collection);
				}

				public override bool DeleteOrphans()
				{
					return true;
				}
			}

			public static CascadingAction ActionRefresh = new ActionRefreshClass();
			private class ActionRefreshClass : CascadingAction
			{
				public override void Cascade(IEventSource eventSource, object child, object anything)
				{
					if (log.IsDebugEnabled)
					{
						//log.Debug("cascading to refresh: " + entityName);
						log.Debug("cascading to refresh");
					}
					eventSource.Refresh(child, (IDictionary)anything);
				}

				public override IEnumerable CascadableChildrenIterator(CollectionType collectionType, object collection)
				{
					return GetLoadedElementsIterator(collectionType, collection);
				}

				public override bool DeleteOrphans()
				{
					return false;
				}
			}

			public static CascadingAction ActionSaveUpdateCopy = new ActionSaveUpdateCopyClass();
			private class ActionSaveUpdateCopyClass : CascadingAction
			{
				public override void Cascade(IEventSource eventSource, object child, object anything)
				{
					if (log.IsDebugEnabled)
					{
						//log.Debug("cascading to saveOrUpdateCopy: " + entityName);
						log.Debug("cascading to saveOrUpdateCopy");
					}
					eventSource.SaveOrUpdateCopy(null, child, (IDictionary)anything); //todo-events source.SaveOrUpdateCopy(entityName, child, (System.Collections.IDictionary)anything);
				}

				public override IEnumerable CascadableChildrenIterator(CollectionType collectionType, object collection)
				{
					return GetLoadedElementsIterator(collectionType, collection);
				}

				public override bool DeleteOrphans()
				{
					// orphans should not be deleted during copy??
					return false;
				}
			}
		}

		private static bool CollectionIsInitialized(object collection)
		{
			return !(collection is IPersistentCollection) || ((IPersistentCollection) collection).WasInitialized;
		}

		/// <summary></summary>
		public abstract class CascadeStyle
		{
			/// <summary>
			/// Should the given action be cascaded?
			/// </summary>
			/// <param name="action"></param>
			/// <returns></returns>
			public abstract bool DoCascade(CascadingAction action);

			/// <summary>
			/// Do we delete orphans automatically?
			/// </summary>
			public virtual bool HasOrphanDelete
			{
				get { return false; }
			}

			/// <summary>
			/// Save / Delete / Update / Evict / Lock / Replicate + delete orphans
			/// </summary>
			public static CascadeStyle StyleAllDeleteOrphan = new StyleAllDeleteOrphanClass();

			private class StyleAllDeleteOrphanClass : CascadeStyle
			{
				public override bool DoCascade(CascadingAction action)
				{
					return true;
				}

				public override bool HasOrphanDelete
				{
					get { return true; }
				}
			}

			/// <summary>
			/// Save / Delete / Update / Evict / Lock / Replicate
			/// </summary>
			public static CascadeStyle StyleAll = new StyleAllClass();


			private class StyleAllClass : CascadeStyle
			{
				public override bool DoCascade(CascadingAction action)
				{
					return true;
				}
			}

			/// <summary>
			/// Save / Update / Lock / Replicate
			/// </summary>
			public static CascadeStyle StyleSaveUpdate = new StyleSaveUpdateClass();

			private class StyleSaveUpdateClass : CascadeStyle
			{
				public override bool DoCascade(CascadingAction action)
				{
					return action == CascadingAction.ActionSaveUpdate ||
					       action == CascadingAction.ActionLock ||
					       action == CascadingAction.ActionReplicate ||
					       action == CascadingAction.ActionCopy;
				}
			}

			/// <summary>
			/// Delete
			/// </summary>
			public static CascadeStyle StyleOnlyDelete = new StyleOnlyDeleteClass();

			private class StyleOnlyDeleteClass : CascadeStyle
			{
				public override bool DoCascade(CascadingAction action)
				{
					return action == CascadingAction.ActionDelete;
				}
			}


			/// <summary>
			/// Delete + delete orphans
			/// </summary>
			public static CascadeStyle StyleDeleteOrphan = new StyleDeleteOrphanClass();

			private class StyleDeleteOrphanClass : CascadeStyle
			{
				public override bool DoCascade(CascadingAction action)
				{
					return action == CascadingAction.ActionDelete;
				}

				public override bool HasOrphanDelete
				{
					get { return true; }
				}
			}

			/// <summary>
			/// No Cascades
			/// </summary>
			public static CascadeStyle StyleNone = new StyleNoneClass();

			private class StyleNoneClass : CascadeStyle
			{
				public override bool DoCascade(CascadingAction action)
				{
					return action == CascadingAction.ActionReplicate;
				}
			}
		}

		/// <summary>
		/// Cascade an action to the child or children
		/// </summary>
		/// <param name="eventSource"></param>
		/// <param name="child"></param>
		/// <param name="type"></param>
		/// <param name="action"></param>
		/// <param name="style"></param>
		/// <param name="cascadeTo"></param>
		/// <param name="anything"></param>
		private static void Cascade(
			IEventSource eventSource,
			object child,
			IType type,
			CascadingAction action,
			CascadeStyle style,
			CascadePoint cascadeTo,
			object anything)
		{
			if (child != null)
			{
				if (type.IsAssociationType)
				{
					if (((IAssociationType) type).ForeignKeyDirection.CascadeNow(cascadeTo))
					{
						if (type.IsEntityType || type.IsAnyType)
						{
							action.Cascade(eventSource, child, anything);
						}
						else if (type.IsCollectionType)
						{
							CascadePoint cascadeVia;
							if (cascadeTo == CascadePoint.CascadeAfterInsertBeforeDelete)
							{
								cascadeVia = CascadePoint.CascadeAfterInsertBeforeDeleteViaCollection;
							}
							else
							{
								cascadeVia = cascadeTo;
							}
							CollectionType pctype = (CollectionType) type;
							ICollectionPersister persister = eventSource.Factory.GetCollectionPersister(pctype.Role);
							IType elemType = persister.ElementType;

							// cascade to current collection elements
							if (elemType.IsEntityType || elemType.IsAnyType || elemType.IsComponentType)
							{
								CascadeCollection(action, style, pctype, elemType, child, cascadeVia, eventSource, anything);
							}
						}
					}
				}
				else if (type.IsComponentType)
				{
					IAbstractComponentType ctype = ((IAbstractComponentType) type);
					object[] children = ctype.GetPropertyValues(child, eventSource);
					IType[] types = ctype.Subtypes;
					for (int i = 0; i < types.Length; i++)
					{
						CascadeStyle componentPropertyStyle = ctype.GetCascadeStyle(i);
						if (componentPropertyStyle.DoCascade(action))
						{
							Cascade(eventSource, children[i], types[i], action, componentPropertyStyle, cascadeTo, anything);
						}
					}
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="eventSource"></param>
		/// <param name="persister"></param>
		/// <param name="parent"></param>
		/// <param name="action"></param>
		/// <param name="cascadeTo"></param>
		public static void Cascade(IEventSource eventSource, IEntityPersister persister, object parent,
		                           CascadingAction action, CascadePoint cascadeTo)
		{
			Cascade(eventSource, persister, parent, action, cascadeTo, null);
		}

		/// <summary>
		/// Cascade an action from the parent object to all its children.
		/// </summary>
		/// <param name="eventSource"></param>
		/// <param name="persister"></param>
		/// <param name="parent"></param>
		/// <param name="action"></param>
		/// <param name="cascadeTo"></param>
		/// <param name="anything"></param>
		public static void Cascade(IEventSource eventSource, IEntityPersister persister, object parent,
		                           CascadingAction action, CascadePoint cascadeTo, object anything)
		{
			if (persister.HasCascades)
			{
				if (log.IsDebugEnabled)
				{
					log.Debug("processing cascades for: " + persister.ClassName);
				}
				IType[] types = persister.PropertyTypes;
				CascadeStyle[] cascadeStyles = persister.PropertyCascadeStyles;
				for (int i = 0; i < types.Length; i++)
				{
					CascadeStyle style = cascadeStyles[i];
					if (style.DoCascade(action))
					{
						Cascade(eventSource, persister.GetPropertyValue(parent, i), types[i], action, style, cascadeTo, anything);
					}
				}
				if (log.IsDebugEnabled)
				{
					log.Debug("done processing cascades for: " + persister.ClassName);
				}
			}
		}

		/// <summary>
		/// Cascade to the collection elements
		/// </summary>
		/// <param name="action"></param>
		/// <param name="style"></param>
		/// <param name="collectionType"></param>
		/// <param name="elemType"></param>
		/// <param name="child"></param>
		/// <param name="cascadeVia"></param>
		/// <param name="eventSource"></param>
		/// <param name="anything"></param>
		private static void CascadeCollection(
			CascadingAction action,
			CascadeStyle style,
			CollectionType collectionType,
			IType elemType,
			object child,
			CascadePoint cascadeVia,
			IEventSource eventSource,
			object anything)
		{
			// cascade to current collection elements
			if (log.IsDebugEnabled)
			{
				log.Debug("cascading to collection: " + collectionType.Role);
			}
			IEnumerable iter = action.CascadableChildrenIterator(collectionType, child);
			foreach (object obj in iter)
			{
				Cascade(eventSource, obj, elemType, action, style, cascadeVia, anything);
			}

			// handle oprhaned entities!!
			if (style.HasOrphanDelete && action.DeleteOrphans() && child is IPersistentCollection)
			{
				// We can do the cast since orphan-delete does not apply to:
				// 1. newly instatiated collections
				// 2. arrays ( we can't track orphans for detached arrays)
				string entityName = collectionType.GetAssociatedEntityName(eventSource.Factory);
				DeleteOrphans(entityName, (IPersistentCollection)child, eventSource);

				if (log.IsInfoEnabled)
				{
					log.Info("done deleting orphans for collection: " + collectionType.Role);
				}
			}
		}

		private static void DeleteOrphans(string entityName, IPersistentCollection pc, IEventSource eventSource)
		{
			//TODO: suck this logic into the collection!
			ICollection orphans;
			if (pc.WasInitialized)
			{
				CollectionEntry ce = eventSource.PersistenceContext.GetCollectionEntry(pc);
				orphans = ce == null ? CollectionHelper.EmptyCollection : ce.GetOrphans(entityName, pc);
			}
			else
			{
				orphans = CollectionHelper.EmptyCollection; // TODO NH: Different pc.GetQueuedOrphans(entityName);
			}
			foreach (object orphan in orphans)
			{
				if (orphan != null)
				{
					if (log.IsInfoEnabled)
						log.Info("deleting orphaned entity instance: " + entityName);

					eventSource.Delete(orphan);
				}
			}
		}

		public static IEnumerable GetLoadedElementsIterator(CollectionType collectionType, object collection)
		{
			if (CollectionIsInitialized(collection))
			{
				// handles arrays and newly instantiated collections
				return collectionType.GetElementsIterator(collection);
			}
			else
			{
				// does not handle arrays (that's ok, cos they can't be lazy)
				// or newly instantiated collections so we can do the cast
				return ((IPersistentCollection) collection).QueuedAdditionIterator;
			}
		}

		private static IEnumerable GetAllElementsIterator(CollectionType collectionType, object collection)
		{
			return collectionType.GetElementsIterator(collection);
		}
	}
}