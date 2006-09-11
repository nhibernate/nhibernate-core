using System;
using System.Data;

using NHibernate.Cfg;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.Type;

using NUnit.Framework;
using NHibernate.SqlTypes;

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

			SqlCommandInfo sqlCommand = insert.ToSqlCommandInfo();
			SqlType[] actualParameterTypes = sqlCommand.ParameterTypes;
			
			string expectedSql = "INSERT INTO test_insert_builder (intColumn, longColumn, literalColumn, stringColumn) VALUES (?, ?, 0, 5)";
			Assert.AreEqual(expectedSql , sqlCommand.Text.ToString(), "SQL String");

			Assert.AreEqual(2, actualParameterTypes.Length);
			Assert.AreEqual(SqlTypeFactory.Int32, actualParameterTypes[0], "First Parameter Type");
			Assert.AreEqual(SqlTypeFactory.Int64, actualParameterTypes[1], "Second Parameter Type");
		}
	}
}
	