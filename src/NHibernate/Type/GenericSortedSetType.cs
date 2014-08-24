using System;
using System.Collections.Generic;

namespace NHibernate.Type
{
	/// <summary>
	/// An <see cref="IType"/> that maps a sorted <see cref="ISet{T}"/> collection
	/// to the database.
	/// </summary>
	[Serializable]
	public class GenericSortedSetType<T> : GenericSetType<T>
	{
		private readonly IComparer<T> comparer;

		/// <summary>
		/// Initializes a new instance of a <see cref="GenericSortedSetType{T}"/> class for
		/// a specific role.
		/// </summary>
		/// <param name="role">The role the persistent collection is in.</param>
		/// <param name="propertyRef">The name of the property in the
		/// owner object containing the collection ID, or <see langword="null" /> if it is
		/// the primary key.</param>
		/// <param name="comparer">The <see cref="IComparer{T}" /> to use to compare
		/// set elements.</param>
		public GenericSortedSetType(string role, string propertyRef, IComparer<T> comparer)
			: base(role, propertyRef)
		{
			this.comparer = comparer;
		}

		public IComparer<T> Comparer
		{
			get { return comparer; }
		}

		public override object Instantiate(int anticipatedSize)
		{
			return new SortedSet<T>(comparer);
		}
	}
}
