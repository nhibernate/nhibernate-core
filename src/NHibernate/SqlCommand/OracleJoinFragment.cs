using System;
using System.Text;
using Iesi.Collections;
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

			for (int j = 0; j < fkColumns.Length; j++)
			{
				//HasThetaJoins = true;
				afterWhere.Add(" and " + fkColumns[j]);
				if (joinType == JoinType.RightOuterJoin || joinType == JoinType.FullJoin)
				{
					afterWhere.Add("(+)");
				}

				afterWhere.Add("=" + alias + StringHelper.Dot + pkColumns[j]);

				if (joinType == JoinType.LeftOuterJoin || joinType == JoinType.FullJoin)
				{
					afterWhere.Add("(+)");
				}
			}
		}

		public override void AddJoin(string tableName, string alias, string[] fkColumns, string[] pkColumns, JoinType joinType,
		                             string on)
		{
			//arbitrary on clause ignored!!
			AddJoin(tableName, alias, fkColumns, pkColumns, joinType);
			if (joinType == JoinType.InnerJoin)
			{
				AddCondition(on);
			}
			else if (joinType == JoinType.LeftOuterJoin)
			{
				AddLeftOuterJoinCondition(on);
			}
			else
			{
				throw new NotSupportedException("join type not supported by OracleJoinFragment (use Oracle9Dialect)");
			}
		}

		/// <summary>
		/// This method is a bit of a hack, and assumes
		/// that the column on the "right" side of the
		/// join appears on the "left" side of the
		/// operator, which is extremely weird if this
		/// was a normal join condition, but is natural
		/// for a filter.
		/// </summary>
		private void AddLeftOuterJoinCondition(string on)
		{
			StringBuilder buf = new StringBuilder(on);
			for (int i = 0; i < buf.Length; i++)
			{
				char character = buf[i];
				bool isInsertPoint = Operators.Contains(character) ||
				                     (character == ' ' && buf.Length > i + 3 && "is ".Equals(buf.ToString(i + 1, 3)));
				if (isInsertPoint)
				{
					buf.Insert(i, "(+)");
					i += 3;
				}
			}
			AddCondition(buf.ToString());
		}

		private static readonly ISet Operators = new HashedSet();

		static OracleJoinFragment()
		{
			Operators.Add('=');
			Operators.Add('<');
			Operators.Add('>');
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
			afterFrom
				.Add(StringHelper.CommaSpace)
				.Add(tableName)
				.Add(" ")
				.Add(alias);
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
