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
			// Firebird 4 follows the standard: the "start with" value is the first value to generate. So it
			// does not need the adjustment done by Firebird 3.
			return GetCreateSequenceString(sequenceName) + " start with " + initialValue + " increment by " + incrementSize;
		}
	}
}
