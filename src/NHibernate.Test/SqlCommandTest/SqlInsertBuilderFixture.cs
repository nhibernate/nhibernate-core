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
	/// Tests all of the functionallity of the SqlInsertBulder
	/// </summary>
	[TestFixture]
	public class SqlInsertBuilderFixture
	{
		
		[Test]
		public void InsertSqlStringTest() 
		{
			Configuration cfg = new Configuration();
			ISessionFactory factory = cfg.BuildSessionFactory( );

			ISessionFactoryImplementor factoryImpl = (ISessionFactoryImplementor)factory;
			SqlInsertBuilder insert = new SqlInsertBuilder(factoryImpl);
			
			insert.SetTableName("test_insert_builder");

			insert.AddColumn(new string[] {"intColumn"}, NHibernate.Int32);
			insert.AddColumn(new string[] {"longColumn"}, NHibernate.Int64);
			insert.AddColumn("literalColumn", false, (Type.ILiteralType) NHibernate.Boolean);
			insert.AddColumn("stringColumn", 5.ToString());
			
			SqlString sqlString = insert.ToSqlString();
			Parameter[] actualParams = new Parameter[2];
			int numOfParameters = 0;
			
			string expectedSql = "INSERT INTO test_insert_builder (intColumn, longColumn, literalColumn, stringColumn) VALUES (:intColumn, :longColumn, 0, 5)";
			Assertion.AssertEquals("SQL String", expectedSql , sqlString.ToString());
			
			foreach(object part in sqlString.SqlParts) 
			{
				if(part is Parameter) 
				{
					actualParams[numOfParameters] = (Parameter)part;
					numOfParameters++;
				}
			}

			Assertion.AssertEquals("Two parameters", 2, numOfParameters);

			Parameter firstParam = new Parameter();
			firstParam.DbType = DbType.Int32;
			firstParam.Name = "intColumn";
		
			Parameter secondParam = new Parameter();
			secondParam.DbType = DbType.Int64;
			secondParam.Name = "longColumn";

			Assertion.AssertEquals("First Parameter Type", firstParam.DbType, actualParams[0].DbType);
			Assertion.AssertEquals("First Parameter Name", firstParam.Name, actualParams[0].Name);

			Assertion.AssertEquals("Second Parameter Type", secondParam.DbType, actualParams[1].DbType);
			Assertion.AssertEquals("Second Parameter Name", secondParam.Name, actualParams[1].Name);
		
				
		}
	}
}
	