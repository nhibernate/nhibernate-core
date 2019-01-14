using System;
using System.Collections.Generic;
using System.Diagnostics;
using NHibernate.Criterion;
using NUnit.Framework;

namespace NHibernate.Test.Criteria.Lambda
{
	[TestFixture, Explicit]
	public class CriteriaGenerationBenchmarks : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override string[] Mappings
		{
			get { return new[] { "Criteria.Lambda.Mappings.hbm.xml" }; }
		}

		protected override void OnSetUp()
		{
		}

		protected override void OnTearDown()
		{
		}
		
		//NH-1200 - Exception occurs when using criteria exist queries
		[Test, Explicit]
		public void Subquery()
		{
			using (var s = OpenSession())
			{
				Child _subqueryChildAlias = null;

				Person person = null;
				ICriteria criteria = s.QueryOver<Person>(() => person)
						.WithSubquery.WhereExists(
						QueryOver.Of<Child>(() => _subqueryChildAlias)
							.Where(() => _subqueryChildAlias.Parent.Id == person.Id).Select(c => c.Id)).UnderlyingCriteria;

				BenchQuery(s, criteria);
			}
		}

		//NH-1200 - Exception occurs when using criteria exist queries
		[Test, Explicit]
		public void SubqueryManyParamsFromOuterQuery()
		{
			using (var s = OpenSession())
			{
				Child _subqueryChildAlias = null;

				Person person = null;
				ICriteria criteria = s.QueryOver<Person>(() => person)
						.WithSubquery.WhereExists(
						QueryOver.Of<Child>(() => _subqueryChildAlias)
							.Where(() => _subqueryChildAlias.Age > person.Age && person.Age > 40 && person.Name == "name").Select(c => c.Id)).UnderlyingCriteria;

				BenchQuery(s, criteria);
			}
		}

		[Test, Explicit]
		public void SimplePropertyCompare()
		{
			using (var s = OpenSession())
			{
				ICriteria criteria = s.QueryOver<Person>()
						.Where(p => p.Name == "aaa").UnderlyingCriteria;
				BenchQuery(s, criteria);
			}
		}

		[Test, Explicit]
		public void ManyAliases()
		{
			using (var s = OpenSession())
			{
				Child child = null;
				Person father = null;
				Person person = null;
				ICriteria criteria = s.QueryOver<Person>(() => person)
					.JoinAlias(p => p.Children, () => child)
					.JoinAlias(p => p.Father, () => father)
						.Where(p => p.Name == father.Name && p.Father.Id == 10 && child.Nickname == "nickname" && child.Age > person.Age).UnderlyingCriteria;

				BenchQuery(s, criteria);

			}
		}

		private static void BenchQuery(ISession s, ICriteria criteria)
		{
			const int iterations = 15000;

			var commands = new List<SqlCommand.ISqlCommand>(iterations);

			for (int j = 0; j < 5; j++)
			{
				using (Timer.Start)
				for (int i = 0; i < iterations; i++)
				{
					var batchItem = new Multi.CriteriaBatchItem<Person>(criteria);
					batchItem.Init(s.GetSessionImplementation());
					commands.AddRange(batchItem.GetCommands());
				}
				Console.WriteLine("Elapsed time (ms): " + Timer.ElapsedMilliseconds);
			}
		}

		/// <summary>
		/// Stopwatch wrapper
		/// </summary>
		class Timer : IDisposable
		{
			static Stopwatch stop = new Stopwatch();

			public Timer()
			{
				stop.Reset();
				stop.Start();
			}

			public static Timer Start { get { return new Timer(); } }

			public void Dispose()
			{
				stop.Stop();
			}

			static public long ElapsedMilliseconds { get { return stop.ElapsedMilliseconds; } }
		}
	}
}
