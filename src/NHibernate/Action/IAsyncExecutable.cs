namespace NHibernate.Action
{
	public interface IAsyncExecutable : IExecutable
	{
		/// <summary>
		/// Get the before-transaction-completion process, if any, for this action.
		/// </summary>
		new IBeforeTransactionCompletionProcess BeforeTransactionCompletionProcess { get; }

		/// <summary>
		/// Get the after-transaction-completion process, if any, for this action.
		/// </summary>
		new IAfterTransactionCompletionProcess AfterTransactionCompletionProcess { get; }
	}
}
