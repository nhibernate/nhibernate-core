using System;
using NHibernate.Collection;
using NHibernate.Engine;

namespace NHibernate.Impl
{
	/// <summary>
	/// A scheduled recreation of the Collection in the database.
	/// </summary>
	[Serializable]
	internal sealed class ScheduledCollectionRecreate : ScheduledCollectionAction
	{
		private IPersistentCollection _collection;

		/// <summary>
		/// Initializes a new instance of <see cref="ScheduledCollectionRecreate"/>.
		/// </summary>
		/// <param name="collection">The <see cref="IPersistentCollection"/> to recreate.</param>
		/// <param name="persister">The <see cref="ICollectionPersister"/> that is responsible for the persisting the Collection.</param>
		/// <param name="id">The identifier of the Collection owner.</param>
		/// <param name="session">The <see cref="ISessionImplementor"/> that the Action is occuring in.</param>
		public ScheduledCollectionRecreate( IPersistentCollection collection, ICollectionPersister persister, object id, ISessionImplementor session )
			: base( persister, id, session )
		{
			_collection = collection;
		}

		/// <summary></summary>
		public override void Execute()
		{
			Persister.Recreate( _collection, Id, Session );
			Evict();
		}
	}
}