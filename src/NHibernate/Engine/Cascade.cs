using System.Collections;

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
		AfterInsertBeforeDelete = 1,

		/// <summary>
		/// A cascade point that occurs just before the insertion of the parent entity
		/// and just after deletion
		/// </summary>
		BeforeInsertAfterDelete = 2,

		/// <summary>
		/// A cascade point that occurs just after the insertion of the parent entity
		/// and just before deletion, inside a collection
		/// </summary>
		AfterInsertBeforeDeleteViaCollection = 3,

		/// <summary>
		/// A cascade point that occurs just after the update of the parent entity
		/// </summary>
		AfterUpdate = 0,

		/// <summary> A cascade point that occurs just before the session is flushed</summary>
		BeforeFlush = 0,

		/// <summary>
		/// A cascade point that occurs just after eviction of the parent entity from the
		/// session cache
		/// </summary>
		AfterEvict = 0,

		/// <summary> 
		/// A cascade point that occurs just after locking a transient parent entity into the
		/// session cache
		/// </summary>
		BeforeRefresh = 0,

		/// <summary>
		/// A cascade point that occurs just after locking a transient parent entity into the session cache
		/// </summary>
		AfterLock = 0,

		/// <summary>
		/// A cascade point that occurs just before merging from a transient parent entity into
		/// the object in the session cache
		/// </summary>
		BeforeMerge = 0,
	}

	/// <summary> 
	/// Delegate responsible, in conjunction with the various
	/// <see cref="CascadingAction"/>, for implementing cascade processing. 
	/// </summary>
	public sealed partial class Cascade
	{
		private static readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof(Cascade));

		private CascadePoint point;
		private readonly IEventSource eventSource;
		private readonly CascadingAction action;

		private readonly Stack componentPathStack = new Stack();

		public Cascade(CascadingAction action, CascadePoint point, IEventSource eventSource)
		{
			this.point = point;
			this.eventSource = eventSource;
			this.action = action;
		}


		/// <summary> Cascade an action from the parent entity instance to all its children. </summary>
		/// <param name="persister">The parent's entity persister </param>
		/// <param name="parent">The parent reference. </param>
		public void CascadeOn(IEntityPersister persister, object parent)
		{
			CascadeOn(persister, parent, null);
		}

		/// <summary> 
		/// Cascade an action from the parent entity instance to all its children.  This
		/// form is typically called from within cascade actions. 
		/// </summary>
		/// <param name="persister">The parent's entity persister </param>
		/// <param name="parent">The parent reference. </param>
		/// <param name="anything">
		/// Typically some form of cascade-local cache
		/// which is specific to each CascadingAction type
		/// </param>
		public void CascadeOn(IEntityPersister persister, object parent, object anything)
		{
			if (persister.HasCascades || action.RequiresNoCascadeChecking)
			{
				log.Info("processing cascade " + action + " for: " + persister.EntityName);

				IType[] types = persister.PropertyTypes;
				CascadeStyle[] cascadeStyles = persister.PropertyCascadeStyles;
				bool hasUninitializedLazyProperties = persister.HasUninitializedLazyProperties(parent);
				for (int i = 0; i < types.Length; i++)
				{
					CascadeStyle style = cascadeStyles[i];
					string propertyName = persister.PropertyNames[i];
					if (hasUninitializedLazyProperties && persister.PropertyLaziness[i] && !action.PerformOnLazyProperty)
					{
						//do nothing to avoid a lazy property initialization
						continue;
					}

					if (style.DoCascade(action))
					{
						CascadeProperty(parent, persister.GetPropertyValue(parent, i), types[i], style, propertyName, anything, false);
					}
					else if (action.RequiresNoCascadeChecking)
					{
						action.NoCascade(eventSource, persister.GetPropertyValue(parent, i), parent, persister, i);
					}
				}

				log.Info("done processing cascade " + action + " for: " + persister.EntityName);
			}
		}

		/// <summary> Cascade an action to the child or children</summary>
		private void CascadeProperty(object parent, object child, IType type, CascadeStyle style, string propertyName, object anything, bool isCascadeDeleteEnabled)
		{
			if (child != null)
			{
				if (type.IsAssociationType)
				{
					IAssociationType associationType = (IAssociationType)type;
					if (CascadeAssociationNow(associationType))
					{
						CascadeAssociation(parent, child, type, style, anything, isCascadeDeleteEnabled);
					}
				}
				else if (type.IsComponentType)
				{
					CascadeComponent(parent, child, (IAbstractComponentType)type, propertyName, anything);
				}
			}
			else
			{
				// potentially we need to handle orphan deletes for one-to-ones here...
				if (type.IsEntityType && ((EntityType)type).IsLogicalOneToOne())
				{
					// We have a physical or logical one-to-one and from previous checks we know we
					// have a null value.  See if the attribute cascade settings and action-type require
					// orphan checking
					if (style.HasOrphanDelete && action.DeleteOrphans)
					{
						// value is orphaned if loaded state for this property shows not null
						// because it is currently null.
						EntityEntry entry = eventSource.PersistenceContext.GetEntry(parent);
						if (entry != null && entry.Status != Status.Saving)
						{
							EntityType entityType = (EntityType)type;
							object loadedValue;
							if (!componentPathStack.Any())
							{
								// association defined on entity
								loadedValue = entry.GetLoadedValue(propertyName);
							}
							else
							{
								// association defined on component
								// todo : this is currently unsupported because of the fact that
								// we do not know the loaded state of this value properly
								// and doing so would be very difficult given how components and
								// entities are loaded (and how 'loaded state' is put into the
								// EntityEntry).  Solutions here are to either:
								//	1) properly account for components as a 2-phase load construct
								//  2) just assume the association was just now orphaned and
								//     issue the orphan delete.  This would require a special
								//     set of SQL statements though since we do not know the
								//     orphaned value, something a delete with a subquery to
								//     match the owner.
								loadedValue = null;
							}

							if (loadedValue != null)
							{
								eventSource.Delete(entry.Persister.EntityName, loadedValue, false, null);
							}
						}
					}
				}
			}
		}

		private bool CascadeAssociationNow(IAssociationType associationType)
		{
			return associationType.ForeignKeyDirection.CascadeNow(point);
		}

		private void CascadeComponent(object parent, object child, IAbstractComponentType componentType, string componentPropertyName, object anything)
		{
			componentPathStack.Push(componentPropertyName);
			object[] children = componentType.GetPropertyValues(child, eventSource);
			IType[] types = componentType.Subtypes;
			for (int i = 0; i < types.Length; i++)
			{
				CascadeStyle componentPropertyStyle = componentType.GetCascadeStyle(i);
				string subPropertyName = componentType.PropertyNames[i];
				if (componentPropertyStyle.DoCascade(action))
				{
					CascadeProperty(parent, children[i], types[i], componentPropertyStyle, subPropertyName, anything, false);
				}
			}
			componentPathStack.Pop();
		}

		private void CascadeAssociation(object parent, object child, IType type, CascadeStyle style, object anything, bool isCascadeDeleteEnabled)
		{
			if (type.IsEntityType || type.IsAnyType)
			{
				CascadeToOne(parent, child, type, style, anything, isCascadeDeleteEnabled);
			}
			else if (type.IsCollectionType)
			{
				CascadeCollection(parent, child, style, anything, (CollectionType)type);
			}
		}

		/// <summary> Cascade an action to a collection</summary>
		private void CascadeCollection(object parent, object child, CascadeStyle style, object anything, CollectionType type)
		{
			ICollectionPersister persister = eventSource.Factory.GetCollectionPersister(type.Role);
			IType elemType = persister.ElementType;

			CascadePoint oldCascadeTo = point;
			if (point == CascadePoint.AfterInsertBeforeDelete)
				point = CascadePoint.AfterInsertBeforeDeleteViaCollection;

			//cascade to current collection elements
			if (elemType.IsEntityType || elemType.IsAnyType || elemType.IsComponentType)
				CascadeCollectionElements(parent, child, type, style, elemType, anything, persister.CascadeDeleteEnabled);

			point = oldCascadeTo;
		}

		/// <summary> Cascade an action to a to-one association or any type</summary>
		private void CascadeToOne(object parent, object child, IType type, CascadeStyle style, object anything, bool isCascadeDeleteEnabled)
		{
			string entityName = type.IsEntityType ? ((EntityType)type).GetAssociatedEntityName() : null;
			if (style.ReallyDoCascade(action))
			{
				//not really necessary, but good for consistency...
				eventSource.PersistenceContext.AddChildParent(child, parent);
				try
				{
					action.Cascade(eventSource, child, entityName, anything, isCascadeDeleteEnabled);
				}
				finally
				{
					eventSource.PersistenceContext.RemoveChildParent(child);
				}
			}
		}

		/// <summary> Cascade to the collection elements</summary>
		private void CascadeCollectionElements(object parent, object child, CollectionType collectionType, CascadeStyle style, IType elemType, object anything, bool isCascadeDeleteEnabled)
		{
			bool reallyDoCascade = style.ReallyDoCascade(action)
			                       && child != CollectionType.UnfetchedCollection;

			if (reallyDoCascade)
			{
				log.Info("cascade " + action + " for collection: " + collectionType.Role);

				foreach (object o in action.GetCascadableChildrenIterator(eventSource, collectionType, child))
					CascadeProperty(parent, o, elemType, style, null, anything, isCascadeDeleteEnabled);

				log.Info("done cascade " + action + " for collection: " + collectionType.Role);
			}

			var childAsPersColl = child as IPersistentCollection;
			bool deleteOrphans = style.HasOrphanDelete && action.DeleteOrphans && elemType.IsEntityType
			                     && childAsPersColl != null; //a newly instantiated collection can't have orphans

			if (deleteOrphans)
			{
				// handle orphaned entities!!
				log.Info("deleting orphans for collection: " + collectionType.Role);

				// we can do the cast since orphan-delete does not apply to:
				// 1. newly instantiated collections
				// 2. arrays (we can't track orphans for detached arrays)
				string entityName = collectionType.GetAssociatedEntityName(eventSource.Factory);
				DeleteOrphans(entityName, childAsPersColl);

				log.Info("done deleting orphans for collection: " + collectionType.Role);
			}
		}

		/// <summary> Delete any entities that were removed from the collection</summary>
		private void DeleteOrphans(string entityName, IPersistentCollection pc)
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
				orphans = pc.GetQueuedOrphans(entityName);
			}

			foreach (object orphan in orphans)
			{
				if (orphan != null)
				{
					log.Info("deleting orphaned entity instance: " + entityName);

					eventSource.Delete(entityName, orphan, false, null);
				}
			}
		}
	}
}