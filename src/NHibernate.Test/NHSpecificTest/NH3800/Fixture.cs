﻿using System.Linq;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3800
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			var tagA = new Tag() { Name = "A" };
			var tagB = new Tag() { Name = "B" };

			var project1 = new Project { Name = "ProjectOne" };
			var compP1_x = new Component() { Name = "PONEx", Project = project1 };
			var compP1_y = new Component() { Name = "PONEy", Project = project1 };

			var project2 = new Project { Name = "ProjectTwo" };
			var compP2_x = new Component() { Name = "PTWOx", Project = project2 };
			var compP2_y = new Component() { Name = "PTWOy", Project = project2 };

			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.Save(tagA);
				session.Save(tagB);
				session.Save(project1);
				session.Save(compP1_x);
				session.Save(compP1_y);
				session.Save(project2);
				session.Save(compP2_x);
				session.Save(compP2_y);

				session.Save(new TimeRecord { TimeInHours = 1, Project = null, Components = { }, Tags = { tagA } });
				session.Save(new TimeRecord { TimeInHours = 2, Project = null, Components = { }, Tags = { tagB } });

				session.Save(new TimeRecord { TimeInHours = 3, Project = project1, Tags = { tagA, tagB } });
				session.Save(new TimeRecord { TimeInHours = 4, Project = project1, Components = { compP1_x }, Tags = { tagB } });
				session.Save(new TimeRecord { TimeInHours = 5, Project = project1, Components = { compP1_y }, Tags = { tagA } });
				session.Save(new TimeRecord { TimeInHours = 6, Project = project1, Components = { compP1_x, compP1_y }, Tags = { } });

				session.Save(new TimeRecord { TimeInHours = 7, Project = project2, Components = { }, Tags = { tagA, tagB } });
				session.Save(new TimeRecord { TimeInHours = 8, Project = project2, Components = { compP2_x }, Tags = { tagB } });
				session.Save(new TimeRecord { TimeInHours = 9, Project = project2, Components = { compP2_y }, Tags = { tagA } });
				session.Save(new TimeRecord { TimeInHours = 10, Project = project2, Components = { compP2_x, compP2_y }, Tags = { } });

				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.Delete("from TimeRecord");
				session.Delete("from Component");
				session.Delete("from Project");
				session.Delete("from Tag");

				transaction.Commit();
			}
		}

		[Test]
		public void ExpectedHql()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var baseQuery = session.Query<TimeRecord>();

				Assert.That(baseQuery.Sum(x => x.TimeInHours), Is.EqualTo(55));

				var query = session.CreateQuery(@"
                    select c.Id, count(t), sum(cast(t.TimeInHours as big_decimal)) 
                    from TimeRecord t 
                    left join t.Components as c 
                    group by c.Id");

				var results = query.List<object[]>();
				Assert.That(results.Select(x => x[1]), Is.EquivalentTo(new[] { 4, 2, 2, 2, 2 }));
				Assert.That(results.Select(x => x[2]), Is.EquivalentTo(new[] { 13, 10, 11, 18, 19 }));

				Assert.That(results.Sum(x => (decimal?)x[2]), Is.EqualTo(71));

				transaction.Rollback();
			}
		}

		[Test]
		public void PureLinq()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var baseQuery = session.Query<TimeRecord>();
				var query = from t in baseQuery
							from c in t.Components.Select(x => (object)x.Id).DefaultIfEmpty()
							let r = new object[] { c, t }
							group r by r[0]
								into g
								select new[] { g.Key, g.Select(x => x[1]).Count(), g.Select(x => x[1]).Sum(x => (decimal?)((TimeRecord)x).TimeInHours) };

				var results = query.ToList();
				Assert.That(results.Select(x => x[1]), Is.EquivalentTo(new[] { 4, 2, 2, 2, 2 }));
				Assert.That(results.Select(x => x[2]), Is.EquivalentTo(new[] { 13, 10, 11, 18, 19 }));

				Assert.That(results.Sum(x => (decimal?)x[2]), Is.EqualTo(71));

				transaction.Rollback();
			}
		}

		[Test]
		public void MethodGroup()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var baseQuery = session.Query<TimeRecord>();
				var query = baseQuery
						.SelectMany(t => t.Components.Select(c => c.Id).DefaultIfEmpty().Select(c => new object[] { c, t }))
						.GroupBy(g => g[0], g => (TimeRecord)g[1])
						.Select(g => new[] { g.Key, g.Count(), g.Sum(x => (decimal?)x.TimeInHours) });

				var results = query.ToList();
				Assert.That(results.Select(x => x[1]), Is.EquivalentTo(new[] { 4, 2, 2, 2, 2 }));
				Assert.That(results.Select(x => x[2]), Is.EquivalentTo(new[] { 13, 10, 11, 18, 19 }));

				Assert.That(results.Sum(x => (decimal?)x[2]), Is.EqualTo(71));

				transaction.Rollback();
			}
		}

		[Test]
		public void ComplexExample()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var baseQuery = session.Query<TimeRecord>();

				Assert.That(baseQuery.Sum(x => x.TimeInHours), Is.EqualTo(55));

				var query = baseQuery.Select(t => new object[] { t })
					.SelectMany(t => ((TimeRecord)t[0]).Components.Select(c => (object)c.Id).DefaultIfEmpty().Select(c => new[] { t[0], c }))
					.SelectMany(t => ((TimeRecord)t[0]).Tags.Select(x => (object)x.Id).DefaultIfEmpty().Select(x => new[] { t[0], t[1], x }))
					.GroupBy(j => new[] { ((TimeRecord)j[0]).Project.Id, j[1], j[2] }, j => (TimeRecord)j[0])
					.Select(g => new object[] { g.Key, g.Count(), g.Sum(t => (decimal?)t.TimeInHours) });

				var results = query.ToList();
				Assert.That(results.Select(x => x[1]), Is.EquivalentTo(new[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }));
				Assert.That(results.Select(x => x[2]), Is.EquivalentTo(new[] { 1, 2, 3, 3, 4, 5, 6, 6, 7, 7, 8, 9, 10, 10 }));

				Assert.That(results.Sum(x => (decimal?)x[2]), Is.EqualTo(81));

				transaction.Rollback();
			}
		}

		[Test]
		public void OuterJoinGroupingWithSubQueryInProjection()
		{
			if (!Dialect.SupportsScalarSubSelects)
				Assert.Ignore("Dialect does not support scalar sub-select");

			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var baseQuery = session.Query<TimeRecord>();
				var query = baseQuery
						.SelectMany(t => t.Components.Select(c => c.Name).DefaultIfEmpty().Select(c => new object[] { c, t }))
						.GroupBy(g => g[0], g => (TimeRecord)g[1])
						.Select(g => new[] { g.Key, g.Count(), session.Query<Component>().Count(c => c.Name == (string)g.Key) });

				var results = query.ToList();
				Assert.That(results.Select(x => x[1]), Is.EquivalentTo(new[] { 4, 2, 2, 2, 2 }));
				Assert.That(results.Select(x => x[2]), Is.EquivalentTo(new[] { 0, 1, 1, 1, 1 }));

				transaction.Rollback();
			}
		}
	}
}
