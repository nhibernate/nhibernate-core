using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NHibernate.Util
{
	internal sealed class JoinedAsyncEnumerable<T> : IAsyncEnumerable<T>
	{
		private readonly IAsyncEnumerable<T>[] _asyncEnumerables;

		public JoinedAsyncEnumerable(IAsyncEnumerable<T>[] asyncEnumerables)
		{
			_asyncEnumerables = asyncEnumerables;
		}

		public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
		{
			return new JoinedAsyncEnumerator(
				_asyncEnumerables.Select(o => o.GetAsyncEnumerator()).ToArray(),
				cancellationToken);
		}

		private class JoinedAsyncEnumerator : IAsyncEnumerator<T>
		{
			private readonly IAsyncEnumerator<T>[] _asyncEnumerators;
			private readonly CancellationToken _cancellationToken;
			private int _current;
			private bool _isAlreadyDisposed;

			public JoinedAsyncEnumerator(IAsyncEnumerator<T>[] asyncEnumerators, CancellationToken cancellationToken)
			{
				_asyncEnumerators = asyncEnumerators;
				_cancellationToken = cancellationToken;
			}

			public T Current => _asyncEnumerators[_current].Current;

			public async ValueTask<bool> MoveNextAsync()
			{
				_cancellationToken.ThrowIfCancellationRequested();

				for (; _current < _asyncEnumerators.Length; _current++)
				{
					var enumerator = _asyncEnumerators[_current];
					if (await enumerator.MoveNextAsync().ConfigureAwait(false))
					{
						return true;
					}

					// there are no items left to iterate over in the current
					// enumerator so go ahead and dispose of it.
					await enumerator.DisposeAsync().ConfigureAwait(false);
				}

				return false;
			}

			public async ValueTask DisposeAsync()
			{
				if (_isAlreadyDisposed)
				{
					// don't dispose of multiple times.
					return;
				}

				// dispose each IAsyncEnumerator that still needs to be disposed of
				for (; _current < _asyncEnumerators.Length; _current++)
				{
					await _asyncEnumerators[_current].DisposeAsync().ConfigureAwait(false);
				}

				_isAlreadyDisposed = true;
			}
		}
	}
}
