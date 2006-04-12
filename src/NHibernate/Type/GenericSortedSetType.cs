#if NET_2_0

using System;
using System.Collections.Generic;
using System.Reflection;

using Iesi.Collections.Generic;

using NHibernate.Collection;
using NHibernate.Collection.Generic;
using NHibernate.Engine;
using NHibernate.Util;
using NHibernate.Persister.Collection;

namespace NHibernate.Type
{
	/// <summary>
	/// An <see cref="IType"/> that maps a sorted <see cref="ISet&lt;T&gt;"/> collection
	/// to the database.
	/// </summary>
	public class GenericSortedSetType<T> : GenericSetType<T>
	{
		private IComparer<T> comparer;

		/// <summary>
		/// Initializes a new instance of a <see cref="GenericSortedSetType"/> class for
		/// a specific role.
		/// </summary>
		/// <param name="role">The role the persistent collection is in.</param>
		public GenericSortedSetType( string role, string propertyRef, IComparer<T> comparer )
			: base( role, propertyRef )
		{
			this.comparer = comparer;
		}

		/// <summary>
		/// Instantiates a new <see cref="IPersistentCollection"/> for the set.
		/// </summary>
		/// <param name="session">The current <see cref="ISessionImplementor"/> for the set.</param>
		public override IPersistentCollection Instantiate( ISessionImplementor session, ICollectionPersister persister )
		{
			return new PersistentGenericSortedSet<T>( session, comparer );
		}

		public override System.Type ReturnedClass
		{
			get { return typeof( ISet<T> ); }
		}

		/// <summary>
		/// Wraps an <see cref="ISet&lt;T&gt;"/> in a <see cref="PersistentGenericSortedSet&lt;T&gt;"/>.
		/// </summary>
		/// <param name="session">The <see cref="ISessionImplementor"/> for the collection to be a part of.</param>
		/// <param name="collection">The unwrapped <see cref="ISet&lt;T&gt;"/>.</param>
		/// <returns>
		/// An <see cref="PersistentGenericSortedSet&lt;T&gt;"/> that wraps the non NHibernate <see cref="ISet&lt;T&gt;"/>.
		/// </returns>
		public override IPersistentCollection Wrap( ISessionImplementor session, object collection )
		{
			return new PersistentGenericSortedSet<T>( session, ( ISet<T> ) collection, comparer );
		}
		
		//TODO: Add() & Clear() methods - need to see if these should be refactored back into
		// their own version of Copy or a DoCopy.  The Copy() method used to be spread out amongst
		// the various collections, but since they all had common code Add() and Clear() were made
		// virtual since that was where most of the logic was.  A different/better way might be to
		// have a Copy on the base collection that handles the standard checks and then a DoCopy
		// that performs the actual copy.
	}
}
#endif