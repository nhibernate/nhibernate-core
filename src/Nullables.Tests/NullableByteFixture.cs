using System;
using Nullables;
using NUnit.Framework;

namespace Nullables.Tests
{
	/// <summary>
	/// Summary description for NullableByteFixture.
	/// </summary>
	[TestFixture]
	public class NullableByteFixture
	{
		[Test]
		public void BasicTestByte()
		{
			NullableByte v1 = 32; //should take an int literal

			Assert.IsTrue( v1.HasValue ); //should have a value;
			Assert.IsTrue( v1.Equals( 32 ) ); //implicit casting should make this result in true.
			Assert.IsTrue( v1.Value == 32 );
			Assert.IsFalse( v1.Equals( NullableByte.Default ) );
			Assert.IsTrue( v1.Equals( new NullableByte( 32 ) ) ); //should == a new instance with the same inner value.

			//same thing, but with == instead of .Equals()
			Assert.IsTrue( v1 == 32 );
			Assert.IsFalse( v1 == 33 );
			Assert.IsFalse( v1 == NullableByte.Default );
			Assert.IsTrue( v1 == new NullableByte( 32 ) );

			//now null v1.
			v1 = DBNull.Value;
			Assert.IsTrue( v1 == NullableByte.Default );
			v1 = NullableByte.Default;
			Assert.IsTrue( v1 == NullableByte.Default );

			NullableByte v2 = NullableByte.Default; //should start as "null"

			Assert.IsFalse( v2.HasValue );
			Assert.IsFalse( v2.Equals( 12 ) );
			Assert.IsTrue( v2.Equals( NullableByte.Default ) );
			Assert.IsTrue( v2.Equals( DBNull.Value ) );
		}

		[Test, ExpectedException(typeof(InvalidOperationException))]
		public void ByteMissingValueTest()
		{
			NullableByte x = NullableByte.Default;

			byte y = x.Value;
		}

		[Test]
		public void ByteIComparableTest()
		{
			NullableByte x;
			NullableByte y;

			//one null, one not
			x = NullableByte.Default;
			y = new NullableByte( 12 );
			Assert.IsTrue( x.CompareTo( y ) < 0 );
			Assert.IsTrue( y.CompareTo( x ) > 0 );

			//now both null
			x = NullableByte.Default;
			y = NullableByte.Default;
			Assert.IsTrue( x.CompareTo( y ) == 0 );
			Assert.IsTrue( y.CompareTo( x ) == 0 );

			//now both with a value
			x = new NullableByte( 5 );
			y = new NullableByte( 101 );
			Assert.IsTrue( x.CompareTo( y ) < 0 );
			Assert.IsTrue( y.CompareTo( x ) > 0 );
		}

		#region Parse test cases 

		private bool ParseToStringValue(byte b) 
		{
			return b == NullableByte.Parse(b.ToString()).Value;
		}


		[Test]
		public void BasicParseTest()
		{
			Assert.IsFalse(NullableByte.Parse(null).HasValue);
			Assert.IsFalse(NullableByte.Parse("").HasValue);
			Assert.IsFalse(NullableByte.Parse("    ").HasValue);
			Assert.IsTrue(ParseToStringValue(0));
			Assert.IsTrue(ParseToStringValue(byte.MinValue));
			Assert.IsTrue(ParseToStringValue(byte.MaxValue));
		}

		[Test, ExpectedException(typeof(FormatException))]
		public void InvalidParseValueTest()
		{
			NullableByte.Parse("invalidvalue");
		}
		
		#endregion
	}
}