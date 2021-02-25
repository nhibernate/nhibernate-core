using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate.Cache;
using NHibernate.Cfg;
using NUnit.Framework;

namespace NHibernate.Test.LazyGroup
{
	[TestFixture]
	public class LazyGroupFixture : TestCase
	{
		protected override string MappingsAssembly => "NHibernate.Test";

		protected override string[] Mappings => new[] { "LazyGroup.Mappings.hbm.xml" };

		protected override void Configure(Configuration configuration)
		{
			base.Configure(configuration);
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
		public void TestGroups()
		{
			using (var s = OpenSession())
			{
				var person = s.Get<Person>(1);
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
		public void TestUpdate(bool fetchBeforeUpdate)
		{
			Sfi.Statistics.Clear();

			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var person = s.Get<Person>(1);
				if (fetchBeforeUpdate)
				{
					var nickName = person.NickName;
				}

				person.NickName = "test";

				tx.Commit();
			}

			Assert.That(Sfi.Statistics.EntityUpdateCount, Is.EqualTo(1));

			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var person = s.Get<Person>(1);
				Assert.That(person.NickName, Is.EqualTo("test"));

				person.NickName = "NickName1"; // reset name

				tx.Commit();
			}
		}

		[Test]
		public void TestCache()
		{
			var persister = Sfi.GetEntityPersister(typeof(Person).FullName);
			var cache = (HashtableCache) persister.Cache.Cache;
			cache.Clear();

			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var person = s.Get<Person>(1);

				var nickName = person.NickName;
				Assert.That(NHibernateUtil.IsPropertyInitialized(person, "NickName"), Is.True);
				Assert.That(NHibernateUtil.IsPropertyInitialized(person, "Address"), Is.False);
				Assert.That(NHibernateUtil.IsPropertyInitialized(person, "Image"), Is.False);
				Assert.That(NHibernateUtil.IsPropertyInitialized(person, "Age"), Is.False);
				Assert.That(nickName, Is.EqualTo("NickName1"));
				
				tx.Commit();
			}

			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var person = s.Get<Person>(1);
				Assert.That(NHibernateUtil.IsPropertyInitialized(person, "NickName"), Is.True);
				Assert.That(NHibernateUtil.IsPropertyInitialized(person, "Address"), Is.False);
				Assert.That(NHibernateUtil.IsPropertyInitialized(person, "Image"), Is.False);
				Assert.That(NHibernateUtil.IsPropertyInitialized(person, "Age"), Is.False);
				Assert.That(person.NickName, Is.EqualTo("NickName1"));

				tx.Commit();
			}
		}

		[Test]
		public void TestInitializeFromCache()
		{
			var persister = Sfi.GetEntityPersister(typeof(Person).FullName);
			var cache = (HashtableCache) persister.Cache.Cache;
			cache.Clear();
			Sfi.Statistics.Clear();

			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var person = s.Get<Person>(1);

				InitializeImage();

				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(2));

				var image = person.Image; // Should be initialized from cache

				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(2));
				Assert.That(NHibernateUtil.IsPropertyInitialized(person, "NickName"), Is.False);
				Assert.That(NHibernateUtil.IsPropertyInitialized(person, "Address"), Is.False);
				Assert.That(NHibernateUtil.IsPropertyInitialized(person, "Image"), Is.True);
				Assert.That(NHibernateUtil.IsPropertyInitialized(person, "Age"), Is.False);
				Assert.That(image, Has.Length.EqualTo(1));

				tx.Commit();
			}
		}

		private void InitializeImage()
		{
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var person = s.Get<Person>(1);
				var image = person.Image;

				tx.Commit();
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
