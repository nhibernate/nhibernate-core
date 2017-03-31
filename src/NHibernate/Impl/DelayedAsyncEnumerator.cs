#if ASYNC
using System;
using System.Collections;
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
			private IEnumerable data;
			private GetResult result;
			private Delegate executeOnEval;
			private IEnumerator enumerator;

			public Enumerator(GetResult result, Delegate executeOnEval)
			{
				this.result = result;
				this.executeOnEval = executeOnEval;
			}

			public T Current { get; private set; }

			public async Task<bool> MoveNext(CancellationToken cancellationToken)
			{
				cancellationToken.ThrowIfCancellationRequested();
				if (enumerator == null)
				{
					data = await result();
					if (executeOnEval != null)
						data = (IEnumerable)executeOnEval.DynamicInvoke(data);
					enumerator = data.GetEnumerator();
				}
				if (!enumerator.MoveNext())
				{
					return false;
				}
				Current = (T)enumerator.Current;
				return true;
			}

			public void Dispose()
			{
			}
		}
	}
}
#endif