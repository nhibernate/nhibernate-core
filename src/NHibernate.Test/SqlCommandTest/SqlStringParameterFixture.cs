using System;
using NHibernate.SqlCommand;

using NUnit.Framework;

namespace NHibernate.Test.SqlCommandTest
{
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
			Parameter x = Parameter.Placeholder;
			Parameter y = Parameter.Placeholder;

			Assert.IsTrue(x.Equals(y));
			Assert.IsTrue(y.Equals(x));
			Assert.IsFalse(x.Equals(null));
			Assert.IsFalse(x.Equals(10));
		}
	}
}
