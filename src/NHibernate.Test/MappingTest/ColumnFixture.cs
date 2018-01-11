using System;
using NHibernate.Dialect;
using NHibernate.Mapping;
using NUnit.Framework;

namespace NHibernate.Test.MappingTest
{
	[TestFixture]
	public class ColumnFixture
	{
		private Dialect.Dialect _dialect;

		[SetUp]
		public void SetUp()
		{
			_dialect = new MsSql2000Dialect();
		}

		[Test]
		public void YesNoSqlType()
		{
			SimpleValue sv = new SimpleValue();
			sv.TypeName = NHibernateUtil.YesNo.Name;
			Column column = new Column();
			column.Value = sv;
			string type = column.GetSqlType(_dialect, null);
			Assert.AreEqual("CHAR(1)", type);
		}

		[Test]
		public void StringSqlType()
		{
			SimpleValue sv = new SimpleValue();
			sv.TypeName = NHibernateUtil.String.Name;
			Column column = new Column();
			column.Value = sv;
			Assert.AreEqual("NVARCHAR(255)", column.GetSqlType(_dialect, null));

			column.Length = 100;
			Assert.AreEqual("NVARCHAR(100)", column.GetSqlType(_dialect, null));
		}

		// Should be kept synchronized with Column same constant.
		private const int _charactersLeftCount = 4;

		[TestCase("xxxxyyyyz")]
		[TestCase("xxxxyyyyzz")]
		[TestCase("xxxxyyyyzzz")]
		[TestCase("xxxxyyy4z", Description = "Non-letter digit character would be cut, make sure we don't skip length check.")]
		[TestCase("xxxxyyyz4z", Description = "Non-letter digit character would be cut, make sure we don't skip length check.")]
		[TestCase("xxxxyyyzz4z", Description = "Non-letter digit character would be cut, make sure we don't skip length check.")]
		[TestCase("xxxxyyyy4", Description = "Non-letter digit character would be cut, make sure we don't skip length check.")]
		[TestCase("xxxxyyyyz4", Description = "Non-letter digit character would be cut, make sure we don't skip length check.")]
		[TestCase("xxxxyyyyzz4", Description = "Non-letter digit character would be cut, make sure we don't skip length check.")]
		public void GetAliasRespectsMaxAliasLength(string columnName)
		{
			var dialect = new GenericDialect();

			// Test case is meant for a max length of 10, adjusts name if it is more.
			columnName = AdjustColumnNameToMaxLength(columnName, dialect, 10);

			var column = new Column(columnName);
			string generatedAlias = column.GetAlias(dialect);

			Assert.That(generatedAlias, Has.Length.LessThanOrEqualTo(dialect.MaxAliasLength - _charactersLeftCount));
		}

		[TestCase("xxxxyyyyz")]
		[TestCase("xxxxyyyyzz")]
		[TestCase("xxxxyyyyzzz")]
		[TestCase("xxxxyyy4z")]
		[TestCase("xxxxyyyz4z")]
		[TestCase("xxxxyyyzz4z")]
		[TestCase("xxxxyyyy4")]
		[TestCase("xxxxyyyyz4")]
		[TestCase("xxxxyyyyzz4")]
		public void GetAliasWithTableSuffixRespectsMaxAliasLength(string columnName)
		{
			var dialect = new GenericDialect();

			// Test case is meant for a max length of 10, adjusts name if it is more.
			columnName = AdjustColumnNameToMaxLength(columnName, dialect, 10);

			var table = new Table() {UniqueInteger = 1};
			var column = new Column(columnName);

			string generatedAlias = column.GetAlias(dialect, table);

			Assert.That(generatedAlias, Has.Length.LessThanOrEqualTo(dialect.MaxAliasLength - _charactersLeftCount));
		}

		private static string AdjustColumnNameToMaxLength(string columnName, GenericDialect dialect, int referenceMaxLength)
		{
			if (dialect.MaxAliasLength > referenceMaxLength)
				columnName = new string('w', dialect.MaxAliasLength - referenceMaxLength) + columnName;
			else if (dialect.MaxAliasLength < referenceMaxLength)
				Assert.Fail("Dialect max alias length is too short for the test.");
			return columnName;
		}
	}
}
