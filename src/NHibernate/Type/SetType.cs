using System.Collections;
using Iesi.Collections;
using NHibernate.Collection;
using NHibernate.Engine;
using Set = NHibernate.Collection.Set;

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
		public override PersistentCollection Instantiate( ISessionImplementor session, CollectionPersister persister )
		{
			return new Set( session );
		}

		/// <summary>
		/// <see cref="AbstractType.ReturnedClass"/>
		/// </summary>
		public override System.Type ReturnedClass
		{
			get { return typeof( ISet ); }
		}

		/// <summary>
		/// <see cref="PersistentCollectionType.Wrap"/>
		/// </summary>
		/// <param name="collection"></param>
		/// <param name="session"></param>
		public override PersistentCollection Wrap( ISessionImplementor session, object collection )
		{
			return new Set( session, ( ISet ) collection );
		}

		/// <summary>
		/// <see cref="PersistentCollectionType.AssembleCachedCollection"/>
		/// </summary>
		/// <param name="session"></param>
		/// <param name="persister"></param>
		/// <param name="disassembled"></param>
		/// <param name="owner"></param>
		/// <returns></returns>
		public override PersistentCollection AssembleCachedCollection( ISessionImplementor session, CollectionPersister persister, object disassembled, object owner )
		{
			return new Set( session, persister, disassembled, owner );
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