using System.Collections;
using System.Data;
using NHibernate.Engine;
using NHibernate.Persister;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.Loader
{
	/// <summary>
	/// Load an entity using outerjoin fetching to fetch associated entities.
	/// </summary>
	/// <remarks>
	/// The <tt>ClassPersister</tt> must implement <tt>ILoadable</tt>. For other entities,
	/// create a customized subclass of <tt>Loader</tt>.
	/// </remarks>
	public class EntityLoader : AbstractEntityLoader, IUniqueEntityLoader
	{
		private readonly IType uniqueKeyType;
		private readonly bool batchLoader;

		/// <summary>
		/// 
		/// </summary>C:\Devel\Sourceforge\NHibernate\nhibernate\src\NHibernate\Loader\EntityLoader.cs
		/// <param name="persister"></param>
		/// <param name="batchSize"></param>
		/// <param name="factory"></param>
		public EntityLoader( IOuterJoinLoadable persister, int batchSize, ISessionFactoryImplementor factory ) : this( persister, persister.IdentifierColumnNames, persister.IdentifierType, batchSize, factory )
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="persister"></param>
		/// <param name="uniqueKey"></param>
		/// <param name="uniqueKeyType"></param>
		/// <param name="batchSize"></param>
		/// <param name="factory"></param>
		public EntityLoader( IOuterJoinLoadable persister, string[] uniqueKey, IType uniqueKeyType, int batchSize, ISessionFactoryImplementor factory ) : base( persister, factory )
		{
			this.uniqueKeyType = uniqueKeyType;

			IList associations = WalkTree( persister, Alias, factory );
			InitClassPersisters( associations );
			SqlString whereString = WhereString( factory, Alias, uniqueKey, uniqueKeyType, batchSize );
			RenderStatement( associations, whereString, factory );

			PostInstantiate();

			batchLoader = batchSize > 1;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="session"></param>
		/// <param name="id"></param>
		/// <param name="optionalObject"></param>
		/// <returns></returns>
		public object Load( ISessionImplementor session, object id, object optionalObject )
		{
			return Load( session, id, optionalObject, id );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="session"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		public object LoadByUniqueKey( ISessionImplementor session, object id )
		{
			return Load( session, id, null, null );
		}
	
		/// <summary>
		/// 
		/// </summary>
		/// <param name="session"></param>
		/// <param name="id"></param>
		/// <param name="optionalObject"></param>
		/// <param name="optionalId"></param>
		/// <returns></returns>
		public object Load( ISessionImplementor session, object id, object optionalObject, object optionalId )
		{
			IList list = LoadEntity( session, id, uniqueKeyType, optionalObject, optionalId );
			if( list.Count == 1 )
			{
				return list[ 0 ];
			}
			else if( list.Count == 0 )
			{
				return null;
			}
			else
			{
				if ( CollectionOwner > -1 )
				{
					return list[ 0 ];
				}
				else
				{
					throw new HibernateException(
						"More than one row with the given identifier was found: " +
						id +
						", for class: " +
						Persister.ClassName );
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="row"></param>
		/// <param name="rs"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		protected override object GetResultColumnOrRow( object[ ] row, IDataReader rs, ISessionImplementor session )
		{
			return row[ row.Length - 1 ];
		}

		/// <summary></summary>
		protected override bool IsSingleRowLoader
		{
			get { return !batchLoader; }
		}
	}
}