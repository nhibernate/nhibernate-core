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
		}
	}
}
