using System;
using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Linq;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3889
{
	[TestFixture]
	public class ByCodeFixture : TestCaseMappingByCode
	{
		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<Job>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
				rc.Property(x => x.Name);
			});
			mapper.Class<Project>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
				rc.Property(x => x.Name);
				rc.ManyToOne(x => x.Job, map =>
				{
					map.Column("JobId");
					map.NotNullable(true);
				});
			});
			mapper.Class<TimeRecord>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
				rc.Property(x => x.Hours);
				rc.ManyToOne(x => x.Project, map =>
				{
					map.Column("ProjectId");
					map.NotNullable(true);
				});
				rc.ManyToOne(x => x.ActualJob, map =>
				{
					map.Column("ActualJobId");
				});
				rc.ManyToOne(x => x.Setting, map =>
				{
					map.Column("SettingId");
				});
			});

			mapper.Class<TimeSetting>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
				rc.ManyToOne(x => x.Include, map =>
				{
					map.Column("IncludeId");
				});
			});

			mapper.Class<TimeInclude>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
				rc.Property(x => x.Flag);
			});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var job_a = new Job { Name = "Big Job" };
				session.Save(job_a);
				var job_b = new Job { Name = "Small Job" };
				session.Save(job_b);

				var project_a = new Project { Job = job_a, Name = "Big Job - Part A" };
				session.Save(project_a);
				var project_b = new Project { Job = job_a, Name = "Big Job - Part B" };
				session.Save(project_b);
				var project_c = new Project { Job = job_b, Name = "Small Job - Rework" };
				session.Save(project_c);

				var include = new TimeInclude { Flag = true };
				session.Save(include);
				var setting = new TimeSetting { Include = include };
				session.Save(setting);

				session.Save(new TimeRecord { Project = project_a, Hours = 2, Setting = setting }/*.AddTime(2)*/);
				session.Save(new TimeRecord { Project = project_a, Hours = 3, Setting = setting }/*.AddTime(3)*/);
				session.Save(new TimeRecord { Project = project_b, Hours = 5, Setting = setting }/*.AddTime(2).AddTime(3)*/);
				session.Save(new TimeRecord { Project = project_b, Hours = 2, Setting = setting }/*.AddTime(1).AddTime(1)*/);
				session.Save(new TimeRecord { Project = project_c, Hours = 7, Setting = setting }/*.AddTime(2).AddTime(3).AddTime(2)*/);
				session.Save(new TimeRecord { Project = project_c, ActualJob = job_a, Hours = 3, Setting = setting }/*.AddTime(1).AddTime(1).AddTime(1)*/);

				session.Flush();
				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.Delete("from TimeRecord");
				session.Delete("from TimeInclude");
				session.Delete("from TimeSetting");
				session.Delete("from Project");
				session.Delete("from Job");

				session.Flush();
				transaction.Commit();
			}
		}

		[Test]
		public void CoalesceOnEntitySum()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var job_a = session.Query<Job>().Single(j => j.Name == "Big Job");
				var job_a_hours = session.Query<TimeRecord>()
					.Where(t => (t.ActualJob ?? t.Project.Job) == job_a)
					.Sum(t => t.Hours);
				Assert.That(job_a_hours, Is.EqualTo(15));

				var job_b = session.Query<Job>().Single(j => j.Name == "Small Job");
				var job_b_hours = session.Query<TimeRecord>()
					.Where(t => (t.ActualJob ?? t.Project.Job) == job_b)
					.Sum(t => t.Hours);
				Assert.That(job_b_hours, Is.EqualTo(7));
			}
		}

		[Test]
		public void CoalesceOnEntitySumWithExtraJoin()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var include = session.Query<TimeInclude>().Single();

				var job_a = session.Query<Job>().Single(j => j.Name == "Big Job");
				var job_a_hours = session.Query<TimeRecord>()
					.Where(t => (t.ActualJob ?? t.Project.Job) == job_a)
					.Where(t => t.Setting.Include == include)
					.Sum(t => t.Hours);
				Assert.That(job_a_hours, Is.EqualTo(15));

				var job_b = session.Query<Job>().Single(j => j.Name == "Small Job");
				var job_b_hours = session.Query<TimeRecord>()
					.Where(t => (t.ActualJob ?? t.Project.Job) == job_b)
					.Where(t => t.Setting.Include == include)
					.Sum(t => t.Hours);
				Assert.That(job_b_hours, Is.EqualTo(7));
			}
		}

		[Test]
		public void CoalesceOnEntitySubselectSum()
		{
			AssertDialect();
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var query = session.Query<Job>()
								.Select(j => new
								{
									Job = j,
									Hours = session.Query<TimeRecord>()
												.Where(t => (t.ActualJob ?? t.Project.Job) == j)
												.Sum(t => (decimal?) t.Hours) ?? 0
								});
				var results = query.ToList();

				Assert.That(results.Count, Is.EqualTo(2));
				Assert.That(results.Single(x => x.Job.Name == "Big Job").Hours, Is.EqualTo(15));
				Assert.That(results.Single(x => x.Job.Name == "Small Job").Hours, Is.EqualTo(7));
			}
		}

		[Test]
		public void CoalesceOnEntitySubselectSumWithExtraJoin()
		{
			AssertDialect();
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var include = session.Query<TimeInclude>().Single();

				var query = session.Query<Job>()
								.Select(j => new
								{
									Job = j,
									Hours = session.Query<TimeRecord>()
												.Where(t => (t.ActualJob ?? t.Project.Job) == j)
												.Where(t => t.Setting.Include == include)
												.Sum(t => (decimal?) t.Hours) ?? 0
								});
				var results = query.ToList();

				Assert.That(results.Count, Is.EqualTo(2));
				Assert.That(results.Single(x => x.Job.Name == "Big Job").Hours, Is.EqualTo(15));
				Assert.That(results.Single(x => x.Job.Name == "Small Job").Hours, Is.EqualTo(7));
			}
		}

		[Test]
		public void CoalesceOnIdSubselectSum()
		{
			AssertDialect();
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var query = session.Query<Job>()
								.Select(j => new
								{
									Job = j,
									Hours = session.Query<TimeRecord>()
												.Where(t => ((Guid?) t.ActualJob.Id ?? t.Project.Job.Id) == j.Id)
												.Sum(t => (decimal?) t.Hours) ?? 0
								});
				var results = query.ToList();

				Assert.That(results.Count, Is.EqualTo(2));
				Assert.That(results.Single(x => x.Job.Name == "Big Job").Hours, Is.EqualTo(15));
				Assert.That(results.Single(x => x.Job.Name == "Small Job").Hours, Is.EqualTo(7));
			}
		}

		[Test]
		public void CoalesceOnIdSubselectSumWithExtraJoin()
		{
			AssertDialect();
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var include = session.Query<TimeInclude>().Single();

				var query = session.Query<Job>()
								.Select(j => new
								{
									Job = j,
									Hours = session.Query<TimeRecord>()
												.Where(t => ((Guid?) t.ActualJob.Id ?? t.Project.Job.Id) == j.Id)
												.Where(t => t.Setting.Include == include)
												.Sum(t => (decimal?) t.Hours) ?? 0
								});
				var results = query.ToList();

				Assert.That(results.Count, Is.EqualTo(2));
				Assert.That(results.Single(x => x.Job.Name == "Big Job").Hours, Is.EqualTo(15));
				Assert.That(results.Single(x => x.Job.Name == "Small Job").Hours, Is.EqualTo(7));
			}
		}

		void AssertDialect()
		{
			if (!Dialect.SupportsScalarSubSelects)
				Assert.Ignore(Dialect.GetType() + " does not support this type of query");
		}
	}
}
