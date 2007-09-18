using System;
using System.Collections;
using log4net;
using NHibernate.Collection;
using NHibernate.Event;
using NHibernate.Impl;
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
		CascadeOnCopy = 0
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
			/// <summary></summary>
			protected CascadingAction()
			{
			}

			// todo-events change ISessionImplementor with IEventSource
			/// <summary>
			/// Cascade the action to the child object
			/// </summary>
			public abstract void Cascade(ISessionImplementor session, object child, object anything);

			/// <summary>
			/// The children to whom we should cascade.
			/// </summary>
			public abstract ICollection CascadableChildrenCollection(CollectionType collectionType, object collection);

			/// <summary>
			/// Do we need to handle orphan delete for this action?
			/// </summary>
			public abstract bool DeleteOrphans();

			/// <summary></summary>
			public static CascadingAction ActionDelete = new ActionDeleteClass();

			private class ActionDeleteClass : CascadingAction
			{
				public override void Cascade(ISessionImplementor session, object child, object anything)
				{
					log.Debug("cascading to delete()");
					if (session.IsSaved(child))
					{
						session.Delete(child);
					}
				}

				public override ICollection CascadableChildrenCollection(CollectionType collectionType, object collection)
				{
					return GetAllElementsCollection(collectionType, collection);
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
				public override void Cascade(ISessionImplementor session, object child, object anything)
				{
					log.Debug("cascading to lock()");
					session.Lock(child, (LockMode) anything);
				}

				public override ICollection CascadableChildrenCollection(CollectionType collectionType, object collection)
				{
					return GetLoadedElementsCollection(collectionType, collection);
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
				public override void Cascade(ISessionImplementor session, object child, object anything)
				{
					log.Debug("cascading to evict()");
					session.Evict(child);
				}

				public override ICollection CascadableChildrenCollection(CollectionType collectionType, object collection)
				{
					return GetLoadedElementsCollection(collectionType, collection);
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
				public override void Cascade(ISessionImplementor session, object child, object anything)
				{
					log.Debug("cascading to SaveOrUpdate()");
					session.SaveOrUpdate(child);
				}

				public override ICollection CascadableChildrenCollection(CollectionType collectionType, object collection)
				{
					return GetLoadedElementsCollection(collectionType, collection);
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
				public override void Cascade(ISessionImplementor session, object child, object anything)
				{
					log.Debug("cascading to Copy()");
					session.Copy(child, (IDictionary) anything);
				}

				public override ICollection CascadableChildrenCollection(CollectionType collectionType, object collection)
				{
					// saves / updates don't cascade to uninitialized collections
					return GetLoadedElementsCollection(collectionType, collection);
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
				public override void Cascade(ISessionImplementor session, object child, object anything)
				{
					log.Debug("cascading to Replicate()");
					session.Replicate(child, (ReplicationMode) anything);
				}

				public override ICollection CascadableChildrenCollection(CollectionType collectionType, object collection)
				{
					return GetLoadedElementsCollection(collectionType, collection);
				}

				public override bool DeleteOrphans()
				{
					return false; // I suppose?
				}
			}

			public static CascadingAction ActionPersist = new ActionPersistClass();
			private class ActionPersistClass : CascadingAction
			{
				public override void Cascade(ISessionImplementor session, object child, object anything)
				{
					if (log.IsDebugEnabled)
					{
						//log.Debug("cascading to persist: " + entityName);
						log.Debug("cascading to persist");
					}
					IEventSource source = (IEventSource) session;
					source.Persist(null, child, (IDictionary)anything); // todo-events session.persist(entityName, child, (System.Collections.IDictionary) anything);
				}

				public override ICollection CascadableChildrenCollection(CollectionType collectionType, object collection)
				{
					return GetLoadedElementsCollection(collectionType, collection);
				}

				public override bool DeleteOrphans()
				{
					return false;
				}
			}

			public static CascadingAction ActionPersistOnFlush = new ActionPersistOnFlushClass();
			private class ActionPersistOnFlushClass : CascadingAction
			{
				public override void Cascade(ISessionImplementor session, object child, object anything)
				{
					if (log.IsDebugEnabled)
					{
						//log.Debug("cascading to persistOnFlush: " + entityName);
						log.Debug("cascading to persistOnFlush");
					}
					IEventSource source = (IEventSource)session;
					source.PersistOnFlush(null, child, (IDictionary)anything); // todo-events session.persistOnFlush(entityName, child, (System.Collections.IDictionary) anything);
				}

				public override ICollection CascadableChildrenCollection(CollectionType collectionType, object collection)
				{
					return GetLoadedElementsCollection(collectionType, collection);
				}

				public override bool DeleteOrphans()
				{
					return true;
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
			/// <summary></summary>
			protected CascadeStyle()
			{
			}

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
		/// A strategy for determining if an identifier value is an identifier of a new 
		/// transient instance or a previously persistent transient instance. The strategy
		/// is determined by the <c>Unsaved-Value</c> attribute in the mapping file.
		/// </summary>
		public class IdentifierValue
		{
			private readonly object value;

			/// <summary></summary>
			protected IdentifierValue()
			{
				this.value = null;
			}

			/// <summary>
			/// Assume the transient instance is newly instantiated if its identifier is null or
			/// equal to <c>Value</c>
			/// </summary>
			/// <param name="value"></param>
			public IdentifierValue(object value)
			{
				this.value = value;
			}

			/// <summary>
			/// Does the given identifier belong to a new instance
			/// </summary>
			public virtual bool IsUnsaved(object id)
			{
				if (log.IsDebugEnabled)
				{
					log.Debug("unsaved-value: " + value);
				}
				return id == null || id.Equals(value);
			}

			/// <summary>
			/// Always assume the transient instance is newly instantiated
			/// </summary>
			public static IdentifierValue SaveAny = new SaveAnyClass();

			private class SaveAnyClass : IdentifierValue
			{
				public override bool IsUnsaved(object id)
				{
					log.Debug("unsaved-value strategy ANY");
					return true;
				}
			}

			/// <summary>
			/// Never assume that transient instance is newly instantiated
			/// </summary>
			public static IdentifierValue SaveNone = new SaveNoneClass();

			private class SaveNoneClass : IdentifierValue
			{
				public override bool IsUnsaved(object id)
				{
					log.Debug("unsaved-value strategy NONE");
					return false;
				}
			}

			/// <summary>
			/// Assume the transient instance is newly instantiated if the identifier
			/// is null.
			/// </summary>
			public static IdentifierValue SaveNull = new SaveNullClass();

			private class SaveNullClass : IdentifierValue
			{
				public override bool IsUnsaved(object id)
				{
					log.Debug("unsaved-value strategy NULL");
					return id == null;
				}
			}
		}


		/// <summary>
		/// A strategy for determining if a version value is an version of
		/// a new transient instance or a previously persistent transient instance.
		/// The strategy is determined by the <c>Unsaved-Value</c> attribute in the mapping file.
		/// </summary>
		public class VersionValue
		{
			private readonly object value;

			/// <summary></summary>
			protected VersionValue()
			{
				this.value = null;
			}

			/// <summary>
			/// Assume the transient instance is newly instantiated if its version is null or
			/// equal to <c>Value</c>
			/// </summary>
			/// <param name="value"></param>
			public VersionValue(object value)
			{
				this.value = value;
			}

			/// <summary>
			/// Does the given identifier belong to a new instance
			/// </summary>
			public virtual object IsUnsaved(object version)
			{
				if (log.IsDebugEnabled)
				{
					log.Debug("unsaved-value: " + value);
				}
				return version == null || version.Equals(value);
			}

			/// <summary>
			/// Assume the transient instance is newly instantiated if the version
			/// is null, otherwise assume it is a detached instance.
			/// </summary>
			public static VersionValue VersionSaveNull = new VersionSaveNullClass();

			private class VersionSaveNullClass : VersionValue
			{
				public override object IsUnsaved(object version)
				{
					log.Debug("version unsaved-value strategy NULL");
					return version == null;
				}
			}

			/// <summary>
			/// Assume the transient instance is newly instantiated if the version
			/// is null, otherwise defer to the identifier unsaved-value.
			/// </summary>
			public static VersionValue VersionUndefined = new VersionUndefinedClass();

			private class VersionUndefinedClass : VersionValue
			{
				public override object IsUnsaved(object version)
				{
					log.Debug("version unsaved-value strategy UNDEFINED");
					//return version == null ? true : null;
					if (version == null)
					{
						return true;
					}
					else
					{
						return null;
					}
				}
			}

			/// <summary>
			/// Assume the transient instance is newly instantiated if the identifier
			/// is null.
			/// </summary>
			public static VersionValue VersionNegative = new VersionNegativeClass();

			private class VersionNegativeClass : VersionValue
			{
				public override object IsUnsaved(object version)
				{
					log.Debug("version unsaved-value strategy NEGATIVE");
					if (version is short || version is int || version is long)
					{
						return Convert.ToInt64(version) < 0L;
					}
					else
					{
						throw new MappingException("unsaved-value strategy NEGATIVE may only be used with short, int and long types");
					}
				}
			}
		}

		/// <summary>
		/// Cascade an action to the child or children
		/// </summary>
		/// <param name="session"></param>
		/// <param name="child"></param>
		/// <param name="type"></param>
		/// <param name="action"></param>
		/// <param name="style"></param>
		/// <param name="cascadeTo"></param>
		/// <param name="anything"></param>
		private static void Cascade(
			ISessionImplementor session,
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
							action.Cascade(session, child, anything);
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
							ICollectionPersister persister = session.Factory.GetCollectionPersister(pctype.Role);
							IType elemType = persister.ElementType;

							// cascade to current collection elements
							if (elemType.IsEntityType || elemType.IsAnyType || elemType.IsComponentType)
							{
								CascadeCollection(action, style, pctype, elemType, child, cascadeVia, session, anything);
							}
						}
					}
				}
				else if (type.IsComponentType)
				{
					IAbstractComponentType ctype = ((IAbstractComponentType) type);
					object[] children = ctype.GetPropertyValues(child, session);
					IType[] types = ctype.Subtypes;
					for (int i = 0; i < types.Length; i++)
					{
						CascadeStyle componentPropertyStyle = ctype.GetCascadeStyle(i);
						if (componentPropertyStyle.DoCascade(action))
						{
							Cascade(session, children[i], types[i], action, componentPropertyStyle, cascadeTo, anything);
						}
					}
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="session"></param>
		/// <param name="persister"></param>
		/// <param name="parent"></param>
		/// <param name="action"></param>
		/// <param name="cascadeTo"></param>
		public static void Cascade(ISessionImplementor session, IEntityPersister persister, object parent,
		                           CascadingAction action, CascadePoint cascadeTo)
		{
			Cascade(session, persister, parent, action, cascadeTo, null);
		}

		/// <summary>
		/// Cascade an action from the parent object to all its children.
		/// </summary>
		/// <param name="session"></param>
		/// <param name="persister"></param>
		/// <param name="parent"></param>
		/// <param name="action"></param>
		/// <param name="cascadeTo"></param>
		/// <param name="anything"></param>
		public static void Cascade(ISessionImplementor session, IEntityPersister persister, object parent,
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
						Cascade(session, persister.GetPropertyValue(parent, i), types[i], action, style, cascadeTo, anything);
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
		/// <param name="session"></param>
		/// <param name="anything"></param>
		private static void CascadeCollection(
			CascadingAction action,
			CascadeStyle style,
			CollectionType collectionType,
			IType elemType,
			object child,
			CascadePoint cascadeVia,
			ISessionImplementor session,
			object anything)
		{
			// cascade to current collection elements
			if (log.IsDebugEnabled)
			{
				log.Debug("cascading to collection: " + collectionType.Role);
			}
			ICollection iter = action.CascadableChildrenCollection(collectionType, child);
			foreach (object obj in iter)
			{
				Cascade(session, obj, elemType, action, style, cascadeVia, anything);
			}

			// handle oprhaned entities!!
			if (style.HasOrphanDelete && action.DeleteOrphans() && child is IPersistentCollection)
			{
				// We can do the cast since orphan-delete does not apply to:
				// 1. newly instatiated collections
				// 2. arrays ( we can't track orphans for detached arrays)
				System.Type entityName = collectionType.GetAssociatedClass(session.Factory);
				DeleteOrphans(entityName, child as IPersistentCollection, session);
			}
		}

		private static void DeleteOrphans(System.Type entityName, IPersistentCollection pc, ISessionImplementor session)
		{
			ICollection orphans;
			if (pc.WasInitialized) // can't be any orphans if it was not initialized
			{
				CollectionEntry ce = session.GetCollectionEntry(pc);
				orphans = ce == null ? CollectionHelper.EmptyCollection :
				          ce.GetOrphans(entityName, pc);
			}
			else
			{
				orphans = CollectionHelper.EmptyCollection;
			}

			foreach (object orphan in orphans)
			{
				if (orphan != null)
				{
					session.Delete(orphan);
				}
			}
		}

		public static ICollection GetLoadedElementsCollection(CollectionType collectionType, object collection)
		{
			if (CollectionIsInitialized(collection))
			{
				// handles arrays and newly instantiated collections
				return collectionType.GetElementsCollection(collection);
			}
			else
			{
				// does not handle arrays (that's ok, cos they can't be lazy)
				// or newly instantiated collections so we can do the cast
				return ((IPersistentCollection) collection).QueuedAddsCollection;
			}
		}

		private static ICollection GetAllElementsCollection(CollectionType collectionType, object collection)
		{
			return collectionType.GetElementsCollection(collection);
		}
	}
}