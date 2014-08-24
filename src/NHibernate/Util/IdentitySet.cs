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


#if !NET_4_0   // Only in Iesi's ISet<>.
		public bool AddAll(ICollection<object> c)
		{
			bool changed = false;

			foreach (object o in c)
				changed |= Add(o);

			return changed;
		}

		public bool ContainsAll(ICollection<object> c)
		{
			foreach (object o in c)
			{
				if (!map.Contains(o))
					return false;
			}
			return true;
		}

		public bool RemoveAll(ICollection<object> c)
		{
			bool changed = false;
			foreach (object o in c)
			{
				changed |= Contains(o);
				Remove(o);
			}
			return changed;
		}

		public bool RetainAll(ICollection<object> c)
		{
			//doable if needed
			throw new NotSupportedException();
		}

		protected void NonGenericCopyTo(Array array, int index)
		{
			map.CopyTo(array, index);
		}

		public bool IsEmpty
		{
			get { return map.Count == 0; }
		}

		public bool IsSynchronized
		{
			get { return false; }
		}

		public object SyncRoot
		{
			get { return this; }
		}

		#region Implementation of ICloneable

		public object Clone()
		{
			return new IdentitySet(this);
		}

		#endregion

		#region Implementation of ISet<object>

		public ISet<object> Union(ISet<object> a)
		{
			return new IdentitySet(this.Concat(a));
		}

		public ISet<object> Intersect(ISet<object> a)
		{
			// Be careful to use the Contains() implementation of the IdentitySet,
			// not the one from the other set.
			var elems = a.Where(e => Contains(a));
			return new IdentitySet(elems);
		}

		public ISet<object> Minus(ISet<object> a)
		{
			var set = new IdentitySet(this);
			set.RemoveAll(a);
			return set;
		}

		public ISet<object> ExclusiveOr(ISet<object> a)
		{
			return Union(a).Minus(Intersect(a));
		}

		#endregion

#endif

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

#if NET_4_0

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

#endif

	}
}
