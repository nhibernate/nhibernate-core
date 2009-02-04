using System.Data;
using System.Data.OracleClient;
using NHibernate.SqlTypes;

namespace NHibernate.Driver
{
	/// <summary>
	/// A NHibernate Driver for using the Oracle DataProvider.
	/// </summary>
	public class OracleClientDriver : DriverBase
	{
		private static readonly SqlType GuidSqlType = new SqlType(DbType.Binary, 16);

		public override IDbConnection CreateConnection()
		{
			return new OracleConnection();
		}

		public override IDbCommand CreateCommand()
		{
			return new OracleCommand();
		}

		public override bool UseNamedPrefixInSql
		{
			get { return true; }
		}

		public override bool UseNamedPrefixInParameter
		{
			get { return true; }
		}

		public override string NamedPrefix
		{
			get { return ":"; }
		}

		protected override void InitializeParameter(IDbDataParameter dbParam, string name, SqlType sqlType)
		{
			if (sqlType.DbType == DbType.Guid)
			{
				base.InitializeParameter(dbParam, name, GuidSqlType);
			}
			else
			{
				base.InitializeParameter(dbParam, name, sqlType);
			}
		}
	}
}