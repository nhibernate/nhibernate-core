using System.Collections;
using System.Data;
using NHibernate.Engine;
using NHibernate.Persister;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.Loader
{
	/// <summary>
	/// Load an entity using outerjoin fetching to fetch associated entities
	/// </summary>
	public class EntityLoader : AbstractEntityLoader, IUniqueEntityLoader
	{
		private IType[ ] idType;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="persister"></param>
		/// <param name="factory"></param>
		public EntityLoader( ILoadable persister, ISessionFactoryImplementor factory ) : base( persister, factory )
		{
			idType = new IType[ ] {persister.IdentifierType};

			SqlSelectBuilder selectBuilder = new SqlSelectBuilder( factory );
			selectBuilder.SetWhereClause( Alias, persister.IdentifierColumnNames, persister.IdentifierType );

			RenderStatement( selectBuilder, factory );
			this.SqlString = selectBuilder.ToSqlString();

			PostInstantiate();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="session"></param>
		/// <param name="id"></param>
		/// <param name="obj"></param>
		/// <returns></returns>
		public object Load( ISessionImplementor session, object id, object obj )
		{
			IList list = LoadEntity( session, new object[ ] {id}, idType, obj, id, false );
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
				throw new HibernateException(
					"More than one row with the given identifier was found: " +
						id +
						", for class: " +
						Persister.ClassName );
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
	}
}