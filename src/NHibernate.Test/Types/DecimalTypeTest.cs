using System;
using System.Data;

using DotNetMock.Framework.Data;

using NHibernate.Type;

using NUnit.Framework;


namespace NHibernate.Test.Types 
{
	/// <summary>
	/// The Unit Test for the DecimalType
	/// </summary>
	[TestFixture]
	public class DecimalType 
	{
		public DecimalType() 
		{
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
		public void GetDiffPrecision() 
		{
			NullableType decimalType = TypeFactory.GetDecimalType(19, 5);
			IDataReader reader = GetReader();

			decimal expected = 5.643510M;
			
			// move to the first record
			reader.Read();

			object actualValue = decimalType.Get(reader, 1);
			Assertion.Assert("Expected double equals Actual", expected.Equals(actualValue));
			Assertion.Assert("Actual double equals Expected", actualValue.Equals(expected));
			
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
			NullableType decimalType = TypeFactory.GetDecimalType(19, 5);
			IDataReader reader = GetReader();

			decimal expected = 5.64351M;
			
			// move to the first record
			reader.Read();

			object actualValue = decimalType.Get(reader, 1);
			Assertion.Assert("Expected double equals Actual", expected.Equals(actualValue));
			Assertion.Assert("Actual double equals Expected", actualValue.Equals(expected));
		}

		private IDataReader GetReader() 
		{
			MockDataReader mockReader = new MockDataReader();
			
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add(new DataColumn("ID", typeof(int)));
			dataTable.Columns.Add(new DataColumn("DecimalValue", typeof(decimal)));
			mockReader.SetSchemaTable(dataTable);

			object[,] rowValues = new object[2,2];
			rowValues[0,0] = 0;
			rowValues[0,1] = 5.64351M;
			rowValues[1,0] = 1;
			rowValues[1,1] = 5.6435101M;
			
			mockReader.SetRows(rowValues);

			return mockReader;

		}
	}
}
