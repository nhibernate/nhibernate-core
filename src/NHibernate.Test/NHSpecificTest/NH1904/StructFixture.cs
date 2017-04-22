using System;
using System.Collections;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1904
{
	[TestFixture]
	public class StructFixture : BugTestCase
	{
		protected override IList Mappings =>
			new string[]
			{
				"NHSpecificTest." + BugNumber + ".StructMappings.hbm.xml"
			};

		[Test]
		public void ExecuteQuery()
		{
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				var invoice = new InvoiceWithAddress
				{
					Issued = DateTime.Now,
					BillingAddress = new Address { Line = "84 rue du 22 septembre", City = "Courbevoie", ZipCode = "92400", Country = "France" }
				};
				session.Save(invoice);
				transaction.Commit();
			}

			using (ISession session = OpenSession())
			{
				var invoices = session.CreateCriteria<Invoice>().List<Invoice>();
			}
		}

		protected override void OnTearDown()
		{
			base.OnTearDown();
			using (ISession session = OpenSession())
			{
				session.CreateQuery("delete from InvoiceWithAddress").ExecuteUpdate();
				session.Flush();
			}
		}
	}
}
