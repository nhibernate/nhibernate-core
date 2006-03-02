#if NET_2_0

/* Copyright © 2002-2004 by Aidant Systems, Inc., and by Jason Smith. */  
using System;
using System.Collections.Generic;


namespace Iesi.Collections.Generic
{
	/// <summary>
	/// <p>Implements an immutable (read-only) <c>Set</c> wrapper.</p>
	/// <p>Although this is advertised as immutable, it really isn't.  Anyone with access to the
	/// <c>basisSet</c> can still change the data-set.  So <c>GetHashCode()</c> is not implemented
	/// for this <c>Set</c>, as is the case for all <c>Set</c> implementations in this library.
	/// This design decision was based on the efficiency of not having to <c>Clone()</c> the 
	/// <c>basisSet</c> every time you wrap a mutable <c>Set</c>.</p>
	/// </summary>
	[Serializable]
	public sealed class ImmutableSet<T> : Set<T>
	{
		private const string ERROR_MESSAGE = "Object is immutable.";
		private ISet<T> mBasisSet;

		internal ISet<T> BasisSet
		{
			get
			{
				return mBasisSet;
			}
		}

		/// <summary>
		/// Constructs an immutable (read-only) <c>Set</c> wrapper.
		/// </summary>
		/// <param name="basisSet">The <c>Set</c> that is wrapped.</param>
		public ImmutableSet(ISet<T> basisSet)
		{
			mBasisSet = basisSet;
		}

		/// <summary>
		/// Adds the specified element to this set if it is not already present.
		/// </summary>
		/// <param name="o">The object to add to the set.</param>
		/// <returns>nothing</returns>
		/// <exception cref="NotSupportedException"> is always thrown</exception>
		public sealed override bool Add(T o)
		{
			throw new NotSupportedException(ERROR_MESSAGE);
		}

		/// <summary>
		/// Adds all the elements in the specified collection to the set if they are not already present.
		/// </summary>
		/// <param name="c">A collection of objects to add to the set.</param>
		/// <returns>nothing</returns>
		/// <exception cref="NotSupportedException"> is always thrown</exception>
		public sealed override bool AddAll(ICollection<T> c)
		{
			throw new NotSupportedException(ERROR_MESSAGE);
		}

		/// <summary>
		/// Removes all objects from the set.
		/// </summary>
		/// <exception cref="NotSupportedException"> is always thrown</exception>
		public sealed override void Clear()
		{
			throw new NotSupportedException(ERROR_MESSAGE);
		}

		/// <summary>
		/// Returns <c>true</c> if this set contains the specified element.
		/// </summary>
		/// <param name="o">The element to look for.</param>
		/// <returns><c>true</c> if this set contains the specified element, <c>false</c> otherwise.</returns>
		public sealed override bool Contains(T o)
		{
			return mBasisSet.Contains(o);
		}

		/// <summary>
		/// Returns <c>true</c> if the set contains all the elements in the specified collection.
		/// </summary>
		/// <param name="c">A collection of objects.</param>
		/// <returns><c>true</c> if the set contains all the elements in the specified collection, <c>false</c> otherwise.</returns>
		public sealed override bool ContainsAll(ICollection<T> c)
		{
			return mBasisSet.ContainsAll(c);
		}

		/// <summary>
		/// Returns <c>true</c> if this set contains no elements.
		/// </summary>
		public sealed override bool IsEmpty
		{
			get{return mBasisSet.IsEmpty;}
		}

		/// <summary>
		/// Removes the specified element from the set.
		/// </summary>
		/// <param name="o">The element to be removed.</param>
		/// <returns>nothing</returns>
		/// <exception cref="NotSupportedException"> is always thrown</exception>
		public sealed override bool Remove(T o)
		{
			throw new NotSupportedException(ERROR_MESSAGE);
		}

		/// <summary>
		/// Remove all the specified elements from this set, if they exist in this set.
		/// </summary>
		/// <param name="c">A collection of elements to remove.</param>
		/// <returns>nothing</returns>
		/// <exception cref="NotSupportedException"> is always thrown</exception>
		public sealed override bool RemoveAll(ICollection<T> c)
		{
			throw new NotSupportedException(ERROR_MESSAGE);
		}

		/// <summary>
		/// Retains only the elements in this set that are contained in the specified collection.
		/// </summary>
		/// <param name="c">Collection that defines the set of elements to be retained.</param>
		/// <returns>nothing</returns>
		/// <exception cref="NotSupportedException"> is always thrown</exception>
		public sealed override bool RetainAll(ICollection<T> c)
		{
			throw new NotSupportedException(ERROR_MESSAGE);
		}

		/// <summary>
		/// Copies the elements in the <c>Set</c> to an array of T. The type of array needs
		/// to be compatible with the objects in the <c>Set</c>, obviously.
		/// </summary>
		/// <param name="array">An array that will be the target of the copy operation.</param>
		/// <param name="index">The zero-based index where copying will start.</param>
		public sealed override void CopyTo(T[] array, int index)
		{
			mBasisSet.CopyTo(array, index);
		}

		/// <summary>
		/// The number of elements contained in this collection.
		/// </summary>
		public sealed override int Count
		{
			get{return mBasisSet.Count;}
		}

		/// <summary>
		/// Returns an object that can be used to synchronize use of the <c>Set</c> across threads.
		/// </summary>
		public sealed override bool IsSynchronized
		{
			get
			{
				return ((System.Collections.ICollection)mBasisSet).IsSynchronized;
			}
		}

		/// <summary>
			/// Returns an object that can be used to synchronize the <c>Set</c> between threads.
		/// </summary>
		public sealed override object SyncRoot
		{
			get
			{
				return ((System.Collections.ICollection)mBasisSet).SyncRoot;
			}
		}

