using NHibernate.Collection;
using NHibernate.Engine;

namespace NHibernate.Impl
{
	/// <summary>
	/// A scheduled removal of the Collection from the database.
	/// </summary>
	/// <remarks>
	/// This Collection is not represented in the database anymore.
	/// </remarks>
	internal sealed class ScheduledCollectionRemove : ScheduledCollectionAction
	{
		private bool _emptySnapshot;

		/// <summary>
		/// Initializes a new instance of <see cref="ScheduledCollectionRemove"/>.
		/// </summary>
		/// <param name="persister">The <see cref="CollectionPersister"/> that is responsible for the persisting the Collection.</param>
		/// <param name="id">The identifier of the Collection owner.</param>
		/// <param name="emptySnapshot">Indicates if the Collection was empty when it was loaded.</param>
		/// <param name="session">The <see cref="ISessionImplementor"/> that the Action is occuring in.</param>
		public ScheduledCollectionRemove( CollectionPersister persister, object id, bool emptySnapshot, ISessionImplementor session )
			: base( persister, id, session )
		{
			_emptySnapshot = emptySnapshot;
		}

		/// <summary></summary>
		public override void Execute()
		{
			Persister.Softlock( Id );

			// if there were no entries in the snapshot of the collection then there
			// is nothing to remove so verify that the snapshot was not empty.
			if( !_emptySnapshot )
			{
				Persister.Remove( Id, Session );
			}
		}
	}
}