using System;
using System.Data;

using NHibernate.Type;
using NUnit.Framework;

namespace NHibernate.Test.TypesTest
{
	/// <summary>
	/// The Unit Tests for the GuidType.
	/// </summary>
	[TestFixture]
	public class GuidTypeFixture : BaseTypeFixture
	{
		public GuidTypeFixture()
		{
		}

		/// <summary>
		/// Test that Get(IDataReader, index) returns a boxed Guid value that is what
		/// we expect.
		/// </summary>
		[Test]
		public void Get() 
		{
			NullableType type = NHibernate.Guid;

			Guid expected = new Guid("{01234567-abcd-abcd-abcd-0123456789ab}");
			
			// move to the first record
			reader.Read();

			Guid actual = (Guid)type.Get(reader, GuidTypeColumnIndex);
			Assert.AreEqual(expected, actual);

		}

		[Test]
		public void EqualsTrue() 
		{
			Guid lhs = new Guid("{01234567-abcd-abcd-abcd-0123456789ab}");
			Guid rhs = new Guid("{01234567-abcd-abcd-abcd-0123456789ab}");

			NullableType type = NHibernate.Guid;
			Assert.IsTrue(type.Equals(lhs, rhs));
		}

		[Test]
		public void EqualsFalse() 
		{
			Guid lhs = new Guid("{01234567-abcd-abcd-abcd-0123456789ab}");
			Guid rhs = new Guid("{11234567-abcd-abcd-abcd-0123456789ab}");

			NullableType type = NHibernate.Guid;
			Assert.IsFalse(type.Equals(lhs, rhs));
		}

		/// <summary>
		/// Test to make sure that a boxed Guid and a struct Guid compare the 
		/// same as two struct Guids. 
		/// </summary>
		[Test]
		public void EqualsWithSnapshot() 
		{
			
			Guid busObjValue = new Guid("{01234567-abcd-abcd-abcd-0123456789ab}");
			object snapshotValue = busObjValue;

			NullableType type = NHibernate.Guid;
			Assert.IsTrue(type.Equals(busObjValue, snapshotValue));

			// simulate the UI changing the busObjValue
			busObjValue = new Guid("{11234567-abcd-abcd-abcd-0123456789ab}");
			Assert.IsFalse(type.Equals(busObjValue, snapshotValue));

		}

	}
}