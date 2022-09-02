using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH2858
{
	[TestFixture]
	public class ByCodeFixture : TestCaseMappingByCode
	{
		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();

			mapper.Class<Department>(
				rc =>
				{
					rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
					rc.Property(x => x.Name);
				});
			mapper.Class<Project>(
				rc =>
				{
					rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
					rc.Property(x => x.Name);
					rc.ManyToOne(x => x.Department, m => m.Column("DepartmentId"));
				});
			mapper.Class<Issue>(
				rc =>
				{
					rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
					rc.Property(x => x.Name);
					rc.IdBag(
						x => x.Departments,
						m => m.Table("IssuesToDepartments"),
						r => r.ManyToMany());
					rc.ManyToOne(x => x.Project, m => m.Column("ProjectId"));
				});
			mapper.Class<TimeChunk>(
				rc =>
				{
					rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
					rc.ManyToOne(x => x.Issue, m => m.Column("IssueId"));
					rc.Property(x => x.Seconds);
				});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var deptA = new Department {Name = "A"};
				session.Save(deptA);
				var deptB = new Department {Name = "B"};
				session.Save(deptB);
				var deptC = new Department {Name = "C"};
				session.Save(deptC);
				var deptD = new Department {Name = "D"};
				session.Save(deptD);
				var deptE = new Department {Name = "E"};
				session.Save(deptE);

				var projectX = new Project {Name = "X", Department = deptA};
				session.Save(projectX);
				var projectY = new Project {Name = "Y", Department = deptC};
				session.Save(projectY);
				var projectZ = new Project {Name = "Z", Department = deptE};
				session.Save(projectZ);

				var issue1 = new Issue {Name = "TEST-1", Project = projectX,};
				session.Save(issue1);
				var issue2 = new Issue {Name = "TEST-2", Project = projectX, Departments = {deptA},};
				session.Save(issue2);
				var issue3 = new Issue {Name = "TEST-3", Project = projectX, Departments = {deptA, deptB},};
				session.Save(issue3);
				var issue4 = new Issue {Name = "TEST-4", Project = projectY,};
				session.Save(issue4);
				var issue5 = new Issue {Name = "TEST-5", Project = projectY, Departments = {deptD}};
				session.Save(issue5);

				session.Save(new TimeChunk {Issue = issue1});
				session.Save(new TimeChunk {Issue = issue1});
				session.Save(new TimeChunk {Issue = issue2});
				session.Save(new TimeChunk {Issue = issue2});
				session.Save(new TimeChunk {Issue = issue3});
				session.Save(new TimeChunk {Issue = issue3});
				session.Save(new TimeChunk {Issue = issue4});
				session.Save(new TimeChunk {Issue = issue4});
				session.Save(new TimeChunk {Issue = issue5});
				session.Save(new TimeChunk {Issue = issue5});

				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.Delete("from System.Object");
				transaction.Commit();
			}
		}

		[Test(Description = "GH-2857")]
		public void GroupLevelQuery()
		{
			using (var session = OpenSession())
			{
				var query = session.Query<ITimeChunk>()
									.Select(x => new object[] {(object) x})
									.GroupBy(g => new object[] {(Guid?) (((ITimeChunk) g[0]).Issue.Project.Id)}, v => (ITimeChunk) v[0])
									.Select(r => new object[] {r.Key, r.Sum(t => (int?) t.Seconds)});

				var results = query.ToList();
				Assert.That(results, Has.Count.EqualTo(2));
			}
		}

		[Test(Description = "GH-2857")]
		public void GroupLevelQuery_Simplified()
		{
			using (var session = OpenSession())
			{
				var query = session.Query<ITimeChunk>()
									.Select(x => new object[] {x})
									.GroupBy(g => new object[] {((ITimeChunk) g[0]).Issue.Project.Id}, v => (ITimeChunk) v[0])
									.Select(r => new object[] {r.Key, r.Sum(t => (int?) t.Seconds)});

				var results = query.ToList();
				Assert.That(results, Has.Count.EqualTo(2));
			}
		}

		[Test]
		public void SelectManySubQueryWithCoalesce()
		{
			using (var session = OpenSession())
			{
				var usedDepartments = session.Query<ITimeChunk>()
											.SelectMany(x => ((IEnumerable<object>) x.Issue.Departments).DefaultIfEmpty().Select(d => (object) ((Guid?) ((Guid?) (((IDepartment) d).Id) ?? x.Issue.Project.Department.Id))))
											.Where(id => id != null)
											.Select(id => (Guid?) id);

				var result = session.Query<IDepartment>()
									.Where(d => usedDepartments.Contains(d.Id))
									.Select(d => new {d.Id, d.Name});

				Assert.That(result.ToList(), Has.Count.EqualTo(4));
			}
		}

		[Test]
		public void SelectManySubQueryWithCoalesce_Simplified()
		{
			using (var session = OpenSession())
			{
				var usedDepartments = session.Query<ITimeChunk>()
											.SelectMany(x => ((IEnumerable<object>) x.Issue.Departments).DefaultIfEmpty().Select(d => (Guid?) ((IDepartment) d).Id ?? x.Issue.Project.Department.Id));

				var result = session.Query<IDepartment>()
									.Where(d => usedDepartments.Contains(d.Id))
									.Select(d => new {d.Id, d.Name});

				Assert.That(result.ToList(), Has.Count.EqualTo(4));
			}
		}
	}
}
