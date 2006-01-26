using System;
using Nullables;
using NUnit.Framework;

namespace Nullables.Tests
{
	/// <summary>
	/// Summary description for NullableBooleanFixture.
	/// </summary>
	[TestFixture]
	public class NullableBooleanFixture
	{
		[Test]
		public void BooleanIComparableTest()
		{
			NullableBoolean x;
			NullableBoolean y;

			//one null, one not
			x = NullableBoolean.Default;
			y = new NullableBoolean( true );
			Assert.IsTrue( x.CompareTo( y ) < 0 );
			Assert.IsTrue( y.CompareTo( x ) > 0 );

			//now both null
			x = NullableBoolean.Default;
			y = NullableBoolean.Default;
			Assert.IsTrue( x.CompareTo( y ) == 0 );
			Assert.IsTrue( y.CompareTo( x ) == 0 );

			//now both with a value
			x = new NullableBoolean( false );
			y = new NullableBoolean( true );
			Assert.IsTrue( x.CompareTo( y ) < 0 );
			Assert.IsTrue( y.CompareTo( x ) > 0 );
		}

		[Test, ExpectedException(typeof(InvalidOperationException))]
		public void BooleanMissingValueTest()
		{
			NullableBoolean x = NullableBoolean.Default;

			bool y = x.Value;
		}

		#region Parse test cases 

		[Test]
		public void BasicParseTest()
		{
			Assert.IsFalse(NullableBoolean.Parse(null).HasValue);
			Assert.IsFalse(NullableBoolean.Parse("").HasValue);
			Assert.IsFalse(NullableBoolean.Parse("    ").HasValue);
			Assert.IsTrue(NullableBoolean.Parse(Boolean.TrueString).Value);
			Assert.IsFalse(NullableBoolean.Parse(Boolean.FalseString).Value);
		}

		[Test, ExpectedException(typeof(FormatException))]
		public void InvalidParseValueTest()
		{
			NullableBoolean.Parse("invalidvalue");
		}
		
		#endregion
	}
}