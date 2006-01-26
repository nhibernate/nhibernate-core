using System;
using Nullables;
using NUnit.Framework;

namespace Nullables.Tests
{
	/// <summary>
	/// Summary description for NullableDateTimeFixture.
	/// </summary>
	[TestFixture]
	public class NullableDateTimeFixture
	{
		[Test]
		public void BasicTestDateTime()
		{
			NullableDateTime v1 = new DateTime( 1979, 11, 8 );

			Assert.IsTrue( v1.HasValue ); //should have a value;
			Assert.IsTrue( v1.Equals( new DateTime( 1979, 11, 8 ) ) ); //implicit casting should make this result in true.
			Assert.IsTrue( v1.Value == new DateTime( 1979, 11, 8 ) );
			Assert.IsFalse( v1.Equals( NullableDateTime.Default ) );
			Assert.IsTrue( v1.Equals( new NullableDateTime( new DateTime( 1979, 11, 8 ) ) ) ); //should == a new instance with the same inner value.

			//same thing, but with == instead of .Equals()
			Assert.IsTrue( v1 == new DateTime( 1979, 11, 8 ) );
			Assert.IsFalse( v1 == new DateTime( 1980, 10, 9 ) );
			Assert.IsFalse( v1 == NullableDateTime.Default );
			Assert.IsTrue( v1 == new NullableDateTime( new DateTime( 1979, 11, 8 ) ) );

			//now null v1.
			v1 = DBNull.Value;
			Assert.IsTrue( v1 == NullableDateTime.Default );
			v1 = NullableDateTime.Default;
			Assert.IsTrue( v1 == NullableDateTime.Default );

			NullableDateTime v2 = NullableDateTime.Default; //should start as "null"

			Assert.IsFalse( v2.HasValue );
			Assert.IsFalse( v2.Equals( new DateTime( 2004, 12, 25 ) ) );
			Assert.IsTrue( v2.Equals( NullableDateTime.Default ) );
			Assert.IsTrue( v2.Equals( DBNull.Value ) );
		}

		[Test, ExpectedException(typeof(InvalidOperationException))]
		public void DateTimeMissingValueTest()
		{
			NullableDateTime x = NullableDateTime.Default;

			DateTime y = x.Value;
		}

		[Test]
		public void DateTimeIComparableTest()
		{
			NullableDateTime x;
			NullableDateTime y;

			//one null, one not
			x = NullableDateTime.Default;
			y = new NullableDateTime( new DateTime( 2000, 12, 25 ) );
			Assert.IsTrue( x.CompareTo( y ) < 0 );
			Assert.IsTrue( y.CompareTo( x ) > 0 );

			//now both null
			x = NullableDateTime.Default;
			y = NullableDateTime.Default;
			Assert.IsTrue( x.CompareTo( y ) == 0 );
			Assert.IsTrue( y.CompareTo( x ) == 0 );

			//now both with a value
			x = new NullableDateTime( new DateTime( 2000, 11, 1 ) );
			y = new NullableDateTime( new DateTime( 2000, 12, 25 ) );
			Assert.IsTrue( x.CompareTo( y ) < 0 );
			Assert.IsTrue( y.CompareTo( x ) > 0 );
		}

		#region Parse test cases 

		private bool ParseToStringValue(DateTime d) 
		{
			//http://msdn.microsoft.com/netframework/programming/bcl/faq/DateAndTimeFAQ.aspx#Question10
			return d == NullableDateTime.Parse(d.ToString("G")).Value;
		}

		[Test]
		public void BasicParseTest()
		{
			Assert.IsFalse( NullableDateTime.Parse(null).HasValue, "null in Parse should have no value" );
			Assert.IsFalse(NullableDateTime.Parse("").HasValue, "empty string in Parse should have no value" );
			Assert.IsFalse(NullableDateTime.Parse("    ").HasValue, "empty trimmed string in Parse should have no value" );
			Assert.IsTrue( ParseToStringValue(DateTime.MinValue), "DateTime.MinValue could not be round-tripped" );
			
			DateTime dateTime = new DateTime( 2005, 01, 01, 15, 12, 00 );
			NullableDateTime nullableDateTime = NullableDateTime.Parse( dateTime.ToString( "G" ) );

			Assert.IsTrue( nullableDateTime.HasValue );
			Assert.AreEqual( dateTime, nullableDateTime.Value, "A specific DateTime.ToString('G') could not be round-tripped" );
		}

		[Test, ExpectedException(typeof(FormatException))]
		public void InvalidParseValueTest()
		{
			NullableDateTime.Parse("invalidvalue");
		}
		
		#endregion
	}
}