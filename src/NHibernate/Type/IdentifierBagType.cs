using System.Collections;
using NHibernate.Collection;
using NHibernate.Engine;

namespace NHibernate.Type
{
	/// <summary>
	/// Summary description for IdentifierBagType.
	/// </summary>
	public class IdentifierBagType : PersistentCollectionType
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="role"></param>
		public IdentifierBagType( string role ) : base( role )
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="session"></param>
		/// <param name="persister"></param>
		/// <returns></returns>
		public override PersistentCollection Instantiate( ISessionImplementor session, ICollectionPersister persister )
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
		public override PersistentCollection Wrap( ISessionImplementor session, object collection )
		{
			return new IdentifierBag( session, ( ICollection ) collection );
		}
	}
}