using System.Collections;
using NHibernate.Collection;
using NHibernate.Engine;

namespace NHibernate.Type
{
	/// <summary>
	/// Summary description for SetType.
	/// </summary>
	public class SetType : PersistentCollectionType
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="role"></param>
		public SetType( string role ) : base( role )
		{
		}

		/// <summary>
		/// <see cref="PersistentCollectionType.Instantiate"/>
		/// </summary>
		/// <param name="persister"></param>
		/// <param name="session"></param>
		public override PersistentCollection Instantiate( ISessionImplementor session, ICollectionPersister persister )
		{
			return new Set( session );
		}

		/// <summary>
		/// <see cref="AbstractType.ReturnedClass"/>
		/// </summary>
		public override System.Type ReturnedClass
		{
			get { return typeof( Iesi.Collections.ISet ); }
		}

		/// <summary>
		/// Wraps an <see cref="Iesi.Collections.ISet"/> in a <see cref="Set"/>.
		/// </summary>
		/// <param name="session">The <see cref="ISessionImplementor"/> for the collection to be a part of.</param>
		/// <param name="collection">The unwrapped <see cref="Iesi.Collections.ISet"/>.</param>
		/// <returns>
		/// An <see cref="Set"/> that wraps the non NHibernate <see cref="Iesi.Collections.ISet"/>.
		/// </returns>
		public override PersistentCollection Wrap( ISessionImplementor session, object collection )
		{
			return new Set( session, ( Iesi.Collections.ISet ) collection );
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


	}
}