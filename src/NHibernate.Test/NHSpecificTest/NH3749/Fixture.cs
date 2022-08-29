using NHibernate.Mapping;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3749
{
	[TestFixture]
	public class Fixture
	{
		[Test]
		public void SupportsNotNullUniqueFalse()
		{
			Table tbl = new Table();
			tbl.Name = "Table";

			Column column = new Column("Column");
			column.IsNullable = true;

			UniqueKey uk = new UniqueKey { Name = "UniqueKey", Table = tbl };
			uk.Columns.Add(column);

			tbl.AddUniqueKey(uk);

			Dialect.Dialect dialect = new TestDialect();

			string sql = tbl.SqlCreateString(dialect, null, "", "");

			Assert.IsFalse(sql.Contains(",)"), "Create command has unnecessary comma: " + sql);
		}
	}
}
