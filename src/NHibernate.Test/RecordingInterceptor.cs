namespace NHibernate.Test
{
	public class RecordingInterceptor : EmptyInterceptor
	{
		public int afterTransactionBeginCalled;
		public int afterTransactionCompletionCalled;
		public int beforeTransactionCompletionCalled;

		public override void AfterTransactionBegin(ITransaction tx)
		{
			afterTransactionBeginCalled++;
		}

		public override void AfterTransactionCompletion(ITransaction tx)
		{
			afterTransactionCompletionCalled++;
		}

		public override void BeforeTransactionCompletion(ITransaction tx)
		{
			beforeTransactionCompletionCalled++;
		}
	}
}