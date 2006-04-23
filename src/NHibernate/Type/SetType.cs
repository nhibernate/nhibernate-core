using System;
using System.Collections;

using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Persister.Collection;
using Iesi.Collections;

namespace NHibernate.Type
{
	/// <summary>
	/// An <see cref="IType"/> that maps an <see cref="Iesi.Collections.ISet"/> collection
	/// to the database.
	/// </summary>
	public class SetType : CollectionType
	{
		/// <summary>
		/// Initializes a new instance of a <see cref="SetType"/> class for
		/// a specific role.
		/// </summary>
		/// <param name="role">The role the persistent collection is in.</param>
		public SetType( string role, string propertyRef )
			: base( role, propertyRef )
		{
		}

		/// <summary>
		/// Instantiates a new <see cref="IPersistentCollection"/> for the set.
		/// </summary>
		/// <param name="session">The current <see cref="ISessionImplementor"/> for the set.</param>
		/// <param name="persister"></param>
		/// <returns></returns>
		public override IPersistentCollection Instantiate( ISessionImplementor session, ICollectionPersister persister )
		{
			return new PersistentSet( session );
		}

		/// <summary>
		/// <see cref="AbstractType.ReturnedClass"/>
		/// </summary>
		public override System.Type ReturnedClass
		{
			get { return typeof( Iesi.Collections.ISet ); }
		}

		/// <summary>
		/// Wraps an <see cref="Iesi.Collections.ISet"/> in a <see cref="PersistentSet"/>.
		/// </summary>
		/// <param name="session">The <see cref="ISessionImplementor"/> for the collection to be a part of.</param>
		/// <param name="collection">The unwrapped <see cref="Iesi.Collections.ISet"/>.</param>
		/// <returns>
		/// An <see cref="PersistentSet"/> that wraps the non NHibernate <see cref="Iesi.Collections.ISet"/>.
		/// </returns>
		public override IPersistentCollection Wrap( ISessionImplementor session, object collection )
		{
			return new PersistentSet( session, ( Iesi.Collections.ISet ) collection );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="collection"></param>
		/// <returns></returns>
		public override ICollection GetElementsCollection( object collection )
		{
			return ( ICollection ) collection;
		}

		protected override void Add( ICollection collection, object element )
		{
			( ( Iesi.Collections.ISet ) collection ).Add( element );
		}

		protected override void Clear( ICollection collection )
		{
			( ( Iesi.Collections.ISet ) collection ).Clear();
		}

		public override object Instantiate()
		{
			return new HashedSet();
		}
	}
}