using System;
using System.Data;
using System.Text;

using NHibernate.Cfg;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.Type;

using NUnit.Framework;

namespace NHibernate.Test.SqlCommandTest 
{
	
	/// <summary>
	/// Tests all of the functionallity of the SqlInsertBulder
	/// </summary>
	[TestFixture]
	public class SqlSimpleSelectBuilderFixture
	{
		
		[Test]
		public void SimpleSelectStringSqlTest() 
		{
			Configuration cfg = new Configuration();
			ISessionFactory factory = cfg.BuildSessionFactory( );

			ISessionFactoryImplementor factoryImpl = (ISessionFactoryImplementor)factory;
			SqlSimpleSelectBuilder select = new SqlSimpleSelectBuilder(factoryImpl);
			
			select.SetTableName("test_simple_select_builder");
			select.AddColumn("column_no_alias");
			select.AddColumn("aliased_column", "aliased_column_alias");

			select.AddColumns(new string[]{"column1_no_alias", "column2_no_alias"});
			select.AddColumns(new string[]{"column1_with_alias", "column2_with_alias"}, new string[] {"c1_alias", "c2_alias"});

			select.SetIdentityColumn(new string[]{"identity_column"}, NHibernate.Int64);
			select.SetVersionColumn(new string[]{"version_column"}, (IVersionType)NHibernate.Int32);

			select.AddWhereFragment(new string[]{"where_frag_column"}, NHibernate.Int32, " = ");
			
			SqlString sqlString = select.ToSqlString();
			Parameter[] actualParams = new Parameter[3];
			int numOfParameters = 0;

			string expectedSql = new StringBuilder().Append("SELECT ")
				.Append("column_no_alias, ")
				.Append("aliased_column AS aliased_column_alias, ")
				.Append("column1_no_alias, ")
				.Append("column2_no_alias, ")
				.Append("column1_with_alias AS c1_alias, ")
				.Append("column2_with_alias AS c2_alias ")
				.Append("FROM test_simple_select_builder ")
				.Append("WHERE identity_column = :identity_column AND version_column = :version_column")
				.Append(" AND where_frag_column = :where_frag_column")
				.ToString();
				

			Assertion.AssertEquals("SQL String", expectedSql , sqlString.ToString());
			
			foreach(object part in sqlString.SqlParts) 
			{
				if(part is Parameter) 
				{
					actualParams[numOfParameters] = (Parameter)part;
					numOfParameters++;
				}
			}
			Assertion.AssertEquals("3 parameters", 3, numOfParameters);

			Parameter firstParam = new Parameter();
			firstParam.SqlType = new SqlTypes.Int64SqlType();
			firstParam.Name = "identity_column";
		
			Parameter secondParam = new Parameter();
			secondParam.SqlType = new SqlTypes.Int32SqlType();
			secondParam.Name = "version_column";

			Parameter thirdParam = new Parameter();
			thirdParam.SqlType = new SqlTypes.Int32SqlType();
			thirdParam.Name = "where_frag_column";

			Assertion.AssertEquals("First Parameter Type", firstParam.SqlType.DbType, actualParams[0].SqlType.DbType);
			Assertion.AssertEquals("First Parameter Name", firstParam.Name, actualParams[0].Name);

			Assertion.AssertEquals("Second Parameter Type", secondParam.SqlType.DbType, actualParams[1].SqlType.DbType);
			Assertion.AssertEquals("Second Parameter Name", secondParam.Name, actualParams[1].Name);
		
			Assertion.AssertEquals("Third Parameter Type", thirdParam.SqlType.DbType, actualParams[2].SqlType.DbType);
			Assertion.AssertEquals("Third Parameter Name", thirdParam.Name, actualParams[2].Name);
			
		}


	}
}
