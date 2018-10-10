namespace NHibernate.Action
{
	/// <summary>
	/// An extension to <see cref="IExecutable"/> which allows async cleanup operations to be
	/// scheduled on transaction completion.
	/// </summary>
	//6.0 TODO: Merge into IExecutable
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
