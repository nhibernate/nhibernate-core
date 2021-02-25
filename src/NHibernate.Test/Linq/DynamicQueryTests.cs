using System.Linq;
using System.Linq.Dynamic.Core;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.Linq
{
	[TestFixture]
	public class DynamicQueryTests : LinqTestCase
	{
		[Test]
		public void CanQueryWithDynamicOrderBy()
		{
			//dynamic orderby clause
			var list = db.Users
				.OrderBy("RegisteredAt")
				.ToList();

			Assert.That(list, Is.Ordered.By("RegisteredAt"));
		}

		[Test(Description = "NH-3239")]
		public void CanCacheDynamicLinq()
		{
			//dynamic orderby clause
			var users = db.Users
				.WithOptions(o => o.SetCacheable(true))
				.Fetch(x => x.Role)
				.OrderBy("RegisteredAt");

			users.ToList();

			using (var log = new SqlLogSpy())
			{
				users.ToList();
				Assert.That(log.GetWholeLog(), Is.Null.Or.Empty);
			}
		}
	}
}
