using System;
using System.Collections;

using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Persister.Collection;

namespace NHibernate.Type
{
	/// <summary>
	/// Extends the <see cref="MapType" /> to provide sorting.
	/// </summary>
	public class SortedMapType : MapType
	{
		private IComparer comparer;

		/// <summary>
		/// Initializes a new instance of a <see cref="SortedMapType"/> class for
		/// a specific role using the <see cref="IComparer"/> to do the sorting.
		/// </summary>
		/// <param name="role">The role the persistent collection is in.</param>
		/// <param name="comparer">The <see cref="IComparer"/> to use for the sorting.</param>
		public SortedMapType( string role, string propertyRef, IComparer comparer )
			: base( role, propertyRef )
		{
			this.comparer = comparer;
		}

		/// <summary>
		/// Instantiates a new <see cref="IPersistentCollection"/> for the sorted map.
		/// </summary>
		/// <param name="session">The current <see cref="ISessionImplementor"/> for the map.</param>
		/// <param name="persister"></param>
		/// <returns></returns>
		public override IPersistentCollection Instantiate( ISessionImplementor session, ICollectionPersister persister )
		{
			SortedMap sortedMap = new SortedMap( session, comparer );
			return sortedMap;
		}

		//public System.Type ReturnedClass {get;} -> was overridden in H2.0.3
		// because they have different Interfaces for Sorted/UnSorted - since .NET
		// doesn't have that I don't need to override it.

		/// <summary>
		/// Wraps an <see cref="IDictionary"/> in a <see cref="SortedMap"/>.
		/// </summary>
		/// <param name="session">The <see cref="ISessionImplementor"/> for the collection to be a part of.</param>
		/// <param name="collection">The unwrapped <see cref="IDictionary"/>.</param>
		/// <returns>
		/// An <see cref="SortedMap"/> that wraps the non NHibernate <see cref="IDictionary"/>.
		/// </returns>
		public override IPersistentCollection Wrap( ISessionImplementor session, object collection )
		{
			return new SortedMap( session, ( IDictionary ) collection, comparer );
		}
	}
}