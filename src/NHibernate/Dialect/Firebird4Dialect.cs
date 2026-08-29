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

		protected override void RegisterFunctions()
		{
			base.RegisterFunctions();
			RegisterFunction("current_timestamp", new NoArgSQLFunction("localtimestamp", NHibernateUtil.LocalDateTime, false));
			RegisterFunction("localtimestamp", new NoArgSQLFunction("localtimestamp", NHibernateUtil.LocalDateTime, false));

      RegisterFunction("base64_encode", new StandardSQLFunction("base64_encode", NHibernateUtil.String));
			RegisterFunction("base64_decode", new StandardSQLFunction("base64_decode", NHibernateUtil.Binary));
			RegisterFunction("hex_encode", new StandardSQLFunction("hex_encode", NHibernateUtil.String));
			RegisterFunction("hex_decode", new StandardSQLFunction("hex_decode", NHibernateUtil.Binary));
			RegisterFunction("first_day", new SQLFunctionTemplate(NHibernateUtil.Date, "first_day(of month from ?1)"));
			RegisterFunction("last_day", new SQLFunctionTemplate(NHibernateUtil.Date, "last_day(of month from ?1)"));
		}
	}
}
