using System;
using System.Threading;
using System.Threading.Tasks;

namespace NHibernate.Action
{
	public class BeforeTransactionCompletionProcess : IBeforeTransactionCompletionProcess
	{
		private readonly System.Action _syncAction;
		private readonly Func<CancellationToken, Task> _asyncAction;

		public BeforeTransactionCompletionProcess(System.Action syncAction, Func<CancellationToken, Task> asyncAction)
		{
			_syncAction = syncAction;
			_asyncAction = asyncAction;
		}

		public BeforeTransactionCompletionProcess(System.Action syncAction)
		{
			_syncAction = syncAction;
			_asyncAction = (cancellationToken) =>
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return Task.FromCanceled<object>(cancellationToken);
				}
				try
				{
					_syncAction();
					return Task.CompletedTask;
				}
				catch (Exception ex)
				{
					return Task.FromException<object>(ex);
				}
			};
		}

		public void Execute()
		{
			_syncAction();
		}

		public Task ExecuteAsync(CancellationToken cancellationToken)
		{
			return _asyncAction(cancellationToken);
		}
	}
}
