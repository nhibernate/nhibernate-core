using System.Collections;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1925
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		private const string NAME_JOE = "Joe";
		private const string NAME_ALLEN = "Allen";

		protected override void OnSetUp()
		{
			base.OnSetUp();
			using (ISession session = OpenSession())
			{
				using (ITransaction tx = session.BeginTransaction())
				{
					var joe = new Customer {Name = NAME_JOE};
					session.Save(joe);

					var allen = new Customer {Name = NAME_ALLEN};
					session.Save(allen);

					var joeInvoice0 = new Invoice {Customer = joe, Number = 0};
					session.Save(joeInvoice0);

					var allenInvoice1 = new Invoice {Customer = allen, Number = 1};
					session.Save(allenInvoice1);

					tx.Commit();
				}
			}
		}

		protected override void OnTearDown()
		{
			using (ISession session = OpenSession())
			{
				using (ITransaction tx = session.BeginTransaction())
				{
					session.Delete("from Invoice");
					session.Delete("from Customer");
					tx.Commit();
				}
			}
			base.OnTearDown();
		}

		private void FindJoesLatestInvoice(string hql)
		{
			using (ISession session = OpenSession())
			{
				using (ITransaction tx = session.BeginTransaction())
				{
					IList list = session.CreateQuery(hql)
						.SetString("name", NAME_JOE)
						.List();

					Assert.That(list, Is.Not.Empty);
					tx.Commit();
				}
			}
		}

		[Test]
		public void Query1()
		{
			FindJoesLatestInvoice(
				@"
                    select invoice
                    from Invoice invoice
                        join invoice.Customer customer
                    where
                        invoice.Number = (
                            select max(invoice2.Number) 
                            from 
                                invoice.Customer d2
                                    join d2.Invoices invoice2
                            where
                                d2 = customer
                        )
                        and customer.Name = :name
                ");
		}

		[Test]
		public void Query2()
		{
			FindJoesLatestInvoice(
				@"
                    select invoice
                    from Invoice invoice
                        join invoice.Customer customer
                    where
                        invoice.Number = (select max(invoice2.Number) from customer.Invoices invoice2)
                        and customer.Name = :name
                ");
		}
	}
}