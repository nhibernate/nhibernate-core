using NHibernate.Dialect;
using NHibernate.Mapping;
using NUnit.Framework;

namespace NHibernate.Test.MappingTest
{
	/// <summary>
	/// Summary description for TableFixture.
	/// </summary>
	[TestFixture]
	public class TableFixture
	{
		[Test]
		public void TableNameQuoted()
		{
			Table tbl = new Table();
			tbl.Name = "`keyword`";

			Dialect.Dialect dialect = new MsSql2000Dialect();

			Assert.AreEqual("[keyword]", tbl.GetQuotedName(dialect));

			Assert.AreEqual("dbo.[keyword]", tbl.GetQualifiedName(dialect, null, "dbo"));

			Assert.AreEqual("[keyword]", tbl.GetQualifiedName(dialect, null, null));

			tbl.Schema = "sch";

			Assert.AreEqual("sch.[keyword]", tbl.GetQualifiedName(dialect));
		}

		[Test]
		public void TableNameNotQuoted()
		{
			Table tbl = new Table();
			tbl.Name = "notkeyword";

			Dialect.Dialect dialect = new MsSql2000Dialect();

			Assert.AreEqual("notkeyword", tbl.GetQuotedName(dialect));

			Assert.AreEqual("dbo.notkeyword", tbl.GetQualifiedName(dialect, null, "dbo"));

			Assert.AreEqual("notkeyword", tbl.GetQualifiedName(dialect, null, null));

			tbl.Schema = "sch";

			Assert.AreEqual("sch.notkeyword", tbl.GetQualifiedName(dialect));
		}

		[Test]
		public void SchemaNameQuoted()
		{
			Table tbl = new Table();
			tbl.Schema = "`schema`";
			tbl.Name = "name";

			Dialect.Dialect dialect = new MsSql2000Dialect();

			Assert.AreEqual("[schema].name", tbl.GetQualifiedName(dialect));
		}

		[Test]
		public void NameIsStable()
		{
			var tbl = new Table { Name = "name" };
			Assert.That(
				Constraint.GenerateName("FK_", tbl, null, new[] {new Column("col1"), new Column("col2")}),
				Is.EqualTo("FK_3B355A0C"));
		}
	}
}
