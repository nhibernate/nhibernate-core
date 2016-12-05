using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NHibernate.Util
{
	/// <summary> 
	/// Set implementation that use reference equals instead of Equals() as its comparison mechanism.
	/// </summary>
	public class IdentitySet : ISet<object>
	{
		private IDictionary map;
		private static readonly object DumpValue = new object();

		public IdentitySet()
		{
			map = IdentityMap.Instantiate(10);
		}

		public IdentitySet(IEnumerable<object> members)
		{
			map = IdentityMap.Instantiate(10);
			foreach (var member in members)
				Add(member);
		}


		#region Implementation of ICollection<object>

		void ICollection<object>.Add(object item)
		{
			Add(item);
		}

		#endregion


		public bool Add(object o)
		{
			object tempObject = map[o];
			map[o] = DumpValue;
			return tempObject == null;
		}


		public void Clear()
		{
			map.Clear();
		}

		public bool Contains(object o)
		{
			return map[o] == DumpValue;
		}

		public bool Remove(object o)
		{
			object tempObject = map[o];
			map.Remove(o);
			return tempObject == DumpValue;
		}

		public void CopyTo(object[] array, int index)
		{
			map.CopyTo(array, index);
		}

		public IEnumerator<object> GetEnumerator()
		{
			return new EnumeratorAdapter<object>(map.GetEnumerator());
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public int Count
		{
			get { return map.Count; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}


		#region Implementation of ISet<object>

		public void UnionWith(IEnumerable<object> other)
		{
			throw new NotImplementedException();

			//foreach (var o in other)
			//    Add(o);
		}

		public void IntersectWith(IEnumerable<object> other)
		{
			throw new NotImplementedException();

			// Potential crude implementation.
			//var otherSet = new HashSet<object>(other, new IdentityEqualityComparer());
			//var ours = map.Keys.Cast<object>().ToList();

			//foreach (var key in ours)
			//    if (!otherSet.Contains(key))
			//        map.Remove(key);

			//foreach (var obj in otherSet)
			//    Add(obj);
		}

		public void ExceptWith(IEnumerable<object> other)
		{
			throw new NotImplementedException();
		}

		public void SymmetricExceptWith(IEnumerable<object> other)
		{
			throw new NotImplementedException();
		}

		public bool IsSubsetOf(IEnumerable<object> other)
		{
			throw new NotImplementedException();
		}

		public bool IsSupersetOf(IEnumerable<object> other)
		{
			throw new NotImplementedException();
		}

		public bool IsProperSupersetOf(IEnumerable<object> other)
		{
			throw new NotImplementedException();
		}

		public bool IsProperSubsetOf(IEnumerable<object> other)
		{
			throw new NotImplementedException();
		}

		public bool Overlaps(IEnumerable<object> other)
		{
			throw new NotImplementedException();
		}

		public bool SetEquals(IEnumerable<object> other)
		{
			throw new NotImplementedException();
		}

		#endregion

	}
}
