using NHibernate.Dialect.Function;

namespace NHibernate.Dialect
{
	/// <summary>
	/// A dialect for Firebird 4 and above.
	/// </summary>
	public class Firebird4Dialect : Firebird3Dialect
	{
		/// <inheritdoc />
		/// <remarks>
		/// <c>CURRENT_TIMESTAMP</c> is time zone aware since Firebird 4, <c>LOCALTIMESTAMP</c> is not.
		/// </remarks>
		public override string CurrentTimestampSelectString => "select LOCALTIMESTAMP from RDB$DATABASE";

		/// <inheritdoc />
		public override string CurrentTimestampSQLFunctionName => "localtimestamp";

		/// <inheritdoc />
		/// <remarks>
		/// Firebird 4 increases the identifier length limit from 31 to 63 characters.
		/// </remarks>
		public override int MaxAliasLength => 63;

		protected override void RegisterFunctions()
		{
			base.RegisterFunctions();
			RegisterFunction("current_timestamp", new NoArgSQLFunction("localtimestamp", NHibernateUtil.LocalDateTime, false));
			RegisterFunction("localtimestamp", new NoArgSQLFunction("localtimestamp", NHibernateUtil.LocalDateTime, false));
		}
	}
}
