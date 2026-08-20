using NHibernate.Dialect.Function;

namespace NHibernate.Dialect
{
	/// <summary>
	/// An SQL dialect for PostgreSQL 13 and above.
	/// </summary>
	public class PostgreSQL13Dialect : PostgreSQL83Dialect
	{
		public PostgreSQL13Dialect()
		{
			RegisterFunction("gen_random_uuid", new NoArgSQLFunction("gen_random_uuid", NHibernateUtil.Guid));
			RegisterFunction("new_uuid", new NoArgSQLFunction("gen_random_uuid", NHibernateUtil.Guid));
		}

		public override string SelectGUIDString => "select gen_random_uuid()";
	}
}
