using System.Linq;
using System.Linq.Dynamic;
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
	}
}
