using System;
using NHibernate.Persister.Entity;

namespace NHibernate.Event
{
	/// <summary> 
	/// Called before injecting property values into a newly loaded entity instance.
	/// </summary>
	[Serializable]
	public class PreLoadEvent : AbstractEvent, IPreDatabaseOperationEventArgs
	{
		private object entity;
		private object[] state;
		private object id;
		private IEntityPersister persister;

		public PreLoadEvent(IEventSource source) : base(source) { }

		public object Entity
		{
			get { return entity; }
			set { entity = value; }
		}

		public object[] State
		{
			get { return state; }
			set { state = value; }
		}

		public object Id
		{
			get { return id; }
			set { id = value; }
		}

		public IEntityPersister Persister
		{
			get { return persister; }
			set { persister = value; }
		}
	}
}
