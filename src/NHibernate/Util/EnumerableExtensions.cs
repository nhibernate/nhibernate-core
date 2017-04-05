using System;
using System.Collections;
using System.Collections.Generic;

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

		public static object First(this IEnumerable source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			IList collection = source as IList;
			if (collection != null)
			{
				if (collection.Count > 0)
				{
					return collection[0];
				}
			}
			else
			{
				using (DisposableEnumerator enumerator = source.GetDisposableEnumerator())
				{
					if (enumerator.MoveNext())
					{
						return enumerator.Current;
					}
				}
			}
			throw new InvalidOperationException("Sequence contains no elements");
		}

		public static object FirstOrNull(this IEnumerable source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			IList collection = source as IList;
			if (collection != null)
			{
				if (collection.Count > 0)
				{
					return collection[0];
				}
			}
			else
			{
				using (DisposableEnumerator enumerator = source.GetDisposableEnumerator())
				{
					if (enumerator.MoveNext())
					{
						return enumerator.Current;
					}
				}
			}
			return null;
		}

		public static void ForEach<T>(this IEnumerable<T> query, Action<T> method)
		{
			foreach (T item in query)
			{
				method(item);
			}
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