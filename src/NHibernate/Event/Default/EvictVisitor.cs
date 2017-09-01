
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Impl;
using NHibernate.Type;

namespace NHibernate.Event.Default
{
	/// <summary> 
	/// Evict any collections referenced by the object from the session cache.
	/// This will NOT pick up any collections that were dereferenced, so they
	/// will be deleted (suboptimal but not exactly incorrect). 
	/// </summary>
	public partial class EvictVisitor : AbstractVisitor
	{
		private static readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof(EvictVisitor));

		public EvictVisitor(IEventSource session) : base(session) { }

		internal override object ProcessCollection(object collection, CollectionType type)
		{

			if (collection != null)
				EvictCollection(collection, type);

			return null;
		}

		public virtual void EvictCollection(object value, CollectionType type)
		{
			IPersistentCollection pc;
			if (type.IsArrayType)
			{
				pc = Session.PersistenceContext.RemoveCollectionHolder(value);
			}
			else if (value is IPersistentCollection)
			{
				pc = (IPersistentCollection)value;
			}
			else
			{
				return; //EARLY EXIT!
			}

			IPersistentCollection collection = pc;
			if (collection.UnsetSession(Session))
				EvictCollection(collection);
		}

		private void EvictCollection(IPersistentCollection collection)
		{
			CollectionEntry ce = (CollectionEntry)Session.PersistenceContext.CollectionEntries[collection];
			Session.PersistenceContext.CollectionEntries.Remove(collection);
			if (log.IsDebugEnabled)
				log.Debug("evicting collection: " + MessageHelper.CollectionInfoString(ce.LoadedPersister, collection, ce.LoadedKey, Session));
			if (ce.LoadedPersister != null && ce.LoadedKey != null)
			{
				//TODO: is this 100% correct?
				Session.PersistenceContext.CollectionsByKey.Remove(new CollectionKey(ce.LoadedPersister, ce.LoadedKey));
			}
		}
	}
}
