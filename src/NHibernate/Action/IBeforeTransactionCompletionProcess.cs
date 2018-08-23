namespace NHibernate.Action
{
	/// <summary>
	/// Contract representing some process that needs to occur during before transaction completion.
	/// </summary>
	public partial interface IBeforeTransactionCompletionProcess
	{
		/// <summary>
		/// Perform whatever processing is encapsulated here before completion of the transaction.
		/// </summary>
		void ExecuteBeforeTransactionCompletion();
	}
}
