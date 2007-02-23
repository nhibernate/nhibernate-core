using System;
using System.Collections;
using System.Collections.Generic;
using log4net;
#if NET_2_0
#endif

namespace NHibernate.Util
{
	/// <summary>
	/// Combines multiple objects implementing <see cref="IEnumerable"/> into one.
	/// </summary>
	public class JoinedEnumerable : IEnumerable, IEnumerator, IDisposable
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(JoinedEnumerable));

		private IEnumerator[] _enumerators;
		private int _current;

		/// <summary>
		/// Creates an IEnumerable object from multiple IEnumerables.
		/// </summary>
		/// <param name="enumerables">The IEnumerables to join together.</param>
		public JoinedEnumerable(IEnumerable[] enumerables)
		{
			_enumerators = new IEnumerator[enumerables.Length];
			for (int i = 0; i < enumerables.Length; i++)
			{
				_enumerators[i] = enumerables[i].GetEnumerator();
			}
			_current = 0;
		}

		#region System.Collections.IEnumerator Members

		/// <summary></summary>
		public bool MoveNext()
		{
			for (; _current < _enumerators.Length; _current++)
			{
				if (_enumerators[_current].MoveNext())
				{
					return true;
				}
				else
				{
					// there are no items left to iterate over in the current
					// enumerator so go ahead and dispose of it.
					IDisposable disposable = _enumerators[_current] as IDisposable;
					if (disposable != null)
					{
						disposable.Dispose();
					}
				}
			}
			return false;
		}

		/// <summary></summary>
		public void Reset()
		{
			for (int i = 0; i < _enumerators.Length; i++)
			{
				_enumerators[i].Reset();
			}
		}

		/// <summary></summary>
		public object Current
		{
			get { return _enumerators[_current].Current; }
		}

		#endregion

		#region System.Collections.IEnumerable Members

		/// <summary></summary>
		public IEnumerator GetEnumerator()
		{
			Reset();
			return this;
		}

		#endregion

		#region IDisposable Members

		/// <summary>
		/// A flag to indicate if <c>Disose()</c> has been called.
		/// </summary>
		private bool _isAlreadyDisposed;

		/// <summary>
		/// Finalizer that ensures the object is correctly disposed of.
		/// </summary>
		~JoinedEnumerable()
		{
			Dispose(false);
		}

		/// <summary>
		/// Takes care of freeing the managed and unmanaged resources that 
		/// this class is responsible for.
		/// </summary>
		public void Dispose()
		{
			log.Debug("running JoinedEnumerable.Dispose()");
			Dispose(true);
		}


		/// <summary>
		/// Takes care of freeing the managed and unmanaged resources that 
		/// this class is responsible for.
		/// </summary>
		/// <param name="isDisposing">Indicates if this JoinedEnumerable is being Disposed of or Finalized.</param>
		/// <remarks>
		/// The command is closed and the reader is disposed.  This allows other ADO.NET
		/// related actions to occur without needing to move all the way through the
		/// EnumerableImpl.
		/// </remarks>
		protected virtual void Dispose(bool isDisposing)
		{
			if (_isAlreadyDisposed)
			{
				// don't dispose of multiple times.
				return;
			}

			// free managed resources that are being managed by the JoinedEnumerable if we
			// know this call came through Dispose()
			if (isDisposing)
			{
				// dispose each IEnumerable that still needs to be disposed of
				for (; _current < _enumerators.Length; _current++)
				{
					IDisposable currentDisposable = _enumerators[_current] as IDisposable;
					if (currentDisposable != null)
					{
						currentDisposable.Dispose();
					}
				}
			}

			// free unmanaged resources here

			_isAlreadyDisposed = true;
			// nothing for Finalizer to do - so tell the GC to ignore it
			GC.SuppressFinalize(this);
		}

		#endregion
	}

#if NET_2_0
	public class GenericJoinedEnumerable<T> : JoinedEnumerable, IEnumerable<T>, IEnumerator<T>
	{
		public GenericJoinedEnumerable(IEnumerable[] enumerables)
			: base(enumerables)
		{
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			Reset();
			return this;
		}

		T IEnumerator<T>.Current
		{
			get { return (T) this.Current; }
		}
	}
#endif
}