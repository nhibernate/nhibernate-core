using System;
using Nullables;
using NUnit.Framework;

namespace Nullables.Tests
{
	/// <summary>
	/// Summary description for NullableInt64Fixture.
	/// </summary>
	[TestFixture]
	public class NullableInt64Fixture
	{
		[Test]
		public void BasicTestInt64()
		{
			NullableInt64 v1 = 46816684; //should take an int literal

			Assert.IsTrue( v1.HasValue ); //should have a value;
			Assert.IsTrue( v1.Equals( 46816684 ) ); //implicit casting should make this result in true.
			Assert.IsTrue( v1.Value == 46816684 );
			Assert.IsFalse( v1.Equals( NullableInt64.Default ) );
			Assert.IsTrue( v1.Equals( new NullableInt64( 46816684 ) ) ); //should == a new instance with the same inner value.

			//same thing, but with == instead of .Equals()
			Assert.IsTrue( v1 == 46816684 );
			Assert.IsFalse( v1 == 448494894 );
			Assert.IsFalse( v1 == NullableInt64.Default );
			Assert.IsTrue( v1 == new NullableInt64( 46816684 ) );

			//now null v1.
			v1 = DBNull.Value;
			Assert.IsTrue( v1 == NullableInt64.Default );
			v1 = NullableInt64.Default;
			Assert.IsTrue( v1 == NullableInt64.Default );

			NullableInt64 v2 = NullableInt64.Default; //should start as "null"

			Assert.IsFalse( v2.HasValue );
			Assert.IsFalse( v2.Equals( 4484 ) );
			Assert.IsTrue( v2.Equals( NullableInt64.Default ) );
			Assert.IsTrue( v2.Equals( DBNull.Value ) );
		}

		[Test, ExpectedException(typeof(InvalidOperationException))]
		public void Int64MissingValueTest()
		{
			NullableInt64 x = NullableInt64.Default;

			Int64 y = x.Value;
		}

		[Test]
		public void Int64IComparableTest()
		{
			NullableInt64 x;
			NullableInt64 y;

			//one null, one not
			x = NullableInt64.Default;
			y = new NullableInt64( 16 );
			Assert.IsTrue( x.CompareTo( y ) < 0 );
			Assert.IsTrue( y.CompareTo( x ) > 0 );

			//now both null
			x = NullableInt64.Default;
			y = NullableInt64.Default;
			Assert.IsTrue( x.CompareTo( y ) == 0 );
			Assert.IsTrue( y.CompareTo( x ) == 0 );

			//now both with a value
			x = new NullableInt64( 5 );
			y = new NullableInt64( 43 );
			Assert.IsTrue( x.CompareTo( y ) < 0 );
			Assert.IsTrue( y.CompareTo( x ) > 0 );
		}

		#region Parse test cases 

		private bool ParseToStringValue(Int64 i) 
		{
			return i == NullableInt64.Parse(i.ToString()).Value;
		}


		[Test]
		public void BasicParseTest()
		{
			Assert.IsFalse(NullableInt64.Parse(null).HasValue);
			Assert.IsFalse(NullableInt64.Parse("").HasValue);
			Assert.IsFalse(NullableInt64.Parse("    ").HasValue);
			Assert.IsTrue(ParseToStringValue(0));
			Assert.IsTrue(ParseToStringValue(Int64.MinValue));
			Assert.IsTrue(ParseToStringValue(Int64.MaxValue));
		}

		[Test, ExpectedException(typeof(FormatException))]
		public void InvalidParseValueTest()
		{
			NullableInt64.Parse("invalidvalue");
		}
		
		#endregion
	}
}