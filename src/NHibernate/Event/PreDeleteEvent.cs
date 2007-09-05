using NHibernate.Persister.Entity;

namespace NHibernate.Event
{
	/// <summary>
	/// Occurs before deleting an item from the datastore
	/// </summary>
	public class PreDeleteEvent
	{
		private readonly object entity;
		private readonly object id;
		private readonly object[] deletedState;
		private readonly IEntityPersister persister;

		public PreDeleteEvent(object entity, object id, object[] deletedState, IEntityPersister persister)
		{
			this.entity = entity;
			this.id = id;
			this.deletedState = deletedState;
			this.persister = persister;
		}

		public object Entity
		{
			get { return entity; }
		}

		public object Id
		{
			get { return id; }
		}

		public object[] DeletedState
		{
			get { return deletedState; }
		}

		public IEntityPersister Persister
		{
			get { return persister; }
		}
	}
}
