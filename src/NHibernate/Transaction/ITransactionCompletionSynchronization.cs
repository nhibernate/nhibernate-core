using NHibernate.Action;

namespace NHibernate.Transaction
{
	/// <summary>
	/// Contract representing processes that needs to occur before or after transaction completion.
	/// </summary>
	public interface ITransactionCompletionSynchronization : IBeforeTransactionCompletionProcess, IAfterTransactionCompletionProcess
	{
	}
}
