namespace NHibernate.Action
{
	/// <summary>
	/// An operation which may be scheduled for later execution.
	/// Usually, the operation is a database insert/update/delete,
	/// together with required second-level cache management.
	/// </summary>
	public interface IExecutable
	{
		/// <summary>
		/// What spaces (tables) are affected by this action?
		/// </summary>
		object[] PropertySpaces { get;}

		/// <summary> Called before executing any actions</summary>
		void BeforeExecutions();

		/// <summary> Execute this action</summary>
		void Execute();

		/// <summary> 
		/// Do we need to retain this instance until after the transaction completes?
		/// </summary>
		/// <returns>
		/// False if this class defines a no-op	has after transaction completion.
		/// </returns>
		bool HasAfterTransactionCompletion();

		/// <summary> Called after the transaction completes</summary>
		void AfterTransactionCompletion(bool success);
	}
}
