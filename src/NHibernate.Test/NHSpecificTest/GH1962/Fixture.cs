using System.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH1962
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		[Test]
		[KnownBug("#1962")]
		public void LinqShouldBeValid()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var result =
					session
						.Query<Product>()
						.Count(p => p.OrderDetails.Any(od => od.Order.OrderDetails[0] == od));
				Assert.That(result, Is.EqualTo(0));
			}
		}
	}
}
