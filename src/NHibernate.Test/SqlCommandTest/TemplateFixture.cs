using NHibernate.Dialect;
using NHibernate.Dialect.Function;
using NHibernate.SqlCommand;

using NUnit.Framework;

namespace NHibernate.Test.SqlCommandTest
{
	[TestFixture]
	public class TemplateFixture
	{
		private Dialect.Dialect dialect;
		private SQLFunctionRegistry functionRegistry;

		[SetUp]
		public void SetUp()
		{
			dialect = new MsSql2000Dialect();
			functionRegistry = new SQLFunctionRegistry(dialect, new HashtableDictionary<string, ISQLFunction>());
		}

		/// <summary>
		/// Tests that a column enclosed by <c>`</c> is enclosed by the Dialect.OpenQuote 
		/// and Dialect.CloseQuote after the Template Renders the Where String.
		/// </summary>
		[Test]
		public void ReplaceWithDialectQuote()
		{
			string whereFragment = "column_name = 'string value' and `backtick` = 1";
			string expectedFragment = "$PlaceHolder$.column_name = 'string value' and $PlaceHolder$.[backtick] = 1";
			Assert.AreEqual(expectedFragment, Template.RenderWhereStringTemplate(whereFragment, dialect, functionRegistry));
		}

		[Test]
		public void OrderBySingleColumn()
		{
			string orderBy = "col1 asc";
			string expectedOrderBy = "$PlaceHolder$.col1 asc";

			Assert.AreEqual(expectedOrderBy, Template.RenderOrderByStringTemplate(orderBy, dialect, functionRegistry));
		}

		[Test]
		public void OrderByMultiColumn()
		{
			Dialect.Dialect dialect = new MsSql2000Dialect();

			string orderBy = "col1 asc, col2 desc, col3";
			string expectedOrderBy = "$PlaceHolder$.col1 asc, $PlaceHolder$.col2 desc, $PlaceHolder$.col3";

			Assert.AreEqual(expectedOrderBy, Template.RenderOrderByStringTemplate(orderBy, dialect, functionRegistry));
		}
	}
}