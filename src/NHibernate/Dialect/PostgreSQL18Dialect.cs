using NHibernate.Dialect.Function;

namespace NHibernate.Dialect
{
	/// <summary>
	/// An SQL dialect for PostgreSQL 18 and above.
	/// </summary>
	public class PostgreSQL18Dialect : PostgreSQL13Dialect
	{
		public PostgreSQL18Dialect()
		{
			RegisterFunction("uuidv4", new NoArgSQLFunction("uuidv4", NHibernateUtil.Guid));
			RegisterFunction("uuidv7", new StandardSQLFunction("uuidv7", NHibernateUtil.Guid));
			RegisterFunction("new_uuid_v7", new StandardSQLFunction("uuidv7", NHibernateUtil.Guid));
		}
	}
}
