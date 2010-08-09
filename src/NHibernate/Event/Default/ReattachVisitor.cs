
using NHibernate.Action;
using NHibernate.Impl;
using NHibernate.Persister.Collection;
using NHibernate.Type;

namespace NHibernate.Event.Default
{
	/// <summary>
	/// Abstract superclass of visitors that reattach collections
	/// </summary>
	public abstract class ReattachVisitor : ProxyVisitor
	{
		private readonly object ownerIdentifier;
		private readonly object owner;
		private static readonly ILogger log = LoggerProvider.LoggerFor(typeof(AbstractFlushingEventListener));

		protected ReattachVisitor(IEventSource session, object ownerIdentifier, object owner)
			: base(session)
		{
			this.ownerIdentifier = ownerIdentifier;
			this.owner = owner;
		}

		public object OwnerIdentifier
		{
			get { return ownerIdentifier; }
		}

		public object Owner
		{
			get { return owner; }
		}

		internal override object ProcessComponent(object component, IAbstractComponentType componentType)
		{
			IType[] types = componentType.Subtypes;
			if (component == null)
			{
				ProcessValues(new object[types.Length], types);
			}
			else
			{
				base.ProcessComponent(component, componentType);
			}

			return null;
		}

		/// <summary> 
		/// Schedules a collection for deletion. 
		/// </summary>
		/// <param name="role">The persister representing the collection to be removed. </param>
		/// <param name="collectionKey">The collection key (differs from owner-id in the case of property-refs). </param>
		/// <param name="source">The session from which the request originated. </param>
		internal void RemoveCollection(ICollectionPersister role, object collectionKey, IEventSource source)
		{
			if (log.IsDebugEnabled)
			{
				log.Debug("collection dereferenced while transient " + 
					MessageHelper.InfoString(role, ownerIdentifier, source.Factory));
			}
			source.ActionQueue.AddAction(new CollectionRemoveAction(owner, role, collectionKey, false, source));
		}

		/// <summary> 
		/// This version is slightly different in that here we need to assume that
		/// the owner is not yet associated with the session, and thus we cannot
		/// rely on the owner's EntityEntry snapshot... 
		/// </summary>
		/// <param name="role">The persister for the collection role being processed. </param>
		/// <returns> </returns>
		internal object ExtractCollectionKeyFromOwner(ICollectionPersister role)
		{
			if (role.CollectionType.UseLHSPrimaryKey)
			{
				return ownerIdentifier;
			}
			else
			{
				return role.OwnerEntityPersister.GetPropertyValue(owner, role.CollectionType.LHSPropertyName, Session.EntityMode);
			}
		}
	}
}