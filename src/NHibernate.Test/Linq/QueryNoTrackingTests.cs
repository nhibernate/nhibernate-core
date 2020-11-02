using System.Linq;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.Linq
{
	[TestFixture]
	public class QueryNoTrackingTests : LinqTestCase
	{
		[Test]
		public void QueryIsNoTracking()
		{
			var customers = db.Customers
				.Where(c => c.ContactTitle == "Sales Representative")
				.AsNoTracking()
				.ToList();

			Assert.NotNull(customers);
			Assert.IsNotEmpty(customers);
			Assert.True(customers.All(c => !session.Contains(c)));
			Assert.AreEqual(0, this.session.GetSessionImplementation().PersistenceContext.EntityEntries.Count);
		}

		[Test]
		public void FutureQueryIsNoTracking()
		{
			var customers = db.Customers
				.Where(c => c.ContactTitle == "Sales Representative")
				.AsNoTracking()
				.ToFuture()
				.ToList();

			Assert.NotNull(customers);
			Assert.IsNotEmpty(customers);
			Assert.True(customers.All(c => !session.Contains(c)));
			Assert.AreEqual(0, this.session.GetSessionImplementation().PersistenceContext.EntityEntries.Count);
		}
	}
}
