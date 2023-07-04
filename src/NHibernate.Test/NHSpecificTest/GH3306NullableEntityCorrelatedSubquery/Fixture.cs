using System.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH3306NullableEntityCorrelatedSubquery
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		private const string NAME_JOE = "Joe";
		private const string NAME_ALLEN = "Allen";

		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var tx = session.BeginTransaction())
			{
				var joe = new Customer { Name = NAME_JOE };
				session.Save(joe);

				var allen = new Customer { Name = NAME_ALLEN };
				session.Save(allen);

				var joeInvoice0 = new Invoice { Customer = joe, Number = 0 };
				session.Save(joeInvoice0);

				var allenInvoice1 = new Invoice { Customer = allen, Number = 1 };
				session.Save(allenInvoice1);

				tx.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var tx = session.BeginTransaction())
			{
				session.Delete("from Invoice");
				session.Delete("from Customer");
				tx.Commit();
			}
		}

		[Test]
		public void NullableEntityInCorrelatedSubquery()
		{
			using (var s = OpenSession())
			{
				var customers = s.Query<Customer>().Where(c => c.Name == NAME_JOE);
				var results = s.Query<Invoice>()
					.Where(i => customers.Any(c => c.Invoices.Any(ci => ci.Customer == i.Customer))).ToList();

				Assert.That(results.Count, Is.EqualTo(1));
			}
		}
	}
}
