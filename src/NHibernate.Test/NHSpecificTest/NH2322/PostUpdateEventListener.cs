using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Event;

namespace NHibernate.Test.NHSpecificTest.NH2322
{
	public partial class PostUpdateEventListener : IPostUpdateEventListener
	{
		void IPostUpdateEventListener.OnPostUpdate(PostUpdateEvent @event)
		{
			if (@event.Entity is Person)
			{
				@event.Session
					.CreateSQLQuery("update Person set Name = :newName")
					.SetString("newName", "new updated name")
					.ExecuteUpdate();
			}
		}
	}
}