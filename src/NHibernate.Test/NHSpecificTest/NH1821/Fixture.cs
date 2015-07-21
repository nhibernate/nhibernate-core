using System.Text.RegularExpressions;
using NHibernate.Dialect;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1821
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return dialect is MsSql2005Dialect;
		}

		[Test]
		public void ShouldNotRemoveLineBreaksFromSqlQueries()
		{
			using (var spy = new SqlLogSpy())
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				const string sql = @"
select Id
from Entity
where 1=1";
				var query = s.CreateSQLQuery(sql);
				Assert.DoesNotThrow(() => query.List());

				string renderedSql = spy.Appender.GetEvents()[0].RenderedMessage;

				Regex whitespaces = new Regex(@"\s+", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);

				Assert.AreEqual(
					string.Compare(
						whitespaces.Replace(sql, " ").Trim(),
						whitespaces.Replace(renderedSql, " ").Trim(),
						true
						),
					0
				);
			}
		}
	}
}
