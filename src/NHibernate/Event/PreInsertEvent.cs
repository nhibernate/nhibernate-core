using NHibernate.Persister.Entity;

namespace NHibernate.Event
{
	/// <summary> 
	/// Represents a <tt>pre-insert</tt> event, which occurs just prior to
	/// performing the insert of an entity into the database.
	/// </summary>
	public class PreInsertEvent : AbstractPreDatabaseOperationEvent
	{
		public PreInsertEvent(object entity, object id, object[] state, IEntityPersister persister, IEventSource source)
			: base(source, entity, id, persister)
		{
			State = state;
		}

		/// <summary> 
		/// These are the values to be inserted. 
		/// </summary>
		public object[] State { get; private set; }
	}
}
