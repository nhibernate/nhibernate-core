using System.Linq;
using System.Threading;
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
		public void TablesUniquelyNamed()
		{
			Table tbl1 = new Table();
			Table tbl2 = new Table();

			Assert.AreEqual(tbl1.UniqueInteger + 1, tbl2.UniqueInteger);
		}

		[Test]
		public void TablesUniquelyNamedOnlyWithinThread()
		{
			var uniqueIntegerList = new System.Collections.Concurrent.ConcurrentBag<int>();
			var method = new ThreadStart(() =>
			                             {
				                             Table tbl1 = new Table();
				                             Table tbl2 = new Table();

				                             // Store these values for later comparison
				                             uniqueIntegerList.Add(tbl1.UniqueInteger);
				                             uniqueIntegerList.Add(tbl2.UniqueInteger);

				                             // Ensure that within a thread we have unique integers
				                             Assert.AreEqual(tbl1.UniqueInteger + 1, tbl2.UniqueInteger);
			                             });

			var thread1 = new CrossThreadTestRunner(method);
			var thread2 = new CrossThreadTestRunner(method);

			thread1.Start();
			thread2.Start();

			thread1.Join();
			thread2.Join();

			// There should in total be 4 tables, but only two distinct identifiers.
			Assert.AreEqual(4, uniqueIntegerList.Count);
			Assert.AreEqual(2, uniqueIntegerList.Distinct().Count());
		}
	}
}