using System;
using System.Data;
using NUnit.Framework;
using NHibernate.Dialect.Schema;

namespace NHibernate.Test.DialectTest.SchemaTests
{

	[TestFixture]
	public class ColumnMetaDataFixture
	{
		private class TestableColumnMetaData : AbstractColumnMetaData
		{
			public TestableColumnMetaData(DataRow rs, object columnSizeValue, object numericalPrecisionValue)
				: base(rs)
			{
				SetColumnSize(columnSizeValue);
				SetNumericalPrecision(numericalPrecisionValue);
			}
		}

		[Test]
		public void SetColumnSizeAndNumericalPrecision()
		{
			object nullValue = DBNull.Value;
			const int standardColumnSize = 13;
			object standardColumnSizeValue = standardColumnSize.ToString();
			object tooLargeColumnSizeValue = 0x1234567890.ToString();
			const int standardNumericalPrecision = 7;
			object standardNumericalPrecisionValue = standardNumericalPrecision.ToString();

			// Check a column that has no size and no precision
			var column1 = new TestableColumnMetaData(null, nullValue, nullValue);
			Assert.AreEqual(0, column1.ColumnSize);
			Assert.AreEqual(0, column1.NumericalPrecision);

			// Check a column that has a numerical precision
			var column2 = new TestableColumnMetaData(null, nullValue, standardNumericalPrecisionValue);
			Assert.AreEqual(0, column2.ColumnSize);
			Assert.AreEqual(standardNumericalPrecision, column2.NumericalPrecision);

			// Check a column that has a size that fits into an int and has no numerical precision
			var column3 = new TestableColumnMetaData(null, standardColumnSizeValue, nullValue);
			Assert.AreEqual(standardColumnSize, column3.ColumnSize);
			Assert.AreEqual(0, column3.NumericalPrecision);

			// Check a column that has a size that does not fits into an int (VARCHAR(MAX)) and has no numerical precision
			var column4 = new TestableColumnMetaData(null, tooLargeColumnSizeValue, nullValue);
			Assert.AreEqual(int.MaxValue, column4.ColumnSize);
			Assert.AreEqual(0, column4.NumericalPrecision);

			// Check a column that has a size that fits into an int and has a numerical precision
			var column5 = new TestableColumnMetaData(null, standardColumnSizeValue, standardNumericalPrecisionValue);
			Assert.AreEqual(standardColumnSize, column5.ColumnSize);
			Assert.AreEqual(standardNumericalPrecision, column5.NumericalPrecision);
		}
	}
}
