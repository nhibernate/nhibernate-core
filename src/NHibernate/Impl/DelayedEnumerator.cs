using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NHibernate.Impl
{
	internal class DelayedEnumerator<T> : IEnumerable<T>, IAsyncEnumerable<T>, IDelayedValue
	{
		public delegate IEnumerable<T> GetResult();
		public delegate Task<IEnumerable<T>> GetResultAsync(CancellationToken cancellationToken);

		private readonly GetResult _result;
		private readonly GetResultAsync _resultAsync;

		public Delegate ExecuteOnEval { get; set; }

		public DelayedEnumerator(GetResult result, GetResultAsync resultAsync)
		{
			_result = result;
			_resultAsync = resultAsync;
		}

		public IEnumerable<T> Enumerable
		{
			get
			{
				var value = _result();
				if (ExecuteOnEval != null)
					value = (IEnumerable<T>) ExecuteOnEval.DynamicInvoke(value);
				foreach (T item in value)
				{
					yield return item;
				}
			}
		}

		#region IEnumerable<T> Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable) Enumerable).GetEnumerator();
		}

		public IEnumerator<T> GetEnumerator()
		{
			return Enumerable.GetEnumerator();
		}

		#endregion

		#region IAsyncEnumerator<T> Members

		IAsyncEnumerator<T> IAsyncEnumerable<T>.GetEnumerator()
		{
			return new AsyncEnumerator(_resultAsync, ExecuteOnEval);
		}

		#endregion

		private sealed class AsyncEnumerator : IAsyncEnumerator<T>
		{
			private IEnumerable<T> _data;
			private readonly GetResultAsync _result;
			private readonly Delegate _executeOnEval;
			private IEnumerator<T> _enumerator;

			public AsyncEnumerator(GetResultAsync result, Delegate executeOnEval)
			{
				_result = result;
				_executeOnEval = executeOnEval;
			}

			public T Current { get; private set; }

			public async Task<bool> MoveNext(CancellationToken cancellationToken)
			{
				cancellationToken.ThrowIfCancellationRequested();
				if (_enumerator == null)
				{
					_data = await _result(cancellationToken).ConfigureAwait(false);
					if (_executeOnEval != null)
						_data = (IEnumerable<T>)_executeOnEval.DynamicInvoke(_data);
					_enumerator = _data.GetEnumerator();
				}
				if (!_enumerator.MoveNext())
				{
					return false;
				}
				Current = _enumerator.Current;
				return true;
			}

			public void Dispose()
			{
				_enumerator?.Dispose();
			}
		}
	}

	internal interface IDelayedValue
	{
		Delegate ExecuteOnEval { get; set; }
	}
}