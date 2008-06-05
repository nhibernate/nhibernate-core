using System;
using System.Collections;

namespace NHibernate.Type
{
	/// <summary>
	/// Extends the <see cref="MapType" /> to provide sorting.
	/// </summary>
	[Serializable]
	public class SortedMapType : MapType
	{
		private readonly IComparer comparer;

		/// <summary>
		/// Initializes a new instance of a <see cref="SortedMapType"/> class for
		/// a specific role using the <see cref="IComparer"/> to do the sorting.
		/// </summary>
		/// <param name="role">The role the persistent collection is in.</param>
		/// <param name="propertyRef">The name of the property in the
		/// owner object containing the collection ID, or <see langword="null" /> if it is
		/// the primary key.</param>
		/// <param name="comparer">The <see cref="IComparer"/> to use for the sorting.</param>
		/// <param name="isEmbeddedInXML"></param>
		public SortedMapType(string role, string propertyRef, IComparer comparer, bool isEmbeddedInXML)
			: base(role, propertyRef, isEmbeddedInXML)
		{
			this.comparer = comparer;
		}


		public IComparer Comparer
		{
			get { return comparer; }
		}

		public override object Instantiate(int anticipatedSize)
		{
			return new SortedList(comparer);
		}

		public override System.Type ReturnedClass
		{
			get { return typeof (SortedList); }
		}
	}
}