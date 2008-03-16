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

		private static bool CollectionIsInitialized(object collection)
		{
			return !(collection is IPersistentCollection) || ((IPersistentCollection) collection).WasInitialized;
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
							string entityName = type.IsEntityType ? ((EntityType)type).GetAssociatedEntityName() : null;
							action.Cascade(eventSource, child, entityName, anything, false);
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
					log.Debug("processing cascades for: " + persister.EntityName);
				}
				IType[] types = persister.PropertyTypes;
				CascadeStyle[] cascadeStyles = persister.PropertyCascadeStyles;
				for (int i = 0; i < types.Length; i++)
				{
					CascadeStyle style = cascadeStyles[i];
					if (style.DoCascade(action))
					{
						Cascade(eventSource, persister.GetPropertyValue(parent, i, eventSource.EntityMode), types[i], action, style,
						        cascadeTo, anything);
					}
				}
				if (log.IsDebugEnabled)
				{
					log.Debug("done processing cascades for: " + persister.EntityName);
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
			IEnumerable iter = action.GetCascadableChildrenIterator(eventSource, collectionType, child);
			foreach (object obj in iter)
			{
				Cascade(eventSource, obj, elemType, action, style, cascadeVia, anything);
			}

			// handle oprhaned entities!!
			if (style.HasOrphanDelete && action.DeleteOrphans && child is IPersistentCollection)
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
	}
}