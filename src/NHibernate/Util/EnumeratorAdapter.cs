using System;
using System.Collections;
using System.Collections.Generic;

namespace NHibernate.Util
{
	/// <summary>
	/// Wrap a non-generic IEnumerator to provide the generic <see cref="IEnumerator{T}" />
	/// interface.
	/// </summary>
	/// <typeparam name="T">The type of the enumerated elements.</typeparam>
	public class EnumeratorAdapter<T> : IEnumerator<T>
	{
		private readonly IEnumerator _wrapped;

		public EnumeratorAdapter(IEnumerator wrapped)
		{
			_wrapped = wrapped;
		}

		public void Dispose()
		{
			var disposable = _wrapped as IDisposable;
			if (disposable != null)
				disposable.Dispose();
		}

		public bool MoveNext()
		{
			return _wrapped.MoveNext();
		}

		public void Reset()
		{
			_wrapped.Reset();
		}

		public T Current
		{
			get { return (T)_wrapped.Current; }
		}

		object IEnumerator.Current
		{
			get { return Current; }
		}
	}
}
