using System.Collections.Generic;
using NHibernate.Criterion;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3171
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		[Test]
		public void SqlShouldIncludeAliasAsJoinWhenRestrictingByCompositeKeyColumn()
		{
			try
			{
				using (var s = OpenSession())
				using (var t = s.BeginTransaction())
				{
					var entrant = new Artist
					{
						Name = "Alex Swings Oscar Sings",
						Song = new Song { Name = "Miss Kiss Kiss Bang" },
						Country = new Country { Name = "Germany" }
					};

					s.Save(entrant.Country);
					s.Save(entrant.Song);
					s.Save(entrant);
				}

				using (var s = OpenSession())
				using (var t = s.BeginTransaction())
				{
					// Should not throw
					// The multi-part identifier "s1_.Name" could not be bound.
					IList<Artist> artists = s.CreateCriteria<Artist>("a")
						.CreateAlias("a.Song", "s")
						.Add(Restrictions.Eq("s.Name", "Miss Kiss Kiss Bang"))
						.List<Artist>();
				}
			}
			finally
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
		}
	}
}