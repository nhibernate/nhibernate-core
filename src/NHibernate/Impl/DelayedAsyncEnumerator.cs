using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NHibernate.Impl
{
	internal class DelayedAsyncEnumerator<T> : IAsyncEnumerable<T>, IDelayedValue
	{
		public delegate Task<IEnumerable<T>> GetResult();

		private readonly GetResult result;

		public Delegate ExecuteOnEval { get; set; }

		public DelayedAsyncEnumerator(GetResult result)
		{
			this.result = result;
		}

		#region IAsyncEnumerator<T> Members

		IAsyncEnumerator<T> IAsyncEnumerable<T>.GetEnumerator()
		{
			return new Enumerator(result, ExecuteOnEval);
		}

		#endregion

		private sealed class Enumerator : IAsyncEnumerator<T>
		{
			private IEnumerable<T> _data;
			private readonly GetResult _result;
			private readonly Delegate _executeOnEval;
			private IEnumerator<T> _enumerator;

			public Enumerator(GetResult result, Delegate executeOnEval)
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
					_data = await _result().ConfigureAwait(false);
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
}