using System;
using System.Collections;

using NHibernate.Collection;
using NHibernate.Engine;

namespace NHibernate.Type
{
	/// <summary>
	/// Extends the <see cref="SetType" /> to provide sorting.
	/// </summary>
	public class SortedSetType : SetType
	{
		private IComparer comparer;

		/// <summary>
		/// Initializes a new instance of a <see cref="SortedSetType"/> class for
		/// a specific role using the <see cref="IComparer"/> to do the sorting.
		/// </summary>
		/// <param name="role">The role the persistent collection is in.</param>
		/// <param name="comparer">The <see cref="IComparer"/> to use for the sorting.</param>
		public SortedSetType( string role, string propertyRef, IComparer comparer )
			: base( role, propertyRef )
		{
			this.comparer = comparer;
		}

		/// <summary>
		/// Instantiates a new <see cref="IPersistentCollection"/> for the sorted set.
		/// </summary>
		/// <param name="session">The current <see cref="ISessionImplementor"/> for the set.</param>
		/// <param name="persister"></param>
		/// <returns></returns>
		public override IPersistentCollection Instantiate( ISessionImplementor session, ICollectionPersister persister )
		{
			SortedSet sortedSet = new SortedSet( session, comparer );
			return sortedSet;
		}

		//public System.Type ReturnedClass {get;} -> was overridden in H2.0.3
		// because they have different Interfaces for Sorted/UnSorted - since .NET
		// doesn't have that I don't need to override it.

		/// <summary>
		/// Wraps an <see cref="Iesi.Collections.ISet"/> in a <see cref="SortedSet"/>.
		/// </summary>
		/// <param name="session">The <see cref="ISessionImplementor"/> for the collection to be a part of.</param>
		/// <param name="collection">The unwrapped <see cref="Iesi.Collections.ISet"/>.</param>
		/// <returns>
		/// An <see cref="SortedSet"/> that wraps the non NHibernate <see cref="Iesi.Collections.ISet"/>.
		/// </returns>
		public override IPersistentCollection Wrap( ISessionImplementor session, object collection )
		{
			return new SortedSet( session, ( Iesi.Collections.ISet ) collection, comparer );

		}
	}
}