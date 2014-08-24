using System;
using NHibernate.Classic;

namespace NHibernate.Event.Default
{
	/// <summary> Call <see cref="ILifecycle"/> interface if necessary </summary>
	[Serializable]
	public class DefaultPostLoadEventListener : IPostLoadEventListener
	{
		public virtual void OnPostLoad(PostLoadEvent @event)
		{
			if (@event.Persister.ImplementsLifecycle(@event.Session.EntityMode))
			{
				//log.debug( "calling onLoad()" );
				((ILifecycle)@event.Entity).OnLoad(@event.Session, @event.Id);
			}
		}
	}
}
