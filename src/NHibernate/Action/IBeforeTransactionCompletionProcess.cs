namespace NHibernate.Action
{
	public partial interface IBeforeTransactionCompletionProcess
	{
		/// <summary>
		/// Perform whatever processing is encapsulated here before completion of the transaction.
		/// </summary>
		void ExecuteBeforeTransactionCompletion();
	}
}
