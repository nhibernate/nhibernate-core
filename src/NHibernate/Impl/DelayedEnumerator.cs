using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NHibernate.Impl
{
	internal class DelayedEnumerator<T> : IFutureEnumerable<T>, IDelayedValue
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

		#region IFutureEnumerable<T> Members

		public Task<IEnumerable<T>> GetEnumerableAsync(CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<IEnumerable<T>>(cancellationToken);
			}
			try
			{
				if (ExecuteOnEval == null)
					return _resultAsync(cancellationToken);
				return getEnumerableAsync();
			}
			catch (Exception ex)
			{
				return Task.FromException<IEnumerable<T>>(ex);
			}

			async Task<IEnumerable<T>> getEnumerableAsync()
			{
				var result = await _resultAsync(cancellationToken).ConfigureAwait(false);
				return (IEnumerable<T>)ExecuteOnEval.DynamicInvoke(result);
			}
		}

		#endregion
	}

	internal interface IDelayedValue
	{
		Delegate ExecuteOnEval { get; set; }
	}
}