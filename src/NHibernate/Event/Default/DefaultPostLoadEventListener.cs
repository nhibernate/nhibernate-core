using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Classic;

namespace NHibernate.Event.Default
{
	/// <summary> Call <see cref="ILifecycle"/> interface if necessary </summary>
	[Serializable]
	public class DefaultPostLoadEventListener : IPostLoadEventListener
	{
		public void OnPostLoad(PostLoadEvent theEvent)
		{
			if (theEvent.Persister.ImplementsLifecycle)
			{
				//log.debug( "calling onLoad()" );
				((ILifecycle)theEvent.Entity).OnLoad(theEvent.Session, theEvent.Id);
			}
		}
	}
}
