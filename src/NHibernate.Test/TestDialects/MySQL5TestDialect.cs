namespace NHibernate.Test.TestDialects
{
	public class MySQL5TestDialect : TestDialect
	{
		public MySQL5TestDialect(Dialect.Dialect dialect)
			: base(dialect)
		{
		}

		public override bool SupportsAggregateInSubSelect => true;

		/// <summary>
		/// In MySQL, you can't modify the same table which you use in the SELECT part.
		/// This behaviour is documented at: http://dev.mysql.com/doc/refman/5.6/en/update.html
		/// </summary>
		public override bool SupportsModifyAndSelectSameTable => false;

		/// <summary>
		/// A correlated column can be present only in the subquery's WHERE clause (and not in the SELECT list,
		/// a JOIN or ORDER BY clause, a GROUP BY list, or a HAVING clause). Nor can there be any correlated column inside a derived table in the subquery's FROM list.
		/// See https://dev.mysql.com/doc/refman/8.0/en/correlated-subqueries.html
		/// </summary>
		public override bool SupportsCorrelatedColumnsInSubselectJoin => false;
	}
}
