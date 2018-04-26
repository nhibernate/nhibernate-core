namespace NHibernate.Test.TestDialects
{
	public abstract class HanaTestDialectBase : TestDialect
	{
		protected HanaTestDialectBase(Dialect.Dialect dialect)
			: base(dialect)
		{
		}

		public override bool SupportsComplexExpressionInGroupBy => false;

		public override bool SupportsEmptyInserts => false;

		/// <inheritdoc />
		/// <remarks>
		/// Looks as a HANA 2 SP3 bug: HANA complains about the main select columns not being part of a group-by
		/// or not being in an aggregation function, while only the sub-select in order by is grouped.
		/// </remarks>
		public override bool SupportsAggregatingScalarSubSelectsInOrderBy => false;

		/// <inheritdoc />
		/// <remarks>
		/// HANA 2 SP3 yields in such case: correlated subquery cannot have TOP or ORDER BY
		/// </remarks>
		public override bool SupportsOrderByAndLimitInSubQueries => false;
	}
}
