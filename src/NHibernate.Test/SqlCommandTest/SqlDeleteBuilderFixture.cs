using System;
using System.Data;

using NHibernate.Cfg;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.Type;

using NUnit.Framework;

namespace NHibernate.Test.SqlCommandTest
{
	
	/// <summary>
	/// Tests all of the functionallity of the SqlDeleteBuilderFixture
	/// </summary>
	[TestFixture]
	public class SqlDeleteBuilderFixture
	{

		[Test]
		public void DeleteSqlStringTest() {
			Configuration cfg = new Configuration();
			ISessionFactory factory = cfg.BuildSessionFactory( );

			ISessionFactoryImplementor factoryImpl = (ISessionFactoryImplementor)factory;
			SqlDeleteBuilder delete = new SqlDeleteBuilder(factoryImpl);
			
			delete.SetTableName("test_delete_builder");

			
			delete.SetIdentityColumn(new string[] {"decimalColumn"}, NHibernate.Decimal);
			delete.SetVersionColumn(new string[] {"versionColumn"}, (IVersionType)NHibernate.Int32);

			delete.AddWhereFragment("a=b");
			SqlString sqlString = delete.ToSqlString();
			Parameter[] actualParams = new Parameter[2];
			int numOfParameters = 0;
			

			string expectedSql = "DELETE FROM test_delete_builder WHERE decimalColumn = :decimalColumn AND versionColumn = :versionColumn AND a=b";
			Assertion.AssertEquals("SQL String", expectedSql , sqlString.ToString());
			
			foreach(object part in sqlString.SqlParts) {
				if(part is Parameter) {
					actualParams[numOfParameters] = (Parameter)part;
					numOfParameters++;
				}
			}
			Assertion.AssertEquals("Two parameters", 2, numOfParameters);


			Parameter firstParam = new Parameter();
			firstParam.SqlType = new SqlTypes.DecimalSqlType();
			firstParam.Name = "decimalColumn";
		
			Parameter secondParam = new Parameter();
			secondParam.SqlType = new SqlTypes.Int32SqlType();
			secondParam.Name = "versionColumn";

			Assertion.AssertEquals("firstParam Type", firstParam.SqlType.DbType, actualParams[0].SqlType.DbType);
			Assertion.AssertEquals("firstParam Name", firstParam.Name, actualParams[0].Name);

			Assertion.AssertEquals("secondParam Type", secondParam.SqlType.DbType, actualParams[1].SqlType.DbType);
			Assertion.AssertEquals("secondParam Name", secondParam.Name, actualParams[1].Name);

			
				
		}
	}
}
