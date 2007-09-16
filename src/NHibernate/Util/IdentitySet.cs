using System;
using System.Collections;
using Iesi.Collections;

namespace NHibernate.Util
{
	/// <summary> 
	/// Set implementation that use == instead of equals() as its comparison mechanism
	/// that base its implementation of IdentityMap
	/// </summary>
	public class IdentitySet : Set
	{
		private IDictionary map;
		private static readonly object DumpValue = new object();

		public IdentitySet()
		{
			map = IdentityMap.Instantiate(10);
		}

		public override bool Add(object o)
		{
			object tempObject = map[o];
			map[o] = DumpValue;
			return tempObject == null;
		}

		public override bool AddAll(ICollection c)
		{
			bool changed = false;

			foreach (object o in c)
				changed |= Add(o);

			return changed;
		}

		public override void Clear()
		{
			map.Clear();
		}

		public override bool Contains(object o)
		{
			return map[o] == DumpValue;
		}

		public override bool ContainsAll(ICollection c)
		{
			foreach (object o in c)
			{
				if(!map.Contains(o))
					return false;
			}
			return true;
		}

		public override bool Remove(object o)
		{
			object tempObject = map[o];
			map.Remove(o);
			return tempObject == DumpValue;
		}

		public override bool RemoveAll(ICollection c)
		{
			bool changed = false;
			foreach (object o in c)
			{
				changed |= Contains(o);
				Remove(o);
			}
			return changed;
		}

		public override bool RetainAll(ICollection c)
		{
			//doable if needed
			throw new NotSupportedException();
		}

		public override void CopyTo(Array array, int index)
		{
			map.CopyTo(array, index);
		}

		public override IEnumerator GetEnumerator()
		{
			return map.GetEnumerator();
		}

		public override bool IsEmpty
		{
			get { return map.Count == 0; }
		}

		public override int Count
		{
			get { return map.Count; }
		}

		public override bool IsSynchronized
		{
			get { return false; }
		}

		public override object SyncRoot
		{
			get { return this; }
		}
	}
}
