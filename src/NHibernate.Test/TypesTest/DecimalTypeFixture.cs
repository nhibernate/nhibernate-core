using System;
using System.Data;

using NHibernate.Type;
using NUnit.Framework;

namespace NHibernate.Test.TypesTest 
{
	/// <summary>
	/// The Unit Tests for the DecimalType
	/// </summary>
	[TestFixture]
	public class DecimalTypeFixture : BaseTypeFixture
	{

		/// <summary>
		/// Test that two decimal fields that are exactly equal are returned
		/// as Equal by the DecimalType.
		/// </summary>
		[Test]
		public void Equals() 
		{
			decimal lhs = 5.64351M;
			decimal rhs = 5.64351M;

			NullableType type = NHibernate.Decimal;
			Assert.IsTrue(type.Equals(lhs, rhs));
		}

		/// <summary>
		/// Test that two decimal fields that are equal except one has a higher precision than
		/// the other one are returned as Equal by the DecimalType.
		/// </summary>
		[Test]
		public void EqualsWithDiffPrecision() 
		{
			decimal lhs = 5.64351M;
			decimal rhs = 5.643510M;

			NullableType type = NHibernate.Decimal;
			Assert.IsTrue(type.Equals(lhs, rhs));
		}

		/// <summary>
		/// Test that Get(IDataReader, index) returns a boxed Decimal value that is what
		/// we expect.
		/// </summary>
		/// <remarks>
		/// A Decimal variable holding <c>5.64531M</c> should be equal to a 
		/// <c>5.46531M</c> read from a IDataReader.
		/// </remarks>
		[Test]
		public void Get() 
		{
			NullableType type = TypeFactory.GetDecimalType(19, 5);

			decimal expected = 5.64351M;
			
			// move to the first record
			reader.Read();

			decimal actualValue = (decimal)type.Get(reader, DecimalTypeColumnIndex);
			Assert.AreEqual(expected, actualValue);

		}
		
		/// <summary>
		/// Test that a System.Decimal created with an extra <c>0</c> will still be equal
		/// to the System.Decimal without the last <c>0</c>
		/// </summary>
		/// <remarks>
		/// A decimal variable initialized to <c>5.643510M</c> should be 
		/// equal to a <c>5.64351M</c> read from a IDataReader.
		/// </remarks>
		[Test]
		public void GetWithDiffPrecision() 
		{
			NullableType type = TypeFactory.GetDecimalType(19, 5);

			decimal expected = 5.643510M;
			
			// move to the first record
			reader.Read();

			decimal actualValue = (decimal)type.Get(reader, DecimalTypeColumnIndex);
			Assert.AreEqual(expected, actualValue, "Expected double equals Actual");
			
			
		}


	}
}
