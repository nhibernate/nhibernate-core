using log4net;
using NHibernate.Event;

namespace NHibernate.Test.Events.PostEvents
{
	public delegate void AssertionAction(PostUpdateEvent @event);
 
	public partial class AssertOldStatePostListener : IPostUpdateEventListener
	{
		private readonly AssertionAction assertionDelegate;

		public AssertOldStatePostListener(AssertionAction assertionDelegate)
		{
			this.assertionDelegate = assertionDelegate;
		}

		public const string LogMessage = "PostUpdateEvent called.";

		private static readonly ILog log = LogManager.GetLogger(typeof(AssertOldStatePostListener));

		public void OnPostUpdate(PostUpdateEvent @event)
		{
			log.Debug(LogMessage);
			assertionDelegate(@event);
		}
	}
}