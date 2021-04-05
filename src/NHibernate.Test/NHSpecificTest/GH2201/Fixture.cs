using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Stat;
using NUnit.Framework;
using NHCfg = NHibernate.Cfg;

namespace NHibernate.Test.NHSpecificTest.GH2201
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void Configure(NHCfg.Configuration configuration)
		{
			configuration.SetProperty(NHCfg.Environment.GenerateStatistics, "true");
		}

		protected override void OnTearDown()
		{
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				s.Delete("from Person");

				tx.Commit();
			}
		}

		protected override void OnSetUp()
		{
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				string[] names = { "Alice", "Bob" };

				for (int i = 0; i < names.Length; i++)
				{
					var name = names[i];

					Person parent = new Person()
					{
						Name = name,
						Details = new Detail()
						{
							Data = $"Details for ${name}"
						}
					};

					for (int j = 1; j <= 3; j++)
					{
						Person child = new Person()
						{
							Name = $"Child ${j} of ${parent.Name}",
							Parent = parent,
							Details = new Detail()
							{
								Data = $"Details for child ${j} of ${name}"
							}
						};

						parent.Children.Add(child);
					}

					s.Save(parent);
				}

				tx.Commit();
			}
		}

		[Test]
		public void QueryOverPersonWithParent()
		{
			var stats = Sfi.Statistics;

			stats.Clear();

			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var people = s.QueryOver<Person>()
					.Fetch(SelectMode.Fetch,p => p.Parent)
					.Where(p => p.Parent != null)
					.List();

				foreach (Person p in people)
				{
					Assert.That(p.Parent, Is.Not.Null);
					Assert.That(p.Parent.Details, Is.Not.Null);
				}

				Assert.That(people.Count, Is.EqualTo(6));
				Assert.That(stats.QueryExecutionCount, Is.EqualTo(1));
				Assert.That(stats.EntityFetchCount, Is.EqualTo(0));
				Assert.That(stats.EntityLoadCount, Is.EqualTo(16));
			}
		}

		[Test]
		public void QueryOverSinglePersonWithParent()
		{
			var stats = Sfi.Statistics;

			stats.Clear();

			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var person = s.QueryOver<Person>()
					.Where(p => p.Parent != null)
					.Fetch(SelectMode.Fetch, p => p.Parent)
					.Take(1)
					.SingleOrDefault();

				Assert.That(person, Is.Not.Null);
				Assert.That(stats.QueryExecutionCount, Is.EqualTo(1));
				Assert.That(stats.EntityFetchCount, Is.EqualTo(0));
				Assert.That(stats.EntityLoadCount, Is.EqualTo(4));
			}
		}
	}
}
