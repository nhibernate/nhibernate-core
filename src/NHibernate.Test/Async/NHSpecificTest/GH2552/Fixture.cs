﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Stat;
using NUnit.Framework;
using NHCfg = NHibernate.Cfg;
using NHibernate.Linq;

namespace NHibernate.Test.NHSpecificTest.GH2552
{
	using System.Threading.Tasks;
	using System.Threading;
	[TestFixture]
	public class FixtureAsync : BugTestCase
	{
		protected override void Configure(NHCfg.Configuration configuration)
		{
			configuration.SetProperty(NHCfg.Environment.UseSecondLevelCache, "true");
			configuration.SetProperty(NHCfg.Environment.GenerateStatistics, "true");
			configuration.SetProperty(NHCfg.Environment.UseMinimalPuts, "true");
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

		private async Task OneToOneFetchTestAsync<TPerson, TDetails>(CancellationToken cancellationToken = default(CancellationToken)) where TPerson : Person, new() where TDetails : Details, new()
		{
			List<object> ids = await (this.CreatePersonAndDetailsAsync<TPerson, TDetails>(cancellationToken));

			IStatistics statistics = Sfi.Statistics;

			// Clear the second level cache and the statistics
			await (Sfi.EvictEntityAsync(typeof(TPerson).FullName, cancellationToken));
			await (Sfi.EvictEntityAsync(typeof(TDetails).FullName, cancellationToken));
			await (Sfi.EvictQueriesAsync(cancellationToken));

			statistics.Clear();

			// Fill the empty caches with data.
			await (this.FetchPeopleByIdAsync<TPerson>(ids, cancellationToken));

			// Verify that no data was retrieved from the cache.
			Assert.AreEqual(0, statistics.SecondLevelCacheHitCount, "Second level cache hit count");

			statistics.Clear();

			await (this.FetchPeopleByIdAsync<TPerson>(ids, cancellationToken));

			Assert.AreEqual(0, statistics.SecondLevelCacheMissCount, "Second level cache miss count");
		}

		private async Task OneToOneUpdateTestAsync<TPerson, TDetails>(CancellationToken cancellationToken = default(CancellationToken)) where TPerson : Person, new() where TDetails : Details, new()
		{
			List<object> ids = await (this.CreatePersonAndDetailsAsync<TPerson, TDetails>(cancellationToken));

			IStatistics statistics = Sfi.Statistics;

			// Clear the second level cache and the statistics
			await (Sfi.EvictEntityAsync(typeof(TPerson).FullName, cancellationToken));
			await (Sfi.EvictEntityAsync(typeof(TDetails).FullName, cancellationToken));
			await (Sfi.EvictQueriesAsync(cancellationToken));

			statistics.Clear();

			// Fill the empty caches with data.
			await (this.FetchPeopleByIdAsync<TPerson>(ids, cancellationToken));

			// Verify that no data was retrieved from the cache.
			Assert.AreEqual(0, statistics.SecondLevelCacheHitCount, "Second level cache hit count");
			statistics.Clear();

			int personId = await (DeleteDetailsFromFirstPersonAsync<TPerson>(cancellationToken));

			// Verify that the cache was updated
			Assert.AreEqual(1, statistics.SecondLevelCachePutCount, "Second level cache put count");
			statistics.Clear();

			// Verify that the Person was updated in the cache
			using (ISession s = Sfi.OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				TPerson person = await (s.GetAsync<TPerson>(personId, cancellationToken));

				Assert.IsNull(person.Details);
			}

			Assert.AreEqual(0, statistics.SecondLevelCacheMissCount, "Second level cache miss count");
			statistics.Clear();

			// Verify that the Details was removed from the cache and deleted.
			using (ISession s = Sfi.OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				TDetails details = await (s.GetAsync<TDetails>(personId, cancellationToken));

				Assert.Null(details);
			}

			Assert.AreEqual(0, statistics.SecondLevelCacheHitCount, "Second level cache hit count");
		}

		private async Task<int> DeleteDetailsFromFirstPersonAsync<TPerson>(CancellationToken cancellationToken = default(CancellationToken)) where TPerson:Person
		{
			using (ISession s = Sfi.OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				// Get the first person with details.
				Person person = await (s.Query<TPerson>()
					.Where(p => p.Details != null)
					.Take(1)
					.SingleOrDefaultAsync(cancellationToken));

				Assert.NotNull(person);
				Assert.NotNull(person.Details);

				person.Details = null;

				await (tx.CommitAsync(cancellationToken));

				return person.Id;
			}
		}

		private async Task<List<object>> CreatePersonAndDetailsAsync<TPerson, TDetails>(CancellationToken cancellationToken = default(CancellationToken)) where TPerson : Person, new() where TDetails : Details, new()
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

					ids.Add(await (s.SaveAsync(person, cancellationToken)));
				}

				await (tx.CommitAsync(cancellationToken));
			}

			return ids;
		}

		public async Task<IList<TPerson>> FetchPeopleByIdAsync<TPerson>(List<object> ids, CancellationToken cancellationToken = default(CancellationToken)) where TPerson : Person
		{
			IList<TPerson> people = new List<TPerson>();

			using (ISession s = Sfi.OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				foreach (object id in ids)
				{
					people.Add(await (s.GetAsync<TPerson>(id, cancellationToken)));
				}

				await (tx.CommitAsync(cancellationToken));
			}

			return people;
		}

		[Test]
		public async Task OneToOneCacheFetchByForeignKeyAsync()
		{
			await (OneToOneFetchTestAsync<PersonByFK, DetailsByFK>());
		}

		[Test]
		public async Task OneToOneCacheFetchByRefAsync()
		{
			await (OneToOneFetchTestAsync<PersonByRef, DetailsByRef>());
		}

		[Test]
		public async Task OneToOneCacheUpdateByForeignKeyAsync()
		{
			await (OneToOneUpdateTestAsync<PersonByFK, DetailsByFK>());
		}

		[Test]
		public async Task OneToOneCacheUpdateByRefAsync()
		{
			await (OneToOneUpdateTestAsync<PersonByRef, DetailsByRef>());
		}
	}
}
