using System.Collections;
using NHibernate.Collection;
using NHibernate.Engine;

namespace NHibernate.Type
{
	/// <summary></summary>
	public class ListType : PersistentCollectionType
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="role"></param>
		public ListType( string role ) : base( role )
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
			return new List( session );
		}

		/// <summary></summary>
		public override System.Type ReturnedClass
		{
			get { return typeof( IList ); }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="session"></param>
		/// <param name="collection"></param>
		/// <returns></returns>
		public override PersistentCollection Wrap( ISessionImplementor session, object collection )
		{
			return new List( session, ( IList ) collection );
		}
	}
}