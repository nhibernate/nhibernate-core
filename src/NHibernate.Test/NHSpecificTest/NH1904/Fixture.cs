using System;
using System.Collections.Generic;

using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1904
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		[Test]
		public void ExecuteQuery()
		{
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				Invoice invoice = new Invoice();
				invoice.Issued = DateTime.Now;

				session.Save(invoice);
				transaction.Commit();
			}

			using (ISession session = OpenSession())
			{
				IList<Invoice> invoices = session.CreateCriteria<Invoice>().List<Invoice>();
			}
		}

		protected override void OnTearDown()
		{
			base.OnTearDown();
			using (ISession session = OpenSession())
			{
				session.CreateQuery("delete from Invoice").ExecuteUpdate();
				session.Flush();
			}
		}
	}
}
