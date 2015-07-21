using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1981
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			// Firebird doesn't support this feature
			return !(dialect is Dialect.FirebirdDialect);
		}

		protected override void OnSetUp()
		{
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				s.Save(new Article { Longitude = 90 });
				s.Save(new Article { Longitude = 90 });
				s.Save(new Article { Longitude = 120 });

				tx.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				s.Delete("from Article");

				tx.Commit();
			}
		}

		[Test]
		public void CanGroupWithParameter()
		{
			using (var s = OpenSession())
			using (s.BeginTransaction())
			{
				const string queryString =
					@"select (Longitude / :divisor)
					  from Article
					  group by (Longitude / :divisor)
					  order by 1";

				var quotients = s.CreateQuery(queryString)
					.SetDouble("divisor", 30)
					.List<double>();

				Assert.That(quotients.Count, Is.EqualTo(2));
				Assert.That(quotients[0], Is.EqualTo(3));
				Assert.That(quotients[1], Is.EqualTo(4));
			}
		}
	}
}
