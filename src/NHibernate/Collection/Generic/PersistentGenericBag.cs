using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using NHibernate.DebugHelpers;
using NHibernate.Engine;
using NHibernate.Persister.Collection;

namespace NHibernate.Collection.Generic
{
	/// <summary>
	/// An unordered, unkeyed collection that can contain the same element
	/// multiple times. The .NET collections API, has no <c>Bag</c>.
	/// Most developers seem to use <see cref="IList{T}"/> to represent bag semantics,
	/// so NHibernate follows this practice.
	/// </summary>
	/// <typeparam name="T">The type of the element the bag should hold.</typeparam>
	/// <remarks>The underlying collection used is an <see cref="List{T}"/></remarks>
	[Serializable]
	[DebuggerTypeProxy(typeof (CollectionProxy<>))]
	public class PersistentGenericBag<T> : PersistentBag, IList<T>
	{
		// TODO NH: find a way to writeonce (no duplicated code from PersistentBag)

		/* NH considerations:
		 * For various reason we know that the underlining type will be a List<T> or a 
		 * PersistentGenericBag<T>; in both cases the class implement all we need to don't duplicate
		 * all code from PersistentBag.
		 * In the explicit implementation of IList<T> we need to duplicate 
		 * code to take advantage from the better performance the use of generic implementation have 
		 * (mean .NET implementation of the underlining list).
		 * In other cases, where PersistentBag use for example bag.Add, a cast, probably, is more
		 * expensive than .NET original implementation.
		 */
		private IList<T> gbag;

		public PersistentGenericBag() {}
		public PersistentGenericBag(ISessionImplementor session) : base(session) {}

		public PersistentGenericBag(ISessionImplementor session, ICollection<T> coll) : base(session, coll as ICollection)
		{
			gbag = coll as IList<T>;
			if (gbag == null)
			{
				List<T> l = new List<T>(coll);
				gbag = l;
				bag = l;
			}
		}

		protected IList<T> InternalBag
		{
			get { return gbag; }
			set
			{
				gbag = value;
				bag = (IList) gbag;
			}
		}

		public override void BeforeInitialize(ICollectionPersister persister, int anticipatedSize)
		{
			InternalBag = (IList<T>) persister.CollectionType.Instantiate(anticipatedSize);
		}

		#region IList<T> Members

		int IList<T>.IndexOf(T item)
		{
			Read();
			return gbag.IndexOf(item);
		}

		void IList<T>.Insert(int index, T item)
		{
			Write();
			gbag.Insert(index, item);
		}

		T IList<T>.this[int index]
		{
			get
			{
				Read();
				return gbag[index];
			}
			set
			{
				Write();
				gbag[index] = value;
			}
		}

		#endregion

		#region ICollection<T> Members

		void ICollection<T>.Add(T item)
		{
			if (!IsOperationQueueEnabled)
			{
				Write();
				gbag.Add(item);
			}
			else
			{
				QueueOperation(new SimpleAddDelayedOperation(this, item));
			}
		}

		bool ICollection<T>.Contains(T item)
		{
			bool? exists = ReadElementExistence(item);
			return !exists.HasValue ? gbag.Contains(item) : exists.Value;
		}

		void ICollection<T>.CopyTo(T[] array, int arrayIndex)
		{
			for (int i = arrayIndex; i < Count; i++)
			{
				array.SetValue(this[i], i);
			}
		}

		bool ICollection<T>.Remove(T item)
		{
			Initialize(true);
			bool result = gbag.Remove(item);
			if (result)
			{
				Dirty();
			}
			return result;
		}

		#endregion

		#region IEnumerable<T> Members

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			Read();
			return gbag.GetEnumerator();
		}

		#endregion
	}
}