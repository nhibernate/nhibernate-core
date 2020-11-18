using System;
using System.Collections.Generic;
using System.Reflection;
using NHibernate.Cache;
using NHibernate.Stat;
using NHibernate.Test.CacheTest.Caches;
using NUnit.Framework;
using NHCfg = NHibernate.Cfg;

namespace NHibernate.Test.NHSpecificTest.GH2552
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override string CacheConcurrencyStrategy => null;

		protected override void Configure(NHCfg.Configuration configuration)
		{
			configuration.SetProperty(NHCfg.Environment.UseSecondLevelCache, "true");
			configuration.SetProperty(NHCfg.Environment.GenerateStatistics, "true");
		}

		protected override void OnTearDown()
		{
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				s.CreateQuery("delete from DetailsByFK").ExecuteUpdate();
				s.CreateQuery("delete from PersonByFK").ExecuteUpdate();
				s.CreateQuery("delete from DetailsByRef").ExecuteUpdate();
				s.CreateQuery("delete from PersonByRef").ExecuteUpdate();

				tx.Commit();
			}

			Sfi.Evict(typeof(PersonByFK));
			Sfi.Evict(typeof(DetailsByFK));
			Sfi.Evict(typeof(PersonByRef));
			Sfi.Evict(typeof(DetailsByRef));
		}

		private void OneToOneFetchTest<TPerson, TDetails>() where TPerson : Person, new() where TDetails : Details, new()
		{
			List<object> ids = this.CreatePersonAndDetails<TPerson, TDetails>();

			IStatistics statistics = Sfi.Statistics;

			// Clear the second level cache and the statistics
			Sfi.EvictEntity(typeof(TPerson).FullName);
			Sfi.EvictEntity(typeof(TDetails).FullName);
			Sfi.EvictQueries();

			statistics.Clear();

			// Fill the empty caches with data.
			this.FetchPeopleById<TPerson>(ids);

			// Verify that no data was retrieved from the cache.
			Assert.AreEqual(0, statistics.SecondLevelCacheHitCount, "Second level cache hit count");

			statistics.Clear();

			this.FetchPeopleById<TPerson>(ids);

			Assert.AreEqual(0, statistics.SecondLevelCacheMissCount, "Second level cache miss count");
		}

		private void OneToOneUpdateTest<TPerson, TDetails>() where TPerson : Person, new() where TDetails : Details, new()
		{
			List<object> ids = this.CreatePersonAndDetails<TPerson, TDetails>();

			IStatistics statistics = Sfi.Statistics;

			// Clear the second level cache and the statistics
			Sfi.EvictEntity(typeof(TPerson).FullName);
			Sfi.EvictEntity(typeof(TDetails).FullName);
			Sfi.EvictQueries();

			statistics.Clear();

			// Fill the empty caches with data.
			this.FetchPeopleById<TPerson>(ids);

			// Verify that no data was retrieved from the cache.
			Assert.AreEqual(0, statistics.SecondLevelCacheHitCount, "Second level cache hit count");
			statistics.Clear();

			int personId = DeleteDetailsFromFirstPerson<TPerson>();

			// Verify that the cache was updated
			Assert.AreEqual(1, statistics.SecondLevelCachePutCount, "Second level cache put count");
			statistics.Clear();

			// Verify that the Person was updated in the cache
			using (ISession s = Sfi.OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				TPerson person = s.Get<TPerson>(personId);

				Assert.IsNull(person.Details);
			}

			Assert.AreEqual(0, statistics.SecondLevelCacheMissCount, "Second level cache miss count");
			statistics.Clear();

			// Verify that the Details was removed from the cache and deleted.
			using (ISession s = Sfi.OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				TDetails details = s.Get<TDetails>(personId);

				Assert.Null(details);
			}

			Assert.AreEqual(0, statistics.SecondLevelCacheHitCount, "Second level cache hit count");
		}

		private int DeleteDetailsFromFirstPerson<TPerson>() where TPerson:Person
		{
			using (ISession s = Sfi.OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				// Get the first person with details.
				Person person = s.QueryOver<TPerson>()
					.Where(p => p.Details != null)
					.Take(1)
					.SingleOrDefault();

				Assert.NotNull(person);
				Assert.NotNull(person.Details);

				s.SaveOrUpdate(person);
				person.Details = null;

				tx.Commit();

				return person.Id;
			}
		}

		private List<object> CreatePersonAndDetails<TPerson, TDetails>() where TPerson : Person, new() where TDetails : Details, new()
		{
			List<object> ids = new List<object>();

			using (ISession s = Sfi.OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				for (int i = 0; i < 6; i++)
				{
					Person person = new TPerson();

					if (i % 2 == 0)
					{
						Details details = new TDetails();

						details.Data = String.Format("{0}{1}", typeof(TDetails).Name, i);

						person.Details = details;
					}

					person.Name = String.Format("{0}{1}", typeof(TPerson).Name, i);

					ids.Add(s.Save(person));
				}

				tx.Commit();
			}

			return ids;
		}

		public IList<TPerson> FetchPeopleById<TPerson>(List<object> ids) where TPerson : Person
		{
			IList<TPerson> people = new List<TPerson>();

			using (ISession s = Sfi.OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				foreach (object id in ids)
				{
					people.Add(s.Get<TPerson>(id));
				}

				tx.Commit();
			}

			return people;
		}

		[Test]
		public void OneToOneCacheFetchByForeignKey()
		{
			OneToOneFetchTest<PersonByFK, DetailsByFK>();
		}

		[Test]
		public void OneToOneCacheFetchByRef()
		{
			OneToOneFetchTest<PersonByRef, DetailsByRef>();
		}

		[Test]
		public void OneToOneCacheUpdateByForeignKey()
		{
			OneToOneUpdateTest<PersonByFK, DetailsByFK>();
		}

		[Test]
		public void OneToOneCacheUpdateByRef()
		{
			OneToOneUpdateTest<PersonByRef, DetailsByRef>();
		}
	}
}
