using System;
using System.Collections;
using System.Linq;
using NUnit.Framework;

namespace NHibernate.Test.Polymorphy
{
	public class PolymorphicQueryTest: TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get { return new[] { "Polymorphy.Mappings.hbm.xml" }; }
		}

		public class ScenarioWithGraphA : IDisposable
		{
			private readonly ISessionFactory factory;
			private readonly GraphA[] aInstances;
			
			public ScenarioWithGraphA(ISessionFactory factory, int count = 1)
			{
				this.factory = factory;
				this.aInstances = new GraphA[count];

				using (var s = factory.OpenSession())
				{
					for (int i = 0; i < count; i++)
					{
						aInstances[i] = new GraphA { Name = "Patrick" + (i > 0 ? " " + (i + 1) : "") };
						s.Save(aInstances[i]);
					}
					s.Flush();
				}
			}

			public GraphA[] As
			{
				get { return aInstances; }
			}

			public void Dispose()
			{
				using (var s = factory.OpenSession())
				{
					foreach (var a in As)
					{
						s.Delete(a);
					}
					s.Flush();
				}
			}
		}

		public class ScenarioWithGraphB : IDisposable
		{
			private readonly ISessionFactory factory;
			private readonly GraphB[] bs;

			public ScenarioWithGraphB(ISessionFactory factory, int count = 1)
			{
				this.factory = factory;
				this.bs = new GraphB[count];
				using (var s = factory.OpenSession())
				{
					for (int i = 0; i < count; i++)
					{
						bs[i] = new GraphB { Name = "Patrick" + (i > 0 ? " " + (i + 1) : "") };
						s.Save(bs[i]);
					}
					s.Flush();
				}
			}

			public GraphB[] Bs
			{
				get { return bs; }
			}

			public void Dispose()
			{
				using (var s = factory.OpenSession())
				{
					foreach (var b in Bs)
					{
						s.Delete(b);
					}
					s.Flush();
				}
			}
		}

		[Test]
		public void WhenFindUsingInterfaceThenNotThrows()
		{
			using (var scenarioA = new ScenarioWithGraphA(Sfi))
			using (var scenarioB = new ScenarioWithGraphB(Sfi))
			{
				using (var s = OpenSession())
				{
					Assert.That(() => s.CreateQuery("select ROOT from NHibernate.Test.Polymorphy.IMultiGraphNamed AS ROOT WHERE 1=1").SetMaxResults(1).List(), Throws.Nothing);
				}
			}
		}

		[Test]
		public void WhenFindUsingInterfaceThenPagingWorks()
		{
			using (var scenarioA = new ScenarioWithGraphA(Sfi, 10))
			using (var scenarioB = new ScenarioWithGraphB(Sfi, 10))
			{
				using (var s = OpenSession())
				{
					const string queryString = "select ROOT from NHibernate.Test.Polymorphy.IMultiGraphNamed AS ROOT WHERE 1=1";
					Assert.That(s.CreateQuery(queryString).SetMaxResults(1).List().Count, Is.EqualTo(1)); // because only first result requested
					Assert.That(s.CreateQuery(queryString).SetMaxResults(10).List().Count, Is.EqualTo(10)); // because 10 first results requested
					Assert.That(s.CreateQuery(queryString).SetMaxResults(10).List().Cast<object>().Distinct().Count(), Is.EqualTo(10)); // because the results must be unique
					Assert.That(s.CreateQuery(queryString).SetMaxResults(10).List().Cast<object>().Select(o => o.GetType()).Distinct().Count(), Is.EqualTo(1)); // because one persister already leads to 10 results
					Assert.That(s.CreateQuery(queryString).SetMaxResults(10).SetFirstResult(5).List().Count, Is.EqualTo(10)); // because 10 after the first 5 results out of 20 requested
					Assert.That(s.CreateQuery(queryString).SetMaxResults(10).SetFirstResult(5).List().Cast<object>().Select(o => o.GetType()).Distinct().Count(), Is.EqualTo(2)); // because both persisters are used
					Assert.That(s.CreateQuery(queryString).SetMaxResults(10).SetFirstResult(15).List().Count, Is.EqualTo(5)); // because 10 after the first 15 results out of 20 requested
					Assert.That(s.CreateQuery(queryString).SetMaxResults(10).SetFirstResult(20).List().Count, Is.EqualTo(0)); // because exactly 20 rows should be skipped
					Assert.That(s.CreateQuery(queryString).SetMaxResults(10).SetFirstResult(1000).List().Count, Is.EqualTo(0)); // because more than 20 rows should be skipped
					Assert.That(s.CreateQuery(queryString).SetFirstResult(5).List().Count, Is.EqualTo(15)); // because all after the first 5 results out of 20 requested
					Assert.That(s.CreateQuery(queryString).SetFirstResult(15).List().Count, Is.EqualTo(5)); // because all after the first 15 results out of 20 requested
					Assert.That(s.CreateQuery(queryString).SetFirstResult(20).List().Count, Is.EqualTo(0)); // because exactly 20 rows should be skipped
					Assert.That(s.CreateQuery(queryString).SetFirstResult(1000).List().Count, Is.EqualTo(0)); // because more than 20 rows should be skipped
				}
			}
		}

		[Test]
		public void WhenSelectValueTypeUsingInterfaceThenNotThrows()
		{
			using (var scenarioA = new ScenarioWithGraphA(Sfi))
			using (var scenarioB = new ScenarioWithGraphB(Sfi))
			{
				using (var s = OpenSession())
				{
					Assert.That(() => s.CreateQuery("select ROOT.Id from NHibernate.Test.Polymorphy.IMultiGraphNamed AS ROOT WHERE 1=1").SetMaxResults(1).List(), Throws.Nothing);
				}
			}
		}

		[Test]
		public void WhenSelectValueTypeUsingInterfaceThenPagingWorks()
		{
			using (var scenarioA = new ScenarioWithGraphA(Sfi, 10))
			using (var scenarioB = new ScenarioWithGraphB(Sfi, 10))
			{
				using (var s = OpenSession())
				{
					const string queryString = "select ROOT.Id from NHibernate.Test.Polymorphy.IMultiGraphNamed AS ROOT WHERE 1=1";
					Assert.That(s.CreateQuery(queryString).SetMaxResults(1).List().Count, Is.EqualTo(1)); // because only first result requested
					Assert.That(s.CreateQuery(queryString).SetMaxResults(10).List().Count, Is.EqualTo(10)); // because 10 first results requested
					Assert.That(s.CreateQuery(queryString).SetMaxResults(10).List().Cast<object>().Distinct().Count(), Is.EqualTo(10)); // because the results must be unique
					Assert.That(s.CreateQuery(queryString).SetMaxResults(10).SetFirstResult(5).List().Count, Is.EqualTo(10)); // because 10 after the first 5 results out of 20 requested
					Assert.That(s.CreateQuery(queryString).SetMaxResults(10).SetFirstResult(15).List().Count, Is.EqualTo(5)); // because 10 after the first 15 results out of 20 requested
					Assert.That(s.CreateQuery(queryString).SetMaxResults(10).SetFirstResult(20).List().Count, Is.EqualTo(0)); // because exactly 20 rows should be skipped
					Assert.That(s.CreateQuery(queryString).SetMaxResults(10).SetFirstResult(1000).List().Count, Is.EqualTo(0)); // because more than 20 rows should be skipped
				}
			}
		}
	}
}