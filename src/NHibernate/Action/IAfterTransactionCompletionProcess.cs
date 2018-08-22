namespace NHibernate.Action
{
	public partial interface IAfterTransactionCompletionProcess
	{
		/// <summary>
		/// Perform whatever processing is encapsulated here after completion of the transaction.
		/// </summary>
		/// <param name="success">Did the transaction complete successfully?  True means it did.</param>
		void ExecuteAfterTransactionCompletion(bool success);
	}
}
