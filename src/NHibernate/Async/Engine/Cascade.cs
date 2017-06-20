﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System.Collections;

using NHibernate.Collection;
using NHibernate.Event;
using NHibernate.Persister.Collection;
using NHibernate.Persister.Entity;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Engine
{
	using System.Threading.Tasks;
	using System.Threading;

	/// <content>
	/// Contains generated async methods
	/// </content>
	public sealed partial class Cascade
	{


		/// <summary> Cascade an action from the parent entity instance to all its children. </summary>
		/// <param name="persister">The parent's entity persister </param>
		/// <param name="parent">The parent reference. </param>
		/// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
		public Task CascadeOnAsync(IEntityPersister persister, object parent, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<object>(cancellationToken);
			}
			return CascadeOnAsync(persister, parent, null, cancellationToken);
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
		/// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
		public async Task CascadeOnAsync(IEntityPersister persister, object parent, object anything, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
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
						await (CascadePropertyAsync(parent, persister.GetPropertyValue(parent, i), types[i], style, propertyName, anything, false, cancellationToken)).ConfigureAwait(false);
					}
					else if (action.RequiresNoCascadeChecking)
					{
						await (action.NoCascadeAsync(eventSource, persister.GetPropertyValue(parent, i), parent, persister, i, cancellationToken)).ConfigureAwait(false);
					}
				}

				log.Info("done processing cascade " + action + " for: " + persister.EntityName);
			}
		}

		/// <summary> Cascade an action to the child or children</summary>
		private Task CascadePropertyAsync(object parent, object child, IType type, CascadeStyle style, string propertyName, object anything, bool isCascadeDeleteEnabled, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<object>(cancellationToken);
			}
			try
			{
				if (child != null)
				{
					if (type.IsAssociationType)
					{
						IAssociationType associationType = (IAssociationType)type;
						if (CascadeAssociationNow(associationType))
						{
							return CascadeAssociationAsync(parent, child, type, style, anything, isCascadeDeleteEnabled, cancellationToken);
						}
					}
					else if (type.IsComponentType)
					{
						return CascadeComponentAsync(parent, child, (IAbstractComponentType)type, propertyName, anything, cancellationToken);
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
									return eventSource.DeleteAsync(entry.Persister.EntityName, loadedValue, false, null, cancellationToken);
								}
							}
						}
					}
				}
				return Task.CompletedTask;
			}
			catch (System.Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		private async Task CascadeComponentAsync(object parent, object child, IAbstractComponentType componentType, string componentPropertyName, object anything, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			componentPathStack.Push(componentPropertyName);
			object[] children = await (componentType.GetPropertyValuesAsync(child, eventSource, cancellationToken)).ConfigureAwait(false);
			IType[] types = componentType.Subtypes;
			for (int i = 0; i < types.Length; i++)
			{
				CascadeStyle componentPropertyStyle = componentType.GetCascadeStyle(i);
				string subPropertyName = componentType.PropertyNames[i];
				if (componentPropertyStyle.DoCascade(action))
				{
					await (CascadePropertyAsync(parent, children[i], types[i], componentPropertyStyle, subPropertyName, anything, false, cancellationToken)).ConfigureAwait(false);
				}
			}
			componentPathStack.Pop();
		}

		private Task CascadeAssociationAsync(object parent, object child, IType type, CascadeStyle style, object anything, bool isCascadeDeleteEnabled, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<object>(cancellationToken);
			}
			try
			{
				if (type.IsEntityType || type.IsAnyType)
				{
					return CascadeToOneAsync(parent, child, type, style, anything, isCascadeDeleteEnabled, cancellationToken);
				}
				else if (type.IsCollectionType)
				{
					return CascadeCollectionAsync(parent, child, style, anything, (CollectionType)type, cancellationToken);
				}
				return Task.CompletedTask;
			}
			catch (System.Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		/// <summary> Cascade an action to a collection</summary>
		private async Task CascadeCollectionAsync(object parent, object child, CascadeStyle style, object anything, CollectionType type, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			ICollectionPersister persister = eventSource.Factory.GetCollectionPersister(type.Role);
			IType elemType = persister.ElementType;

			CascadePoint oldCascadeTo = point;
			if (point == CascadePoint.AfterInsertBeforeDelete)
				point = CascadePoint.AfterInsertBeforeDeleteViaCollection;

			//cascade to current collection elements
			if (elemType.IsEntityType || elemType.IsAnyType || elemType.IsComponentType)
				await (CascadeCollectionElementsAsync(parent, child, type, style, elemType, anything, persister.CascadeDeleteEnabled, cancellationToken)).ConfigureAwait(false);

			point = oldCascadeTo;
		}

		/// <summary> Cascade an action to a to-one association or any type</summary>
		private async Task CascadeToOneAsync(object parent, object child, IType type, CascadeStyle style, object anything, bool isCascadeDeleteEnabled, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			string entityName = type.IsEntityType ? ((EntityType)type).GetAssociatedEntityName() : null;
			if (style.ReallyDoCascade(action))
			{
				//not really necessary, but good for consistency...
				eventSource.PersistenceContext.AddChildParent(child, parent);
				try
				{
					await (action.CascadeAsync(eventSource, child, entityName, anything, isCascadeDeleteEnabled, cancellationToken)).ConfigureAwait(false);
				}
				finally
				{
					eventSource.PersistenceContext.RemoveChildParent(child);
				}
			}
		}

		/// <summary> Cascade to the collection elements</summary>
		private async Task CascadeCollectionElementsAsync(object parent, object child, CollectionType collectionType, CascadeStyle style, IType elemType, object anything, bool isCascadeDeleteEnabled, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			bool reallyDoCascade = style.ReallyDoCascade(action)
			                       && child != CollectionType.UnfetchedCollection;

			if (reallyDoCascade)
			{
				log.Info("cascade " + action + " for collection: " + collectionType.Role);

				foreach (object o in action.GetCascadableChildrenIterator(eventSource, collectionType, child))
					await (CascadePropertyAsync(parent, o, elemType, style, null, anything, isCascadeDeleteEnabled, cancellationToken)).ConfigureAwait(false);

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
				await (DeleteOrphansAsync(entityName, childAsPersColl, cancellationToken)).ConfigureAwait(false);

				log.Info("done deleting orphans for collection: " + collectionType.Role);
			}
		}

		/// <summary> Delete any entities that were removed from the collection</summary>
		private async Task DeleteOrphansAsync(string entityName, IPersistentCollection pc, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			//TODO: suck this logic into the collection!
			ICollection orphans;
			if (pc.WasInitialized)
			{
				CollectionEntry ce = eventSource.PersistenceContext.GetCollectionEntry(pc);
				orphans = ce == null ? CollectionHelper.EmptyCollection : await (ce.GetOrphansAsync(entityName, pc, cancellationToken)).ConfigureAwait(false);
			}
			else
			{
				orphans = await (pc.GetQueuedOrphansAsync(entityName, cancellationToken)).ConfigureAwait(false);
			}

			foreach (object orphan in orphans)
			{
				if (orphan != null)
				{
					log.Info("deleting orphaned entity instance: " + entityName);

					await (eventSource.DeleteAsync(entityName, orphan, false, null, cancellationToken)).ConfigureAwait(false);
				}
			}
		}
	}
}