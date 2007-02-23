using System;
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

		public QueryJoinFragment(Dialect.Dialect dialect, bool useThetaStyleInnerJoins)
		{
			this.dialect = dialect;
			this.useThetaStyleInnerJoins = useThetaStyleInnerJoins;
		}

		public override void AddJoin(string tableName, string alias, string[] fkColumns, string[] pkColumns, JoinType joinType)
		{
			AddJoin(tableName, alias, alias, fkColumns, pkColumns, joinType, null);
		}

		private void AddJoin(string tableName, string alias, string concreteAlias, string[] fkColumns, string[] pkColumns,
		                     JoinType joinType, string on)
		{
			if (!useThetaStyleInnerJoins || joinType != JoinType.InnerJoin)
			{
				JoinFragment jf = dialect.CreateOuterJoinFragment();
				jf.AddJoin(tableName, alias, fkColumns, pkColumns, joinType, on);
				AddFragment(jf);
			}
			else
			{
				AddCrossJoin(tableName, alias);
				AddCondition(concreteAlias, fkColumns, pkColumns);
				AddCondition(on);
			}
		}

		public override void AddJoin(string tableName, string alias, string[] fkColumns, string[] pkColumns, JoinType joinType,
		                             string on)
		{
			AddJoin(tableName, alias, alias, fkColumns, pkColumns, joinType, on);
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
			afterFrom.Add(StringHelper.CommaSpace + tableName + ' ' + alias);
		}

		private void AddCondition(string alias, string[] fkColumns, string[] pkColumns)
		{
			for (int j = 0; j < fkColumns.Length; j++)
			{
				afterWhere
					.Add(" and ")
					.Add(fkColumns[j])
					.Add("=")
					.Add(alias)
					.Add(".")
					.Add(pkColumns[j]);
			}
		}

		public override bool AddCondition(string condition)
		{
			//TODO: this seems hackish
			if (
				afterFrom.ToSqlString().ToString().IndexOf(condition.Trim()) < 0 &&
				afterWhere.ToSqlString().ToString().IndexOf(condition.Trim()) < 0)
			{
				if (!condition.StartsWith(" and "))
				{
					afterWhere.Add(" and ");
				}
				afterWhere.Add(condition);
				return true;
			}

			return false;
		}


		public override bool AddCondition(SqlString condition)
		{
			//TODO: this seems hackish
			if (
				afterFrom.ToString().IndexOf(condition.Trim().ToString()) < 0 &&
				afterWhere.ToString().IndexOf(condition.Trim().ToString()) < 0)
			{
				if (!condition.StartsWithCaseInsensitive(" and "))
				{
					afterWhere.Add(" and ");
				}

				afterWhere.Add(condition);
				return true;
			}

			return false;
		}

		public override void AddFromFragmentString(SqlString fromFragmentString)
		{
			afterFrom.Add(fromFragmentString);
		}

		public void ClearWherePart()
		{
			afterWhere.Clear();
		}
	}
}