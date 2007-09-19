using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Persister.Entity;

namespace NHibernate.Event.Default
{
	/// <summary> 
	/// Called before injecting property values into a newly 
	/// loaded entity instance. 
	/// </summary>
	[Serializable]
	public class DefaultPreLoadEventListener : IPreLoadEventListener
	{
		public void OnPreLoad(PreLoadEvent @event)
		{
			IEntityPersister persister = @event.Persister;
			@event.Session.Interceptor.OnLoad(@event.Entity, @event.Id, @event.State, persister.PropertyNames, persister.PropertyTypes);
		}
	}
}
