using NHibernate.Util;

namespace NHibernate.SqlCommand
{
	/// <summary>
	/// An ANSI-style Join.
	/// </summary>
	public class ANSIJoinFragment : JoinFragment
	{
		private SqlStringBuilder _fromFragment = new SqlStringBuilder();
		private readonly SqlStringBuilder _whereFragment = new SqlStringBuilder();

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

			_fromFragment.Add(joinString + tableName + ' ' + alias + " on ");

			for (int j = 0; j < fkColumns.Length; j++)
			{
				_fromFragment.Add(fkColumns[j] + "=" + alias + StringHelper.Dot + pkColumns[j]);
				if (j < fkColumns.Length - 1)
				{
					_fromFragment.Add(" and ");
				}
			}

			AddCondition(_fromFragment, on);
		}

		public override SqlString ToFromFragmentString
		{
			get { return _fromFragment.ToSqlString(); }
		}

		public override SqlString ToWhereFragmentString
		{
			get { return _whereFragment.ToSqlString(); }
		}

		public override void AddJoins(SqlString fromFragment, SqlString whereFragment)
		{
			_fromFragment.Add(fromFragment);
			//where fragment must be empty!
		}

		public JoinFragment Copy()
		{
			var copy = new ANSIJoinFragment
			{
				_fromFragment = new SqlStringBuilder(_fromFragment.ToSqlString())
			};
			return copy;
		}

		public override void AddCrossJoin(string tableName, string alias)
		{
			_fromFragment.Add(StringHelper.CommaSpace + tableName + " " + alias);
		}

		public override bool AddCondition(string condition)
		{
			return AddCondition(_whereFragment, condition);
		}

		public override bool AddCondition(SqlString condition)
		{
			return AddCondition(_whereFragment, condition);
		}

		public override void AddFromFragmentString(SqlString fromFragmentString)
		{
			_fromFragment.Add(fromFragmentString);
		}
	}
}