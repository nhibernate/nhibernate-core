using System;
using System.Collections.Generic;

namespace Iesi.Collections.Generic
{
	/// <summary>
	/// Implements an ordered <c>Set</c> based on a dictionary.
	/// </summary>
	[Serializable]
	public class OrderedSet<T> : DictionarySet<T>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="OrderedSet{T}" /> class.
		/// </summary>
		public OrderedSet()
		{
			InternalDictionary = new Dictionary<T, object>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="OrderedSet{T}"/> class.
		/// </summary>
		/// <param name="initialValues">A collection of elements that defines the initial set contents.</param>
		public OrderedSet(ICollection<T> initialValues)
			: this()
		{
			AddAll(initialValues);
		}
	}
}
