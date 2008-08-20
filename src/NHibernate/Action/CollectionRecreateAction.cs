using System;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Event;
using NHibernate.Persister.Collection;

namespace NHibernate.Action
{
	[Serializable]
	public sealed class CollectionRecreateAction : CollectionAction
	{
		public CollectionRecreateAction(IPersistentCollection collection, ICollectionPersister persister, object key, ISessionImplementor session)
			: base(persister, collection, key, session) { }

		/// <summary> Execute this action</summary>
		public override void Execute()
		{
			IPersistentCollection collection = Collection;

			PreRecreate();

			Persister.Recreate(collection, Key, Session);

			Session.PersistenceContext.GetCollectionEntry(collection).AfterAction(collection);

			Evict();

			PostRecreate();

			if (Session.Factory.Statistics.IsStatisticsEnabled)
			{
			  Session.Factory.StatisticsImplementor.RecreateCollection(Persister.Role);
			}
		}

		private void PreRecreate()
		{
			IPreCollectionRecreateEventListener[] preListeners = Session.Listeners.PreCollectionRecreateEventListeners;
			if (preListeners.Length > 0)
			{
				PreCollectionRecreateEvent preEvent = new PreCollectionRecreateEvent(Persister, Collection, (IEventSource)Session);
				for (int i = 0; i < preListeners.Length; i++)
				{
					preListeners[i].OnPreRecreateCollection(preEvent);
				}
			}
		}

		private void PostRecreate()
		{
			IPostCollectionRecreateEventListener[] postListeners = Session.Listeners.PostCollectionRecreateEventListeners;
			if (postListeners.Length > 0)
			{
				PostCollectionRecreateEvent postEvent = new PostCollectionRecreateEvent(Persister, Collection, (IEventSource)Session);
				for (int i = 0; i < postListeners.Length; i++)
				{
					postListeners[i].OnPostRecreateCollection(postEvent);
				}
			}
		}
	}
}