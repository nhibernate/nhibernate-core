using NHibernate.Persister.Entity;

namespace NHibernate.Event
{
	/// <summary> 
	/// Represents a <tt>pre-update</tt> event, which occurs just prior to
	/// performing the update of an entity in the database.
	/// </summary>
	public class PreUpdateEvent : AbstractPreDatabaseOperationEvent
	{
		public PreUpdateEvent(object entity, object id, object[] state, object[] oldState, IEntityPersister persister,
		                      IEventSource source) : base(source, entity, id, persister)
		{
			State = state;
			OldState = oldState;
		}

		/// <summary>
		/// Retrieves the state to be used in the update.
		/// </summary>
		public object[] State { get; private set; }

		/// <summary>
		/// The old state of the entity at the time it was last loaded from the
		/// database; can be null in the case of detached entities.
		/// </summary>
		public object[] OldState { get; private set; }
	}
}