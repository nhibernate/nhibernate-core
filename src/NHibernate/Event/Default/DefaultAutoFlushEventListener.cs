using System;
using log4net;

namespace NHibernate.Event.Default
{
	/// <summary> 
	/// Defines the default flush event listeners used by hibernate for 
	/// flushing session state in response to generated auto-flush events. 
	/// </summary>
	[Serializable]
	public class DefaultAutoFlushEventListener : AbstractFlushingEventListener, IAutoFlushEventListener
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(DefaultAutoFlushEventListener));

		#region IAutoFlushEventListener Members

		/// <summary>
		/// Handle the given auto-flush event.
		/// </summary>
		/// <param name="event">The auto-flush event to be handled.</param>
		public void OnAutoFlush(AutoFlushEvent @event)
		{
			IEventSource source = @event.Session;

			if (FlushMightBeNeeded(source))
			{
				int oldSize = source.ActionQueue.CollectionRemovalsCount;

				FlushEverythingToExecutions(@event);

				if (FlushIsReallyNeeded(@event, source))
				{
					if (log.IsDebugEnabled)
						log.Debug("Need to execute flush");

					PerformExecutions(source);
					PostFlush(source);
					// note: performExecutions() clears all collectionXxxxtion 
					// collections (the collection actions) in the session

					// TODO: H3.2 not ported
					//if (source.Factory.Statistics.StatisticsEnabled)
					//{
					//  source.Factory.StatisticsImplementor.flush();
					//}
				}
				else
				{

					if (log.IsDebugEnabled)
						log.Debug("Dont need to execute flush");
					source.ActionQueue.ClearFromFlushNeededCheck(oldSize);
				}

				@event.FlushRequired = FlushIsReallyNeeded(@event, source);
			}
		}

		#endregion

		private bool FlushIsReallyNeeded(AutoFlushEvent @event, IEventSource source)
		{
			return source.ActionQueue.AreTablesToBeUpdated(@event.QuerySpaces) || source.FlushMode == FlushMode.Always;
		}

		private bool FlushMightBeNeeded(IEventSource source)
		{
			return !(source.FlushMode < FlushMode.Auto) && source.DontFlushFromFind == 0 && source.HasNonReadOnlyEntities;
		}
	}
}
