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
		/// MySql.Data sends parameters as strings when the query is not prepared.
		/// </summary>
		/// <remarks>
		/// Sending parameters as strings has an impact on <see cref="double"/> and <see cref="float"/>
		/// parameters as they can be differently evaluated by the database. For example when there 
		/// is no e-notation in the string the value will be evaluated as NUMBER type, which may cause
		/// issues for <see cref="double"/> and <see cref="float"/> parameters. When there is an e-notation
		/// the value will be evaluated as DOUBLE by the database, which may produce unexpected results
		/// for <see cref="float"/> parameters.
		/// </remarks>
		public override bool SendsParameterValuesAsStrings => true;
	}
}
