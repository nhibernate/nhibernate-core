using System;

using Iesi.Collections;

using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Persister.Collection;

namespace NHibernate.Type
{
	/// <summary>
	/// A <see cref="SetType" /> implemented using a collection that maintains
	/// the order in which elements are inserted into it.
	/// </summary>
	public class OrderedSetType : SetType
	{
		/// <summary>
		/// Initializes a new instance of a <see cref="SortedMapType"/> class for
		/// a specific role using the <see cref="IComparer"/> to do the sorting.
		/// </summary>
		/// <param name="role">The role the persistent collection is in.</param>
		/// <param name="comparer">The <see cref="IComparer"/> to use for the sorting.</param>
		public OrderedSetType( string role, string propertyRef )
			: base( role, propertyRef )
		{
		}

		public override object Instantiate()
		{
			return new ListSet();
		}
	}
}