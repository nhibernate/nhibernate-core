using System;
using NHibernate.Util;

namespace NHibernate.SqlCommand
{
	/// <summary>
	/// An Informix-style (theta) Join
	/// </summary>
	public class InformixJoinFragment : JoinFragment
	{
		private readonly SqlStringBuilder afterFrom = new SqlStringBuilder();
		private readonly SqlStringBuilder afterWhere = new SqlStringBuilder();

		public override void AddJoin(string tableName, string alias, string[] fkColumns, string[] pkColumns, JoinType joinType)
		{
			switch (joinType)
			{
				case JoinType.InnerJoin:
					AddCrossJoin(tableName, alias);
					break;
				case JoinType.LeftOuterJoin:
					afterFrom.Add(StringHelper.CommaSpace).Add("outer ").Add(tableName).Add(" ").Add(alias);
					break;
				case JoinType.RightOuterJoin:
					int i = GetPrevTableInsertPoint(afterFrom.ToSqlString());
					afterFrom.Insert(i, "outer ");
					break;
				case JoinType.FullJoin:
					throw new NotSupportedException("join type not supported by Informix");
				default:
					throw new AssertionFailure("undefined join type");
			}

			for (int j = 0; j < fkColumns.Length; j++)
			{
				HasThetaJoins = true;
				afterWhere.Add(" and " + fkColumns[j]);
				afterWhere.Add("=" + alias + StringHelper.Dot + pkColumns[j]);
			}
		}

		public override void AddJoin(string tableName, string alias, string[] fkColumns, string[] pkColumns, JoinType joinType,
		                             SqlString on)
		{
			//arbitrary on clause ignored!!
			AddJoin(tableName, alias, fkColumns, pkColumns, joinType);
			AddCondition(on);
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
			afterFrom.Add(StringHelper.CommaSpace).Add(tableName).Add(" ").Add(alias);
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

		private static int GetPrevTableInsertPoint(SqlString text)
		{
			int i = text.LastIndexOfCaseInsensitive("from");
			int j = text.LastIndexOfCaseInsensitive(",");
			if (i == -1 && j == -1)
			{
				return -1;
			}
			if (j > i)
			{
				return j + 1;
			}
			return i + 5;
		}
	}
}