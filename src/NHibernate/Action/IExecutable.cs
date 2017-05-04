using NHibernate.Engine;

namespace NHibernate.Action
{
	/// <summary>
	/// Delegate representing some process that needs to occur before transaction completion.
	/// </summary>
	/// <remarks>
	/// NH specific: C# does not support dynamic interface proxies so a delegate is used in
	/// place of the Hibernate interface (see Action/BeforeTransactionCompletionProcess). The
	/// delegate omits the <see cref="ISessionImplementor" /> parameter as it is not used.
	/// </remarks>
	public delegate void BeforeTransactionCompletionProcessDelegate();
	
	/// <summary>
	/// Delegate representing some process that needs to occur after transaction completion.
	/// </summary>
	/// <param name="success"> Did the transaction complete successfully? True means it did.</param>
	/// <remarks>
	/// NH specific: C# does not support dynamic interface proxies so a delegate is used in
	/// place of the Hibernate interface (see Action/AfterTransactionCompletionProcess). The
	/// delegate omits the <see cref="ISessionImplementor" /> parameter as it is not used.
	/// </remarks>
	public delegate void AfterTransactionCompletionProcessDelegate(bool success);
	
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
		BeforeTransactionCompletionProcessDelegate BeforeTransactionCompletionProcess { get; }
		
		/// <summary>
		/// Get the after-transaction-completion process, if any, for this action.
		/// </summary>
		AfterTransactionCompletionProcessDelegate AfterTransactionCompletionProcess { get; }
	}
}
