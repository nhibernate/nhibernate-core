namespace NHibernate.Test.TestDialects
{
	public class SapSQLAnywhere17TestDialect : TestDialect
	{
		public SapSQLAnywhere17TestDialect(Dialect.Dialect dialect)
			: base(dialect)
		{
		}

		/// <inheritdoc />
		/// <remarks>
		/// The personal edition of SAP SQL Anywhere does not allow more than ten simultaneous connections.
		/// </remarks>
		public override int? MaxNumberOfConnections => 10;

		public override bool SupportsDuplicatedColumnAliases => false;

		/// <inheritdoc />
		/// <remarks>
		/// See https://help.sap.com/viewer/40c01c3500744c85a02db71276495de5/17.0/en-US/8170eb5b6ce21014a7e1a2fd6b4a85fc.html
		/// It seems they have decide to remove this support starting from version 16.
		/// </remarks>
		public override bool SupportsSquareBracketInIdentifiers => false;

		/// <inheritdoc />
		/// <remarks>
		/// SQL Anywhere freezes on some commit cases and the transaction ends by timeout.
		/// </remarks>
		public override bool SupportsUsingConnectionOnSystemTransactionPrepare => false;

		/// <inheritdoc />
		/// <remarks>
		/// SQL Anywhere treats string parameters concatenation as yielding <c>numeric</c> and fails casting
		/// the actual string value if it is not castable to a number. Likewise, in <c>case when</c>
		/// statement, it treats them as yielding <c>integer</c> if all case yields parameter values, whatever
		/// the actual type of the parameters. And in case of <c>numeric</c> computations with a <c>numeric</c>
		/// parameter, the result is treated as a fractional digit lossy <c>double<c> instead of being kept
		/// <c>numeric</c>. See https://stackoverflow.com/q/52558715/1178314.
		/// </remarks>
		public override bool HasBrokenTypeInferenceOnSelectedParameters => true;
	}
}
