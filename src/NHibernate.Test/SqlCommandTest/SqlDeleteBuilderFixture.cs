using System;
using NHibernate.Cfg;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;
using NHibernate.Type;
using NUnit.Framework;

namespace NHibernate.Test.SqlCommandTest
{
	/// <summary>
	/// Tests all of the functionality of the SqlDeleteBuilderFixture
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
			SqlDeleteBuilder delete = new SqlDeleteBuilder(factoryImpl.Dialect, factoryImpl);

			delete.SetTableName("test_delete_builder");


			delete.SetIdentityColumn(new string[] {"decimalColumn"}, NHibernateUtil.Decimal);
			delete.SetVersionColumn(new string[] {"versionColumn"}, (IVersionType) NHibernateUtil.Int32);

			delete.AddWhereFragment("a=b");

			SqlCommandInfo sqlCommand = delete.ToSqlCommandInfo();

			string expectedSql = "DELETE FROM test_delete_builder WHERE decimalColumn = ? AND versionColumn = ? AND a=b";
			Assert.AreEqual(expectedSql, sqlCommand.Text.ToString(), "SQL String");

			SqlType[] actualParameterTypes = sqlCommand.ParameterTypes;
			Assert.AreEqual(2, actualParameterTypes.Length, "Two parameters");

			Assert.AreEqual(SqlTypeFactory.Decimal, actualParameterTypes[0], "firstParam Type");
			Assert.AreEqual(SqlTypeFactory.Int32, actualParameterTypes[1], "secondParam Type");
		}
	}
}