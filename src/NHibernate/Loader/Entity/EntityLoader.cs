using System.Collections;
using System.Data;
using NHibernate.Engine;
using NHibernate.Loader.Entity;
using NHibernate.Persister;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.Loader.Entity
{
	/// <summary>
	/// Load an entity using outerjoin fetching to fetch associated entities.
	/// </summary>
	/// <remarks>
	/// The <see cref="IEntityPersister"/> must implement <see cref="ILoadable" />. For other entities,
	/// create a customized subclass of <see cref="Loader" />.
	/// </remarks>
	public class EntityLoader : AbstractEntityLoader, IUniqueEntityLoader
	{
		private readonly IType uniqueKeyType;
		private readonly bool batchLoader;

		public EntityLoader( IOuterJoinLoadable persister, int batchSize, ISessionFactoryImplementor factory )
			: this( persister, persister.IdentifierColumnNames, persister.IdentifierType, batchSize, factory )
		{
		}

		public EntityLoader( IOuterJoinLoadable persister, string[] uniqueKey, IType uniqueKeyType, int batchSize, ISessionFactoryImplementor factory )
			: base( persister, factory )
		{
			this.uniqueKeyType = uniqueKeyType;

			IList associations = WalkTree( persister, Alias, factory );
			InitClassPersisters( associations );
			SqlString whereString = WhereString( factory, Alias, uniqueKey, uniqueKeyType, batchSize ).ToSqlString();
			RenderStatement( associations, whereString, factory );

			PostInstantiate();

			batchLoader = batchSize > 1;
		}

		public object Load( ISessionImplementor session, object id, object optionalObject )
		{
			return Load( session, id, optionalObject, id );
		}

		public object LoadByUniqueKey( ISessionImplementor session, object id )
		{
			return Load( session, id, null, null );
		}
	
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