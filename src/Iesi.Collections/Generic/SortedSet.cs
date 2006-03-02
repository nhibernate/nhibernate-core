#if NET_2_0

/* Copyright © 2002-2004 by Aidant Systems, Inc., and by Jason Smith. */ 
using System;
using System.Collections.Generic;

namespace Iesi.Collections.Generic
{
	/// <summary>
	/// Implements a <c>Set</c> based on a sorted tree.  This gives good performance for operations on very
	/// large data-sets, though not as good - asymptotically - as a <c>HashedSet</c>.  However, iteration
	/// occurs in order.  Elements that you put into this type of collection must implement <c>IComparable</c>,
	/// and they must actually be comparable.  You can't mix <c>string</c> and <c>int</c> values, for example.
	/// </summary>
	[Serializable]
	public class SortedSet<T> : DictionarySet<T>
	{
		/// <summary>
		/// Creates a new set instance based on a sorted tree.
		/// </summary>
		public SortedSet()
		{
			InternalDictionary = new SortedDictionary<T, object>();
		}

		/// <summary>
		/// Creates a new set instance based on a sorted tree.
		/// </summary>
		/// <param name="comparer">The <see cref="IComparer&lt;T&gt;"/> to use for sorting.</param>
		public SortedSet(IComparer<T> comparer) 
		{
			InternalDictionary = new SortedList<T, object>(comparer);
		}

		/// <summary>
		/// Creates a new set instance based on a sorted tree and
		/// initializes it based on a collection of elements.
		/// </summary>
		/// <param name="initialValues">A collection of elements that defines the initial set contents.</param>
		public SortedSet(ICollection<T> initialValues)
			: this()
		{
			this.AddAll(initialValues);
		}


		/// <summary>
		/// Creates a new set instance based on a sorted tree and
		/// initializes it based on a collection of elements.
		/// </summary>
		/// <param name="initialValues">A collection of elements that defines the initial set contents.</param>
		public SortedSet(System.Collections.ICollection initialValues)
			: this()
		{
			((ISet)this).AddAll(initialValues);
		}

		/// <summary>
		/// Creates a new set instance based on a sorted tree and
		/// initializes it based on a collection of elements.
		/// </summary>
		/// <param name="initialValues">A collection of elements that defines the initial set contents.</param>
		/// <param name="comparer">The <see cref="IComparer&lt;T&gt;"/> to use for sorting.</param>
		public SortedSet(ICollection<T> initialValues, IComparer<T> comparer)
			: this(comparer)
		{
			this.AddAll(initialValues);
		}
	}
}
#endif
