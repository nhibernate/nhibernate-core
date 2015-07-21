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
			Assert.That(selectSql.GetValue(tg).ToString(), Is.StringContaining(customWhere));
			Assert.That(updateSql.GetValue(tg).ToString(), Is.StringContaining(customWhere));
		}
	}
}
