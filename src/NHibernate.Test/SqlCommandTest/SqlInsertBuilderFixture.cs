using NHibernate.Cfg;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;
using NHibernate.Type;
using NUnit.Framework;

namespace NHibernate.Test.SqlCommandTest
{
	/// <summary>
	/// Tests all of the functionality of the SqlInsertBulder
	/// </summary>
	[TestFixture]
	public class SqlInsertBuilderFixture
	{
		[Test]
		public void InsertSqlStringTest()
		{
			Configuration cfg = new Configuration();
			ISessionFactory factory = cfg.BuildSessionFactory();

			ISessionFactoryImplementor factoryImpl = (ISessionFactoryImplementor) factory;
			SqlInsertBuilder insert = new SqlInsertBuilder(factoryImpl);

			insert.SetTableName("test_insert_builder");

			insert.AddColumn("intColumn", NHibernateUtil.Int32);
			insert.AddColumn("longColumn", NHibernateUtil.Int64);
#pragma warning disable CS0618 // Type or member is obsolete
			insert.AddColumn("literalColumn", false, (ILiteralType) NHibernateUtil.Boolean);
#pragma warning restore CS0618 // Type or member is obsolete
			insert.AddColumn("stringColumn", 5.ToString());

			SqlCommandInfo sqlCommand = insert.ToSqlCommandInfo();
			SqlType[] actualParameterTypes = sqlCommand.ParameterTypes;

		    string falseString = factoryImpl.Dialect.ToBooleanValueString(false);
			string expectedSql =
                "INSERT INTO test_insert_builder (intColumn, longColumn, literalColumn, stringColumn) VALUES (?, ?, " + falseString + ", 5)";
			Assert.AreEqual(expectedSql, sqlCommand.Text.ToString(), "SQL String");

			Assert.AreEqual(2, actualParameterTypes.Length);
			Assert.AreEqual(SqlTypeFactory.Int32, actualParameterTypes[0], "First Parameter Type");
			Assert.AreEqual(SqlTypeFactory.Int64, actualParameterTypes[1], "Second Parameter Type");
		}

		[Test]
		public void Commented()
		{
			Configuration cfg = new Configuration();
			ISessionFactory factory = cfg.BuildSessionFactory();

			ISessionFactoryImplementor factoryImpl = (ISessionFactoryImplementor)factory;
			SqlInsertBuilder insert = new SqlInsertBuilder(factoryImpl);

			insert.SetTableName("test_insert_builder");

#pragma warning disable CS0618 // Type or member is obsolete
			insert.AddColumn("stringColumn", "aSQLValue", (ILiteralType)NHibernateUtil.String);
#pragma warning restore CS0618 // Type or member is obsolete
			insert.SetComment("Test insert");
			string expectedSql =
	"/* Test insert */ INSERT INTO test_insert_builder (stringColumn) VALUES ('aSQLValue')";
			Assert.AreEqual(expectedSql, insert.ToSqlString().ToString(), "SQL String");
		}

		[Test]
		public void MixingParametersAndValues()
		{
			Configuration cfg = new Configuration();
			ISessionFactory factory = cfg.BuildSessionFactory();

			ISessionFactoryImplementor factoryImpl = (ISessionFactoryImplementor)factory;
			SqlInsertBuilder insert = new SqlInsertBuilder(factoryImpl);

			insert.SetTableName("test_insert_builder");

#pragma warning disable CS0618 // Type or member is obsolete
			insert.AddColumn("literalColumn", false, (ILiteralType)NHibernateUtil.Boolean);
#pragma warning restore CS0618 // Type or member is obsolete
			insert.AddColumn("intColumn", NHibernateUtil.Int32);
			insert.AddColumn("stringColumn", 5.ToString());
			insert.AddColumn("longColumn", NHibernateUtil.Int64);

			SqlCommandInfo sqlCommand = insert.ToSqlCommandInfo();
			SqlType[] actualParameterTypes = sqlCommand.ParameterTypes;

            string falseString = factoryImpl.Dialect.ToBooleanValueString(false);
            string expectedSql =
                "INSERT INTO test_insert_builder (literalColumn, intColumn, stringColumn, longColumn) VALUES (" + falseString + ", ?, 5, ?)";
			Assert.AreEqual(expectedSql, sqlCommand.Text.ToString(), "SQL String");

			Assert.AreEqual(2, actualParameterTypes.Length);
			Assert.AreEqual(SqlTypeFactory.Int32, actualParameterTypes[0], "First Parameter Type");
			Assert.AreEqual(SqlTypeFactory.Int64, actualParameterTypes[1], "Second Parameter Type");			
		}
	}
}
