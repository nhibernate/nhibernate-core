using System;
using Nullables;
using NUnit.Framework;

namespace Nullables.Tests
{
	/// <summary>
	/// Summary description for NullableSByteFixture.
	/// </summary>
	[TestFixture]
	public class NullableSByteFixture
	{
		public NullableSByteFixture()
		{
		}

		[Test]
		public void BasicTestSByte()
		{
			NullableSByte v1 = 32; //should take an int literal

			Assert.IsTrue( v1.HasValue ); //should have a value;
			Assert.IsTrue( v1.Equals( 32 ) ); //implicit casting should make this result in true.
			Assert.IsTrue( v1.Value == 32 );
			Assert.IsFalse( v1.Equals( NullableSByte.Default ) );
			Assert.IsTrue( v1.Equals( new NullableSByte( 32 ) ) ); //should == a new instance with the same inner value.

			//same thing, but with == instead of .Equals()
			Assert.IsTrue( v1 == 32 );
			Assert.IsFalse( v1 == 33 );
			Assert.IsFalse( v1 == NullableSByte.Default );
			Assert.IsTrue( v1 == new NullableSByte( 32 ) );

			//now null v1.
			v1 = DBNull.Value;
			Assert.IsTrue( v1 == NullableSByte.Default );
			v1 = NullableSByte.Default;
			Assert.IsTrue( v1 == NullableSByte.Default );

			NullableSByte v2 = NullableSByte.Default; //should start as "null"

			Assert.IsFalse( v2.HasValue );
			Assert.IsFalse( v2.Equals( 12 ) );
			Assert.IsTrue( v2.Equals( NullableSByte.Default ) );
			Assert.IsTrue( v2.Equals( DBNull.Value ) );
		}

		[Test, ExpectedException(typeof(InvalidOperationException))]
		public void SByteMissingValueTest()
		{
			NullableSByte x = NullableSByte.Default;

			SByte y = x.Value;
		}

		[Test]
		public void SByteIComparableTest()
		{
			NullableSByte x;
			NullableSByte y;

			//one null, one not
			x = NullableSByte.Default;
			y = new NullableSByte( 16 );
			Assert.IsTrue( x.CompareTo( y ) < 0 );
			Assert.IsTrue( y.CompareTo( x ) > 0 );

			//now both null
			x = NullableSByte.Default;
			y = NullableSByte.Default;
			Assert.IsTrue( x.CompareTo( y ) == 0 );
			Assert.IsTrue( y.CompareTo( x ) == 0 );

			//now both with a value
			x = new NullableSByte( 5 );
			y = new NullableSByte( 43 );
			Assert.IsTrue( x.CompareTo( y ) < 0 );
			Assert.IsTrue( y.CompareTo( x ) > 0 );
		}

		#region Parse test cases 

		private bool ParseToStringValue(SByte s) 
		{
			return s == NullableSByte.Parse(s.ToString()).Value;
		}


		[Test]
		public void BasicParseTest()
		{
			Assert.IsFalse(NullableSByte.Parse(null).HasValue);
			Assert.IsFalse(NullableSByte.Parse("").HasValue);
			Assert.IsFalse(NullableSByte.Parse("    ").HasValue);
			Assert.IsTrue(ParseToStringValue(0));
			Assert.IsTrue(ParseToStringValue(SByte.MinValue));
			Assert.IsTrue(ParseToStringValue(SByte.MaxValue));
		}

		[Test, ExpectedException(typeof(FormatException))]
		public void InvalidParseValueTest()
		{
			NullableSByte.Parse("invalidvalue");
		}
		
		#endregion
	}
}