using System.Linq;
using NHibernate.Cfg;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.Linq
{
	[TestFixture]
	public class QueryCommentTests : LinqTestCase
	{
		protected override void Configure(Configuration configuration)
		{
			configuration.SetProperty(Environment.UseSqlComments, "true");
		}

		[Test]
		public void CanSetCommentOnLinqQueries()
		{
			using (var sl = new SqlLogSpy())
			{
				var comment = "This is my comment";
				var result = (from e in db.Customers
							  where e.CompanyName == "Bon app'"
							  select e).WithOptions(o => o.SetComment(comment)).ToList();
				var sql = sl.Appender.GetEvents()[0].RenderedMessage;

				Assert.That(sql.IndexOf(comment), Is.GreaterThan(0));
			}
		}

		[Test]
		public void CanSetCommentOnLinqPagingQuery()
		{
			using (var sl = new SqlLogSpy())
			{
				var comment = "This is my comment";
				var result = (from e in db.Customers
							  select e).Skip(1).Take(1).WithOptions(o => o.SetComment(comment)).ToList();
				var sql = sl.Appender.GetEvents()[0].RenderedMessage;

				Assert.That(sql.IndexOf(comment), Is.GreaterThan(0));
			}
		}

		[Test]
		public void CanSetCommentBeforeSkipOnLinqOrderedPageQuery()
		{
			using (var sl = new SqlLogSpy())
			{
				var comment = "This is my comment";
				var result = (from e in db.Customers
							  orderby e.CompanyName
							  select e).WithOptions(o => o.SetComment(comment)).Skip(5).Take(5).ToList();
				var sql = sl.Appender.GetEvents()[0].RenderedMessage;

				Assert.That(sql.IndexOf(comment), Is.GreaterThan(0));
			}
		}
	}
}
