using System;

namespace NHibernate.Shards.Session
{
	public class SetCacheModeOpenSessionEvent : IOpenSessionEvent
	{
		#region IOpenSessionEvent Members

		public void OnOpenSession(ISession session)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}