using NHibernate.Persister.Entity;

namespace NHibernate.Event
{
	/// <summary>
	/// Represents a <tt>pre-delete</tt> event, which occurs just prior to
	/// performing the deletion of an entity from the database.
	/// </summary>
	public class PreDeleteEvent : AbstractPreDatabaseOperationEvent
	{
		/// <summary> 
		/// Constructs an event containing the pertinent information. 
		/// </summary>
		/// <param name="entity">The entity to be deleted. </param>
		/// <param name="id">The id to use in the deletion. </param>
		/// <param name="deletedState">The entity's state at deletion time. </param>
		/// <param name="persister">The entity's persister. </param>
		/// <param name="source">The session from which the event originated. </param>
		public PreDeleteEvent(object entity, object id, object[] deletedState, IEntityPersister persister, IEventSource source)
			: base(source, entity, id, persister)
		{
			DeletedState = deletedState;
		}

		/// <summary> 
		/// This is the entity state at the
		/// time of deletion (useful for optomistic locking and such). 
		/// </summary>
		public object[] DeletedState { get; private set; }
	}
}