using System;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NUnit.Framework;
using System.Text.RegularExpressions;

namespace NHibernate.Test.NHSpecificTest.NH1785
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		[Test]
		public void Bug()
		{
			using (var session = OpenSession())
			{
				ICriteria criteria = session.CreateCriteria(typeof (Entity1));
				criteria.CreateAlias("Entities2", "ent2", JoinType.InnerJoin);
				criteria.CreateAlias("ent2.Id.Entity3", "ent3", JoinType.InnerJoin);
				criteria.CreateAlias("ent3.Entity4", "ent4", JoinType.InnerJoin);
				criteria.Add(Restrictions.Eq("ent4.Id", Guid.NewGuid()));
				Assert.DoesNotThrow(() => criteria.List<Entity1>());
			}
		}

		[Test]
		public void ShouldNotContainJoinWhereNotRequired()
		{
			using (var session = OpenSession())
			{
				using (var ls = new SqlLogSpy())
				{
					ICriteria criteria = session.CreateCriteria(typeof(Entity1));
					criteria.CreateAlias("Entities2", "ent2", JoinType.InnerJoin);
					criteria.List<Entity1>();
					var sql = ls.GetWholeLog();
					var rx = new Regex(@"\bjoin\b");
					Assert.That(rx.Matches(sql).Count, Is.EqualTo(1));
				}
			}
		}
	}
}
