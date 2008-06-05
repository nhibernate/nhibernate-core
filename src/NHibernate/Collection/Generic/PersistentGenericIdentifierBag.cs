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
	/// Implements "bag" semantics more efficiently than <see cref="PersistentBag" /> by adding
	/// a synthetic identifier column to the table.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The identifier is unique for all rows in the table, allowing very efficient
	/// updates and deletes.  The value of the identifier is never exposed to the 
	/// application. 
	/// </para>
	/// <para>
	/// Identifier bags may not be used for a many-to-one association.  Furthermore,
	/// there is no reason to use <c>inverse="true"</c>.
	/// </para>
	/// </remarks>
	[Serializable]
	[DebuggerTypeProxy(typeof (CollectionProxy<>))]
	public class PersistentIdentifierBag<T> : PersistentIdentifierBag, IList<T>
	{
		// TODO NH: find a way to writeonce (no duplicated code from PersistentIdentifierBag)

		/* NH considerations:
		 * For various reason we know that the underlining type will be a List<T> or a 
		 * PersistentGenericBag<T>; in both cases the class implement all we need to don't duplicate
		 * many code from PersistentBag.
		 * In the explicit implementation of IList<T> we need to duplicate code to take advantage
		 * from the better performance the use of generic implementation have.
		 */
		private IList<T> gvalues;
		public PersistentIdentifierBag() {}
		public PersistentIdentifierBag(ISessionImplementor session) : base(session) {}

		public PersistentIdentifierBag(ISessionImplementor session, ICollection<T> coll) : base(session, coll as ICollection)
		{
			gvalues = coll as IList<T>;
			if (gvalues == null)
			{
				List<T> l = new List<T>(coll);
				gvalues = l;
				values = l;
			}
		}

		protected IList<T> InternalValues
		{
			get { return gvalues; }
			set
			{
				gvalues = value;
				values = (IList) gvalues;
			}
		}

		public override void BeforeInitialize(ICollectionPersister persister, int anticipatedSize)
		{
			identifiers = anticipatedSize <= 0 ? new Dictionary<int, object>() : new Dictionary<int, object>(anticipatedSize + 1);
			InternalValues = anticipatedSize <= 0 ? new List<T>() : new List<T>(anticipatedSize);
		}

		#region IList<T> Members

		int IList<T>.IndexOf(T item)
		{
			Read();
			return gvalues.IndexOf(item);
		}

		void IList<T>.Insert(int index, T item)
		{
			Write();
			BeforeAdd(index);
			gvalues.Insert(index, item);
		}

		T IList<T>.this[int index]
		{
			get
			{
				Read();
				return gvalues[index];
			}
			set
			{
				Write();
				gvalues[index] = value;
			}
		}

		#endregion

		#region ICollection<T> Members

		void ICollection<T>.Add(T item)
		{
			Write();
			gvalues.Add(item);
		}

		bool ICollection<T>.Contains(T item)
		{
			Read();
			return gvalues.Contains(item);
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
			int index = gvalues.IndexOf(item);
			if (index >= 0)
			{
				BeforeRemove(index);
				gvalues.RemoveAt(index);
				Dirty();
				return true;
			}
			return false;
		}

		#endregion

		#region IEnumerable<T> Members

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			Read();
			return gvalues.GetEnumerator();
		}

		#endregion
	}
}