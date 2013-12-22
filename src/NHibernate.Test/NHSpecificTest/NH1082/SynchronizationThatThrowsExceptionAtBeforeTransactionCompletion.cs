using NHibernate.Transaction;

namespace NHibernate.Test.NHSpecificTest.NH1082
{
	public class SynchronizationThatThrowsExceptionAtBeforeTransactionCompletion : ISynchronization
	{
		public void BeforeCompletion()
		{
			throw new BadException();
		}

		public void AfterCompletion(bool success)
		{
		}
	}
}