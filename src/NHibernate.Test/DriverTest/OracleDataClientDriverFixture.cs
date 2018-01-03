#if !NETCOREAPP2_0
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;
using NHibernate.Util;
using NUnit.Framework;
using Environment = NHibernate.Cfg.Environment;

namespace NHibernate.Test.DriverTest
{
	[TestFixture]
	public class OracleDataClientDriverFixture
	{
		/// <summary>
		/// Testing NH-302 to verify that a DbType.Boolean gets replaced
		/// with an appropriate type.
		/// </summary>
		[Test]
		[Category("ODP.NET")]
		[Theory]
		public void NoBooleanParameters(bool managed)
		{
			var driver = GetDriver(managed, TestConfigurationHelper.GetDefaultConfiguration().Properties);
			var param = GetParameterForType(driver, SqlTypeFactory.Boolean);

			Assert.That(param.DbType, Is.Not.EqualTo(DbType.Boolean), "should not still be a DbType.Boolean");
		}

		[Test]
		[Category("ODP.NET")]
		[Theory]
		public void UnicodeParameters(bool managed)
		{
			var cfg = TestConfigurationHelper.GetDefaultConfiguration();
			var driver = GetDriver(managed, cfg.Properties);
			var useNPrefixedTypesForUnicode = PropertiesHelper.GetBoolean(Environment.OracleUseNPrefixedTypesForUnicode, cfg.Properties, false);

			Assert.That(driver.UseNPrefixedTypesForUnicode, Is.EqualTo(useNPrefixedTypesForUnicode),
				$"Unexpected value for {nameof(OracleDataClientDriverBase)}.{nameof(OracleDataClientDriverBase.UseNPrefixedTypesForUnicode)}");

			var param = GetParameterForType(driver, SqlTypeFactory.GetString(200));
			var oracleParamType = GetOracleParameterType(param);
			Assert.That(oracleParamType.ToString(), Is.EqualTo(useNPrefixedTypesForUnicode ? "NVarchar2" : "Varchar2").IgnoreCase,
				"Unexpected Unicode string parameter type");

			param = GetParameterForType(driver, new StringFixedLengthSqlType(10));
			oracleParamType = GetOracleParameterType(param);
			Assert.That(oracleParamType.ToString(), Is.EqualTo(useNPrefixedTypesForUnicode ? "NChar" : "Char").IgnoreCase,
				"Unexpected Unicode string fixed length parameter type");
		}

		[Test]
		[Category("ODP.NET")]
		[Theory]
		public void UnicodeParametersNoPrefix(bool managed)
		{
			var cfg = TestConfigurationHelper.GetDefaultConfiguration();
			cfg.SetProperty(Environment.OracleUseNPrefixedTypesForUnicode, "false");
			var driver = GetDriver(managed, cfg.Properties);

			Assert.That(driver.UseNPrefixedTypesForUnicode, Is.False,
				$"Unexpected value for {nameof(OracleDataClientDriverBase)}.{nameof(OracleDataClientDriverBase.UseNPrefixedTypesForUnicode)}");

			var param = GetParameterForType(driver, SqlTypeFactory.GetString(200));
			var oracleParamType = GetOracleParameterType(param);
			Assert.That(oracleParamType.ToString(), Is.EqualTo("Varchar2").IgnoreCase, "Unexpected Unicode string parameter type");

			param = GetParameterForType(driver, new StringFixedLengthSqlType(10));
			oracleParamType = GetOracleParameterType(param);
			Assert.That(oracleParamType.ToString(), Is.EqualTo("Char").IgnoreCase, "Unexpected Unicode string fixed length parameter type");
		}

		[Test]
		[Category("ODP.NET")]
		[Theory]
		public void UnicodeParametersWithPrefix(bool managed)
		{
			var cfg = TestConfigurationHelper.GetDefaultConfiguration();
			cfg.SetProperty(Environment.OracleUseNPrefixedTypesForUnicode, "true");
			var driver = GetDriver(managed, cfg.Properties);

			Assert.That(driver.UseNPrefixedTypesForUnicode, Is.True,
				$"Unexpected value for {nameof(OracleDataClientDriverBase)}.{nameof(OracleDataClientDriverBase.UseNPrefixedTypesForUnicode)}");

			var param = GetParameterForType(driver, SqlTypeFactory.GetString(200));
			var oracleParamType = GetOracleParameterType(param);
			Assert.That(oracleParamType.ToString(), Is.EqualTo("NVarchar2").IgnoreCase, "Unexpected Unicode string parameter type");

			param = GetParameterForType(driver, new StringFixedLengthSqlType(10));
			oracleParamType = GetOracleParameterType(param);
			Assert.That(oracleParamType.ToString(), Is.EqualTo("NChar").IgnoreCase, "Unexpected Unicode string fixed length parameter type");
		}

		[Test]
		[Category("ODP.NET")]
		[Theory]
		public void HasSameUnicodeDefaultThanDialect(bool managed)
		{
			var cfg = TestConfigurationHelper.GetDefaultConfiguration();
			cfg.Properties.Remove(Environment.OracleUseNPrefixedTypesForUnicode);
			var driver = GetDriver(managed, cfg.Properties);
			var dialect = new Oracle8iDialect();
			dialect.Configure(cfg.Properties);

			Assert.That(driver.UseNPrefixedTypesForUnicode, Is.EqualTo(dialect.UseNPrefixedTypesForUnicode),
				$"Default {nameof(Oracle8iDialect.UseNPrefixedTypesForUnicode)} values mismatch between driver and dialect");
		}


		private static OracleDataClientDriverBase GetDriver(bool managed, IDictionary<string, string> settings)
		{
			OracleDataClientDriverBase driver = null;
			try
			{
				driver = managed
					? (OracleDataClientDriverBase)new OracleManagedDriver()
					: new OracleDataClientDriver();
			}
			catch (Exception ex)
			{
				Assert.Ignore("Unable to load the driver: {0}", ex);
			}

			driver.Configure(settings);

			return driver;
		}

		private static DbParameter GetParameterForType(IDriver driver, SqlType paramType)
		{
			var builder = new SqlStringBuilder();
			builder.Add("select * from table1 where col1=");
			builder.Add(Parameter.Placeholder);

			var cmd = driver.GenerateCommand(CommandType.Text, builder.ToSqlString(), new[] { paramType });

			Assert.That(cmd.Parameters, Has.Count.EqualTo(1), "Unexpected parameters count");
			var param = cmd.Parameters[0];
			return param;
		}

		private static object GetOracleParameterType(DbParameter dbParameter)
		{
			var parameterType = dbParameter.GetType();
			var typeProperty = parameterType.GetProperty("OracleDbType") ??
				throw new InvalidOperationException("Unable to find OracleDbType property");
			return typeProperty.GetValue(dbParameter);
		}
	}
}
#endif
