using System.Collections;
using System.Data;
using System.Text;
using NHibernate.Engine;
using NHibernate.Expression;
using NHibernate.Persister;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Loader
{
	/// <summary>
	/// A <c>Loader</c> for <see cref="ICriteria"/> queries. 
	/// </summary>
	/// <remarks>
	/// Note that criteria
	/// queries are more like multi-object <c>Load()</c>s than like HQL queries.
	/// </remarks>
	public class CriteriaLoader : AbstractEntityLoader
	{
		private ICriteria criteria;
		//private static readonly IType[] noTypes = new IType[0];

		/// <summary>
		/// 
		/// </summary>
		/// <param name="persister"></param>
		/// <param name="factory"></param>
		/// <param name="criteria"></param>
		public CriteriaLoader( ILoadable persister, ISessionFactoryImplementor factory, ICriteria criteria ) : base( persister, factory )
		{
			this.criteria = criteria;

			IEnumerator iter = criteria.IterateExpressions();

			StringBuilder orderByBuilder = new StringBuilder( 60 );

			bool commaNeeded = false;
			iter = criteria.IterateOrderings();

			while( iter.MoveNext() )
			{
				Order ord = ( Order ) iter.Current;

				if( commaNeeded )
				{
					orderByBuilder.Append( StringHelper.CommaSpace );
				}
				commaNeeded = true;

				orderByBuilder.Append( ord.ToStringForSql( factory, criteria.PersistentClass, Alias ) );
			}

			RenderStatement( criteria.Expression.ToSqlString( factory, criteria.PersistentClass, Alias ), orderByBuilder.ToString(), factory );

			PostInstantiate();

		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="session"></param>
		/// <returns></returns>
		public IList List( ISessionImplementor session )
		{
			ArrayList values = new ArrayList();
			ArrayList types = new ArrayList();

			IEnumerator iter = criteria.IterateExpressions();
			while( iter.MoveNext() )
			{
				Expression.Expression expr = ( Expression.Expression ) iter.Current;
				TypedValue[ ] tv = expr.GetTypedValues( session.Factory, criteria.PersistentClass );
				for( int i = 0; i < tv.Length; i++ )
				{
					values.Add( tv[ i ].Value );
					types.Add( tv[ i ].Type );
				}
			}
			object[ ] valueArray = values.ToArray();
			IType[ ] typeArray = ( IType[ ] ) types.ToArray( typeof( IType ) );

			QueryParameters qp = new QueryParameters( typeArray, valueArray, null, criteria.Selection );
			return Find( session, qp, true );
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

		/// <summary>
		/// Navigate associations, returning the aliased columns.  Adds extra table joins
		/// to this loader.
		/// </summary>
		/// <param name="pathExpression"></param>
		/// <returns></returns>
		public string[ ] ToColumns( string pathExpression )
		{
			return null;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="mappingDefault"></param>
		/// <param name="path"></param>
		/// <param name="table"></param>
		/// <param name="foreignKeyColumns"></param>
		/// <returns></returns>
		protected override bool EnableJoinedFetch( bool mappingDefault, string path, string table, string[ ] foreignKeyColumns )
		{
			FetchMode fm = criteria.GetFetchMode( path );
			//fm==null ||  - an Enum can't be null
			if( fm == FetchMode.Default )
			{
				return mappingDefault;
			}
			else
			{
				return fm == FetchMode.Eager;
			}
		}

		/// <summary></summary>
		protected override bool UseQueryWhereFragment
		{
			get { return true; }
		}

	}
}