using NHibernate.Collection;
using NHibernate.Engine;

namespace NHibernate.Impl
{
	/// <summary>
	/// A scheduled recreation of the Collection in the database.
	/// </summary>
	internal sealed class ScheduledCollectionRecreate : ScheduledCollectionAction
	{
		private PersistentCollection _collection;

		/// <summary>
		/// Initializes a new instance of <see cref="ScheduledCollectionRecreate"/>.
		/// </summary>
		/// <param name="collection">The <see cref="PersistentCollection"/> to recreate.</param>
		/// <param name="persister">The <see cref="CollectionPersister"/> that is responsible for the persisting the Collection.</param>
		/// <param name="id">The identifier of the Collection owner.</param>
		/// <param name="session">The <see cref="ISessionImplementor"/> that the Action is occuring in.</param>
		public ScheduledCollectionRecreate( PersistentCollection collection, CollectionPersister persister, object id, ISessionImplementor session )
			: base( persister, id, session )
		{
			_collection = collection;
		}

		/// <summary></summary>
		public override void Execute()
		{
			Persister.Softlock( Id );
			Persister.Recreate( _collection, Id, Session );
		}
	}
}