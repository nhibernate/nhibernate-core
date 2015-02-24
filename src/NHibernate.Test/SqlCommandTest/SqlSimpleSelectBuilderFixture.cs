using System;
using System.Text;
using NHibernate.Cfg;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NUnit.Framework;

namespace NHibernate.Test.SqlCommandTest
{
	/// <summary>
	/// Tests all of the functionality of the SqlInsertBulder
	/// </summary>
	[TestFixture]
	public class SqlSimpleSelectBuilderFixture
	{
		[Test]
		public void SimpleSelectStringSqlTest()
		{
			Configuration cfg = new Configuration();
			ISessionFactory factory = cfg.BuildSessionFactory();

			ISessionFactoryImplementor factoryImpl = (ISessionFactoryImplementor) factory;
			SqlSimpleSelectBuilder select = new SqlSimpleSelectBuilder(factoryImpl.Dialect, factoryImpl);

			select.SetTableName("test_simple_select_builder");
			select.AddColumn("column_no_alias");
			select.AddColumn("aliased_column", "aliased_column_alias");

			select.AddColumns(new string[] {"column1_no_alias", "column2_no_alias"});
			select.AddColumns(new string[] {"column1_with_alias", "column2_with_alias"}, new string[] {"c1_alias", "c2_alias"});

			select.SetIdentityColumn(new string[] {"identity_column"}, NHibernateUtil.Int64);
			select.SetVersionColumn(new string[] {"version_column"}, (IVersionType) NHibernateUtil.Int32);

			select.AddWhereFragment(new string[] {"where_frag_column"}, NHibernateUtil.Int32, " = ");

			SqlString sqlString = select.ToSqlString();
			Parameter[] actualParams = new Parameter[3];

			string expectedSql = new StringBuilder().Append("SELECT ")
				.Append("column_no_alias, ")
				.Append("aliased_column AS aliased_column_alias, ")
				.Append("column1_no_alias, ")
				.Append("column2_no_alias, ")
				.Append("column1_with_alias AS c1_alias, ")
				.Append("column2_with_alias AS c2_alias ")
				.Append("FROM test_simple_select_builder ")
				.Append("WHERE identity_column = ? AND version_column = ?")
				.Append(" AND where_frag_column = ?")
				.ToString();


			Assert.AreEqual(expectedSql, sqlString.ToString(), "SQL String");
			Assert.AreEqual(3, sqlString.GetParameterCount(), "3 parameters");
		}
	}
}