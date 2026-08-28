using NHibernate.Dialect.Function;

namespace NHibernate.Dialect
{
	public class Firebird4Dialect: Firebird3Dialect
	{
		private const string UtcTimestampExpression = "cast(CURRENT_TIMESTAMP at time zone 'UTC' as timestamp)";

		public override string CurrentTimestampSelectString => "select LOCALTIMESTAMP from RDB$DATABASE";

		/// <inheritdoc />
		public override bool SupportsCurrentUtcTimestampSelection => true;

		/// <inheritdoc />
		public override string CurrentUtcTimestampSelectString =>
			"select " + CurrentUtcTimestampSQLFunctionName + " from RDB$DATABASE";

		/// <inheritdoc />
		/// <remarks>
		/// Firebird has no UTC function. Move the time zone aware <c>CURRENT_TIMESTAMP</c> to UTC and
		/// remove the time zone.
		/// </remarks>
		public override string CurrentUtcTimestampSQLFunctionName => UtcTimestampExpression;

		protected override void RegisterFunctions()
		{
			base.RegisterFunctions();
			RegisterFunction("current_timestamp", new NoArgSQLFunction("localtimestamp", NHibernateUtil.LocalDateTime, false));
			RegisterFunction("current_utctimestamp", new SQLFunctionTemplate(NHibernateUtil.UtcDateTime, UtcTimestampExpression));
		}
	}
}
