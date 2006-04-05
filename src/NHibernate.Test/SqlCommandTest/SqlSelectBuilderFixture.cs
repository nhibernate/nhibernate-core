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
			select.SetOuterJoins(
				new SqlString(" LEFT OUTER JOIN before ON select_test_alias.column1 = before.column1"),
				new SqlString(" after.some_field = after.another_field ") );
			select.SetOrderByClause("column1 DESC");

			select.SetWhereClause("select_test_alias", new string[] {"identity_column"}, NHibernateUtil.Int64);
 
			SqlString sqlString = select.ToSqlString();

			string expectedSql = new StringBuilder().Append("SELECT ")
				.Append("column1, column2 ")
				.Append("FROM select_test select_test_alias ")
				.Append("LEFT OUTER JOIN before ON select_test_alias.column1 = before.column1 ")
				.Append("WHERE ")
				.Append("after.some_field = after.another_field")
				.Append(" AND ")
				.Append("select_test_alias.identity_column = :select_test_alias.identity_column ")
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

			Assert.AreEqual(expectedSql , sqlString.ToString(), "SQL String");
			Assert.AreEqual(1, numOfParams, "One parameter");

			Parameter firstParam = new Parameter( "identity_column", "select_test_alias", new SqlTypes.Int64SqlType() );
			Assert.AreEqual(firstParam.SqlType.DbType, expectedParam.SqlType.DbType, "First Parameter Type");
			Assert.AreEqual(firstParam.Name, expectedParam.Name, "First Parameter Name");	
		}
	}
}
