using System;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NHibernate.Proxy;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH2067
{
	[TestFixture]
	public class Fixture : TestCaseMappingByCode
	{
		private object domesticCatId;
		private object catId;

		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<Cat>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
				rc.Property(x => x.Name);
			});

			mapper.JoinedSubclass<DomesticCat>(rc =>
			{
				rc.Proxy(typeof(IDomesticCat));
				rc.Property(x => x.OwnerName);
			});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				catId = session.Save(new Cat { Name = "Bob" });

				domesticCatId = session.Save(new DomesticCat {Name = "Tom", OwnerName = "Jerry"});

				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.CreateQuery("delete from System.Object").ExecuteUpdate();

				transaction.Commit();
			}
		}

		[Test]
		public void CanLoadDomesticCatUsingBaseClass()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var cat = session.Load<Cat>(domesticCatId);
				Assert.That(cat, Is.Not.Null);
				Assert.That(cat.Name, Is.EqualTo("Tom"));
				var domesticCat = (IDomesticCat) cat;
				Assert.That(domesticCat.Name, Is.EqualTo("Tom"));
				Assert.That(domesticCat.OwnerName, Is.EqualTo("Jerry"));
				var proxy = (INHibernateProxy) cat;
				Assert.That(proxy.HibernateLazyInitializer.PersistentClass, Is.EqualTo(typeof(Cat)));
			}
		}

		[Test]
		public void CanLoadDomesticCatUsingBaseClassInterface()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var cat = session.Load<ICat>(domesticCatId);
				Assert.That(cat, Is.Not.Null);
				Assert.That(cat.Name, Is.EqualTo("Tom"));
				var domesticCat = (IDomesticCat) cat;
				Assert.That(domesticCat.Name, Is.EqualTo("Tom"));
				Assert.That(domesticCat.OwnerName, Is.EqualTo("Jerry"));
				var proxy = (INHibernateProxy) cat;
				Assert.That(proxy.HibernateLazyInitializer.PersistentClass, Is.EqualTo(typeof(Cat)));
			}
		}

		[Test]
		public void CanLoadDomesticCatUsingInterface()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var cat = session.Load<IDomesticCat>(domesticCatId);
				Assert.That(cat, Is.Not.Null);
				Assert.That(cat.Name, Is.EqualTo("Tom"));
				Assert.That(cat.OwnerName, Is.EqualTo("Jerry"));
				var proxy = (INHibernateProxy) cat;
				Assert.That(proxy.HibernateLazyInitializer.PersistentClass, Is.EqualTo(typeof(DomesticCat)));
			}
		}

		[Test]
		public void ThrowWhenTryToLoadDomesticCatUsingSealedClass()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				Assert.Throws<InvalidCastException>(() => session.Load<DomesticCat>(domesticCatId));
			}
		}

		[Test]
		public void CanLoadDomesticCatUsingSealedClass()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var cat = (IDomesticCat) session.Load(typeof(DomesticCat), domesticCatId);
				Assert.That(cat, Is.Not.Null);
				Assert.That(cat.Name, Is.EqualTo("Tom"));
				Assert.That(cat.OwnerName, Is.EqualTo("Jerry"));
				var proxy = (INHibernateProxy) cat;
				Assert.That(proxy.HibernateLazyInitializer.PersistentClass, Is.EqualTo(typeof(DomesticCat)));
			}
		}

		[Test]
		public void CanLoadDomesticCatUsingSideInterface()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var cat = session.Load<IPet>(domesticCatId);
				Assert.That(cat, Is.Not.Null);
				Assert.That(cat.OwnerName, Is.EqualTo("Jerry"));
				var proxy = (INHibernateProxy) cat;
				Assert.That(proxy.HibernateLazyInitializer.PersistentClass, Is.EqualTo(typeof(DomesticCat)));
			}
		}

		[Test]
		public void CanLoadCatUsingClass()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var cat = session.Load<Cat>(catId);
				Assert.That(cat, Is.Not.Null);
				Assert.That(cat.Name, Is.EqualTo("Bob"));
				var proxy = (INHibernateProxy) cat;
				Assert.That(proxy.HibernateLazyInitializer.PersistentClass, Is.EqualTo(typeof(Cat)));
			}
		}
		
		[Test]
		public void CanLoadCatUsingInterface()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var cat = session.Load<ICat>(catId);
				Assert.That(cat, Is.Not.Null);
				Assert.That(cat.Name, Is.EqualTo("Bob"));
				var proxy = (INHibernateProxy) cat;
				Assert.That(proxy.HibernateLazyInitializer.PersistentClass, Is.EqualTo(typeof(Cat)));
			}
		}
	}
}
