using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

		public IEnumerable<T> GetEnumerable()
		{
			var value = _result();
			foreach (T item in value)
			{
				yield return item;
			}
		}

		// Remove in 6.0
		#region IEnumerable<T> Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)GetEnumerable()).GetEnumerator();
		}

		[Obsolete("Please use GetEnumerable() or GetEnumerableAsync(cancellationToken) instead")]
		public IEnumerator<T> GetEnumerator()
		{
			return GetEnumerable().GetEnumerator();
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
				return _resultAsync(cancellationToken);
			}
			catch (Exception ex)
			{
				return Task.FromException<IEnumerable<T>>(ex);
			}
		}

		#endregion

		public IList TransformList(IList collection)
		{
			if (ExecuteOnEval == null)
				return collection;

			return ((IEnumerable) ExecuteOnEval.DynamicInvoke(collection)).Cast<T>().ToList();
		}
	}

	internal interface IDelayedValue
	{
		Delegate ExecuteOnEval { get; set; }

		IList TransformList(IList collection);
	}
}
