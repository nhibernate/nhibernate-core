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

			update.AddColumns(new string[] {"intColumn"}, NHibernateUtil.Int32);
			update.AddColumns(new string[] {"longColumn"}, NHibernateUtil.Int64);
			update.AddColumn("literalColumn", false, (Type.ILiteralType) NHibernateUtil.Boolean);
			update.AddColumn("stringColumn", 5.ToString());
			
			update.SetIdentityColumn(new string[] {"decimalColumn"}, NHibernateUtil.Decimal);
			update.SetVersionColumn(new string[] {"versionColumn"}, (IVersionType)NHibernateUtil.Int32);

			update.AddWhereFragment("a=b");
			SqlString sqlString = update.ToSqlString();
			Parameter[] actualParams = new Parameter[4];
			int numOfParameters = 0;

			string expectedSql = "UPDATE test_update_builder SET intColumn = :intColumn, longColumn = :longColumn, literalColumn = 0, stringColumn = 5 WHERE decimalColumn = :decimalColumn AND versionColumn = :versionColumn AND a=b";

			Assert.AreEqual(expectedSql , sqlString.ToString(), "SQL String");
			
			foreach(object part in sqlString.SqlParts) 
			{
				if(part is Parameter) 
				{
					actualParams[numOfParameters] = (Parameter)part;
					numOfParameters++;
				}
			}
			Assert.AreEqual(4, numOfParameters, "Four parameters");

			
			Parameter firstParam = new Parameter( "intColumn", new SqlTypes.Int32SqlType() );
			
			Parameter secondParam = new Parameter( "longColumn", new SqlTypes.Int64SqlType() );
			
			Parameter thirdParam = new Parameter( "decimalColumn", new SqlTypes.DecimalSqlType() );
			
			Parameter fourthParam = new Parameter( "versionColumn", new SqlTypes.Int32SqlType() );
			
			Assert.AreEqual(firstParam.SqlType.DbType, actualParams[0].SqlType.DbType, "firstParam Type");
			Assert.AreEqual(firstParam.Name, actualParams[0].Name, "firstParam Name");

			Assert.AreEqual(secondParam.SqlType.DbType, actualParams[1].SqlType.DbType, "secondParam Type");
			Assert.AreEqual(secondParam.Name, actualParams[1].Name, "secondParam Name");

			Assert.AreEqual(thirdParam.SqlType.DbType, actualParams[2].SqlType.DbType, "thirdParam Type");
			Assert.AreEqual(thirdParam.Name, actualParams[2].Name, "thirdParam Name");

			Assert.AreEqual(fourthParam.SqlType.DbType, actualParams[3].SqlType.DbType, "fourthParam Type");
			Assert.AreEqual(fourthParam.Name, actualParams[3].Name, "fourthParam Name");
		
				
		}

		
	}
}
	