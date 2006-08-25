using System;
using System.Data;

using NHibernate.Cfg;
using NHibernate.Engine;
using NHibernate.SqlCommand;

using NUnit.Framework;
using NHibernate.SqlTypes;

namespace NHibernate.Test.SqlCommandTest
{
	/// <summary>
	/// Summary description for SqlFragmentTest.
	/// </summary>
	[TestFixture]
	public class SqlStringParameterFixture
	{
		/*
		 * "The equals() method implements an equivalence relation: 
				- It is reflexive: For any reference value x, x.equals(x) should return true
				- It is symmetric: For any reference values x and y, x.equals(y) should return 
					true if and only if y.equals(x) returns true
				- It is transitive: For any reference values x, y, and z, if x.equals(y) returns 
					true and y.equals(z) returns true, then x.equals(z) should return true
				- It is consistent: For any reference values x and y, multiple invocations of 
					x.equals(y) consistently return true or consistently return false, provided no 
					information used in equals comparisons on the object is modified
				- For any non-null reference value x, x.equals(null) should return false"
		*/

		[Test]
		public void EqualsSameType()
		{
			Parameter x = new Parameter("name", "alias", SqlTypeFactory.Int32);
			Parameter y = new Parameter("name", "alias", SqlTypeFactory.Int32);
			Parameter z = new Parameter("name", "alias", SqlTypeFactory.Int32);

			Assert.IsTrue(x.Equals(y));
			Assert.IsTrue(y.Equals(x));
			Assert.IsTrue(y.Equals(z));
			Assert.IsTrue(x.Equals(z));
			Assert.IsFalse(x.Equals(null));

			y = new Parameter("name2", "alias", SqlTypeFactory.Int32);

			Assert.IsFalse(x.Equals(y));
			Assert.IsFalse(y.Equals(x));
		}

		[Test]
		public void EqualsLengthType()
		{
			Parameter x = new Parameter("name", "alias", new SqlTypes.AnsiStringSqlType(5));
			Parameter y = new Parameter("name", "alias", new SqlTypes.AnsiStringSqlType(5));
			Parameter z = new Parameter("name", "alias", new SqlTypes.AnsiStringSqlType(5));

			Assert.IsTrue(x.Equals(y));
			Assert.IsTrue(y.Equals(x));
			Assert.IsTrue(y.Equals(z));
			Assert.IsTrue(x.Equals(z));
			Assert.IsFalse(x.Equals(null));

			y = new Parameter("name2", "alias", new SqlTypes.AnsiStringSqlType(5));

			Assert.IsFalse(x.Equals(y));
			Assert.IsFalse(y.Equals(x));
		}

		[Test]
		public void EqualsPrecisionType()
		{
			Parameter x = new Parameter("name", "alias", SqlTypeFactory.GetDecimal(20, 4));
			Parameter y = new Parameter("name", "alias", SqlTypeFactory.GetDecimal(20, 4));
			Parameter z = new Parameter("name", "alias", SqlTypeFactory.GetDecimal(20, 4));

			Assert.IsTrue(x.Equals(y));
			Assert.IsTrue(y.Equals(x));
			Assert.IsTrue(y.Equals(z));
			Assert.IsTrue(x.Equals(z));
			Assert.IsFalse(x.Equals(null));

			y = new Parameter("name2", "alias", SqlTypeFactory.GetDecimal(20, 4));

			Assert.IsFalse(x.Equals(y));
			Assert.IsFalse(y.Equals(x));

			Parameter t = new Parameter("name", "alias", SqlTypeFactory.GetDecimal(20, 5));
			Assert.IsFalse(x.Equals(t));
		}



		[Test]
		public void TestParameterClone()
		{
			Parameter original = new Parameter("originalName", SqlTypeFactory.Int32);
			Parameter cloned = null;

			cloned = (Parameter) original.Clone();

			Assert.IsFalse(original == cloned, "Not the same object by ==");
			Assert.AreEqual(original.SqlType.DbType, cloned.SqlType.DbType, "Same DbType");
			Assert.AreEqual(original.Name, cloned.Name, "Same Name");
		}

		[Test]
		public void TestSqlStringClone()
		{
			SqlStringBuilder sqlBuilder = new SqlStringBuilder();

			SqlString original = null;
			SqlString cloned = null;

			Parameter[] origParams = new Parameter[2];

			origParams[0] = new Parameter("OP1", SqlTypeFactory.Int32);
			origParams[1] = new Parameter("OP2", SqlTypeFactory.Int64);

			sqlBuilder.Add("UPDATE tablename set param0 = ")
				.Add(origParams[0])
				.Add(", param1 =")
				.Add(origParams[1]);


			original = sqlBuilder.ToSqlString();

			cloned = original.Clone();

			Assert.IsTrue((original == cloned) == false, "Not the same SqlString by ==");
			Assert.AreEqual(original.SqlParts.Length, cloned.SqlParts.Length, "Same # of parts");
			Assert.IsTrue((original.SqlParts[1] == cloned.SqlParts[1]) == false, "First param not the same by ==");
			Assert.IsTrue((original.SqlParts[3] == cloned.SqlParts[3]) == false, "Second param not the same by ==");

			Assert.AreEqual(original.ToString(), cloned.ToString(), "The ToString() is the same");

			// modify the first parameter of the clone to ensure they are not the same
			cloned.SqlParts[0] = "UPDATE changedtablename set param0 = ";
			cloned.SqlParts[3] = new Parameter("modifiedOP2");

			Assert.IsTrue(cloned.ToString() != original.ToString(), "Should not be the same ToString()");

		}
	}
}
