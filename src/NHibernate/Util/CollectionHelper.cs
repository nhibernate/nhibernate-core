using System;
using System.Collections;

namespace NHibernate.Util
{
	public sealed class CollectionHelper
	{
		private class EmptyEnumerator : IDictionaryEnumerator
		{
			public object Key
			{
				get { throw new InvalidOperationException("EmptyEnumerator.get_Key"); }
			}

			public object Value
			{
				get { throw new InvalidOperationException("EmptyEnumerator.get_Value"); }
			}

			public DictionaryEntry Entry
			{
				get { throw new InvalidOperationException("EmptyEnumerator.get_Entry"); }
			}

			public void Reset()
			{
			}

			public object Current
			{
				get { throw new InvalidOperationException("EmptyEnumerator.get_Current"); }
			}

			public bool MoveNext()
			{
				return false;
			}
		}

		/// <summary>
		/// A read-only dictionary that is always empty and permits lookup by <c>null</c> key.
		/// </summary>
		private class EmptyMapClass : IDictionary
		{
			private static readonly EmptyEnumerator EmptyEnumerator = new EmptyEnumerator();

			public bool Contains( object key )
			{
				return false;
			}

			public void Add( object key, object value )
			{
				throw new NotSupportedException("EmptyMap.Add");
			}

			public void Clear()
			{
				throw new NotSupportedException("EmptyMap.Clear");
			}

			IDictionaryEnumerator IDictionary.GetEnumerator()
			{
				return EmptyEnumerator;
			}

			public void Remove( object key )
			{
				throw new NotSupportedException("EmptyMap.Remove");
			}

			public object this[ object key ]
			{
				get { return null; }
				set { throw new NotSupportedException("EmptyMap.set_Item"); }
			}

			public ICollection Keys
			{
				get { return this; }
			}

			public ICollection Values
			{
				get { return this; }
			}

			public bool IsReadOnly
			{
				get { return true; }
			}

			public bool IsFixedSize
			{
				get { return true; }
			}

			public void CopyTo( Array array, int index )
			{
			}

			public int Count
			{
				get { return 0; }
			}

			public object SyncRoot
			{
				get { return this; }
			}

			public bool IsSynchronized
			{
				get { return false; }
			}

			public IEnumerator GetEnumerator()
			{
				return EmptyEnumerator;
			}
		}

		public static readonly IDictionary EmptyMap = new EmptyMapClass();

		public static bool CollectionEquals( ICollection c1, ICollection c2 )
		{
			if( c1 == c2 )
			{
				return true;
			}

			if( c1.Count != c2.Count )
			{
				return false;
			}

			IEnumerator e1 = c1.GetEnumerator();
			IEnumerator e2 = c2.GetEnumerator();

			while( e1.MoveNext() )
			{
				e2.MoveNext();
				if( !object.Equals( e1.Current, e2.Current ) )
				{
					return false;
				}
			}

			return true;
		}

		private CollectionHelper() {}
	}
}
