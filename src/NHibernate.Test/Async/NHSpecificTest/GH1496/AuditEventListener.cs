﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System.Collections.Generic;
using NHibernate.Event;

namespace NHibernate.Test.NHSpecificTest.GH1496
{
	using System.Threading.Tasks;
	using System.Threading;
	public partial class AuditEventListener : IPostUpdateEventListener
	{

		public Task OnPostUpdateAsync(PostUpdateEvent @event, CancellationToken cancellationToken)
		{
			try
			{
				if (isActive == false)
				{ return Task.CompletedTask; }

				var modifiedItems = @event.Persister.FindModified(@event.OldState, @event.State, @event.Entity, @event.Session);
				foreach (int index in modifiedItems)
				{
					ModifiedItems.Add(new Item
					{
						Index = index,
						OldState = @event.OldState[index],
						State = @event.State[index]
					});
				}
				return Task.CompletedTask;
			}
			catch (System.Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}
	}
}
