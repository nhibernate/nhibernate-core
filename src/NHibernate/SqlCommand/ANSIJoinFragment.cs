using System;
using NHibernate.Util;

namespace NHibernate.SqlCommand
{
	/// <summary>
	/// An ANSI-style Join.
	/// </summary>
	public class ANSIJoinFragment : JoinFragment
	{
		private SqlStringBuilder buffer = new SqlStringBuilder();
		private readonly SqlStringBuilder conditions = new SqlStringBuilder();

		public override void AddJoin(string tableName, string alias, string[] fkColumns, string[] pkColumns, JoinType joinType)
		{
			AddJoin(tableName, alias, fkColumns, pkColumns, joinType, null);
		}

		public override void AddJoin(string tableName, string alias, string[] fkColumns, string[] pkColumns, JoinType joinType,
		                             SqlString on)
		{
			string joinString;
			switch (joinType)
			{
				case JoinType.InnerJoin:
					joinString = " inner join ";
					break;
				case JoinType.LeftOuterJoin:
					joinString = " left outer join ";
					break;
				case JoinType.RightOuterJoin:
					joinString = " right outer join ";
					break;
				case JoinType.FullJoin:
					joinString = " full outer join ";
					break;
				default:
					throw new AssertionFailure("undefined join type");
			}

			buffer.Add(joinString + tableName + ' ' + alias + " on ");

			for (int j = 0; j < fkColumns.Length; j++)
			{
				buffer.Add(fkColumns[j] + "=" + alias + StringHelper.Dot + pkColumns[j]);
				if (j < fkColumns.Length - 1)
				{
					buffer.Add(" and ");
				}
			}

			AddCondition(buffer, on);
		}

		public override SqlString ToFromFragmentString
		{
			get { return buffer.ToSqlString(); }
		}

		public override SqlString ToWhereFragmentString
		{
			get { return conditions.ToSqlString(); }
		}

		public override void AddJoins(SqlString fromFragment, SqlString whereFragment)
		{
			buffer.Add(fromFragment);
			//where fragment must be empty!
		}

		public JoinFragment Copy()
		{
			ANSIJoinFragment copy = new ANSIJoinFragment();
			copy.buffer = new SqlStringBuilder(buffer.ToSqlString());
			return copy;
		}

		public override void AddCrossJoin(string tableName, string alias)
		{
			buffer.Add(StringHelper.CommaSpace + tableName + " " + alias);
		}

		public override bool AddCondition(string condition)
		{
			return AddCondition(conditions, condition);
		}

		public override bool AddCondition(SqlString condition)
		{
			return AddCondition(conditions, condition);
		}

		public override void AddFromFragmentString(SqlString fromFragmentString)
		{
			buffer.Add(fromFragmentString);
		}
	}
}