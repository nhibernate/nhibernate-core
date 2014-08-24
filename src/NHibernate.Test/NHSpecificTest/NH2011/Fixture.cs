using System.Collections.Generic;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2011
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		[Test]
		public void Test()
		{
			using (ISession session = OpenSession())
			{
				using (ITransaction tx = session.BeginTransaction())
				{
					session.Save(new Country {CountryCode = "SE"});
					tx.Commit();
				}
			}

			var newOrder = new Order();
			newOrder.GroupComponent = new GroupComponent();
			newOrder.GroupComponent.Countries = new List<Country>();
			newOrder.GroupComponent.Countries.Add(new Country {CountryCode = "SE"});

			Order mergedCopy;
			using (ISession session = OpenSession())
			{
				using (ITransaction tx = session.BeginTransaction())
				{
					mergedCopy = (Order) session.Merge(newOrder);
					tx.Commit();
				}
			}

			using (ISession session = OpenSession())
			{
				var order = session.Get<Order>(mergedCopy.Id);
				Assert.That(order.GroupComponent.Countries.Count, Is.EqualTo(1));
			}

			using (ISession session = OpenSession())
			{
				using (ITransaction tx = session.BeginTransaction())
				{
					session.Delete("from Order");
					session.Delete("from Country");
					tx.Commit();
				}
			}
		}
	}
}
