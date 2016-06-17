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
            public TestableColumnMetaData(DataRow rs, object columnSizeValue, object numericalPrecisionValue, object numericalScaleValue)
                : base(rs)
            {
                SetColumnSize(columnSizeValue);
                SetNumericalPrecision(numericalPrecisionValue);
                SetNumericalScale(numericalScaleValue);
            }
        }

        [TestCase(null, null, null, 0, 0, 0)]  // No size and no precision.
        [TestCase(null, "7", "5", 0, 7, 5)]   // No size, but with numerical precision.
        [TestCase("13", null, null, 13, 0, 0)] // Size, but no precision.
        [TestCase("5000000000", null, null, int.MaxValue, 0, 0)]  // Oversize column should be bounded to int.MaxValue.
        [TestCase("13", "7", "5", 13, 7, 5)]  // Can handle both size and precision together.
        public void SetColumnSizeAndNumericalPrecision(
            object columnSizeInput,
            object precisionInput,
            object scaleInput,
            int expectedColumnSize,
            int expectedPrecision,
            int expectedScale)
        {
            // The *Input above is supposed to be either null or strings.

            if (columnSizeInput == null)
                columnSizeInput = DBNull.Value;
            if (precisionInput == null)
                precisionInput = DBNull.Value;

            var column = new TestableColumnMetaData(null, columnSizeInput, precisionInput, scaleInput);

            Assert.AreEqual(expectedColumnSize, column.ColumnSize);
            Assert.AreEqual(expectedPrecision, column.NumericalPrecision);
            Assert.AreEqual(expectedPrecision, column.NumericalScale);
        }
    }
}
