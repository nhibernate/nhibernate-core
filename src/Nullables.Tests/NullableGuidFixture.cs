using System;

using NUnit.Framework;

namespace Nullables.Tests
{
	/// <summary>
	/// Summary description for NullableGuidFixture.
	/// </summary>
	[TestFixture]
	public class NullableGuidFixture
	{
		[Test]
		public void BasicTestGuid()
		{
			NullableGuid v1 = new Guid("00000000-0000-0000-0000-000000000005"); //should take an int literal

			Assert.IsTrue(v1.HasValue); //should have a value;
			Assert.IsTrue(v1.Equals(new Guid("00000000-0000-0000-0000-000000000005")));
				//implicit casting should make this result in true.
			Assert.IsTrue(v1.Value == new Guid("00000000-0000-0000-0000-000000000005"));
			Assert.IsFalse(v1.Equals(NullableGuid.Default));
			Assert.IsTrue(v1.Equals(new NullableGuid(new Guid("00000000-0000-0000-0000-000000000005"))));
				//should == a new instance with the same inner value.

			//same thing, but with == instead of .Equals()
			Assert.IsTrue(v1 == new Guid("00000000-0000-0000-0000-000000000005"));
			Assert.IsFalse(v1 == new Guid("00000000-0000-0000-0000-000000000008"));
			Assert.IsFalse(v1 == NullableGuid.Default);
			Assert.IsTrue(v1 == new NullableGuid(new Guid("00000000-0000-0000-0000-000000000005")));

			//now null v1.
			v1 = DBNull.Value;
			Assert.IsTrue(v1 == NullableGuid.Default);
			v1 = NullableGuid.Default;
			Assert.IsTrue(v1 == NullableGuid.Default);

			NullableGuid v2 = NullableGuid.Default; //should start as "null"

			Assert.IsFalse(v2.HasValue);
			Assert.IsFalse(v2.Equals(new Guid("00000000-0000-0000-0000-000000000002")));
			Assert.IsTrue(v2.Equals(NullableGuid.Default));
			Assert.IsTrue(v2.Equals(DBNull.Value));
		}

		[Test]
		public void StringCtorTest()
		{
			Assert.IsFalse(new NullableGuid(null).HasValue, "null in ctor should have no value");
			Assert.IsFalse(new NullableGuid("").HasValue, "empty string in ctor should have no value");
			Assert.IsFalse(new NullableGuid("    ").HasValue, "empty trimmed string in ctor should have no value");

			string g = "{C6CD8CE7-C36F-40d0-851A-D8247368BF6A}";
			Guid guid = new Guid(g);
			NullableGuid nullGuid = new NullableGuid(g);

			Assert.IsTrue(nullGuid.HasValue);
			Assert.AreEqual(guid, nullGuid.Value, "A System.GUID and NullableGuid created with same string, should be equal");
		}

		[Test, ExpectedException(typeof(FormatException))]
		public void InvalidStringCtroTest()
		{
			new NullableGuid("invalidvalue");
		}

		[Test, ExpectedException(typeof(InvalidOperationException))]
		public void GuidMissingValueTest()
		{
			NullableGuid x = NullableGuid.Default;

			Guid y = x.Value;
		}

		[Test]
		public void GuidIComparableTest()
		{
			NullableGuid x;
			NullableGuid y;

			//one null, one not
			x = NullableGuid.Default;
			y = new NullableGuid(new Guid("12345678-1234-1234-1234-123456789012"));
			Assert.IsTrue(x.CompareTo(y) < 0);
			Assert.IsTrue(y.CompareTo(x) > 0);

			//now both null
			x = NullableGuid.Default;
			y = NullableGuid.Default;
			Assert.IsTrue(x.CompareTo(y) == 0);
			Assert.IsTrue(y.CompareTo(x) == 0);

			//now both with a value
			x = new NullableGuid(new Guid("12345678-1234-1234-1234-123456789001"));
			y = new NullableGuid(new Guid("12345678-1234-1234-1234-123456789099"));
			Assert.IsTrue(x.CompareTo(y) < 0);
			Assert.IsTrue(y.CompareTo(x) > 0);
		}
	}
}