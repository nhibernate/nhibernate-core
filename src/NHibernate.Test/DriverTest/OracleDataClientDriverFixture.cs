using System;
using System.Data;
using System.Data.Common;
using NHibernate.Driver;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;
using NUnit.Framework;

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
		[Explicit]
		public void NoBooleanParameters()
		{
			OracleDataClientDriver driver = new OracleDataClientDriver();

			SqlStringBuilder builder = new SqlStringBuilder();
			builder.Add("select * from table1 where col1=");
			builder.Add(Parameter.Placeholder);

			var cmd = driver.GenerateCommand(CommandType.Text, builder.ToSqlString(), new SqlType[] {SqlTypeFactory.Boolean});

			var param = cmd.Parameters[0];

			Assert.AreEqual("col1", param.ParameterName, "kept same param name");
			Assert.IsFalse(param.DbType == DbType.Boolean, "should not still be a DbType.Boolean");
		}
	}
}