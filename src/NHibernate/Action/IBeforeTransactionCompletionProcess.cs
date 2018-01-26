namespace NHibernate.Action
{
	public partial interface IBeforeTransactionCompletionProcess
	{
		void Execute();
	}
}
