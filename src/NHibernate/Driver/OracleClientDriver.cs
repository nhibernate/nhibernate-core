using System.Data;
using System.Data.Common;
using NHibernate.Engine.Query;
using NHibernate.SqlTypes;

namespace NHibernate.Driver
{
	/// <summary>
	/// A NHibernate Driver for using the Oracle DataProvider.
	/// </summary>
	public class OracleClientDriver : ReflectionBasedDriver
	{
		private static readonly SqlType GuidSqlType = new SqlType(DbType.Binary, 16);

		public OracleClientDriver() :
			base(
			"System.Data.OracleClient",
			"System.Data.OracleClient, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", 
			"System.Data.OracleClient.OracleConnection", 
			"System.Data.OracleClient.OracleCommand") { }

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

		protected override void InitializeParameter(DbParameter dbParam, string name, SqlType sqlType)
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

		protected override void OnBeforePrepare(DbCommand command)
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