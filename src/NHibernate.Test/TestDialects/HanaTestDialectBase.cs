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

		/// <inheritdoc />
		/// <remarks>
		/// HANA 2 SP3 with its .Net data provider yields 0 instead of the value.
		/// </remarks>
		public override bool SupportsSelectingDoubleLiteral => false;

		/// <inheritdoc />
		/// <remarks>
		/// HANA 2 SP3 fails creating the foreign key with the message
		/// "expression is of wrong type: cannot convert 'boolean' to 'varchar'".
		/// </remarks>
		public override bool SupportsFKOnCompositeKeyWithBoolean => false;

		/// <inheritdoc />
		/// <remarks>
		/// HANA 2 SP3 with its .Net data provider regularly fails to open connections in tests involving
		/// many parallel connection openings. It yields a message beginning with "Could not open connection to:"
		/// followed by the connection string, with a <c>System.Runtime.InteropServices.SEHException</c> inner
		/// exception (External component has thrown an exception).
		/// </remarks>
		public override bool SupportsConcurrencyTests => false;
	}
}
