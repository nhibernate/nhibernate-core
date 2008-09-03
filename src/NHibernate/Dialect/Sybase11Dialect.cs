using System;
using NHibernate.Dialect;
using NHibernate.SqlCommand;
using NHibernate.Util;

namespace NHibernate.Dialect
{
	/// <summary>
	/// This is a subclass of SybaseDialect for sybase 11 databases (specifically tested against 11.9.2). 11.9.2 does not support ANSI JOINs
	/// therefore we have to provide  a special join fragment for left/right joins (*= and =* respectively).
	/// </summary>
	public class Sybase11Dialect : SybaseDialect
	{
		public override JoinFragment CreateOuterJoinFragment()
		{
			return new Sybase11JoinFragment();
		}
	}

	/// <summary>
	/// This class is basically a port of the hibernate 3.2  Sybase 11 join fragment. It uses concepts from that join fragment and the Oracle join fragment in NHibernate
	/// </summary>
	internal class Sybase11JoinFragment : JoinFragment
	{
		private readonly SqlStringBuilder afterFrom = new SqlStringBuilder();
		private readonly SqlStringBuilder afterWhere = new SqlStringBuilder();
		
		public override void AddJoin(string tableName, string alias, string[] fkColumns, string[] pkColumns, JoinType joinType)
		{
			AddCrossJoin(tableName, alias);

			for ( int j=0; j<fkColumns.Length; j++) 
			{
				//full joins are not supported.. yet!
				if (joinType==JoinType.FullJoin ) throw new InvalidOperationException("full joins are not supported yet");

				afterWhere.Add(" and " + fkColumns[j] );
				
				if (joinType==JoinType.LeftOuterJoin) afterWhere.Add("*");
				afterWhere.Add("=");
				if (joinType==JoinType.RightOuterJoin) afterWhere.Add("*");

				afterWhere.Add (alias + StringHelper.Dot + pkColumns[j]);
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
		
		public override void AddCrossJoin(string tableName, string alias)
		{
			afterFrom.Add(StringHelper.CommaSpace)
				.Add(tableName)
				.Add(" ")
				.Add(alias);
		}

		public override void AddJoin(string tableName, string alias, string[] fkColumns, string[] pkColumns, JoinType joinType,
		                             string on)
		{
			AddJoin(tableName, alias, fkColumns, pkColumns, joinType);
			AddCondition(on);  
		}

		public override bool AddCondition(string condition)
		{
			return AddCondition(afterWhere, condition);
		}

		public override bool AddCondition(SqlString condition)
		{
			return AddCondition(afterWhere, condition);
		}

		public override void AddFromFragmentString(SqlString fromFragmentString)
		{
			afterFrom.Add(fromFragmentString);
		}
	}
}
