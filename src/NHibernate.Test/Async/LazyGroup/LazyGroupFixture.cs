﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate.Cache;
using NHibernate.Cfg;
using NUnit.Framework;

namespace NHibernate.Test.LazyGroup
{
	using System.Threading;
	[TestFixture]
	public class LazyGroupFixtureAsync : TestCase
	{
		protected override string CacheConcurrencyStrategy => "nonstrict-read-write";

		protected override string MappingsAssembly => "NHibernate.Test";

		protected override string[] Mappings => new[] { "LazyGroup.Mappings.hbm.xml" };

		protected override void Configure(Configuration configuration)
		{
			configuration.Properties[Environment.CacheProvider] = typeof(HashtableCacheProvider).AssemblyQualifiedName;
			configuration.Properties[Environment.UseSecondLevelCache] = "true";
			configuration.Properties[Environment.GenerateStatistics] = "true";
		}

		protected override void OnSetUp()
		{
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				for (var i = 1; i <= 5; i++)
				{
					var person = GeneratePerson(i);
					s.Save(person);
				}

				tx.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				s.CreateQuery("delete from Person").ExecuteUpdate();
				tx.Commit();
			}
		}

		[Test]
		public async Task TestGroupsAsync()
		{
			using (var s = OpenSession())
			{
				var person = await (s.GetAsync<Person>(1));
				Assert.That(NHibernateUtil.IsPropertyInitialized(person, "Name"), Is.True);
				Assert.That(NHibernateUtil.IsPropertyInitialized(person, "NickName"), Is.False);
				Assert.That(NHibernateUtil.IsPropertyInitialized(person, "Address"), Is.False);
				Assert.That(NHibernateUtil.IsPropertyInitialized(person, "Image"), Is.False);
				Assert.That(NHibernateUtil.IsPropertyInitialized(person, "Age"), Is.False);

				var nickName = person.NickName;
				Assert.That(NHibernateUtil.IsPropertyInitialized(person, "NickName"), Is.True);
				Assert.That(NHibernateUtil.IsPropertyInitialized(person, "Address"), Is.False);
				Assert.That(NHibernateUtil.IsPropertyInitialized(person, "Image"), Is.False);
				Assert.That(NHibernateUtil.IsPropertyInitialized(person, "Age"), Is.False);
				Assert.That(nickName, Is.EqualTo("NickName1"));

				var address = person.Address;
				Assert.That(NHibernateUtil.IsPropertyInitialized(person, "Address"), Is.True);
				Assert.That(NHibernateUtil.IsPropertyInitialized(person, "Image"), Is.False);
				Assert.That(NHibernateUtil.IsPropertyInitialized(person, "Age"), Is.False);
				Assert.That(address.City, Is.EqualTo("City1"));
				Assert.That(address.Street, Is.EqualTo("Street1"));
				Assert.That(address.PostCode, Is.EqualTo(1001));

				var image = person.Image;
				Assert.That(NHibernateUtil.IsPropertyInitialized(person, "Image"), Is.True);
				Assert.That(NHibernateUtil.IsPropertyInitialized(person, "Age"), Is.False);
				Assert.That(person.Image, Has.Length.EqualTo(1));

				var age = person.Age;
				Assert.That(NHibernateUtil.IsPropertyInitialized(person, "Age"), Is.True);
				Assert.That(person.Age, Is.EqualTo(1));
			}
		}

		[TestCase(true)]
		[TestCase(false)]
		public async Task TestUpdateAsync(bool fetchBeforeUpdate)
		{
			Sfi.Statistics.Clear();

			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var person = await (s.GetAsync<Person>(1));
				if (fetchBeforeUpdate)
				{
					var nickName = person.NickName;
				}

				person.NickName = "test";

				await (tx.CommitAsync());
			}

			Assert.That(Sfi.Statistics.EntityUpdateCount, Is.EqualTo(1));

			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var person = await (s.GetAsync<Person>(1));
				Assert.That(person.NickName, Is.EqualTo("test"));

				person.NickName = "NickName1"; // reset name

				await (tx.CommitAsync());
			}
		}

		[Test]
		public async Task TestCacheAsync()
		{
			var persister = Sfi.GetEntityPersister(typeof(Person).FullName);
			var cache = (HashtableCache) persister.Cache.Cache;
			await (cache.ClearAsync(CancellationToken.None));

			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var person = await (s.GetAsync<Person>(1));

				var nickName = person.NickName;
				Assert.That(NHibernateUtil.IsPropertyInitialized(person, "NickName"), Is.True);
				Assert.That(NHibernateUtil.IsPropertyInitialized(person, "Address"), Is.False);
				Assert.That(NHibernateUtil.IsPropertyInitialized(person, "Image"), Is.False);
				Assert.That(NHibernateUtil.IsPropertyInitialized(person, "Age"), Is.False);
				Assert.That(nickName, Is.EqualTo("NickName1"));
				
				await (tx.CommitAsync());
			}

			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var person = await (s.GetAsync<Person>(1));
				Assert.That(NHibernateUtil.IsPropertyInitialized(person, "NickName"), Is.True);
				Assert.That(NHibernateUtil.IsPropertyInitialized(person, "Address"), Is.False);
				Assert.That(NHibernateUtil.IsPropertyInitialized(person, "Image"), Is.False);
				Assert.That(NHibernateUtil.IsPropertyInitialized(person, "Age"), Is.False);
				Assert.That(person.NickName, Is.EqualTo("NickName1"));

				await (tx.CommitAsync());
			}
		}

		[Test]
		public async Task TestInitializeFromCacheAsync()
		{
			var persister = Sfi.GetEntityPersister(typeof(Person).FullName);
			var cache = (HashtableCache) persister.Cache.Cache;
			await (cache.ClearAsync(CancellationToken.None));
			Sfi.Statistics.Clear();

			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var person = await (s.GetAsync<Person>(1));

				await (InitializeImageAsync());

				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(2));

				var image = person.Image; // Should be initialized from cache

				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(2));
				Assert.That(NHibernateUtil.IsPropertyInitialized(person, "NickName"), Is.False);
				Assert.That(NHibernateUtil.IsPropertyInitialized(person, "Address"), Is.False);
				Assert.That(NHibernateUtil.IsPropertyInitialized(person, "Image"), Is.True);
				Assert.That(NHibernateUtil.IsPropertyInitialized(person, "Age"), Is.False);
				Assert.That(image, Has.Length.EqualTo(1));

				await (tx.CommitAsync());
			}
		}

		private async Task InitializeImageAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var person = await (s.GetAsync<Person>(1, cancellationToken));
				var image = person.Image;

				await (tx.CommitAsync(cancellationToken));
			}
		}

		private static Person GeneratePerson(int i)
		{
			return new Person
			{
				Id = i,
				Name = $"Person{i}",
				Address = new Address
				{
					City = $"City{i}",
					PostCode = 1000+i,
					Street = $"Street{i}"
				},
				Image = new byte[i],
				NickName = $"NickName{i}"
			};
		}
	}
}
