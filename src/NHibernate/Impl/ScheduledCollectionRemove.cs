using System;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Persister.Collection;

namespace NHibernate.Impl
{
	/// <summary>
	/// A scheduled removal of the Collection from the database.
	/// </summary>
	/// <remarks>
	/// This Collection is not represented in the database anymore.
	/// </remarks>
	[Serializable]
	internal sealed class ScheduledCollectionRemove : ScheduledCollectionAction
	{
		private bool _emptySnapshot;
		private IPersistentCollection _collection;

		/// <summary>
		/// Initializes a new instance of <see cref="ScheduledCollectionRemove"/>.
		/// </summary>
		/// <param name="collection">The <see cref="IPersistentCollection" /> that is being removed.</param>
		/// <param name="persister">The <see cref="ICollectionPersister"/> that is responsible for the persisting the Collection.</param>
		/// <param name="id">The identifier of the Collection owner.</param>
		/// <param name="emptySnapshot">Indicates if the Collection was empty when it was loaded.</param>
		/// <param name="session">The <see cref="ISessionImplementor"/> that the Action is occuring in.</param>
		public ScheduledCollectionRemove(
			IPersistentCollection collection,
			ICollectionPersister persister,
			object id,
			bool emptySnapshot,
			ISessionImplementor session)
			: base(persister, id, session)
		{
			_emptySnapshot = emptySnapshot;
			_collection = collection;
		}

		/// <summary></summary>
		public override void Execute()
		{
			// if there were no entries in the snapshot of the collection then there
			// is nothing to remove so verify that the snapshot was not empty.
			if (!_emptySnapshot)
			{
				Persister.Remove(Id, Session);
			}

			if (_collection != null)
			{
				Session.GetCollectionEntry(_collection).AfterAction(_collection);
			}

			Evict();
		}
	}
}