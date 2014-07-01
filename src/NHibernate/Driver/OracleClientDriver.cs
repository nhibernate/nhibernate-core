using System.Data;
using System.Reflection;
using NHibernate.Engine.Query;
using NHibernate.SqlTypes;
using NHibernate.Util;

namespace NHibernate.Driver
{
	/// <summary>
	/// A NHibernate Driver for using the Oracle DataProvider.
	/// </summary>
	public class OracleClientDriver : ReflectionBasedDriver
	{
		private const string driverAssemblyName = "System.Data.OracleClient, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";

		private static readonly SqlType GuidSqlType = new SqlType(DbType.Binary, 16);
		private readonly object oracleTypeNClob;
		private readonly PropertyInfo dbParamOracleTypeProperty;

		public OracleClientDriver() :
			base(
			"System.Data.OracleClient",
			driverAssemblyName,
			"System.Data.OracleClient.OracleConnection",
			"System.Data.OracleClient.OracleCommand")
		{
			System.Type oracleDbTypeEnum = ReflectHelper.TypeFromAssembly("System.Data.OracleClient.OracleType", driverAssemblyName, false);
			oracleTypeNClob = System.Enum.Parse(oracleDbTypeEnum, "NClob");

			using (IDbCommand cmd = CreateCommand())
			{
				IDbDataParameter dbParam = cmd.CreateParameter();
				dbParamOracleTypeProperty = dbParam.GetType().GetProperty("OracleType");
			}
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
			base.InitializeParameter(dbParam, name, AdjustSqlType(sqlType));

			AdjustDbParamTypeForLargeObjects(dbParam, sqlType);
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

		private static SqlType AdjustSqlType(SqlType sqlType)
		{
			switch (sqlType.DbType)
			{
				case DbType.Guid:
					return GuidSqlType;
				default:
					return sqlType;
			}
		}

		private void AdjustDbParamTypeForLargeObjects(IDbDataParameter dbParam, SqlType sqlType)
		{
			if (sqlType is StringClobSqlType)
			{
				// See: http://stackoverflow.com/a/14473277/718033
				// System.Data.OracleClient.dll driver generates an ORA-01461 exception because 
				// the driver mistakenly infers the column type of the string being saved, and 
				// tries forcing the server to update a LONG value into a CLOB/NCLOB column type. 
				// The reason for the incorrect behavior is even more obscure and only happens 
				// when all the following conditions are met.
				//   1.) IDbDataParameter.Value = (string whose length: 4000 > length > 2000 )
				//   2.) IDbDataParameter.DbType = DbType.String
				//   3.) DB Column is of type NCLOB/CLOB

				// The above is the default behavior for NHibernate.OracleClientDriver
				// So we use the built-in StringClobSqlType to tell the driver to use the NClob Oracle type
				// This will work for both NCLOB/CLOBs without issues.
				// Mapping file must be updated to use StringClob as the property type
				// See also: http://thebasilet.blogspot.be/2009/07/nhibernate-oracle-clobs.html
				dbParamOracleTypeProperty.SetValue(dbParam, oracleTypeNClob, null);
			}
		}
	}
}