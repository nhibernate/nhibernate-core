using System;
using Nullables;
using NUnit.Framework;

namespace Nullables.Tests
{
	/// <summary>
	/// Summary description for NullableDoubleFixture.
	/// </summary>
	[TestFixture]
	public class NullableDoubleFixture
	{
		[Test]
		public void BasicTestDouble()
		{
			NullableDouble v1 = 32; //should take an int literal

			Assert.IsTrue( v1.HasValue ); //should have a value;
			Assert.IsTrue( v1.Equals( 32 ) ); //implicit casting should make this result in true.
			Assert.IsTrue( v1.Value == 32 );
			Assert.IsFalse( v1.Equals( NullableDouble.Default ) );
			Assert.IsTrue( v1.Equals( new NullableDouble( 32 ) ) ); //should == a new instance with the same inner value.

			//same thing, but with == instead of .Equals()
			Assert.IsTrue( v1 == 32 );
			Assert.IsFalse( v1 == 33 );
			Assert.IsFalse( v1 == NullableDouble.Default );
			Assert.IsTrue( v1 == new NullableDouble( 32 ) );

			//now null v1.
			v1 = DBNull.Value;
			Assert.IsTrue( v1 == NullableDouble.Default );
			v1 = NullableDouble.Default;
			Assert.IsTrue( v1 == NullableDouble.Default );

			NullableDouble v2 = NullableDouble.Default; //should start as "null"

			Assert.IsFalse( v2.HasValue );
			Assert.IsFalse( v2.Equals( 12 ) );
			Assert.IsTrue( v2.Equals( NullableDouble.Default ) );
			Assert.IsTrue( v2.Equals( DBNull.Value ) );
		}

		[Test, ExpectedException(typeof(InvalidOperationException))]
		public void DoubleMissingValueTest()
		{
			NullableDouble x = NullableDouble.Default;

			double y = x.Value;
		}

		[Test]
		public void DoubleIComparableTest()
		{
			NullableDouble x;
			NullableDouble y;

			//one null, one not
			x = NullableDouble.Default;
			y = new NullableDouble( 0.5 );
			Assert.IsTrue( x.CompareTo( y ) < 0 );
			Assert.IsTrue( y.CompareTo( x ) > 0 );

			//now both null
			x = NullableDouble.Default;
			y = NullableDouble.Default;
			Assert.IsTrue( x.CompareTo( y ) == 0 );
			Assert.IsTrue( y.CompareTo( x ) == 0 );

			//now both with a value
			x = new NullableDouble( 0.6 );
			y = new NullableDouble( 0.67 );
			Assert.IsTrue( x.CompareTo( y ) < 0 );
			Assert.IsTrue( y.CompareTo( x ) > 0 );
		}
		#region Parse test cases 

		private bool ParseToStringValue(Double d) 
		{
			//The Double type gives no roundtrip guarantees when using Parse after ToString
			//unless "R" is specified in the format - roundtrip gains precedence over precision 
			//Check http://msdn.microsoft.com/netframework/programming/bcl/faq/NumericTypesFAQ.aspx
			return d == NullableDouble.Parse(d.ToString("R")).Value;
		}


		[Test]
		public void BasicParseTest()
		{
			Assert.IsFalse(NullableDouble.Parse(null).HasValue);
			Assert.IsFalse(NullableDouble.Parse("").HasValue);
			Assert.IsFalse(NullableDouble.Parse("    ").HasValue);
			Assert.IsTrue(ParseToStringValue(0));
			Assert.IsTrue(ParseToStringValue(Double.MinValue));
			Assert.IsTrue(ParseToStringValue(Double.MaxValue));
		}

		[Test, ExpectedException(typeof(FormatException))]
		public void InvalidParseValueTest()
		{
			NullableDouble.Parse("invalidvalue");
		}
		
		#endregion
	}
}