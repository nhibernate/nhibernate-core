using System;
using NHibernate.Engine;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.SqlCommand
{
	/// <summary>
	/// An Oracle-style (theta) Join
	/// </summary>
	public class OracleJoinFragment : JoinFragment
	{
		private SqlStringBuilder afterFrom = new SqlStringBuilder();
		private SqlStringBuilder afterWhere = new SqlStringBuilder();

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
			AddCrossJoin( tableName, alias );

			for( int j = 0; j < fkColumns.Length; j++ )
			{
				afterWhere.Add( " and " + fkColumns[ j ] );
				if( joinType == JoinType.RightOuterJoin || joinType == JoinType.FullJoin )
				{
					afterWhere.Add( "(+)" );
				}

				afterWhere.Add( "=" + alias + StringHelper.Dot + pkColumns[ j ] );

				if( joinType == JoinType.LeftOuterJoin || joinType == JoinType.FullJoin )
				{
					afterWhere.Add( "(+)" );
				}
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
			OracleJoinFragment copy = new OracleJoinFragment();
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
			afterFrom.Add( StringHelper.CommaSpace + tableName + " " + alias );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="alias"></param>
		/// <param name="fkColumns"></param>
		/// <param name="pkColumns"></param>
		public override void AddCondition( string alias, string[ ] fkColumns, string[ ] pkColumns )
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="condition"></param>
		public override void AddCondition( string condition )
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="condition"></param>
		public override void AddCondition( SqlString condition )
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="fromFragmentString"></param>
		public override void AddFromFragmentString( SqlString fromFragmentString )
		{
			afterFrom.Add( fromFragmentString );
		}
	}
}