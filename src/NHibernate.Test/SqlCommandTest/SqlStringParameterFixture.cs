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

			Assertion.Assert("Not the same object by ==", (original==cloned)==false);
			Assertion.AssertEquals("Same DbType", original.SqlType.DbType, cloned.SqlType.DbType);
			Assertion.AssertEquals("Same Name", original.Name, cloned.Name);
			
			// change some of the values of the clone to ensure the original doesn't change

			cloned.SqlType = new SqlTypes.StringSqlType();
			cloned.Name = "Cloned name";

			Assertion.Assert("Should not the same db type anymore",  cloned.SqlType.DbType!=original.SqlType.DbType);
			Assertion.Assert("Should not the same name anymore",  cloned.Name!=original.Name);
			

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

			Assertion.Assert("Not the same object by ==", (original==cloned)==false);
			Assertion.AssertEquals("Same DbType", original.SqlType.DbType, cloned.SqlType.DbType);
			Assertion.AssertEquals("Same Name", original.Name, cloned.Name);
			Assertion.AssertEquals("Same Length", original.Length, cloned.Length);
			
			// change some of the values of the clone to ensure the original doesn't change

			cloned.SqlType = new SqlTypes.AnsiStringSqlType(175);
			cloned.Name = "Cloned name";
			cloned.Length = 175;

			Assertion.Assert("Should not the same db type anymore",  cloned.SqlType.DbType!=original.SqlType.DbType);
			Assertion.Assert("Should not the same name anymore",  cloned.Name!=original.Name);
			Assertion.Assert("Should not the same length anymore",  cloned.Length!=original.Length);

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

			Assertion.Assert("Not the same object by ==", (original==cloned)==false);
			Assertion.AssertEquals("Same DbType", original.SqlType.DbType, cloned.SqlType.DbType);
			Assertion.AssertEquals("Same Name", original.Name, cloned.Name);
			Assertion.AssertEquals("Same Precision", original.Precision, cloned.Precision);
			Assertion.AssertEquals("Same Scale", original.Scale, cloned.Scale);
			
			// change some of the values of the clone to ensure the original doesn't change

			// I know AnsiString is not a precision based but for this example we just need
			// to make sure the clonging is working...
			cloned.SqlType = new SqlTypes.AnsiStringSqlType();
			cloned.Name = "Cloned name";
			cloned.Precision = 15;
			cloned.Scale = 4;

			Assertion.Assert("Should not the same db type anymore",  cloned.SqlType.DbType!=original.SqlType.DbType);
			Assertion.Assert("Should not the same name anymore",  cloned.Name!=original.Name);
			Assertion.Assert("Should not the same Precision anymore",  cloned.Precision!=original.Precision);
			Assertion.Assert("Should not the same Scale anymore",  cloned.Scale!=original.Scale);

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

			Assertion.Assert("Not the same SqlString by ==", (original==cloned)==false);
			Assertion.AssertEquals("Same # of parts", original.SqlParts.Length, cloned.SqlParts.Length);
			Assertion.Assert("First param not the same by ==", (original.SqlParts[1]==cloned.SqlParts[1])==false);
			Assertion.Assert("Second param not the same by ==", (original.SqlParts[3]==cloned.SqlParts[3])==false);

			Assertion.AssertEquals("The ToString() is the same", original.ToString(), cloned.ToString());

			// modify the first parameter of the clone to ensure they are not the same
			cloned.SqlParts[0] = "UPDATE changedtablename set param0 = ";
			Parameter lastParamPart = (Parameter)cloned.SqlParts[3];
			lastParamPart.Name = "modifiedOP2";

			Assertion.Assert("Should not be the same ToString()", cloned.ToString()!=original.ToString());

		}
	}
}
