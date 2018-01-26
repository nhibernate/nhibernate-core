using System;
using System.Threading;
using System.Threading.Tasks;

namespace NHibernate.Action
{
	public partial class AfterTransactionCompletionProcess : IAfterTransactionCompletionProcess
	{
		private readonly Action<bool> _syncAction;
		private readonly Func<bool, CancellationToken, Task> _asyncAction;

		public AfterTransactionCompletionProcess(Action<bool> syncAction, Func<bool, CancellationToken, Task> asyncAction)
		{

			_syncAction = syncAction;
			_asyncAction = asyncAction;
		}

		public AfterTransactionCompletionProcess(Action<bool> syncAction)
		{
			_syncAction = syncAction;
			_asyncAction = (success, cancellationToken) =>
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return Task.FromCanceled<object>(cancellationToken);
				}
				try
				{
					_syncAction(success);
					return Task.CompletedTask;
				}
				catch (Exception ex)
				{
					return Task.FromException<object>(ex);
				}
			};
		}

		public void Execute(bool success)
		{
			_syncAction(success);
		}

		public Task ExecuteAsync(bool success, CancellationToken cancellationToken)
		{
			return _asyncAction(success, cancellationToken);
		}
	}	
}
