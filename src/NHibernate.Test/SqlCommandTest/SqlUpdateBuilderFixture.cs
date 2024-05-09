using System;
using System.Data;
using NHibernate.Cfg;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;
using NHibernate.Type;
using NUnit.Framework;

namespace NHibernate.Test.SqlCommandTest
{
	/// <summary>
	/// Tests all of the functionallity of the SqlUpdateBuilder
	/// </summary>
	[TestFixture]
	public class SqlUpdateBuilderFixture
	{
		[Test]
		public void UpdateStringSqlTest()
		{
			Configuration cfg = new Configuration();
			ISessionFactory factory = cfg.BuildSessionFactory();

			ISessionFactoryImplementor factoryImpl = (ISessionFactoryImplementor) factory;
			SqlUpdateBuilder update = new SqlUpdateBuilder(factoryImpl.Dialect, factoryImpl);

			update.SetTableName("test_update_builder");

			update.AddColumns(new string[] {"intColumn"}, NHibernateUtil.Int32);
			update.AddColumns(new string[] {"longColumn"}, NHibernateUtil.Int64);
#pragma warning disable CS0618 // Type or member is obsolete
			update.AddColumn("literalColumn", false, (ILiteralType) NHibernateUtil.Boolean);
#pragma warning restore CS0618 // Type or member is obsolete
			update.AddColumn("stringColumn", 5.ToString());

			update.SetIdentityColumn(new string[] {"decimalColumn"}, NHibernateUtil.Decimal);
			update.SetVersionColumn(new string[] {"versionColumn"}, (IVersionType) NHibernateUtil.Int32);

			update.AddWhereFragment("a=b");
			SqlCommandInfo sqlCommand = update.ToSqlCommandInfo();

			Assert.AreEqual(CommandType.Text, sqlCommand.CommandType);
            string falseString = factoryImpl.Dialect.ToBooleanValueString(false);
            string expectedSql =
                "UPDATE test_update_builder SET intColumn = ?, longColumn = ?, literalColumn = " + falseString + ", stringColumn = 5 WHERE decimalColumn = ? AND versionColumn = ? AND a=b";
			Assert.AreEqual(expectedSql, sqlCommand.Text.ToString(), "SQL String");

			SqlType[] actualParameterTypes = sqlCommand.ParameterTypes;
			Assert.AreEqual(4, actualParameterTypes.Length, "Four parameters");

			SqlType[] expectedParameterTypes = new SqlType[]
				{
					SqlTypeFactory.Int32,
					SqlTypeFactory.Int64,
					SqlTypeFactory.Decimal,
					SqlTypeFactory.Int32
				};

			Assert.AreEqual(expectedParameterTypes[0], actualParameterTypes[0], "firstParam Type");
			Assert.AreEqual(expectedParameterTypes[1], actualParameterTypes[1], "secondParam Type");
			Assert.AreEqual(expectedParameterTypes[2], actualParameterTypes[2], "thirdParam Type");
			Assert.AreEqual(expectedParameterTypes[3], actualParameterTypes[3], "fourthParam Type");
		}
	}
}
