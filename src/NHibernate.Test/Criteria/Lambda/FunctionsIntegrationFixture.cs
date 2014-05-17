using System;
using System.Collections;
using NHibernate.Criterion;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.Criteria.Lambda
{
	[TestFixture]
	public class FunctionsIntegrationFixture : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get { return new[] { "Criteria.Lambda.Mappings.hbm.xml" }; }
		}

		protected override void OnTearDown()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.Delete("from Person");
				t.Commit();
			}
		}

		protected override void OnSetUp()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.Save(new Person { Name = "p2", BirthDate = new DateTime(2008, 07, 07) });
				s.Save(new Person { Name = "p1", BirthDate = new DateTime(2009, 08, 07), Age = 90 });
				s.Save(new Person { Name = "pP3", BirthDate = new DateTime(2007, 06, 05) });

				t.Commit();
			}
		}

		[Test]
		public void YearPartEqual()
		{
			using (var s = OpenSession())
			using (s.BeginTransaction())
			{
				var persons = s.QueryOver<Person>()
					.Where(p => p.BirthDate.YearPart() == 2008)
					.List();

				persons.Count.Should().Be(1);
				persons[0].Name.Should().Be("p2");
			}
		}

		[Test]
		public void YearPartIsIn()
		{
			using (var s = OpenSession())
			using (s.BeginTransaction())
			{
				var persons = s.QueryOver<Person>()
					.Where(p => p.BirthDate.YearPart().IsIn(new[] { 2008, 2009 }))
					.OrderBy(p => p.Name).Asc
					.List();

				persons.Count.Should().Be(2);
				persons[0].Name.Should().Be("p1");
				persons[1].Name.Should().Be("p2");
			}
		}

		[Test]
		public void YearPartSingleOrDefault()
		{
			using (var s = OpenSession())
			using (s.BeginTransaction())
			{
				var yearOfBirth = s.QueryOver<Person>()
					.Where(p => p.Name == "p2")
					.Select(p => p.BirthDate.YearPart())
					.SingleOrDefault<object>();

				yearOfBirth.GetType().Should().Be(typeof (int));
				yearOfBirth.Should().Be(2008);
			}
		}

		[Test]
		public void SelectAvgYearPart()
		{
			using (var s = OpenSession())
			using (s.BeginTransaction())
			{
				var avgYear = s.QueryOver<Person>()
					.SelectList(list => list.SelectAvg(p => p.BirthDate.YearPart()))
					.SingleOrDefault<object>();

				avgYear.GetType().Should().Be(typeof (double));
				string.Format("{0:0}", avgYear).Should().Be("2008");
			}
		}

		[Test]
		public void SqrtSingleOrDefault()
		{
			using (var s = OpenSession())
			using (s.BeginTransaction())
			{
				var sqrtOfAge = s.QueryOver<Person>()
					.Where(p => p.Name == "p1")
					.Select(p => p.Age.Sqrt())
					.SingleOrDefault<object>();

				sqrtOfAge.Should().Be.InstanceOf<double>();
				string.Format("{0:0.00}", sqrtOfAge).Should().Be((9.49).ToString());
			}
		}

		[Test]
		public void FunctionsToLowerToUpper()
		{
			using (var s = OpenSession())
			using (s.BeginTransaction())
			{
				var names = s.QueryOver<Person>()
					.Where(p => p.Name == "pP3")
					.Select(p => p.Name.Lower(), p => p.Name.Upper())
					.SingleOrDefault<object[]>();

				names[0].Should().Be("pp3");
				names[1].Should().Be("PP3");
			}
		}

		[Test]
		public void Concat()
		{
			using (var s = OpenSession())
			using (s.BeginTransaction())
			{
				var name = s.QueryOver<Person>()
					.Where(p => p.Name == "p1")
					.Select(p => Projections.Concat(p.Name, ", ", p.Name))
					.SingleOrDefault<string>();

				name.Should().Be("p1, p1");
			}
		}

		[Test]
		public void MonthPartEqualsDayPart()
		{
			using (var s = OpenSession())
			using (s.BeginTransaction())
			{
				var persons = s.QueryOver<Person>()
					.Where(p => p.BirthDate.MonthPart() == p.BirthDate.DayPart())
					.List();

				persons.Count.Should().Be(1);
				persons[0].Name.Should().Be("p2");
			}
		}

		[Test]
		public void OrderByYearPart()
		{
			using (var s = OpenSession())
			using (s.BeginTransaction())
			{
				var persons = s.QueryOver<Person>()
					.OrderBy(p => p.BirthDate.YearPart()).Desc
					.List();

				persons.Count.Should().Be(3);
				persons[0].Name.Should().Be("p1");
				persons[1].Name.Should().Be("p2");
				persons[2].Name.Should().Be("pP3");
			}
		}

		[Test]
		public void YearEqual()
		{
			using (var s = OpenSession())
			using (s.BeginTransaction())
			{
				var persons = s.QueryOver<Person>()
					.Where(p => p.BirthDate.Year == 2008)
					.List();

				persons.Count.Should().Be(1);
				persons[0].Name.Should().Be("p2");
			}
		}

		[Test]
		public void YearIsIn()
		{
			using (var s = OpenSession())
			using (s.BeginTransaction())
			{
				var persons = s.QueryOver<Person>()
					.Where(p => p.BirthDate.Year.IsIn(new[] { 2008, 2009 }))
					.OrderBy(p => p.Name).Asc
					.List();

				persons.Count.Should().Be(2);
				persons[0].Name.Should().Be("p1");
				persons[1].Name.Should().Be("p2");
			}
		}

		[Test]
		public void YearSingleOrDefault()
		{
			using (var s = OpenSession())
			using (s.BeginTransaction())
			{
				var yearOfBirth = s.QueryOver<Person>()
					.Where(p => p.Name == "p2")
					.Select(p => p.BirthDate.Year)
					.SingleOrDefault<object>();

				yearOfBirth.GetType().Should().Be(typeof(int));
				yearOfBirth.Should().Be(2008);
			}
		}

		[Test]
		public void SelectAvgYear()
		{
			using (var s = OpenSession())
			using (s.BeginTransaction())
			{
				var avgYear = s.QueryOver<Person>()
					.SelectList(list => list.SelectAvg(p => p.BirthDate.Year))
					.SingleOrDefault<object>();

				avgYear.GetType().Should().Be(typeof(double));
				string.Format("{0:0}", avgYear).Should().Be("2008");
			}
		}

		[Test]
		public void OrderByYear()
		{
			using (var s = OpenSession())
			using (s.BeginTransaction())
			{
				var persons = s.QueryOver<Person>()
					.OrderBy(p => p.BirthDate.Year).Desc
					.List();

				persons.Count.Should().Be(3);
				persons[0].Name.Should().Be("p1");
				persons[1].Name.Should().Be("p2");
				persons[2].Name.Should().Be("pP3");
			}
		}

		[Test]
		public void MonthEqualsDay()
		{
			using (var s = OpenSession())
			using (s.BeginTransaction())
			{
				var persons = s.QueryOver<Person>()
					.Where(p => p.BirthDate.Month == p.BirthDate.Day)
					.List();

				persons.Count.Should().Be(1);
				persons[0].Name.Should().Be("p2");
			}
		}
	}
}
