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

			insert.AddColumn(new string[] {"intColumn"}, NHibernateUtil.Int32);
			insert.AddColumn(new string[] {"longColumn"}, NHibernateUtil.Int64);
			insert.AddColumn("literalColumn", false, (Type.ILiteralType) NHibernateUtil.Boolean);
			insert.AddColumn("stringColumn", 5.ToString());
			
			SqlString sqlString = insert.ToSqlString();
			Parameter[] actualParams = new Parameter[2];
			int numOfParameters = 0;
			
			string expectedSql = "INSERT INTO test_insert_builder (intColumn, longColumn, literalColumn, stringColumn) VALUES (:intColumn, :longColumn, 0, 5)";
			Assert.AreEqual(expectedSql , sqlString.ToString(), "SQL String");
			
			foreach(object part in sqlString.SqlParts) 
			{
				if(part is Parameter) 
				{
					actualParams[numOfParameters] = (Parameter)part;
					numOfParameters++;
				}
			}

			Assert.AreEqual(2, numOfParameters, "Two parameters");

			Parameter firstParam = new Parameter( "intColumn", new SqlTypes.Int32SqlType() );
			
			Parameter secondParam = new Parameter( "longColumn", new SqlTypes.Int64SqlType() );
			
			Assert.AreEqual(firstParam.SqlType.DbType, actualParams[0].SqlType.DbType, "First Parameter Type");
			Assert.AreEqual(firstParam.Name, actualParams[0].Name, "First Parameter Name");

			Assert.AreEqual(secondParam.SqlType.DbType, actualParams[1].SqlType.DbType, "Second Parameter Type");
			Assert.AreEqual(secondParam.Name, actualParams[1].Name, "Second Parameter Name");
		
				
		}
	}
}
	