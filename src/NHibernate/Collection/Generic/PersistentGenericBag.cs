#if NET_2_0

using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.Collection.Generic
{
	/// <summary>
	/// An unordered, unkeyed collection that can contain the same element
	/// multiple times. The .net collections API, has no <c>Bag</c>.
	/// The <see cref="ICollection&lt;T&gt;" /> interface closely resembles bag semantics,
	/// so NHibernate follows this practice.
	/// </summary>
	/// <typeparam name="T">The type of the element the bag should hold.</typeparam>
	/// <remarks>The underlying collection used is an <see cref="List&lt;T&gt;"/></remarks>
	[Serializable]
	class PersistentGenericBag<T> : PersistentCollection, IList<T>
	{
		private IList<T> bag;

		/// <summary>
		/// Initializes an instance of the <see cref="PersistentGenericBag&lt;T&gt;"/>
		/// in the <paramref name="session"/>.
		/// </summary>
		/// <param name="session">The <see cref="ISessionImplementor"/> the bag is in.</param>
        internal PersistentGenericBag(ISessionImplementor session)
			: base(session)
		{
		}

		/// <summary>
		/// Initializes an instance of the <see cref="PersistentGenericBag&lt;T&gt;"/>
		/// that wraps an existing <see cref="IList&lt;T&gt;"/> in the <paramref name="session"/>.
		/// </summary>
		/// <param name="session">The <see cref="ISessionImplementor"/> the bag is in.</param>
		/// <param name="coll">The <see cref="IList&lt;T&gt;"/> to wrap.</param>
		internal PersistentGenericBag(ISessionImplementor session, IList<T> coll)
			: base(session)
		{
			bag = coll;

			if (bag == null)
			{
				bag = new List<T>();
				((List<T>)bag).AddRange(coll);
			}
			SetInitialized();
			IsDirectlyAccessible = true;
		}

		#region ICollection<T> Members

		void ICollection<T>.Add(T item)
		{
			if (!QueueAdd(item))
			{
				Write();
				bag.Add(item);
			}
		}

		void ICollection<T>.Clear()
		{
			Write();
			bag.Clear();
		}

		bool ICollection<T>.Contains(T item)
		{
			Read();
			return bag.Contains(item);
		}

		void ICollection<T>.CopyTo(T[] array, int arrayIndex)
		{
			Read();
			bag.CopyTo(array, arrayIndex);
		}

		int ICollection<T>.Count
		{
			get 
			{
				Read();
				return bag.Count;
			}
		}

		bool ICollection<T>.IsReadOnly
		{
			get { return false; }
		}

		bool ICollection<T>.Remove(T item)
		{
			Write();
			return bag.Remove(item);
		}

		#endregion

		#region IList<T> Members

		public int IndexOf(T item)
		{
			Read();
			return bag.IndexOf(item);
		}

		public void Insert(int index, T item)
		{
			Write();
			bag.Insert(index, item);
		}

		public void RemoveAt(int index)
		{
			Write();
			bag.RemoveAt(index);
		}

		public T this[int index]
		{
			get
			{
				Read();
				return bag[index];
			}
			set
			{
				Write();
				bag[index] = value;
			}
		}

		#endregion

		#region IEnumerable<T> Members

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			Read();
			return bag.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		public override System.Collections.IEnumerator GetEnumerator()
		{
			Read();
			return (System.Collections.IEnumerator)bag.GetEnumerator();
		}

		#endregion

		#region PersistentCollection Members

		/// <summary>
		/// Is the initialized GenericBag empty?
		/// </summary>
		/// <value><c>true</c> if the bag has a Count==0, <c>false</c> otherwise.</value>
		public override bool Empty
		{
			get { return bag.Count == 0; }
		}

		public override void InitializeFromCache(ICollectionPersister persister, object disassembled, object owner)
		{
			BeforeInitialize(persister);
			object[] array = (object[])disassembled;
			for (int i = 0; i < array.Length; i++)
			{
				bag.Add((T)persister.ElementType.Assemble(array[i], Session, owner));
			}
			SetInitialized();
		}

		/// <summary>
		/// Gets a <see cref="Boolean"/> indicating if this Bag needs to be recreated
		/// in the database.
		/// </summary>
		/// <param name="persister">The <see cref="ICollectionPersister"/> for this Collection.</param>
		/// <returns>
		/// <c>false</c> if this is a <c>one-to-many</c> Bag, <c>true</c> if this is not
		/// a <c>one-to-many</c> Bag.  Since a Bag is an unordered, unindexed collection 
		/// that permits duplicates it is not possible to determine what has changed in a
		/// <c>many-to-many</c> so it is just recreated.
		/// </returns>
		public override bool NeedsRecreate(ICollectionPersister persister)
		{
			return !persister.IsOneToMany;
		}

		public override System.Collections.ICollection Entries()
		{
			// TODO: not sure if this is what I want to do - an List<T> is
			// an nongeneric ICollection, but not sure this is good
			return (System.Collections.ICollection)bag;
		}

		public override object ReadFrom(System.Data.IDataReader reader, ICollectionPersister persister, object owner)
		{
			object element = persister.ReadElement(reader, owner, Session);
			// TODO: to make this more net-2.0 friendly the value returned from persister.ReadElement
			// should be specified by a type parameter.  However, that would really break NH with net-1.1
			// and I don't want to do that yet - so the cast is appropriate.
			bag.Add((T)element);
			return element;
		}

		public override void WriteTo(IDbCommand st, ICollectionPersister persister, object entry, int i, bool writeOrder)
		{
			persister.WriteElement(st, entry, writeOrder, Session);
		}

		public override object GetIndex(object entry, int i)
		{
			throw new NotSupportedException("Bags don't have indexes");
		}

		public override void BeforeInitialize(ICollectionPersister persister)
		{
			this.bag = new List<T>();
		}

		public override bool EqualsSnapshot(IType elementType)
		{
			IList<T> sn = (IList<T>)GetSnapshot();
			if (sn.Count != bag.Count)
			{
				return false;
			}

			foreach (T elt in bag)
			{
				if (CountOccurrences(elt, bag, elementType) != CountOccurrences(elt, sn, elementType))
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Counts the number of times that the <paramref name="element"/> occurs
		/// in the <paramref name="list"/>.
		/// </summary>
		/// <param name="element">The element to find in the list.</param>
		/// <param name="list">The <see cref="ICollection&lt;T&gt;"/> to search.</param>
		/// <param name="elementType">The <see cref="IType"/> that can determine equality.</param>
		/// <returns>
		/// The number of occurrences of the element in the list.
		/// </returns>
		private int CountOccurrences(T element, ICollection<T> list, IType elementType)
		{
			int result = 0;
			foreach (T obj in list)
			{
				if (elementType.Equals(element, obj))
				{
					result++;
				}
			}

			return result;
		}

		protected override System.Collections.ICollection Snapshot(ICollectionPersister persister)
		{
			List<T> clonedList = new List<T>();
			foreach (T obj in bag)
			{
				clonedList.Add((T)persister.ElementType.DeepCopy(obj));
			}

			return clonedList;
		}

		public override object Disassemble(ICollectionPersister persister)
		{
			int length = bag.Count;
			object[] result = new object[length];

			int i = 0;
			foreach (T item in bag)
			{
				result[i] = persister.ElementType.Disassemble(item, Session);
				i++;
			}

			return result;
		}

		public override bool EntryExists(object entry, int i)
		{
			return entry != null;
		}

		public override bool NeedsInserting(object entry, int i, IType elemType)
		{
			System.Collections.IList sn = (System.Collections.IList)GetSnapshot();
			if (sn.Count > i && elemType.Equals(sn[i], entry))
			{
				// a shortcut if its location didn't change
				return false;
			}
			else
			{
				//search for it
				foreach (object oldObject in sn)
				{
					if (elemType.Equals(oldObject, entry))
					{
						return false;
					}
				}
				return true;
			}
		}

		public override bool NeedsUpdating(object entry, int i, IType elemType)
		{
			return false;
		}

		public override System.Collections.ICollection GetDeletes(IType elemType)
		{
			System.Collections.ArrayList deletes = new System.Collections.ArrayList();
			System.Collections.IList sn = (System.Collections.IList)GetSnapshot();

			int i = 0;

			foreach (object oldObject in sn)
			{
				bool found = false;
				if (bag.Count > i && elemType.Equals(oldObject, bag[i++]))
				{
					//a shortcut if its location didn't change!
					found = true;
				}
				else
				{
					//search for it
					foreach (object newObject in bag)
					{
						if (elemType.Equals(oldObject, newObject))
						{
							found = true;
							break;
						}
					}
				}
				if (!found)
				{
					deletes.Add(oldObject);
				}
			}

			return deletes;
		}

		/// <summary>
		/// Is this the wrapper for the given underlying bag instance?
		/// </summary>
		/// <param name="collection">The bag that might be wrapped.</param>
		/// <returns>
		/// <c>true</c> if the <paramref name="collection"/> is equal to the
		/// wrapped collection by object reference.
		/// </returns>
		public override bool IsWrapper(object collection)
		{
			return bag == collection;
		}

		public override System.Collections.ICollection GetOrphans(object snapshot)
		{
			System.Collections.IList sn = (System.Collections.IList)snapshot;
			System.Collections.ArrayList result = new System.Collections.ArrayList();
			result.AddRange(sn);
			// HACK: careful with cast here...
			PersistentCollection.IdentityRemoveAll(result, (System.Collections.ICollection)bag, Session);
			return result;
		}

		#region System.Collections.ICollection Members

		public override int Count
		{
			get
			{
				Read();
				return bag.Count;
			}
		}

		public override bool IsSynchronized
		{
			get { return false; }
		}

		public override void CopyTo(Array array, int index)
		{
			Read();
			((System.Collections.ICollection)bag).CopyTo(array, index);
		}

		public override object SyncRoot
		{
			get { return this; }
		}

		#endregion

		#endregion
	}
}
#endif