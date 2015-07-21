using System;
using System.Collections;
using NHibernate.Criterion;
using NUnit.Framework;

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

				Assert.That(persons.Count, Is.EqualTo(1));
				Assert.That(persons[0].Name, Is.EqualTo("p2"));
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

				Assert.That(persons.Count, Is.EqualTo(2));
				Assert.That(persons[0].Name, Is.EqualTo("p1"));
				Assert.That(persons[1].Name, Is.EqualTo("p2"));
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

				Assert.That(yearOfBirth.GetType(), Is.EqualTo(typeof (int)));
				Assert.That(yearOfBirth, Is.EqualTo(2008));
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

				Assert.That(avgYear.GetType(), Is.EqualTo(typeof (double)));
				Assert.That(string.Format("{0:0}", avgYear), Is.EqualTo("2008"));
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
					.Select(p => Math.Round(p.Age.Sqrt(), 2))
					.SingleOrDefault<object>();

				Assert.That(sqrtOfAge, Is.InstanceOf<double>());
				Assert.That(string.Format("{0:0.00}", sqrtOfAge), Is.EqualTo((9.49).ToString()));
			}
		}


		[Test]
		public void RoundDoubleWithOneArgument()
		{
			using (var s = OpenSession())
			using (s.BeginTransaction())
			{
				var roundedValue = s.QueryOver<Person>()
									.Where(p => p.Name == "p1")
									.Select(p => Math.Round(p.Age.Sqrt()))
									.SingleOrDefault<object>();

				Assert.That(roundedValue, Is.InstanceOf<double>());
				Assert.That(roundedValue, Is.EqualTo(9));
			}
		}

		[Test]
		public void RoundDecimalWithOneArgument()
		{
			using (var s = OpenSession())
			using (s.BeginTransaction())
			{
				var roundedValue = s.QueryOver<Person>()
									.Where(p => p.Name == "p1")
									.Select(p => Math.Round((decimal) p.Age.Sqrt()))
									.SingleOrDefault<object>();

				Assert.That(roundedValue, Is.InstanceOf<double>());
				Assert.That(roundedValue, Is.EqualTo(9));
			}
		}

		[Test]
		public void RoundDoubleWithTwoArguments()
		{
			using (var s = OpenSession())
			using (s.BeginTransaction())
			{
				var roundedValue = s.QueryOver<Person>()
									.Where(p => p.Name == "p1")
									.Select(p => Math.Round(p.Age.Sqrt() , 3))
									.SingleOrDefault<object>();

				Assert.That(roundedValue, Is.InstanceOf<double>());
				Assert.That(roundedValue, Is.EqualTo(9.487).Within(0.000001));
			}
		}

		[Test]
		public void RoundDecimalWithTwoArguments()
		{
			using (var s = OpenSession())
			using (s.BeginTransaction())
			{
				var roundedValue = s.QueryOver<Person>()
									.Where(p => p.Name == "p1")
									.Select(p => Math.Round((decimal) p.Age.Sqrt(), 3))
									.SingleOrDefault<object>();

				Assert.That(roundedValue, Is.InstanceOf<double>());
				Assert.That(roundedValue, Is.EqualTo(9.487).Within(0.000001));
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

				Assert.That(names[0], Is.EqualTo("pp3"));
				Assert.That(names[1], Is.EqualTo("PP3"));
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

				Assert.That(name, Is.EqualTo("p1, p1"));
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

				Assert.That(persons.Count, Is.EqualTo(1));
				Assert.That(persons[0].Name, Is.EqualTo("p2"));
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

				Assert.That(persons.Count, Is.EqualTo(3));
				Assert.That(persons[0].Name, Is.EqualTo("p1"));
				Assert.That(persons[1].Name, Is.EqualTo("p2"));
				Assert.That(persons[2].Name, Is.EqualTo("pP3"));
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

				Assert.That(persons.Count, Is.EqualTo(1));
				Assert.That(persons[0].Name, Is.EqualTo("p2"));
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

				Assert.That(persons.Count, Is.EqualTo(2));
				Assert.That(persons[0].Name, Is.EqualTo("p1"));
				Assert.That(persons[1].Name, Is.EqualTo("p2"));
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

				Assert.That(yearOfBirth.GetType(), Is.EqualTo(typeof(int)));
				Assert.That(yearOfBirth, Is.EqualTo(2008));
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

				Assert.That(avgYear.GetType(), Is.EqualTo(typeof(double)));
				Assert.That(string.Format("{0:0}", avgYear), Is.EqualTo("2008"));
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

				Assert.That(persons.Count, Is.EqualTo(3));
				Assert.That(persons[0].Name, Is.EqualTo("p1"));
				Assert.That(persons[1].Name, Is.EqualTo("p2"));
				Assert.That(persons[2].Name, Is.EqualTo("pP3"));
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

				Assert.That(persons.Count, Is.EqualTo(1));
				Assert.That(persons[0].Name, Is.EqualTo("p2"));
			}
		}
	}
}
