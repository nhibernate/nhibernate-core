using System;
using System.Threading;
using System.Threading.Tasks;

namespace NHibernate.Transaction
{
	internal partial class AfterTransactionCompletion : ITransactionCompletionSynchronization
	{
		private readonly Action<bool> _whenCompleted;
		private readonly Func<bool, CancellationToken, Task> _whenCompletedAsync;

		public AfterTransactionCompletion(Action<bool> whenCompleted, Func<bool, CancellationToken, Task> whenCompletedAsync)
		{
			_whenCompleted = whenCompleted ?? throw new ArgumentNullException(nameof(whenCompleted));
			_whenCompletedAsync = whenCompletedAsync ??  throw new ArgumentNullException(nameof(whenCompletedAsync));
		}

		public void ExecuteBeforeTransactionCompletion()
		{
			// Nothing to do.
		}

		public void ExecuteAfterTransactionCompletion(bool success)
		{
			_whenCompleted(success);
		}

		public Task ExecuteAfterTransactionCompletionAsync(bool success, CancellationToken cancellationToken)
		{
			return _whenCompletedAsync(success, cancellationToken);
		}
	}
}
