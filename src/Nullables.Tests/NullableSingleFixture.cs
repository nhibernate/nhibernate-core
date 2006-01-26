using System;
using Nullables;
using NUnit.Framework;

namespace Nullables.Tests
{
	/// <summary>
	/// Summary description for NullableSingleFixture.
	/// </summary>
	[TestFixture]
	public class NullableSingleFixture
	{
		[Test]
		public void BasicTestSingle()
		{
			NullableSingle v1 = 32; //should take an int literal

			Assert.IsTrue( v1.HasValue ); //should have a value;
			Assert.IsTrue( v1.Equals( 32 ) ); //implicit casting should make this result in true.
			Assert.IsTrue( v1.Value == 32 );
			Assert.IsFalse( v1.Equals( NullableSingle.Default ) );
			Assert.IsTrue( v1.Equals( new NullableSingle( 32 ) ) ); //should == a new instance with the same inner value.

			//same thing, but with == instead of .Equals()
			Assert.IsTrue( v1 == 32 );
			Assert.IsFalse( v1 == 33 );
			Assert.IsFalse( v1 == NullableSingle.Default );
			Assert.IsTrue( v1 == new NullableSingle( 32 ) );

			//now null v1.
			v1 = DBNull.Value;
			Assert.IsTrue( v1 == NullableSingle.Default );
			v1 = NullableSingle.Default;
			Assert.IsTrue( v1 == NullableSingle.Default );

			NullableSingle v2 = NullableSingle.Default; //should start as "null"

			Assert.IsFalse( v2.HasValue );
			Assert.IsFalse( v2.Equals( 12 ) );
			Assert.IsTrue( v2.Equals( NullableSingle.Default ) );
			Assert.IsTrue( v2.Equals( DBNull.Value ) );
		}

		[Test, ExpectedException(typeof(InvalidOperationException))]
		public void SingleMissingValueTest()
		{
			NullableSingle x = NullableSingle.Default;

			float y = x.Value;
		}

		[Test]
		public void SingleIComparableTest()
		{
			NullableSingle x;
			NullableSingle y;

			//one null, one not
			x = NullableSingle.Default;
			y = new NullableSingle( 1.6f );
			Assert.IsTrue( x.CompareTo( y ) < 0 );
			Assert.IsTrue( y.CompareTo( x ) > 0 );

			//now both null
			x = NullableSingle.Default;
			y = NullableSingle.Default;
			Assert.IsTrue( x.CompareTo( y ) == 0 );
			Assert.IsTrue( y.CompareTo( x ) == 0 );

			//now both with a value
			x = new NullableSingle( 5.1f );
			y = new NullableSingle( 8.9f );
			Assert.IsTrue( x.CompareTo( y ) < 0 );
			Assert.IsTrue( y.CompareTo( x ) > 0 );
		}

		#region Parse test cases 

		private bool ParseToStringValue(Single s) 
		{
			//The Sginle type gives no roundtrip guarantees when using Parse after ToString
			//unless "R" is specified in the format - roundtrip gains precedence over precision 
			//Check http://msdn.microsoft.com/netframework/programming/bcl/faq/NumericTypesFAQ.aspx
			return s == NullableSingle.Parse(s.ToString("R")).Value;
		}


		[Test]
		public void BasicParseTest()
		{
			Assert.IsFalse(NullableSingle.Parse(null).HasValue);
			Assert.IsFalse(NullableSingle.Parse("").HasValue);
			Assert.IsFalse(NullableSingle.Parse("    ").HasValue);
			Assert.IsTrue(ParseToStringValue(0));

			Assert.IsTrue(ParseToStringValue(Single.MinValue));
			Assert.IsTrue(ParseToStringValue(Single.MaxValue));
		}

		[Test, ExpectedException(typeof(FormatException))]
		public void InvalidParseValueTest()
		{
			NullableSingle.Parse("invalidvalue");
		}
		
		#endregion
	}
}