using NHibernate.Dialect.Function;

namespace NHibernate.Dialect
{
	public class Firebird4Dialect: Firebird3Dialect
	{
		public override string CurrentTimestampSelectString => "select LOCALTIMESTAMP from RDB$DATABASE";

		protected override void RegisterFunctions()
		{
			base.RegisterFunctions();
			RegisterFunction("current_timestamp", new NoArgSQLFunction("localtimestamp", NHibernateUtil.LocalDateTime, false));
			RegisterUdfReplacementFunctions();
		}

		/// <summary>
		/// Maps the <c>ib_udf</c> and <c>fbudf</c> names to built-in functions, because Firebird 4 disables UDFs.
		/// </summary>
		/// <remarks>
		/// <c>div</c>, <c>dow</c>, <c>sdow</c> and <c>getexacttimestamp</c> have no built-in equivalent. Use the <c>udf_compat</c> UDR.
		/// </remarks>
		private void RegisterUdfReplacementFunctions()
		{
			RegisterFunction("dpower", new StandardSQLFunction("power", NHibernateUtil.Double));
			RegisterFunction("sright", new StandardSQLFunction("right"));
			RegisterFunction("strlen", new StandardSQLFunction("char_length", NHibernateUtil.Int16));
			// ib_udf substr takes the first and the last position.
			RegisterFunction("substr", new SQLFunctionTemplate(null, "substring(?1 from ?2 for ?3 - ?2 + 1)"));
			RegisterFunction("substrlen", new SQLFunctionTemplate(NHibernateUtil.Int16, "substring(?1 from ?2 for ?3)"));
			RegisterFunction("string2blob", new SQLFunctionTemplate(null, "cast(?1 as blob sub_type text)"));
			RegisterFunction("addday", new SQLFunctionTemplate(NHibernateUtil.DateTime, "dateadd(day, ?2, ?1)"));
			RegisterFunction("addhour", new SQLFunctionTemplate(NHibernateUtil.DateTime, "dateadd(hour, ?2, ?1)"));
			RegisterFunction("addmillisecond", new SQLFunctionTemplate(NHibernateUtil.DateTime, "dateadd(millisecond, ?2, ?1)"));
			RegisterFunction("addminute", new SQLFunctionTemplate(NHibernateUtil.DateTime, "dateadd(minute, ?2, ?1)"));
			RegisterFunction("addmonth", new SQLFunctionTemplate(NHibernateUtil.DateTime, "dateadd(month, ?2, ?1)"));
			RegisterFunction("addsecond", new SQLFunctionTemplate(NHibernateUtil.DateTime, "dateadd(second, ?2, ?1)"));
			RegisterFunction("addweek", new SQLFunctionTemplate(NHibernateUtil.DateTime, "dateadd(week, ?2, ?1)"));
			RegisterFunction("addyear", new SQLFunctionTemplate(NHibernateUtil.DateTime, "dateadd(year, ?2, ?1)"));
			// Native names used above.
			RegisterFunction("power", new StandardSQLFunction("power", NHibernateUtil.Double));
			RegisterFunction("dateadd", new StandardSQLFunction("dateadd", NHibernateUtil.DateTime));
		}
	}
}
