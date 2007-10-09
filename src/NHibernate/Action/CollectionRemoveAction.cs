using System;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Persister.Collection;

namespace NHibernate.Action
{
	[Serializable]
	public sealed class CollectionRemoveAction : CollectionAction 
	{
		private readonly bool emptySnapshot;

		public CollectionRemoveAction(IPersistentCollection collection, ICollectionPersister persister,
			 object key, bool emptySnapshot, ISessionImplementor session)
			: base(persister, collection, key, session)
		{
			this.emptySnapshot = emptySnapshot;
		}

		public override void Execute()
		{
			if (!emptySnapshot)
				Persister.Remove(Key, Session);

			if (Collection != null)
			{
				Session.PersistenceContext.GetCollectionEntry(Collection).AfterAction(Collection);
			}

			Evict();

			if (Session.Factory.Statistics.IsStatisticsEnabled)
			{
				Session.Factory.StatisticsImplementor.RemoveCollection(Persister.Role);
			}
		}

		public override int CompareTo(CollectionAction other)
		{
			return 0;
		}
	}
}
