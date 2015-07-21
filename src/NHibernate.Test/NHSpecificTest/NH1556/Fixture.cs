using System;
using System.Collections;
using NHibernate.Dialect;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1556
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return Dialect is MsSql2005Dialect;
		}

		// This test not fail but something very strange happen in various others tests
		// probably the problem is the implementation of QuotedAndParenthesisStringTokenizer in MsSql2005Dialect
		// but i'm not sure
		private Patient patient;

		protected override void OnSetUp()
		{
			var prozac = new Product("Prozac");
			var prozacId1 = new ProductIdentifier("12345-2345-11", prozac);
			var prozacId2 = new ProductIdentifier("12345-2345-12", prozac);

			var warfarin = new Product("Warfarin");
			var warfarinId3 = new ProductIdentifier("12345-4321-13", warfarin);

			patient = new Patient("John", "Doe");

			var prozacClaim1 = new Claim(patient, new DateTime(2000, 1, 1), prozacId1);
			var prozacClaim2 = new Claim(patient, new DateTime(2001, 1, 1), prozacId2);
			var warfarinClaim1 = new Claim(patient, new DateTime(2000, 4, 1), warfarinId3);

			using (ISession session = OpenSession())
			{
				using (ITransaction tx = session.BeginTransaction())
				{
					session.Save(prozac);
					session.Save(warfarin);
					session.Save(patient);
					session.Save(prozacClaim1);
					session.Save(prozacClaim2);
					session.Save(warfarinClaim1);

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
					session.Delete("from Claim");
					session.Delete("from Patient");
					session.Delete("from ProductIdentifier");
					session.Delete("from Product");
					tx.Commit();
				}
			}
		}

		[Test]
		public void CanOrderByAggregate()
		{
			using (ISession session = OpenSession())
			{
				using (ITransaction tx = session.BeginTransaction())
				{
					var loadedPatient = session.Get<Patient>(patient.Id);

					IList list =
						session.CreateQuery(
							@"select p.Id, p.ProductName, max(c.LastFilled), count(c.Id)
from Claim as c
join c.ProductIdentifier.Product as p
where c.Patient = :patient
group by p.Id, p.ProductName
order by max(c.LastFilled) asc, p.ProductName")
							.SetParameter("patient", loadedPatient).SetFirstResult(0).SetMaxResults(2).List();

					Assert.AreEqual(2, list.Count);
					Assert.AreEqual(new DateTime(2000, 4, 1), ((object[]) list[0])[2]);
					Assert.AreEqual(new DateTime(2001, 1, 1), ((object[]) list[1])[2]);
					Assert.AreEqual(1, ((object[]) list[0])[3]);
					Assert.AreEqual(2, ((object[]) list[1])[3]);
				}
			}
		}
	}
}