using System;
using NHibernate.Engine;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.SqlCommand
{
	/// <summary>
	/// Summary description for QueryJoinFragment.
	/// </summary>
	public class QueryJoinFragment : JoinFragment
	{
		private SqlStringBuilder afterFrom = new SqlStringBuilder();
		private SqlStringBuilder afterWhere = new SqlStringBuilder();
		private Dialect.Dialect dialect;
		private bool useThetaStyleInnerJoins;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dialect"></param>
		[Obsolete( "Use the ctor with Dialect and bool as the parameters.  This will be removed." )]
		public QueryJoinFragment( Dialect.Dialect dialect ) : this( dialect, false )
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dialect"></param>
		/// <param name="useThetaStyleInnerJoins"></param>
		public QueryJoinFragment( Dialect.Dialect dialect, bool useThetaStyleInnerJoins )
		{
			this.dialect = dialect;
			this.useThetaStyleInnerJoins = useThetaStyleInnerJoins;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="tableName"></param>
		/// <param name="alias"></param>
		/// <param name="fkColumns"></param>
		/// <param name="pkColumns"></param>
		/// <param name="joinType"></param>
		public override void AddJoin( string tableName, string alias, string[ ] fkColumns, string[ ] pkColumns, JoinType joinType )
		{
			AddJoin( tableName, alias, alias, fkColumns, pkColumns, joinType );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="tableName"></param>
		/// <param name="alias"></param>
		/// <param name="concreteAlias"></param>
		/// <param name="fkColumns"></param>
		/// <param name="pkColumns"></param>
		/// <param name="joinType"></param>
		public void AddJoin( string tableName, string alias, string concreteAlias, string[ ] fkColumns, string[ ] pkColumns, JoinType joinType )
		{
			if( !useThetaStyleInnerJoins || joinType != JoinType.InnerJoin )
			{
				JoinFragment jf = dialect.CreateOuterJoinFragment();
				jf.AddJoin( tableName, alias, fkColumns, pkColumns, joinType );
				AddFragment( jf );
			}
			else
			{
				AddCrossJoin( tableName, alias );
				AddCondition( concreteAlias, fkColumns, pkColumns );
			}
		}

		/// <summary></summary>
		public override SqlString ToFromFragmentString
		{
			get { return afterFrom.ToSqlString(); }
		}

		/// <summary></summary>
		public override SqlString ToWhereFragmentString
		{
			get { return afterWhere.ToSqlString(); }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="fromFragment"></param>
		/// <param name="whereFragment"></param>
		public override void AddJoins( SqlString fromFragment, SqlString whereFragment )
		{
			afterFrom.Add( fromFragment );
			afterWhere.Add( whereFragment );
		}

		/// <summary></summary>
		public override JoinFragment Copy()
		{
			QueryJoinFragment copy = new QueryJoinFragment( dialect, useThetaStyleInnerJoins );
			copy.afterFrom = new SqlStringBuilder( afterFrom.ToSqlString() );
			copy.afterWhere = new SqlStringBuilder( afterWhere.ToSqlString() );
			return copy;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="alias"></param>
		/// <param name="columns"></param>
		/// <param name="condition"></param>
		public override void AddCondition( string alias, string[ ] columns, string condition )
		{
			for( int i = 0; i < columns.Length; i++ )
			{
				afterWhere.Add( " and " + alias + StringHelper.Dot + columns[ i ] + condition );
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="alias"></param>
		/// <param name="columns"></param>
		/// <param name="condition"></param>
		/// <param name="conditionType"></param>
		/// <param name="factory"></param>
		public override void AddCondition( string alias, string[ ] columns, string condition, IType conditionType, ISessionFactoryImplementor factory )
		{
			Parameter[ ] parameters = Parameter.GenerateParameters( factory, alias, columns, conditionType );

			for( int i = 0; i < columns.Length; i++ )
			{
				afterWhere.Add( " and " + alias + StringHelper.Dot + columns[ i ] + condition );
				afterWhere.Add( parameters[ i ] );
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="tableName"></param>
		/// <param name="alias"></param>
		public override void AddCrossJoin( string tableName, string alias )
		{
			afterFrom.Add( StringHelper.CommaSpace + tableName + ' ' + alias );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="alias"></param>
		/// <param name="fkColumns"></param>
		/// <param name="pkColumns"></param>
		public override void AddCondition( string alias, string[ ] fkColumns, string[ ] pkColumns )
		{
			for( int j = 0; j < fkColumns.Length; j++ )
			{
				afterWhere.Add( " and " + fkColumns[ j ] + '=' + alias + StringHelper.Dot + pkColumns[ j ] );
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="condition"></param>
		public override void AddCondition( string condition )
		{
			//TODO: this seems hackish
			if(
				afterFrom.ToSqlString().ToString().IndexOf( condition.Trim() ) < 0 &&
					afterWhere.ToSqlString().ToString().IndexOf( condition.Trim() ) < 0 )
			{
				if( !condition.StartsWith( " and " ) )
				{
					afterWhere.Add( " and " );
				}
				afterWhere.Add( condition );
			}
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="condition"></param>
		public override void AddCondition( SqlString condition )
		{
			//TODO: this seems hackish
			if(
				afterFrom.ToSqlString().ToString().IndexOf( condition.Trim().ToString() ) < 0 &&
					afterWhere.ToSqlString().ToString().IndexOf( condition.Trim().ToString() ) < 0 )
			{
				if( !condition.StartsWith( " and " ) )
				{
					afterWhere.Add( " and " );
				}

				afterWhere.Add( condition );
			}

		}


	}
}