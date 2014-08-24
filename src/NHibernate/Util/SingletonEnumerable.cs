using System.Collections;
using System.Collections.Generic;

namespace NHibernate.Util
{
	public sealed class SingletonEnumerable<T> : IEnumerable<T>
	{
		private readonly T value;

		public SingletonEnumerable(T value)
		{
			this.value = value;
		}

		#region IEnumerable<T> Members

		public IEnumerator<T> GetEnumerator()
		{
			return new SingletonEnumerator(value);
		}

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion

		private class SingletonEnumerator : IEnumerator<T>
		{
			private readonly T current;
			private bool hasNext = true;

			public SingletonEnumerator(T value)
			{
				current = value;
			}

			#region IEnumerator<T> Members

			public T Current
			{
				get { return current; }
			}

			#endregion

			#region IDisposable Members

			public void Dispose()
			{
			}

			#endregion

			#region IEnumerator Members

			public bool MoveNext()
			{
				var result = hasNext;
				hasNext = false;
				return result;
			}

			public void Reset()
			{
			}

			object IEnumerator.Current
			{
				get { return Current; }
			}

			#endregion
		}
	}
}