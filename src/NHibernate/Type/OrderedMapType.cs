using System;
using System.Collections.Specialized;

using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Persister.Collection;

namespace NHibernate.Type
{
	/// <summary>
	/// A <see cref="MapType" /> implemented using a collection that maintains
	/// the order in which elements are inserted into it.
	/// </summary>
	public class OrderedMapType : MapType
	{
		/// <summary>
		/// Initializes a new instance of a <see cref="SortedMapType"/> class for
		/// a specific role using the <see cref="IComparer"/> to do the sorting.
		/// </summary>
		/// <param name="role">The role the persistent collection is in.</param>
		/// <param name="comparer">The <see cref="IComparer"/> to use for the sorting.</param>
		public OrderedMapType( string role, string propertyRef )
			: base( role, propertyRef )
		{
		}

		public override object Instantiate()
		{
			return new ListDictionary();
		}
	}
}