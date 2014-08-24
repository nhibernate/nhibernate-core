using System.Linq;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH646
{
	[TestFixture, Ignore("Not fixed yet.")]
	public class Fixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var station = new Station();
				session.Save(station);

				session.Save(new Policeman {Name = "2Bob", Station = station});

				session.Save(new Policeman {Name = "1Sally", Station = station});

				session.Flush();
				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.Delete("from System.Object");

				session.Flush();
				transaction.Commit();
			}
		}

		[Test]
		public void CanGetCountOfPolicemen()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var station = session.Query<Station>().Single();

				var policemen = station.Policemen;

				Assert.AreEqual(2, station.Policemen.Count());
				foreach (var policeman in policemen)
				{
					Assert.NotNull(policeman);
				}
			}
		}

		[Test]
		public void PolicemenOrderedByRank()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var station = session.Query<Station>().Single();


				Assert.AreEqual(2, station.Policemen.Count());
				Assert.That(station.Policemen, Is.Ordered.By("Name"));
			}
		}
	}
}
