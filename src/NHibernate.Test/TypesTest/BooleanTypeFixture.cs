using System;
using System.Data;

using NHibernate.Type;
using NUnit.Framework;

namespace NHibernate.Test.TypesTest
{
	/// <summary>
	/// The Unit Tests for the BooleanType.
	/// </summary>
	[TestFixture]
	public class BooleanTypeFixture : BaseTypeFixture
	{
		public BooleanTypeFixture()
		{
		}

		/// <summary>
		/// Test that Get(IDataReader, index) returns a boxed Boolean value that is what
		/// we expect.
		/// </summary>
		[Test]
		public void Get() 
		{
			NullableType type = NHibernate.Boolean;

			bool expected = true;
			
			// move to the first record
			reader.Read();

			bool actual = (bool)type.Get(reader, BooleanTypeColumnIndex);
			Assert.AreEqual(expected, actual);

		}
	}
}
