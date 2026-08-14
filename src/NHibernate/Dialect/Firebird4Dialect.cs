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

		/// <inheritdoc />
		protected override string GetCreateSequenceString(string sequenceName, int initialValue, int incrementSize)
		{
			// Unlike Firebird 3, "start with" sets the first value to generate.
			return GetCreateSequenceString(sequenceName) + " start with " + initialValue + " increment by " + incrementSize;
		}
	}
}
