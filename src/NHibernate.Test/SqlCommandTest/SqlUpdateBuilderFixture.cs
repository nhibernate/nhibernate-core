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
	/// Tests all of the functionallity of the SqlUpdateBuilder
	/// </summary>
	[TestFixture]
	public class SqlUpdateBuilderFixture
	{
		
		[Test]
		public void UpdateStringSqlTest() 
		{
			Configuration cfg = new Configuration();
			ISessionFactory factory = cfg.BuildSessionFactory( );

			ISessionFactoryImplementor factoryImpl = (ISessionFactoryImplementor)factory;
			SqlUpdateBuilder update = new SqlUpdateBuilder(factoryImpl);
			
			update.SetTableName("test_update_builder");

			update.AddColumns(new string[] {"intColumn"}, NHibernate.Int32);
			update.AddColumns(new string[] {"longColumn"}, NHibernate.Int64);
			update.AddColumn("literalColumn", false, (Type.ILiteralType) NHibernate.Boolean);
			update.AddColumn("stringColumn", 5.ToString());
			
			update.SetIdentityColumn(new string[] {"decimalColumn"}, NHibernate.Decimal);
			update.SetVersionColumn(new string[] {"versionColumn"}, (IVersionType)NHibernate.Int32);

			update.AddWhereFragment("a=b");
			SqlString sqlString = update.ToSqlString();
			Parameter[] actualParams = new Parameter[4];
			int numOfParameters = 0;

			string expectedSql = "UPDATE test_update_builder SET intColumn = :intColumn, longColumn = :longColumn, literalColumn = 0, stringColumn = 5 WHERE decimalColumn = :decimalColumn AND versionColumn = :versionColumn AND a=b";

			Assertion.AssertEquals("SQL String", expectedSql , sqlString.ToString());
			
			foreach(object part in sqlString.SqlParts) 
			{
				if(part is Parameter) 
				{
					actualParams[numOfParameters] = (Parameter)part;
					numOfParameters++;
				}
			}
			Assertion.AssertEquals("Four parameters", 4, numOfParameters);

			
			Parameter firstParam = new Parameter();
			firstParam.DbType = DbType.Int32;
			firstParam.Name = "intColumn";
		
			Parameter secondParam = new Parameter();
			secondParam.DbType = DbType.Int64;
			secondParam.Name = "longColumn";

			Parameter thirdParam = new Parameter();
			thirdParam.DbType = DbType.Decimal;
			thirdParam.Name = "decimalColumn";
		
			Parameter fourthParam = new Parameter();
			fourthParam.DbType = DbType.Int32;
			fourthParam.Name = "versionColumn";

			Assertion.AssertEquals("firstParam Type", firstParam.DbType, actualParams[0].DbType);
			Assertion.AssertEquals("firstParam Name", firstParam.Name, actualParams[0].Name);

			Assertion.AssertEquals("secondParam Type", secondParam.DbType, actualParams[1].DbType);
			Assertion.AssertEquals("secondParam Name", secondParam.Name, actualParams[1].Name);

			Assertion.AssertEquals("thirdParam Type", thirdParam.DbType, actualParams[2].DbType);
			Assertion.AssertEquals("thirdParam Name", thirdParam.Name, actualParams[2].Name);

			Assertion.AssertEquals("fourthParam Type", fourthParam.DbType, actualParams[3].DbType);
			Assertion.AssertEquals("fourthParam Name", fourthParam.Name, actualParams[3].Name);
		
				
		}

		
	}
}
	