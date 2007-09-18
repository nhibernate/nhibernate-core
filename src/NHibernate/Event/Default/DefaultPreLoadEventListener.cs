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
		public void OnPreLoad(PreLoadEvent theEvent)
		{
			IEntityPersister persister = theEvent.Persister;
			theEvent.Session.Interceptor.OnLoad(theEvent.Entity, theEvent.Id, theEvent.State, persister.PropertyNames, persister.PropertyTypes);
		}
	}
}
