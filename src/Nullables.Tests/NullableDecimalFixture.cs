using System;

using NUnit.Framework;

namespace Nullables.Tests
{
	/// <summary>
	/// Summary description for NullableDecimalFixture.
	/// </summary>
	[TestFixture]
	public class NullableDecimalFixture
	{
		[Test]
		public void BasicTestDecimal()
		{
			NullableDecimal v1 = 4.99m; //should take an int literal

			Assert.IsTrue(v1.HasValue); //should have a value;
			Assert.IsTrue(v1.Equals(4.99m)); //implicit casting should make this result in true.
			Assert.IsTrue(v1.Value == 4.99m);
			Assert.IsFalse(v1.Equals(NullableDecimal.Default));
			Assert.IsTrue(v1.Equals(new NullableDecimal(4.99m))); //should == a new instance with the same inner value.

			//same thing, but with == instead of .Equals()
			Assert.IsTrue(v1 == 4.99m);
			Assert.IsFalse(v1 == 5.01m);
			Assert.IsFalse(v1 == NullableDecimal.Default);
			Assert.IsTrue(v1 == new NullableDecimal(4.99m));

			//now null v1.
			v1 = DBNull.Value;
			Assert.IsTrue(v1 == NullableDecimal.Default);
			v1 = NullableDecimal.Default;
			Assert.IsTrue(v1 == NullableDecimal.Default);

			NullableDecimal v2 = NullableDecimal.Default; //should start as "null"

			Assert.IsFalse(v2.HasValue);
			Assert.IsFalse(v2.Equals(4.01m));
			Assert.IsTrue(v2.Equals(NullableDecimal.Default));
			Assert.IsTrue(v2.Equals(DBNull.Value));
		}

		[Test, ExpectedException(typeof(InvalidOperationException))]
		public void DecimalMissingValueTest()
		{
			NullableDecimal x = NullableDecimal.Default;

			decimal y = x.Value;
		}

		[Test]
		public void DecimalIComparableTest()
		{
			NullableDecimal x;
			NullableDecimal y;

			//one null, one not
			x = NullableDecimal.Default;
			y = new NullableDecimal(10.03m);
			Assert.IsTrue(x.CompareTo(y) < 0);
			Assert.IsTrue(y.CompareTo(x) > 0);

			//now both null
			x = NullableDecimal.Default;
			y = NullableDecimal.Default;
			Assert.IsTrue(x.CompareTo(y) == 0);
			Assert.IsTrue(y.CompareTo(x) == 0);

			//now both with a value
			x = new NullableDecimal(15.3m);
			y = new NullableDecimal(18.02m);
			Assert.IsTrue(x.CompareTo(y) < 0);
			Assert.IsTrue(y.CompareTo(x) > 0);
		}

		#region Parse test cases 

		private bool ParseToStringValue(Decimal d)
		{
			return d == NullableDecimal.Parse(d.ToString()).Value;
		}


		[Test]
		public void BasicParseTest()
		{
			Assert.IsFalse(NullableDecimal.Parse(null).HasValue);
			Assert.IsFalse(NullableDecimal.Parse("").HasValue);
			Assert.IsFalse(NullableDecimal.Parse("    ").HasValue);
			Assert.IsTrue(ParseToStringValue(0));
			Assert.IsTrue(ParseToStringValue(Decimal.MinValue));
			Assert.IsTrue(ParseToStringValue(Decimal.MaxValue));
		}

		[Test, ExpectedException(typeof(FormatException))]
		public void InvalidParseValueTest()
		{
			NullableDecimal.Parse("invalidvalue");
		}

		#endregion
	}
}