using System.Collections;

namespace NHibernate.Util
{
	/// <summary>
	/// Combines multiple objects implementing <see cref="IEnumerable"/> into one.
	/// </summary>
	public class JoinedEnumerable : IEnumerable, IEnumerator
	{
		private IEnumerator[ ] _enumerators;
		private int _current;

		/// <summary>
		/// Creates an IEnumerable object from multiple IEnumerables.
		/// </summary>
		/// <param name="enumerables">The IEnumerables to join together.</param>
		public JoinedEnumerable( IEnumerable[ ] enumerables )
		{
			_enumerators = new IEnumerator[enumerables.Length];
			for( int i = 0; i < enumerables.Length; i++ )
			{
				_enumerators[ i ] = enumerables[ i ].GetEnumerator();
			}
			_current = 0;
		}

		#region System.Collections.IEnumerator Members

		/// <summary></summary>
		public bool MoveNext()
		{
			for(; _current < _enumerators.Length; _current++ )
			{
				if( _enumerators[ _current ].MoveNext() ) return true;
			}
			return false;
		}

		/// <summary></summary>
		public void Reset()
		{
			for( int i = 0; i < _enumerators.Length; i++ )
			{
				_enumerators[ i ].Reset();
			}
		}

		/// <summary></summary>
		public object Current
		{
			get { return _enumerators[ _current ].Current; }
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
	}
}