using System;

using NUnit.Framework;

namespace Nullables.Tests
{
	/// <summary>
	/// Summary description for NullableInt16Fixture.
	/// </summary>
	[TestFixture]
	public class NullableInt16Fixture
	{
		[Test]
		public void BasicTestInt16()
		{
			NullableInt16 v1 = 32; //should take an int literal

			Assert.IsTrue(v1.HasValue); //should have a value;
			Assert.IsTrue(v1.Equals(32)); //implicit casting should make this result in true.
			Assert.IsTrue(v1.Value == 32);
			Assert.IsFalse(v1.Equals(NullableInt16.Default));
			Assert.IsTrue(v1.Equals(new NullableInt16(32))); //should == a new instance with the same inner value.

			//same thing, but with == instead of .Equals()
			Assert.IsTrue(v1 == 32);
			Assert.IsFalse(v1 == 33);
			Assert.IsFalse(v1 == NullableInt16.Default);
			Assert.IsTrue(v1 == new NullableInt16(32));

			//now null v1.
			v1 = DBNull.Value;
			Assert.IsTrue(v1 == NullableInt16.Default);
			v1 = NullableInt16.Default;
			Assert.IsTrue(v1 == NullableInt16.Default);

			NullableInt16 v2 = NullableInt16.Default; //should start as "null"

			Assert.IsFalse(v2.HasValue);
			Assert.IsFalse(v2.Equals(12));
			Assert.IsTrue(v2.Equals(NullableInt16.Default));
			Assert.IsTrue(v2.Equals(DBNull.Value));
		}

		[Test, ExpectedException(typeof(InvalidOperationException))]
		public void Int16MissingValueTest()
		{
			NullableInt16 x = NullableInt16.Default;

			Int16 y = x.Value;
		}

		[Test]
		public void Int16IComparableTest()
		{
			NullableInt16 x;
			NullableInt16 y;

			//one null, one not
			x = NullableInt16.Default;
			y = new NullableInt16(16);
			Assert.IsTrue(x.CompareTo(y) < 0);
			Assert.IsTrue(y.CompareTo(x) > 0);

			//now both null
			x = NullableInt16.Default;
			y = NullableInt16.Default;
			Assert.IsTrue(x.CompareTo(y) == 0);
			Assert.IsTrue(y.CompareTo(x) == 0);

			//now both with a value
			x = new NullableInt16(5);
			y = new NullableInt16(43);
			Assert.IsTrue(x.CompareTo(y) < 0);
			Assert.IsTrue(y.CompareTo(x) > 0);
		}

		#region Parse test cases 

		private bool ParseToStringValue(Int16 i)
		{
			return i == NullableInt16.Parse(i.ToString()).Value;
		}


		[Test]
		public void BasicParseTest()
		{
			Assert.IsFalse(NullableInt16.Parse(null).HasValue);
			Assert.IsFalse(NullableInt16.Parse("").HasValue);
			Assert.IsFalse(NullableInt16.Parse("    ").HasValue);
			Assert.IsTrue(ParseToStringValue(0));
			Assert.IsTrue(ParseToStringValue(Int16.MinValue));
			Assert.IsTrue(ParseToStringValue(Int16.MaxValue));
		}

		[Test, ExpectedException(typeof(FormatException))]
		public void InvalidParseValueTest()
		{
			NullableInt16.Parse("invalidvalue");
		}

		#endregion
	}
}