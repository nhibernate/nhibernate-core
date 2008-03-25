using NHibernate.Engine;
using NHibernate.Persister.Entity;

namespace NHibernate.Event
{
	/// <summary> 
	/// Occurs before inserting an item in the datastore
	/// </summary>
	public class PreInsertEvent
	{
		private readonly object entity;
		private readonly object id;
		private readonly object[] state;
		private readonly IEntityPersister persister;
		private readonly ISessionImplementor source;

		public PreInsertEvent(object entity, object id, object[] state, IEntityPersister persister, ISessionImplementor source)
		{
			this.entity = entity;
			this.id = id;
			this.state = state;
			this.persister = persister;
			this.source = source;
		}

		public object Entity
		{
			get { return entity; }
		}

		public object Id
		{
			get { return id; }
		}

		public object[] State
		{
			get { return state; }
		}

		public IEntityPersister Persister
		{
			get { return persister; }
		}

		public ISessionImplementor Source
		{
			get { return source; }
		}
	}
}
