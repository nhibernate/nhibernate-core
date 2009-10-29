using System.Data;
using System.Data.OracleClient;
using NHibernate.Engine.Query;
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

		protected override void OnBeforePrepare(IDbCommand command)
		{
			base.OnBeforePrepare(command);

			CallableParser.Detail detail = CallableParser.Parse(command.CommandText);

			if (!detail.IsCallable)
				return;

			throw new System.NotImplementedException(GetType().Name +
				" does not support CallableStatement syntax (stored procedures)." +
				" Consider using OracleDataClientDriver instead.");
		}
	}
}