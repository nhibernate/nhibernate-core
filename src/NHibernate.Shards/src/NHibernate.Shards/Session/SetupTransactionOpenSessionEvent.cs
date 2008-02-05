using System;

namespace NHibernate.Shards.Session
{
	public class SetupTransactionOpenSessionEvent : IOpenSessionEvent
	{
		#region IOpenSessionEvent Members

		public void OnOpenSession(ISession session)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}