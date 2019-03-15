using NHibernate.Transaction;

namespace NHibernate.Test.NHSpecificTest.NH1082
{
	public partial class TransactionSynchronizationThatThrowsExceptionAtBeforeTransactionCompletion : ITransactionCompletionSynchronization
	{
		public void ExecuteBeforeTransactionCompletion()
		{
			throw new BadException();
		}

		public void ExecuteAfterTransactionCompletion(bool success)
		{
		}
	}
}
