using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NHibernate.Cfg;
using NHibernate.Event;
using NHibernate.Persister.Entity;

namespace NHibernate.Test.NHSpecificTest.GH2508
{
	public partial class AuditEventListener : IPreCollectionUpdateEventListener
	{
		public void OnPreUpdateCollection(PreCollectionUpdateEvent @event)
		{
			var ownerEntity = @event.AffectedOwnerOrNull;
			var collectionEntry = @event.Session.PersistenceContext.GetCollectionEntry(@event.Collection);
			if (!collectionEntry.LoadedPersister.IsInverse)
				return;

			var abstractCollectionPersister = collectionEntry.LoadedPersister as Persister.Collection.AbstractCollectionPersister;
			if (abstractCollectionPersister == null)
				return;

			var ownerEntityPersister = abstractCollectionPersister.OwnerEntityPersister;
			ownerEntityPersister.GetPropertyValues(ownerEntity);
		}
	}
}
