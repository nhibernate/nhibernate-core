using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NHibernate.Util
{
	// 6.0 TODO: Remove it in favor of await foreach, also AsyncJoinedEnumerableFixture needs to be removed
	internal sealed class JoinedAsyncEnumerable<T> : IAsyncEnumerable<T>
	{
		private readonly IEnumerable<IAsyncEnumerable<T>> _asyncEnumerables;

		public JoinedAsyncEnumerable(IEnumerable<IAsyncEnumerable<T>> asyncEnumerables)
		{
			_asyncEnumerables = asyncEnumerables;
		}

		public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
		{
			return new JoinedAsyncEnumerator(_asyncEnumerables, cancellationToken);
		}

		private class JoinedAsyncEnumerator : IAsyncEnumerator<T>
		{
			private readonly IEnumerable<IAsyncEnumerable<T>> _asyncEnumerables;
			private readonly CancellationToken _cancellationToken;
			private IEnumerator<IAsyncEnumerable<T>> _currentEnumerator;
			private IAsyncEnumerator<T> _currentAsyncEnumerator;
			private bool _isAlreadyDisposed;

			public JoinedAsyncEnumerator(IEnumerable<IAsyncEnumerable<T>> asyncEnumerables, CancellationToken cancellationToken)
			{
				_asyncEnumerables = asyncEnumerables;
				_cancellationToken = cancellationToken;
			}

			public T Current { get; private set; }

			public async ValueTask<bool> MoveNextAsync()
			{
				_cancellationToken.ThrowIfCancellationRequested();
				if (_isAlreadyDisposed)
				{
					throw new InvalidOperationException("The enumerator was disposed.");
				}

				if (_currentEnumerator == null)
				{
					_currentEnumerator = _asyncEnumerables.GetEnumerator();
					if (!MoveNext())
					{
						return false;
					}
				}

				if (_currentAsyncEnumerator == null)
				{
					return false; // MoveNextAsync called after we reached the end of the enumeration
				}

				while (true)
				{
					if (await _currentAsyncEnumerator.MoveNextAsync().ConfigureAwait(false))
					{
						Current = _currentAsyncEnumerator.Current;
						return true;
					}

					// there are no items left to iterate over in the current
					// async enumerator so go ahead and dispose of it.
					await _currentAsyncEnumerator.DisposeAsync().ConfigureAwait(false);
					if (!MoveNext())
					{
						return false;
					}
				}
			}

			private bool MoveNext()
			{
				if (!_currentEnumerator.MoveNext())
				{
					_currentAsyncEnumerator = null;
					return false;
				}

				_currentAsyncEnumerator = _currentEnumerator.Current.GetAsyncEnumerator(_cancellationToken);
				return true;
			}

			public async ValueTask DisposeAsync()
			{
				if (_isAlreadyDisposed)
				{
					// don't dispose of multiple times.
					return;
				}

				// Dispose only the current async enumerator when DisposeAsync is called before the enumeration ended
				if (_currentAsyncEnumerator != null)
				{
					await _currentAsyncEnumerator.DisposeAsync().ConfigureAwait(false);
					_currentAsyncEnumerator = null;
				}

				Current = default;
				_currentEnumerator.Dispose();
				_currentEnumerator = null;
				_isAlreadyDisposed = true;
			}
		}
	}
}
