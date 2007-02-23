using System;
using NHibernate.Type;
using NUnit.Framework;

namespace NHibernate.Test.TypesTest
{
	/// <summary>
	/// The Unit Tests for the DecimalType
	/// </summary>
	[TestFixture]
	public class DecimalTypeFixture : TypeFixtureBase
	{
		protected override string TypeName
		{
			get { return "Decimal"; }
		}

		/// <summary>
		/// Test that two decimal fields that are exactly equal are returned
		/// as Equal by the DecimalType.
		/// </summary>
		[Test]
		public void Equals()
		{
			decimal lhs = 5.64351M;
			decimal rhs = 5.64351M;

			DecimalType type = (DecimalType) NHibernateUtil.Decimal;
			Assert.IsTrue(type.Equals(lhs, rhs));

			// Test that two decimal fields that are equal except one has a higher precision than
			// the other one are returned as Equal by the DecimalType.
			rhs = 5.643510M;
			Assert.IsTrue(type.Equals(lhs, rhs));
		}

		[Test]
		public void ReadWrite()
		{
			decimal expected = 5.64351M;

			DecimalClass basic = new DecimalClass();
			basic.Id = 1;
			basic.DecimalValue = expected;

			ISession s = OpenSession();
			s.Save(basic);
			s.Flush();
			s.Close();

			s = OpenSession();
			basic = (DecimalClass) s.Load(typeof(DecimalClass), 1);

			Assert.AreEqual(expected, basic.DecimalValue);
			Assert.AreEqual(5.643510M, basic.DecimalValue);

			s.Delete(basic);
			s.Flush();
			s.Close();
		}

		[Test]
		public void UnsavedValue()
		{
			DecimalType type = (DecimalType) NHibernateUtil.Decimal;
			object mappedValue = type.StringToObject("0");
			Assert.AreEqual(0m, mappedValue);
			Assert.IsTrue(type.Equals(mappedValue, 0m), "'0' in the mapping file should have been converted to a 0m");
		}
	}
}