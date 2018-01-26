namespace NHibernate.Action
{
	/// <summary>
	/// An operation which may be scheduled for later execution.
	/// Usually, the operation is a database insert/update/delete,
	/// together with required second-level cache management.
	/// </summary>
	public partial interface IExecutable
	{
		/// <summary>
		/// What spaces (tables) are affected by this action?
		/// </summary>
		string[] PropertySpaces { get; }

		/// <summary> Called before executing any actions</summary>
		void BeforeExecutions();

		/// <summary> Execute this action</summary>
		void Execute();

		/// <summary>
		/// Get the before-transaction-completion process, if any, for this action.
		/// </summary>
		IBeforeTransactionCompletionProcess BeforeTransactionCompletionProcess { get; }

		/// <summary>
		/// Get the after-transaction-completion process, if any, for this action.
		/// </summary>
		IAfterTransactionCompletionProcess AfterTransactionCompletionProcess { get; }
	}
}
