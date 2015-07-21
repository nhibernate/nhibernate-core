using System.Data;
using NHibernate.SqlCommand;

namespace NHibernate.Dialect
{
	public class MySQL5Dialect : MySQLDialect
	{
		public MySQL5Dialect()
		{
			RegisterColumnType(DbType.Decimal, "DECIMAL(19,5)");
			RegisterColumnType(DbType.Decimal, 19, "DECIMAL($p, $s)");
			RegisterColumnType(DbType.Guid, "BINARY(16)");
		}

		protected override void RegisterCastTypes() {
			base.RegisterCastTypes();
			// MySql 5 also supports DECIMAL as a cast type target
			// http://dev.mysql.com/doc/refman/5.0/en/cast-functions.html
			RegisterCastType(DbType.Decimal, "DECIMAL(19,5)");
			RegisterCastType(DbType.Decimal, 19, "DECIMAL($p, $s)");
			RegisterCastType(DbType.Double, "DECIMAL(19,5)");
			RegisterCastType(DbType.Single, "DECIMAL(19,5)");
			RegisterCastType(DbType.Guid, "BINARY(16)");
		}

		//Reference 5.x
		//Numeric:
		//http://dev.mysql.com/doc/refman/5.0/en/numeric-type-overview.html
		//Date and time:
		//http://dev.mysql.com/doc/refman/5.0/en/date-and-time-type-overview.html
		//String:
		//http://dev.mysql.com/doc/refman/5.0/en/string-type-overview.html
		//default:
		//http://dev.mysql.com/doc/refman/5.0/en/data-type-defaults.html

		public override bool SupportsSubSelects
		{
			get
			{
				//subquery in mysql? yes! From 4.1!
				//http://dev.mysql.com/doc/refman/5.1/en/subqueries.html
				return true;
			}
		}

		public override string SelectGUIDString
		{
			get
			{
				return "select uuid()";
			}
		}

		public override SqlString AppendIdentitySelectToInsert (NHibernate.SqlCommand.SqlString insertString)
		{
			return insertString.Append(";" + IdentitySelectString);
		}

		public override bool SupportsInsertSelectIdentity
		{
			get { return true; }
		}
	}
}