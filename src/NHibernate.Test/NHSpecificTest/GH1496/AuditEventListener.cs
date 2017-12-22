using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NHibernate.Event;

namespace NHibernate.Test.NHSpecificTest.GH1496
{
	public class AuditEventListener : IPostUpdateEventListener
	{
		public IList<Item> ModifiedItems { get; set; } = new List<Item>();
		private bool isActive = false;

		public void Start()
		{
			ModifiedItems.Clear();
			isActive = true;
		}

		public void Stop()
		{
			isActive = false;
		}

		public async Task OnPostUpdateAsync(PostUpdateEvent @event, CancellationToken cancellationToken)
		{
			if (isActive == false)
			{ return; }

			var modifiedItems = await @event.Persister.FindModifiedAsync(@event.OldState, @event.State, @event.Entity, @event.Session, cancellationToken);
			foreach (int index in modifiedItems)
			{
				ModifiedItems.Add(new Item
				{
					Index = index,
					OldState = @event.OldState[index],
					State = @event.State[index]
				});
			}
		}

		public void OnPostUpdate(PostUpdateEvent @event)
		{
			if (isActive == false)
			{ return; }

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
		}

		public class Item
		{
			public int Index { get; set; }
			public object OldState { get; set; }
			public object State { get; set; }
		}

	}


}
