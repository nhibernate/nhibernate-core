using System;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.Event
{
	/// <summary>  
	/// Defines an event class for the refreshing of an object.
	/// </summary>
	[Serializable]
	public class RefreshEvent : AbstractEvent
	{
		private readonly LockMode lockMode= LockMode.Read;
		private readonly object entity;

		public RefreshEvent(object entity, IEventSource source)
			: base(source)
		{
			if (entity == null)
				throw new ArgumentNullException("entity", "Attempt to generate refresh event with null object");
			this.entity = entity;
		}

		public RefreshEvent(object entity, LockMode lockMode, IEventSource source)
			: this(entity, source)
		{
			if (lockMode == null)
				throw new ArgumentNullException("lockMode", "Attempt to generate refresh event with null lock mode");

			this.lockMode = lockMode;
		}

		public object Entity
		{
			get { return entity; }
		}

		public LockMode LockMode
		{
			get { return lockMode; }
		}
	}
}
