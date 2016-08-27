using System.Linq;
using System.Linq.Dynamic;
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
		public void CanCahceDynamicLinq()
		{
			//dynamic orderby clause
			var users = db.Users
						  .Cacheable()
						  .Fetch(x => x.Role)
						  .OrderBy("RegisteredAt");

			users
			  .ToList();

			using (var log = new SqlLogSpy())
			{
				users.ToList();
				Assert.IsNullOrEmpty(log.GetWholeLog());
			}
		}
	}
}
