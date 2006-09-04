using System;
using System.Data;

using NHibernate.Cfg;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.Type;

using NUnit.Framework;
using NHibernate.SqlTypes;

namespace NHibernate.Test.SqlCommandTest
{

	/// <summary>
	/// Tests all of the functionallity of the SqlDeleteBuilderFixture
	/// </summary>
	[TestFixture]
	public class SqlDeleteBuilderFixture
	{

		[Test]
		public void DeleteSqlStringTest()
		{
			Configuration cfg = new Configuration();
			ISessionFactory factory = cfg.BuildSessionFactory();

			ISessionFactoryImplementor factoryImpl = (ISessionFactoryImplementor) factory;
			SqlDeleteBuilder delete = new SqlDeleteBuilder(factoryImpl);

			delete.SetTableName("test_delete_builder");


			delete.SetIdentityColumn(new string[] { "decimalColumn" }, NHibernateUtil.Decimal);
			delete.SetVersionColumn(new string[] { "versionColumn" }, (IVersionType) NHibernateUtil.Int32);

			delete.AddWhereFragment("a=b");
			SqlString sqlString = delete.ToSqlString();
			Parameter[] actualParams = new Parameter[2];
			int numOfParameters = 0;

			string expectedSql = "DELETE FROM test_delete_builder WHERE decimalColumn = ? AND versionColumn = ? AND a=b";
			Assert.AreEqual(expectedSql, sqlString.ToString(), "SQL String");

			foreach (object part in sqlString.SqlParts)
			{
				if (part is Parameter)
				{
					actualParams[numOfParameters] = (Parameter) part;
					numOfParameters++;
				}
			}
			Assert.AreEqual(2, numOfParameters, "Two parameters");

			Parameter firstParam = new Parameter(SqlTypeFactory.Decimal);
			Parameter secondParam = new Parameter(SqlTypeFactory.Int32);

			Assert.AreEqual(firstParam.SqlType.DbType, actualParams[0].SqlType.DbType, "firstParam Type");
			Assert.AreEqual(secondParam.SqlType.DbType, actualParams[1].SqlType.DbType, "secondParam Type");
		}
	}
}
