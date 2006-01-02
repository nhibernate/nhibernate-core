using System.Collections;
using NHibernate.Collection;
using NHibernate.Engine;

namespace NHibernate.Type
{
	/// <summary>
	/// An <see cref="IType"/> that maps an <see cref="IList"/> collection
	/// using bag semantics with an identifier to the database.
	/// </summary>
	public class IdentifierBagType : PersistentCollectionType
	{
		/// <summary>
		/// Initializes a new instance of a <see cref="IdentifierBagType"/> class for
		/// a specific role.
		/// </summary>
		/// <param name="role">The role the persistent collection is in.</param>
		public IdentifierBagType( string role ) : base( role )
		{
		}

		/// <summary>
		/// Instantiates a new <see cref="IPersistentCollection"/> for the identifier bag.
		/// </summary>
		/// <param name="session">The current <see cref="ISessionImplementor"/> for the identifier bag.</param>
		/// <param name="persister"></param>
		/// <returns></returns>
		public override IPersistentCollection Instantiate( ISessionImplementor session, ICollectionPersister persister )
		{
			return new IdentifierBag( session );
		}

		/// <summary></summary>
		public override System.Type ReturnedClass
		{
			get { return typeof( ICollection ); }
		}

		/// <summary>
		/// Wraps an <see cref="IList"/> in a <see cref="IdentifierBag"/>.
		/// </summary>
		/// <param name="session">The <see cref="ISessionImplementor"/> for the collection to be a part of.</param>
		/// <param name="collection">The unwrapped <see cref="IList"/>.</param>
		/// <returns>
		/// An <see cref="IdentifierBag"/> that wraps the non NHibernate <see cref="IList"/>.
		/// </returns>
		public override IPersistentCollection Wrap( ISessionImplementor session, object collection )
		{
			return new IdentifierBag( session, ( ICollection ) collection );
		}

		protected override void Clear( ICollection collection )
		{
			( ( IList ) collection ).Clear();
		}

		protected override void Add( ICollection collection, object element )
		{
			( ( IList ) collection ).Add( element );
		}

	}
}