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
	/// Tests all of the functionallity of the SqlSelectBuilderTest
	/// </summary>
	[TestFixture]
	public class SqlSelectBuilderFixture
	{
		
		

		[Test]
		public void SelectStringSqlTest() 
		{
			Configuration cfg = new Configuration();
			ISessionFactory factory = cfg.BuildSessionFactory( );

			ISessionFactoryImplementor factoryImpl = (ISessionFactoryImplementor)factory;
			SqlSelectBuilder select = new SqlSelectBuilder(factoryImpl);
			
			select.SetSelectClause("column1, column2");
			select.SetFromClause("select_test", "select_test_alias");
			select.SetOuterJoins( new SqlString(" LEFT OUTER JOIN before ON select_test_alias.column1 = before.column1"), new SqlString(" LEFT OUTER JOIN after ON select_test_alias.column1 = after.column1") );
			select.SetOrderByClause("column1 DESC");

			select.SetWhereClause("select_test_alias", new string[] {"identity_column"}, NHibernate.Int64);
 
			SqlString sqlString = select.ToSqlString();

			string expectedSql = new StringBuilder().Append("SELECT ")
				.Append("column1, column2 ")
				.Append("FROM select_test select_test_alias ")
				.Append("LEFT OUTER JOIN before ON select_test_alias.column1 = before.column1 ")
				.Append("WHERE select_test_alias.identity_column = :select_test_alias.identity_column ")
				.Append("LEFT OUTER JOIN after ON select_test_alias.column1 = after.column1 ")
				.Append("ORDER BY column1 DESC")
				.ToString();
				

			int numOfParams = 0;
			Parameter expectedParam = null;

			foreach(object part in sqlString.SqlParts) 
			{
				if(part is Parameter) 
				{
					numOfParams++;
					expectedParam = (Parameter)part;
				}
			}

			Assertion.AssertEquals("SQL String", expectedSql , sqlString.ToString());
			Assertion.AssertEquals("One parameter", 1, numOfParams);

			Parameter firstParam = new Parameter();
			firstParam.SqlType = new SqlTypes.Int64SqlType();
			firstParam.TableAlias = "select_test_alias";
			firstParam.Name = "identity_column";

			Assertion.AssertEquals("First Parameter Type", firstParam.SqlType.DbType, expectedParam.SqlType.DbType);
			Assertion.AssertEquals("First Parameter TableAlias", firstParam.TableAlias, expectedParam.TableAlias);
			Assertion.AssertEquals("First Parameter Name", firstParam.Name, expectedParam.Name);
			
				
		}

	}
}
