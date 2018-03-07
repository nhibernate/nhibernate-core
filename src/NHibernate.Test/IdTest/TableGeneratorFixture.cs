using System.Collections.Generic;
using System.Reflection;
using NHibernate.Dialect;
using NHibernate.Id;
using NUnit.Framework;

namespace NHibernate.Test.IdTest
{
	[TestFixture]
	public class TableGeneratorFixture
	{
		private const BindingFlags Flags =
			BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

		private readonly FieldInfo updateSql = typeof (TableGenerator).GetField("updateSql", Flags);
		private readonly FieldInfo selectSql = typeof (TableGenerator).GetField("query", Flags);

		[Test]
		public void SelectAndUpdateStringContainCustomWhere()
		{
			const string customWhere = "table_name='second'";
			var dialect = new MsSql2005Dialect();
			var tg = new TableGenerator();
			tg.Configure(NHibernateUtil.Int64, new Dictionary<string, string> {{"where", customWhere}}, dialect);
			Assert.That(selectSql.GetValue(tg).ToString(), Does.Contain(customWhere));
			Assert.That(updateSql.GetValue(tg).ToString(), Does.Contain(customWhere));
		}

		[Test]
		public void GenerateSelectAndUpdateWithoutQuotes()
		{
			var dialect = new MsSql2005Dialect();
			var tableGenerator = new TableGenerator();
			tableGenerator.Configure(NHibernateUtil.Int64, new Dictionary<string, string> {{"table", "customTableName"}, {"column", "customColumnName"}}, dialect);
			Assert.That(selectSql.GetValue(tableGenerator).ToString(), Is.EqualTo("select customColumnName from customTableName with (updlock, rowlock)"));
			Assert.That(updateSql.GetValue(tableGenerator).ToString(), Is.EqualTo("update customTableName set customColumnName = ? where customColumnName = ?"));
		}

		[Test]
		public void GenerateSelectAndUpdateWithQuotesForMsSql2005()
		{
			var dialect = new MsSql2005Dialect();
			var tableGenerator = new TableGenerator();
			tableGenerator.Configure(NHibernateUtil.Int64, new Dictionary<string, string> {{"table", "`customTableName`"}, {"column", "`customColumnName`"}}, dialect);
			Assert.That(selectSql.GetValue(tableGenerator).ToString(), Is.EqualTo("select [customColumnName] from [customTableName] with (updlock, rowlock)"));
			Assert.That(updateSql.GetValue(tableGenerator).ToString(), Is.EqualTo("update [customTableName] set [customColumnName] = ? where [customColumnName] = ?"));
		}

		[Test]
		public void GenerateSelectAndUpdateWithQuotesForPostgres()
		{
			var dialect = new PostgreSQLDialect();
			var tableGenerator = new TableGenerator();
			tableGenerator.Configure(NHibernateUtil.Int64, new Dictionary<string, string> {{"table", "`customTableName`"}, {"column", "`customColumnName`"}}, dialect);
			Assert.That(selectSql.GetValue(tableGenerator).ToString(), Is.EqualTo("select \"customColumnName\" from \"customTableName\" for update"));
			Assert.That(updateSql.GetValue(tableGenerator).ToString(), Is.EqualTo("update \"customTableName\" set \"customColumnName\" = ? where \"customColumnName\" = ?"));
		}
	}
}
