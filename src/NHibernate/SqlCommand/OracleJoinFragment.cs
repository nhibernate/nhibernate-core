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

		public override void AddJoin(string tableName, string alias, string[] fkColumns, string[] pkColumns, JoinType joinType) 
		{
			AddCrossJoin(tableName, alias);

			for (int j=0; j<fkColumns.Length; j++) 
			{
				afterWhere.Add( " and " + fkColumns[j] );
				if (joinType == JoinType.RightOuterJoin || joinType == JoinType.FullJoin) afterWhere.Add("(+)");
				
				afterWhere.Add( "=" + alias + StringHelper.Dot + pkColumns[j] );

				if (joinType == JoinType.LeftOuterJoin || joinType == JoinType.FullJoin) afterWhere.Add("(+)");
			}
		}

		public override SqlString ToFromFragmentString 
		{
			get { return afterFrom.ToSqlString(); }
		}

		public override SqlString ToWhereFragmentString 
		{
			get { return afterWhere.ToSqlString(); }
		}

		public override void AddJoins(SqlString fromFragment, SqlString whereFragment) 
		{
			afterFrom.Add(fromFragment);
			afterWhere.Add(whereFragment);
		}

		public override JoinFragment Copy() 
		{
			OracleJoinFragment copy = new OracleJoinFragment();
			copy.afterFrom = new SqlStringBuilder( afterFrom.ToSqlString() );
			copy.afterWhere = new SqlStringBuilder( afterWhere.ToSqlString() );
			return copy;
		}

		public override void AddCondition(string alias, string[] columns, string condition) 
		{
			for (int i=0; i<columns.Length; i++) 
			{
				afterWhere.Add(" and " + alias + StringHelper.Dot + columns[i] + condition );
			}
		}

		public override void AddCondition(string alias, string[] columns, string condition, IType conditionType, ISessionFactoryImplementor factory)
		{
			Parameter[] parameters = Parameter.GenerateParameters(factory, alias, columns, conditionType);
			for( int i=0; i<columns.Length; i++) 
			{
				afterWhere.Add( " and " + alias + StringHelper.Dot + columns[i] + condition );
				afterWhere.Add( parameters[i] );
			}
		}

		public override void AddCrossJoin(string tableName, string alias) 
		{
			afterFrom.Add(StringHelper.CommaSpace + tableName + " " + alias);
		}

		public override void AddCondition(string alias, string[] fkColumns, string[] pkColumns) 
		{
			throw new NotSupportedException();
		}

	


		public override void AddCondition(string condition) 
		{
			throw new NotSupportedException();
		}

		public override void AddCondition(SqlString condition)
		{
			throw new NotSupportedException();
		}

	}
}