		/// <summary>
		/// Gets an enumerator for the elements in the <c>Set</c>.
		/// </summary>
		/// <returns>An <c>IEnumerator</c> over the elements in the <c>Set</c>.</returns>
		public sealed override IEnumerator<T> GetEnumerator()
		{
			return mBasisSet.GetEnumerator();
		}

		/// <summary>
		/// Returns a clone of the <c>Set</c> instance.  
		/// </summary>
		/// <returns>A clone of this object.</returns>
		public sealed override object Clone()
		{
			return new ImmutableSet<T>(mBasisSet);
		}

		/// <summary>
		/// Performs a "union" of the two sets, where all the elements
		/// in both sets are present.  That is, the element is included if it is in either <c>a</c> or <c>b</c>.
		/// Neither this set nor the input set are modified during the operation.  The return value
		/// is a <c>Clone()</c> of this set with the extra elements added in.
		/// </summary>
		/// <param name="a">A collection of elements.</param>
		/// <returns>A new <c>Set</c> containing the union of this <c>Set</c> with the specified collection.
		/// Neither of the input objects is modified by the union.</returns>
		public sealed override ISet<T> Union(ISet<T> a)
		{
			ISet<T> m = GetUltimateBasisSet();
			return new ImmutableSet<T>(m.Union(a));
		}

		/// <summary>
		/// Performs an "intersection" of the two sets, where only the elements
		/// that are present in both sets remain.  That is, the element is included if it exists in
		/// both sets.  The <c>Intersect()</c> operation does not modify the input sets.  It returns
		/// a <c>Clone()</c> of this set with the appropriate elements removed.
		/// </summary>
		/// <param name="a">A set of elements.</param>
		/// <returns>The intersection of this set with <c>a</c>.</returns>
		public sealed override ISet<T> Intersect(ISet<T> a)
		{
			ISet<T> m = GetUltimateBasisSet();
			return new ImmutableSet<T>(m.Intersect(a));
		}

		/// <summary>
		/// Performs a "minus" of set <c>b</c> from set <c>a</c>.  This returns a set of all
		/// the elements in set <c>a</c>, removing the elements that are also in set <c>b</c>.
		/// The original sets are not modified during this operation.  The result set is a <c>Clone()</c>
		/// of this <c>Set</c> containing the elements from the operation.
		/// </summary>
		/// <param name="a">A set of elements.</param>
		/// <returns>A set containing the elements from this set with the elements in <c>a</c> removed.</returns>
		public sealed override ISet<T> Minus(ISet<T> a)
		{
			ISet<T> m = GetUltimateBasisSet();
			return new ImmutableSet<T>(m.Minus(a));
		}

		/// <summary>
		/// Performs an "exclusive-or" of the two sets, keeping only the elements that
		/// are in one of the sets, but not in both.  The original sets are not modified
		/// during this operation.  The result set is a <c>Clone()</c> of this set containing
		/// the elements from the exclusive-or operation.
		/// </summary>
		/// <param name="a">A set of elements.</param>
		/// <returns>A set containing the result of <c>a ^ b</c>.</returns>
		public sealed override ISet<T> ExclusiveOr(ISet<T> a)
		{
			ISet<T> m = GetUltimateBasisSet();
			return new ImmutableSet<T>(m.ExclusiveOr(a));
		}

		/// <summary>
		/// Indicates that the given instance is read-only
		/// </summary>
		public sealed override bool IsReadOnly
		{
			get { return true; }
		}

		/// <summary>
		/// Performs CopyTo when called trhough non-generic ISet (ICollection) interface
		/// </summary>
		/// <param name="array"></param>
		/// <param name="index"></param>
		protected override void NonGenericCopyTo(Array array, int index)
		{
			((System.Collections.ICollection)this.BasisSet).CopyTo(array, index);
		}

		/// <summary>
		/// Performs Union when called trhough non-generic ISet interface
		/// </summary>
		/// <param name="a"></param>
		/// <returns></returns>
		protected sealed override ISet NonGenericUnion(ISet a)
		{
			ISet<T> m = GetUltimateBasisSet();
			return new ImmutableSet(((ISet)m).Union(a));
		}

		/// <summary>
		/// Performs Minus when called trhough non-generic ISet interface
		/// </summary>
		/// <param name="a"></param>
		/// <returns></returns>
		protected sealed override ISet NonGenericMinus(ISet a)
		{
			ISet<T> m = GetUltimateBasisSet();
			return new ImmutableSet(((ISet)m).Minus(a));
		}

		/// <summary>
		/// Performs Intersect when called trhough non-generic ISet interface
		/// </summary>
		/// <param name="a"></param>
		/// <returns></returns>
		protected sealed override ISet NonGenericIntersect(ISet a)
		{
			ISet<T> m = GetUltimateBasisSet();
			return new ImmutableSet(((ISet)m).Intersect(a));
		}

		/// <summary>
		/// Performs ExclusiveOr when called trhough non-generic ISet interface
		/// </summary>
		/// <param name="a"></param>
		/// <returns></returns>
		protected sealed override ISet NonGenericExclusiveOr(ISet a)
		{
			ISet<T> m = GetUltimateBasisSet();
			return new ImmutableSet(((ISet)m).ExclusiveOr(a));
		}

		private ISet<T> GetUltimateBasisSet()
		{
			ISet<T> m = this.mBasisSet;
			while (m is ImmutableSet<T>)
				m = ((ImmutableSet<T>)m).mBasisSet;
			return m;
		}
	}
}
#endif
