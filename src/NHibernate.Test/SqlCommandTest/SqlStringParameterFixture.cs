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
			Parameter x = new Parameter(SqlTypeFactory.Int32);
			Parameter y = new Parameter(SqlTypeFactory.Int32);
			Parameter z = new Parameter(SqlTypeFactory.Int32);

			Assert.IsTrue(x.Equals(y));
			Assert.IsTrue(y.Equals(x));
			Assert.IsTrue(y.Equals(z));
			Assert.IsTrue(x.Equals(z));
			Assert.IsFalse(x.Equals(null));
		}

		[Test]
		public void EqualsLengthType()
		{
			Parameter x = new Parameter(new SqlTypes.AnsiStringSqlType(5));
			Parameter y = new Parameter(new SqlTypes.AnsiStringSqlType(5));
			Parameter z = new Parameter(new SqlTypes.AnsiStringSqlType(5));

			Assert.IsTrue(x.Equals(y));
			Assert.IsTrue(y.Equals(x));
			Assert.IsTrue(y.Equals(z));
			Assert.IsTrue(x.Equals(z));
			Assert.IsFalse(x.Equals(null));
		}

		[Test]
		public void EqualsPrecisionType()
		{
			Parameter x = new Parameter(SqlTypeFactory.GetDecimal(20, 4));
			Parameter y = new Parameter(SqlTypeFactory.GetDecimal(20, 4));
			Parameter z = new Parameter(SqlTypeFactory.GetDecimal(20, 4));

			Assert.IsTrue(x.Equals(y));
			Assert.IsTrue(y.Equals(x));
			Assert.IsTrue(y.Equals(z));
			Assert.IsTrue(x.Equals(z));
			Assert.IsFalse(x.Equals(null));
			
			Parameter t = new Parameter(SqlTypeFactory.GetDecimal(20, 5));
			Assert.IsFalse(x.Equals(t));
		}
		
		// TODO: test ReplaceParameterTypes
	}
}
