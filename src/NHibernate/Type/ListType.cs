using System.Collections;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Persister.Collection;

namespace NHibernate.Type
{
	/// <summary>
	/// An <see cref="IType"/> that maps an <see cref="IList"/> collection
	/// using list semantics to the database.
	/// </summary>
	public class ListType : PersistentCollectionType
	{
		/// <summary>
		/// Initializes a new instance of a <see cref="ListType"/> class for
		/// a specific role.
		/// </summary>
		/// <param name="role">The role the persistent collection is in.</param>
		public ListType( string role, string propertyRef )
			: base( role, propertyRef )
		{
		}

		/// <summary>
		/// Instantiates a new <see cref="IPersistentCollection"/> for the bag.
		/// </summary>
		/// <param name="session">The current <see cref="ISessionImplementor"/> for the bag.</param>
		/// <param name="persister"></param>
		/// <returns>A new <see cref="NHibernate.Collection.List"/>.</returns>
		public override IPersistentCollection Instantiate(ISessionImplementor session, ICollectionPersister persister)
		{
			return new List( session );
		}

		/// <summary></summary>
		public override System.Type ReturnedClass
		{
			get { return typeof( IList ); }
		}

		/// <summary>
		/// Wraps an exist <see cref="IList"/> in a NHibernate <see cref="List"/>.
		/// </summary>
		/// <param name="session">The <see cref="ISessionImplementor"/> for the collection to be a part of.</param>
		/// <param name="collection">The unwrapped <see cref="IList"/>.</param>
		/// <returns>
		/// An <see cref="List"/> that wraps the non NHibernate <see cref="IList"/>.
		/// </returns>
		public override IPersistentCollection Wrap(ISessionImplementor session, object collection)
		{
			return new List( session, ( IList ) collection );
		}

		protected override void Add( ICollection collection, object element )
		{
			( ( IList ) collection ).Add( element );
		}

		protected override void Clear( ICollection collection )
		{
			( ( IList ) collection ).Clear();
		}

	}
}