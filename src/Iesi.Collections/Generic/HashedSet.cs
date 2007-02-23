#if NET_2_0

/* Copyright © 2002-2004 by Aidant Systems, Inc., and by Jason Smith. */
using System;
using System.Collections.Generic;

namespace Iesi.Collections.Generic
{
	/// <summary>
	/// Implements a <c>Set</c> based on a Dictionary (which is equivalent of 
	/// non-genric <c>HashTable</c>) This will give the best lookup, add, and remove
	/// performance for very large data-sets, but iteration will occur in no particular order.
	/// </summary>
	[Serializable]
	public class HashedSet<T> : DictionarySet<T>
	{
		/// <summary>
		/// Creates a new set instance based on a Dictinary.
		/// </summary>
		public HashedSet()
		{
			InternalDictionary = new Dictionary<T, object>();
		}

		/// <summary>
		/// Creates a new set instance based on a Dictinary and
		/// initializes it based on a collection of elements.
		/// </summary>
		/// <param name="initialValues">A collection of elements that defines the initial set contents.</param>
		public HashedSet(ICollection<T> initialValues) : this()
		{
			this.AddAll(initialValues);
		}
	}
}

#endif