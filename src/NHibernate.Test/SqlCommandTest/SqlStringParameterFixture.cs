using System;
using System.Data;

using NHibernate.Cfg;
using NHibernate.Engine;
using NHibernate.SqlCommand;

using NUnit.Framework;

namespace NHibernate.Test.SqlCommandTest
{
	/// <summary>
	/// Summary description for SqlFragmentTest.
	/// </summary>
	[TestFixture]
	public class SqlStringParameterFixture
	{
	
		[Test]
		public void TestParameterClone()
		{
			Parameter original = new Parameter();
			Parameter cloned = null;

			original.SqlType = new SqlTypes.Int32SqlType();
			original.Name = "originalName";
			
			cloned = (Parameter)original.Clone();

			Assert.IsTrue((original==cloned)==false, "Not the same object by ==");
			Assert.AreEqual(original.SqlType.DbType, cloned.SqlType.DbType, "Same DbType");
			Assert.AreEqual(original.Name, cloned.Name, "Same Name");
			
			// change some of the values of the clone to ensure the original doesn't change

			cloned.SqlType = new SqlTypes.StringSqlType();
			cloned.Name = "Cloned name";

			Assert.IsTrue(cloned.SqlType.DbType!=original.SqlType.DbType, "Should not the same db type anymore");
			Assert.IsTrue(cloned.Name!=original.Name, "Should not the same name anymore");
			

		}

		[Test]
		public void TestParameterLengthClone()
		{
			ParameterLength original = new ParameterLength();
			ParameterLength cloned = null;

			original.SqlType = new SqlTypes.StringSqlType(275);
			original.Name = "originalName";
			original.Length = 275;
			
			cloned = (ParameterLength)original.Clone();

			Assert.IsTrue((original==cloned)==false, "Not the same object by ==");
			Assert.AreEqual(original.SqlType.DbType, cloned.SqlType.DbType, "Same DbType");
			Assert.AreEqual(original.Name, cloned.Name, "Same Name");
			Assert.AreEqual(original.Length, cloned.Length, "Same Length");
			
			// change some of the values of the clone to ensure the original doesn't change

			cloned.SqlType = new SqlTypes.AnsiStringSqlType(175);
			cloned.Name = "Cloned name";
			cloned.Length = 175;

			Assert.IsTrue(cloned.SqlType.DbType!=original.SqlType.DbType, "Should not the same db type anymore");
			Assert.IsTrue(cloned.Name!=original.Name, "Should not the same name anymore");
			Assert.IsTrue(cloned.Length!=original.Length, "Should not the same length anymore");

		}

		[Test]
		public void TestParameterPrecisionClone()
		{
			ParameterPrecisionScale original = new ParameterPrecisionScale();
			ParameterPrecisionScale cloned = null;

			original.SqlType = new SqlTypes.DecimalSqlType(19, 5);
			original.Name = "originalName";
			original.Precision = 19;
			original.Scale = 5;
			
			cloned = (ParameterPrecisionScale)original.Clone();

			Assert.IsTrue((original==cloned)==false, "Not the same object by ==");
			Assert.AreEqual(original.SqlType.DbType, cloned.SqlType.DbType, "Same DbType");
			Assert.AreEqual(original.Name, cloned.Name, "Same Name");
			Assert.AreEqual(original.Precision, cloned.Precision, "Same Precision");
			Assert.AreEqual(original.Scale, cloned.Scale, "Same Scale");
			
			// change some of the values of the clone to ensure the original doesn't change

			// I know AnsiString is not a precision based but for this example we just need
			// to make sure the clonging is working...
			cloned.SqlType = new SqlTypes.AnsiStringSqlType();
			cloned.Name = "Cloned name";
			cloned.Precision = 15;
			cloned.Scale = 4;

			Assert.IsTrue(cloned.SqlType.DbType!=original.SqlType.DbType, "Should not the same db type anymore");
			Assert.IsTrue(cloned.Name!=original.Name, "Should not the same name anymore");
			Assert.IsTrue(cloned.Precision!=original.Precision, "Should not the same Precision anymore");
			Assert.IsTrue(cloned.Scale!=original.Scale, "Should not the same Scale anymore");

		}
		
		[Test]
		public void TestSqlStringClone() 
		{

			SqlStringBuilder sqlBuilder = new SqlStringBuilder();

			SqlString original = null;
			SqlString cloned = null;

			Parameter[] origParams = new Parameter[2];
			
			origParams[0] = new Parameter();
			origParams[0].SqlType = new SqlTypes.Int32SqlType();
			origParams[0].Name = "OP1";
			
			origParams[1] = new Parameter();
			origParams[1].SqlType = new SqlTypes.Int64SqlType();
			origParams[1].Name = "OP2";

			sqlBuilder.Add("UPDATE tablename set param0 = ")
				.Add(origParams[0])
				.Add(", param1 =")
				.Add(origParams[1]);

		
			original = sqlBuilder.ToSqlString();

			cloned = original.Clone();

			Assert.IsTrue((original==cloned)==false, "Not the same SqlString by ==");
			Assert.AreEqual(original.SqlParts.Length, cloned.SqlParts.Length, "Same # of parts");
			Assert.IsTrue((original.SqlParts[1]==cloned.SqlParts[1])==false, "First param not the same by ==");
			Assert.IsTrue((original.SqlParts[3]==cloned.SqlParts[3])==false, "Second param not the same by ==");

			Assert.AreEqual(original.ToString(), cloned.ToString(), "The ToString() is the same");

			// modify the first parameter of the clone to ensure they are not the same
			cloned.SqlParts[0] = "UPDATE changedtablename set param0 = ";
			Parameter lastParamPart = (Parameter)cloned.SqlParts[3];
			lastParamPart.Name = "modifiedOP2";

			Assert.IsTrue(cloned.ToString()!=original.ToString(), "Should not be the same ToString()");

		}
	}
}
