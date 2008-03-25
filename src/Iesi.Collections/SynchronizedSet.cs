/* Copyright © 2002-2004 by Aidant Systems, Inc., and by Jason Smith. */
using System;
using System.Collections;

namespace Iesi.Collections
{
	/// <summary>
	/// Implements a thread-safe <see cref="ISet" /> wrapper.
	/// </summary>
	/// <remarks>
	/// The implementation is extremely conservative, serializing critical sections
	/// to prevent possible deadlocks, and locking on everything. The one exception
	/// is for enumeration, which is inherently not thread-safe.  For this, you have
	/// to <see langword="lock" /> the <see cref="SyncRoot" /> object for the duration
	/// of the enumeration.
	/// </remarks>
	[Serializable]
	public sealed class SynchronizedSet : Set
	{
		private ISet mBasisSet;
		private object mSyncRoot;

		/// <summary>
		/// Constructs a thread-safe <see cref="ISet" /> wrapper.
		/// </summary>
		/// <param name="basisSet">The <see cref="ISet" /> object that this object will wrap.</param>
		public SynchronizedSet(ISet basisSet)
		{
			mBasisSet = basisSet;
			mSyncRoot = basisSet.SyncRoot;
			if (mSyncRoot == null)
				throw new NullReferenceException("The Set you specified returned a null SyncRoot.");
		}

		/// <summary>
		/// Adds the specified element to this set if it is not already present.
		/// </summary>
		/// <param name="o">The object to add to the set.</param>
		/// <returns><see langword="true" /> is the object was added, <see langword="false" /> if it was already present.</returns>
		public override sealed bool Add(object o)
		{
			lock (mSyncRoot)
			{
				return mBasisSet.Add(o);
			}
		}

		/// <summary>
		/// Adds all the elements in the specified collection to the set if they are not already present.
		/// </summary>
		/// <param name="c">A collection of objects to add to the set.</param>
		/// <returns><see langword="true" /> is the set changed as a result of this operation, <see langword="false" /> if not.</returns>
		public override sealed bool AddAll(ICollection c)
		{
			Set temp;
			lock (c.SyncRoot)
			{
				temp = new HybridSet(c);
			}

			lock (mSyncRoot)
			{
				return mBasisSet.AddAll(temp);
			}
		}

		/// <summary>
		/// Removes all objects from the set.
		/// </summary>
		public override sealed void Clear()
		{
			lock (mSyncRoot)
			{
				mBasisSet.Clear();
			}
		}

		/// <summary>
		/// Returns <see langword="true" /> if this set contains the specified element.
		/// </summary>
		/// <param name="o">The element to look for.</param>
		/// <returns><see langword="true" /> if this set contains the specified element, <see langword="false" /> otherwise.</returns>
		public override sealed bool Contains(object o)
		{
			lock (mSyncRoot)
			{
				return mBasisSet.Contains(o);
			}
		}

		/// <summary>
		/// Returns <see langword="true" /> if the set contains all the elements in the specified collection.
		/// </summary>
		/// <param name="c">A collection of objects.</param>
		/// <returns><see langword="true" /> if the set contains all the elements in the specified collection, <see langword="false" /> otherwise.</returns>
		public override sealed bool ContainsAll(ICollection c)
		{
			Set temp;
			lock (c.SyncRoot)
			{
				temp = new HybridSet(c);
			}
			lock (mSyncRoot)
			{
				return mBasisSet.ContainsAll(temp);
			}
		}

		/// <summary>
		/// Returns <see langword="true" /> if this set contains no elements.
		/// </summary>
		public override sealed bool IsEmpty
		{
			get
			{
				lock (mSyncRoot)
				{
					return mBasisSet.IsEmpty;
				}
			}
		}


		/// <summary>
		/// Removes the specified element from the set.
		/// </summary>
		/// <param name="o">The element to be removed.</param>
		/// <returns><see langword="true" /> if the set contained the specified element, <see langword="false" /> otherwise.</returns>
		public override sealed bool Remove(object o)
		{
			lock (mSyncRoot)
			{
				return mBasisSet.Remove(o);
			}
		}

		/// <summary>
		/// Remove all the specified elements from this set, if they exist in this set.
		/// </summary>
		/// <param name="c">A collection of elements to remove.</param>
		/// <returns><see langword="true" /> if the set was modified as a result of this operation.</returns>
		public override sealed bool RemoveAll(ICollection c)
		{
			Set temp;
			lock (c.SyncRoot)
			{
				temp = new HybridSet(c);
			}
			lock (mSyncRoot)
			{
				return mBasisSet.RemoveAll(temp);
			}
		}

		/// <summary>
		/// Retains only the elements in this set that are contained in the specified collection.
		/// </summary>
		/// <param name="c">Collection that defines the set of elements to be retained.</param>
		/// <returns><see langword="true" /> if this set changed as a result of this operation.</returns>
		public override sealed bool RetainAll(ICollection c)
		{
			Set temp;
			lock (c.SyncRoot)
			{
				temp = new HybridSet(c);
			}
			lock (mSyncRoot)
			{
				return mBasisSet.RetainAll(temp);
			}
		}

		/// <summary>
		/// Copies the elements in the set to an array.  The type of array needs
		/// to be compatible with the objects in the set, obviously.
		/// </summary>
		/// <param name="array">An array that will be the target of the copy operation.</param>
		/// <param name="index">The zero-based index where copying will start.</param>
		public override sealed void CopyTo(Array array, int index)
		{
			lock (mSyncRoot)
			{
				mBasisSet.CopyTo(array, index);
			}
		}

		/// <summary>
		/// The number of elements contained in this collection.
		/// </summary>
		public override sealed int Count
		{
			get
			{
				lock (mSyncRoot)
				{
					return mBasisSet.Count;
				}
			}
		}

		/// <summary>
		/// Returns <see langword="true" />, indicating that this object is thread-safe.  The exception to this
		/// is enumeration, which is inherently not thread-safe.  Use the <see cref="SyncRoot" /> object to
		/// lock this object for the entire duration of the enumeration.
		/// </summary>
		public override sealed bool IsSynchronized
		{
			get { return true; }
		}

		/// <summary>
		/// Returns an object that can be used to synchronize the set between threads.
		/// </summary>
		public override sealed object SyncRoot
		{
			get { return mSyncRoot; }
		}

		/// <summary>
		/// Returns an enumerator that iterates through the set.
		/// </summary>
		/// <returns>
		/// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the set.
		/// </returns>
		/// <remarks>
		/// Enumeration is, by definition, not thread-safe.  Use a <see langword="lock" /> on the <see cref="SyncRoot" /> 
		/// to synchronize the entire enumeration process.
		/// </remarks>
		public override sealed IEnumerator GetEnumerator()
		{
			return mBasisSet.GetEnumerator();
		}

		/// <summary>
		/// Returns a clone of this instance.  
		/// </summary>
		/// <returns>A clone of this object.</returns>
		public override object Clone()
		{
			return new SynchronizedSet((ISet) mBasisSet.Clone());
		}
	}
}