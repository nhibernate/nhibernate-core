using NHibernate.Transaction;

namespace NHibernate.Engine.Transaction
{
	internal partial class IsolatedWorkAfterTransaction : ITransactionCompletionSynchronization
	{
		private readonly IIsolatedWork _work;
		private readonly ISessionImplementor _session;

		internal IsolatedWorkAfterTransaction(IIsolatedWork work, ISessionImplementor session)
		{
			_work = work;
			_session = session;
		}

		public void ExecuteBeforeTransactionCompletion()
		{
		}

		public void ExecuteAfterTransactionCompletion(bool success)
		{
			if (_session.Factory.Settings.IsDataDefinitionInTransactionSupported)
			{
				Isolater.DoIsolatedWork(_work, _session);
			}
			else
			{
				Isolater.DoNonTransactedWork(_work, _session);
			}
		}
	}
}
