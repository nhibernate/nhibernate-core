using System;
using Nullables;
using NUnit.Framework;

namespace Nullables.Tests
{
	/// <summary>
	/// Summary description for NullableCharFixture.
	/// </summary>
	[TestFixture]
	public class NullableCharFixture
	{
		[Test]
		public void BasicTestChar()
		{
			NullableChar v1 = 'd'; //chould take a char literal

			Assert.IsTrue( v1.HasValue ); //should have a value;
			Assert.IsTrue( v1.Equals( 'd' ) ); //implicit casting should make this result in true.
			Assert.IsTrue( v1.Value == 'd' );
			Assert.IsFalse( v1.Equals( NullableChar.Default ) );
			Assert.IsTrue( v1.Equals( new NullableChar( 'd' ) ) ); //should == a new instance with the same inner value.

			//same thing, but with == instead of .Equals()
			Assert.IsTrue( v1 == 'd' );
			Assert.IsFalse( v1 == 'g' );
			Assert.IsFalse( v1 == NullableChar.Default );
			Assert.IsTrue( v1 == new NullableChar( 'd' ) );

			//now null v1.
			v1 = DBNull.Value;
			Assert.IsTrue( v1 == NullableChar.Default );
			v1 = NullableChar.Default;
			Assert.IsTrue( v1 == NullableChar.Default );

			NullableChar v2 = NullableChar.Default; //should start as "null"

			Assert.IsFalse( v2.HasValue );
			Assert.IsFalse( v2.Equals( '2' ) );
			Assert.IsTrue( v2.Equals( NullableChar.Default ) );
			Assert.IsTrue( v2.Equals( DBNull.Value ) );
		}

		[Test, ExpectedException(typeof(InvalidOperationException))]
		public void CharMissingValueTest()
		{
			NullableChar x = NullableChar.Default;

			Char y = x.Value;
		}

		[Test]
		public void CharIComparableTest()
		{
			NullableChar x;
			NullableChar y;

			//one null, one not
			x = NullableChar.Default;
			y = new NullableChar( 'y' );
			Assert.IsTrue( x.CompareTo( y ) < 0 );
			Assert.IsTrue( y.CompareTo( x ) > 0 );

			//now both null
			x = NullableChar.Default;
			y = NullableChar.Default;
			Assert.IsTrue( x.CompareTo( y ) == 0 );
			Assert.IsTrue( y.CompareTo( x ) == 0 );

			//now both with a value
			x = new NullableChar( 'a' );
			y = new NullableChar( 'u' );
			Assert.IsTrue( x.CompareTo( y ) < 0 );
			Assert.IsTrue( y.CompareTo( x ) > 0 );
		}

		#region Parse test cases 


		private bool ParseToStringValue(char c) 
		{
			return c == NullableChar.Parse(c.ToString()).Value;
		}

		[Test]
		public void BasicParseTest()
		{
			Assert.IsFalse(NullableChar.Parse(null).HasValue);
			Assert.IsFalse(NullableChar.Parse("").HasValue);
			Assert.IsFalse(NullableChar.Parse("    ").HasValue);
			Assert.IsTrue(ParseToStringValue('A'));
			Assert.IsTrue(ParseToStringValue('Ç'));
		}

		[Test, ExpectedException(typeof(FormatException))]
		public void InvalidParseValueTest()
		{
			NullableChar.Parse("invalidvalue");
		}
		
		#endregion
	}
}