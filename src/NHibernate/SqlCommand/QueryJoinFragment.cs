using System;

using NHibernate.Dialect;
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
	
		[Obsolete("Use the ctor with Dialect and bool as the parameters.  This will be removed.")]
		public QueryJoinFragment(Dialect.Dialect dialect) : this (dialect, false) 
		{
		}

		public QueryJoinFragment(Dialect.Dialect dialect, bool useThetaStyleInnerJoins) 
		{
			this.dialect = dialect;
			this.useThetaStyleInnerJoins = useThetaStyleInnerJoins;
		}

		public override void AddJoin(string tableName, string alias, string[] fkColumns, string[] pkColumns, JoinType joinType) 
		{
			AddJoin(tableName, alias, alias, fkColumns, pkColumns, joinType);
		}

		public void AddJoin(string tableName, string alias, string concreteAlias, string[] fkColumns, string[] pkColumns, JoinType joinType) 
		{
			if (!useThetaStyleInnerJoins || joinType!=JoinType.InnerJoin) 
			{
				JoinFragment jf = dialect.CreateOuterJoinFragment();
				jf.AddJoin(tableName, alias, fkColumns, pkColumns, joinType);
				AddFragment(jf);
			}
			else 
			{
				AddCrossJoin(tableName, alias);
				AddCondition(concreteAlias, fkColumns, pkColumns);
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
			QueryJoinFragment copy = new QueryJoinFragment(dialect, useThetaStyleInnerJoins);
			copy.afterFrom = new SqlStringBuilder( afterFrom.ToSqlString() );
			copy.afterWhere = new SqlStringBuilder( afterWhere.ToSqlString() );
			return copy;
		}

		public override void AddCondition(string alias, string[] columns, string condition) 
		{
			for ( int i=0; i<columns.Length; i++ ) 
			{
				afterWhere.Add(" and " + alias + StringHelper.Dot + columns[i] + condition);
			}
		}


		public override void AddCrossJoin(string tableName, string alias) 
		{
			afterFrom.Add(StringHelper.CommaSpace + tableName + ' ' + alias);
		}

		public override void AddCondition(string alias, string[] fkColumns, string[] pkColumns) 
		{
			for ( int j=0; j<fkColumns.Length; j++) 
			{
				afterWhere.Add( " and " + fkColumns[j]  + '=' + alias + StringHelper.Dot + pkColumns[j] );
			}
		}

		public override void AddCondition(string condition) 
		{
			//TODO: this seems hackish
			if ( 
				afterFrom.ToSqlString().ToString().IndexOf( condition.Trim() ) < 0 &&
				afterWhere.ToSqlString().ToString().IndexOf( condition.Trim() ) < 0 )
			{

			
				if ( !condition.StartsWith(" and ") ) 
				{
					afterWhere.Add(" and ");
				}
				afterWhere.Add(condition);
			}
		}

		
	}
}
