using System;
using System.Collections;

namespace NHibernate.Util
{
	public static class EnumerableExtensions
	{
		public static bool Any(this IEnumerable source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			using (DisposableEnumerator enumerator = source.GetDisposableEnumerator())
			{
				if (enumerator.MoveNext())
				{
					return true;
				}
			}
			return false;
		}

		private static DisposableEnumerator GetDisposableEnumerator(this IEnumerable source)
		{
			return new DisposableEnumerator(source);
		}

		#region Nested type: DisposableEnumerator

		internal class DisposableEnumerator : IDisposable, IEnumerator
		{
			private readonly IEnumerator wrapped;

			public DisposableEnumerator(IEnumerable source)
			{
				wrapped = source.GetEnumerator();
			}

			#region IDisposable Members

			public void Dispose()
			{
				var disposable = wrapped as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}

			#endregion

			#region IEnumerator Members

			public bool MoveNext()
			{
				return wrapped.MoveNext();
			}

			public void Reset()
			{
				wrapped.Reset();
			}

			public object Current
			{
				get { return wrapped.Current; }
			}

			#endregion
		}

		#endregion
	}
}