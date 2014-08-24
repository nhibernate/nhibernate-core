using NHibernate.Criterion;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3171
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var entrant = new Artist
				{
					Name = "Alex Swings Oscar Sings",
					Song = new Song { Name = "Miss Kiss Kiss Bang" },
					Country = new Country { Name = "Germany" }
				};

				session.Save(entrant.Country);
				session.Save(entrant.Song);
				session.Save(entrant);
				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.Delete("from Artist");
				s.Delete("from Song");
				s.Delete("from Country");
				t.Commit();
			}
		}

		[Test]
		public void SqlShouldIncludeAliasAsJoinWhenRestrictingByCompositeKeyColumn()
		{
			using (var s = OpenSession())
			using (s.BeginTransaction())
			{
				// Should not throw
				// The multi-part identifier "s1_.Name" could not be bound.
				s.CreateCriteria<Artist>("a")
					.CreateAlias("a.Song", "s")
					.Add(Restrictions.Eq("s.Name", "Miss Kiss Kiss Bang"))
					.List<Artist>();
			}
		}
	}
}