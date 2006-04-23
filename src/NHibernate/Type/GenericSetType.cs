#if NET_2_0

using System;
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
	/// An <see cref="IType"/> that maps an <see cref="ISet&lt;T&gt;"/> collection
	/// to the database.
	/// </summary>
	public class GenericSetType<T> : SetType
	{
		/// <summary>
		/// Initializes a new instance of a <see cref="GenericSetType"/> class for
		/// a specific role.
		/// </summary>
		/// <param name="role">The role the persistent collection is in.</param>
		public GenericSetType( string role, string propertyRef )
			: base( role, propertyRef )
		{
		}

		/// <summary>
		/// Instantiates a new <see cref="IPersistentCollection"/> for the set.
		/// </summary>
		/// <param name="session">The current <see cref="ISessionImplementor"/> for the set.</param>
		public override IPersistentCollection Instantiate( ISessionImplementor session, ICollectionPersister persister )
		{
			return new PersistentGenericSet<T>( session );
		}

		public override System.Type ReturnedClass
		{
			get { return typeof( ISet<T> ); }
		}

		/// <summary>
		/// Wraps an <see cref="IList&lt;T&gt;"/> in a <see cref="PersistentGenericSet&lt;T&gt;"/>.
		/// </summary>
		/// <param name="session">The <see cref="ISessionImplementor"/> for the collection to be a part of.</param>
		/// <param name="collection">The unwrapped <see cref="IList&lt;T&gt;"/>.</param>
		/// <returns>
		/// An <see cref="PersistentGenericSet&lt;T&gt;"/> that wraps the non NHibernate <see cref="IList&lt;T&gt;"/>.
		/// </returns>
		public override IPersistentCollection Wrap( ISessionImplementor session, object collection )
		{
			return new PersistentGenericSet<T>( session, ( ISet<T> ) collection );
		}

		public override object Instantiate()
		{
			return new HashedSet<T>();
		}
	}
}
#endif