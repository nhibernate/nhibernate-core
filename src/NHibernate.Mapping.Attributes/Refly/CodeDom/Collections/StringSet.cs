using System;
using System.Collections;
using System.Collections.Specialized;

namespace Refly.CodeDom.Collections
{

	public class StringSet : ICollection
	{
		private IDictionary dictionary = new HybridDictionary();

		#region ICollection Members

		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		public int Count
		{
			get
			{
				return dictionary.Count;
			}
		}

		public void CopyTo(Array array, int index)
		{
			this.dictionary.Keys.CopyTo(array,index);
		}

		public object SyncRoot
		{
			get
			{
				return null;
			}
		}

		#endregion

		#region IEnumerable Members

		public IEnumerator GetEnumerator()
		{
			return this.dictionary.Keys.GetEnumerator();
		}

		#endregion

		#region IList Members

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public void Remove(string value)
		{
			this.dictionary.Remove(value);
		}

		public bool Contains(object value)
		{
			return this.dictionary.Contains(value);
		}

		public void Clear()
		{
			this.dictionary.Clear();
		}

		public void Add(string value)
		{
			if (!this.dictionary.Contains(value))
				this.dictionary.Add(value,null);
		}

		public bool IsFixedSize
		{
			get
			{
				return false;
			}
		}

		#endregion
	}
}
