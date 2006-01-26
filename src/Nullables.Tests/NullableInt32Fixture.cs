using System;
using Nullables;
using NUnit.Framework;

namespace Nullables.Tests
{
	/// <summary>
	/// Summary description for NullableInt32Fixture.
	/// </summary>
	[TestFixture]
	public class NullableInt32Fixture
	{
		[Test]
		public void BasicTestInt32()
		{
			NullableInt32 v1 = 32; //should take an int literal

			Assert.IsTrue( v1.HasValue ); //should have a value;
			Assert.IsTrue( v1.Equals( 32 ) ); //implicit casting should make this result in true.
			Assert.IsTrue( v1.Value == 32 );
			Assert.IsFalse( v1.Equals( NullableInt32.Default ) );
			Assert.IsTrue( v1.Equals( new NullableInt32( 32 ) ) ); //should == a new instance with the same inner value.

			//same thing, but with == instead of .Equals()
			Assert.IsTrue( v1 == 32 );
			Assert.IsFalse( v1 == 33 );
			Assert.IsFalse( v1 == NullableInt32.Default );
			Assert.IsTrue( v1 == new NullableInt32( 32 ) );

			//now null v1.
			v1 = DBNull.Value;
			Assert.IsTrue( v1 == NullableInt32.Default );
			v1 = NullableInt32.Default;
			Assert.IsTrue( v1 == NullableInt32.Default );

			NullableInt32 v2 = NullableInt32.Default; //should start as "null"

			Assert.IsFalse( v2.HasValue );
			
			Assert.IsFalse( v2.Equals( 12 ) );
			Assert.IsTrue( v2.Equals( NullableInt32.Default ) );
			Assert.IsTrue( v2.Equals( DBNull.Value ) );
			Assert.IsTrue( v2 == null );
		}

		[Test, ExpectedException(typeof(InvalidOperationException))]
		public void Int32MissingValueTest()
		{
			NullableInt32 x = NullableInt32.Default;

			int y = x.Value;
		}

		[Test]
		public void Int32IComparableTest()
		{
			NullableInt32 x;
			NullableInt32 y;

			//one null, one not
			x = NullableInt32.Default;
			y = new NullableInt32( 16 );
			Assert.IsTrue( x.CompareTo( y ) < 0 );
			Assert.IsTrue( y.CompareTo( x ) > 0 );

			//now both null
			x = NullableInt32.Default;
			y = NullableInt32.Default;
			Assert.IsTrue( x.CompareTo( y ) == 0 );
			Assert.IsTrue( y.CompareTo( x ) == 0 );

			//now both with a value
			x = new NullableInt32( 5 );
			y = new NullableInt32( 43 );
			Assert.IsTrue( x.CompareTo( y ) < 0 );
			Assert.IsTrue( y.CompareTo( x ) > 0 );
		}
		#region Parse test cases 

		private bool ParseToStringValue(Int32 i) 
		{
			return i == NullableInt32.Parse(i.ToString()).Value;
		}


		[Test]
		public void BasicParseTest()
		{
			Assert.IsFalse(NullableInt32.Parse(null).HasValue);
			Assert.IsFalse(NullableInt32.Parse("").HasValue);
			Assert.IsFalse(NullableInt32.Parse("    ").HasValue);
			Assert.IsTrue(ParseToStringValue(0));
			Assert.IsTrue(ParseToStringValue(Int32.MinValue));
			Assert.IsTrue(ParseToStringValue(Int32.MaxValue));
		}

		[Test, ExpectedException(typeof(FormatException))]
		public void InvalidParseValueTest()
		{
			NullableInt32.Parse("invalidvalue");
		}
		
		#endregion
	}
}