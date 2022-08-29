using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Cache;
using NHibernate.Cfg;
using NHibernate.Hql.Ast.ANTLR;
using NHibernate.Linq;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using Environment = NHibernate.Cfg.Environment;

namespace NHibernate.Test.FetchLazyProperties
{
	[TestFixture]
	public class FetchLazyPropertiesFixture : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override string[] Mappings
		{
			get { return new[] { "FetchLazyProperties.Mappings.hbm.xml" }; }
		}

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return dialect.SupportsTemporaryTables;
		}

		protected override void Configure(Configuration configuration)
		{
			base.Configure(configuration);
			configuration.Properties[Environment.CacheProvider] = typeof(HashtableCacheProvider).AssemblyQualifiedName;
			configuration.Properties[Environment.UseSecondLevelCache] = "true";
		}

		protected override void OnSetUp()
		{
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var currAnimalId = 1;
				Person lastPerson = null;
				for (var i = 2; i > 0; i--)
				{
					var person = lastPerson = GeneratePerson(i, lastPerson);
					person.Pets.Add(GenerateCat(currAnimalId++, person));
					person.Pets.Add(GenerateDog(currAnimalId++, person));
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
				s.CreateQuery("delete from Animal").ExecuteUpdate();
				s.CreateQuery("update Person set BestFriend = null").ExecuteUpdate();
				s.CreateQuery("delete from Person").ExecuteUpdate();
				s.CreateQuery("delete from Continent").ExecuteUpdate();
				tx.Commit();
			}
		}

		#region FetchComponent

		[Test]
		public void TestHqlFetchComponent()
		{
			Person person;
			using (var s = OpenSession())
			{
				person = s.CreateQuery("from Person fetch Address where Id = 1").UniqueResult<Person>();
			}

			AssertFetchComponent(person);
		}

		[Test]
		public void TestLinqFetchComponent()
		{
			Person person;
			using (var s = OpenSession())
			{
				person = s.Query<Person>().Fetch(o => o.Address).FirstOrDefault(o => o.Id == 1);
			}

			AssertFetchComponent(person);
		}

		private static void AssertFetchComponent(Person person)
		{
			Assert.That(person, Is.Not.Null);

			Assert.That(NHibernateUtil.IsPropertyInitialized(person, "Image"), Is.False);
			Assert.That(NHibernateUtil.IsPropertyInitialized(person, "Address"), Is.True);
			Assert.That(NHibernateUtil.IsPropertyInitialized(person, "Formula"), Is.False);

			Assert.That(person.Address.City, Is.EqualTo("City1"));
			Assert.That(person.Address.Country, Is.EqualTo("Country1"));
		}

		#endregion

		#region FetchFormula

		[Test]
		public void TestHqlFetchFormula()
		{
			Person person;
			using (var s = OpenSession())
			{
				person = s.CreateQuery("from Person fetch Formula where Id = 1").UniqueResult<Person>();
			}

			AssertFetchFormula(person);
		}

		[Test]
		public void TestLinqFetchFormula()
		{
			Person person;
			using (var s = OpenSession())
			{
				person = s.Query<Person>().Fetch(o => o.Formula).FirstOrDefault(o => o.Id == 1);
			}

			AssertFetchFormula(person);
		}

		private static void AssertFetchFormula(Person person)
		{
			Assert.That(person, Is.Not.Null);

			Assert.That(NHibernateUtil.IsPropertyInitialized(person, "Image"), Is.False);
			Assert.That(NHibernateUtil.IsPropertyInitialized(person, "Address"), Is.False);
			Assert.That(NHibernateUtil.IsPropertyInitialized(person, "Formula"), Is.True);

			Assert.That(person.Formula, Is.EqualTo(1));
		}

		#endregion

		#region FetchProperty

		[Test]
		public void TestHqlFetchProperty()
		{
			Person person;
			using (var s = OpenSession())
			{
				person = s.CreateQuery("from Person fetch Image where Id = 1").UniqueResult<Person>();
			}

			AssertFetchProperty(person);
		}

		[Test]
		public void TestLinqFetchProperty()
		{
			Person person;
			using (var s = OpenSession())
			{
				person = s.Query<Person>().Fetch(o => o.Image).FirstOrDefault(o => o.Id == 1);
			}

			AssertFetchProperty(person);
		}

		private static void AssertFetchProperty(Person person)
		{
			Assert.That(person, Is.Not.Null);

			Assert.That(NHibernateUtil.IsPropertyInitialized(person, "Image"), Is.True);
			Assert.That(NHibernateUtil.IsPropertyInitialized(person, "Address"), Is.False);
			Assert.That(NHibernateUtil.IsPropertyInitialized(person, "Formula"), Is.False);

			Assert.That(person.Image, Has.Length.EqualTo(1));
		}

		#endregion

		#region FetchComponentAndFormulaTwoQueryReadOnly

		[TestCase(true)]
		[TestCase(false)]
		public void TestHqlFetchComponentAndFormulaTwoQueryReadOnly(bool readOnly)
		{
			Person person;
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				person = s.CreateQuery("from Person fetch Address where Id = 1").SetReadOnly(readOnly).UniqueResult<Person>();
				person = s.CreateQuery("from Person fetch Formula where Id = 1").SetReadOnly(readOnly).UniqueResult<Person>();

				tx.Commit();
			}

			AssertFetchComponentAndFormulaTwoQuery(person);
		}

		[TestCase(true)]
		[TestCase(false)]
		public void TestLinqFetchComponentAndFormulaTwoQuery(bool readOnly)
		{
			Person person;
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				person = s.Query<Person>().Fetch(o => o.Address).WithOptions(o => o.SetReadOnly(readOnly)).FirstOrDefault(o => o.Id == 1);
				person = s.Query<Person>().Fetch(o => o.Formula).WithOptions(o => o.SetReadOnly(readOnly)).FirstOrDefault(o => o.Id == 1);

				tx.Commit();
			}

			AssertFetchComponentAndFormulaTwoQuery(person);
		}

		private static void AssertFetchComponentAndFormulaTwoQuery(Person person)
		{
			Assert.That(person, Is.Not.Null);

			Assert.That(NHibernateUtil.IsPropertyInitialized(person, "Image"), Is.False);
			Assert.That(NHibernateUtil.IsPropertyInitialized(person, "Address"), Is.True);
			Assert.That(NHibernateUtil.IsPropertyInitialized(person, "Formula"), Is.True);

			Assert.That(person.Address.City, Is.EqualTo("City1"));
			Assert.That(person.Address.Country, Is.EqualTo("Country1"));
			Assert.That(person.Formula, Is.EqualTo(1));
		}

		#endregion

		#region FetchAllProperties

		[Test]
		public void TestHqlFetchAllProperties()
		{
			Person person;
			using (var s = OpenSession())
			{
				person = s.CreateQuery("from Person fetch all properties where Id = 1").UniqueResult<Person>();
			}

			AssertFetchAllProperties(person);
		}

		[Test]
		public void TestLinqFetchAllProperties()
		{
			Person person;
			using (var s = OpenSession())
			{
				person = s.Query<Person>().FetchLazyProperties().FirstOrDefault(o => o.Id == 1);
			}

			AssertFetchAllProperties(person);
		}

		private static void AssertFetchAllProperties(Person person)
		{
			Assert.That(person, Is.Not.Null);

			Assert.That(NHibernateUtil.IsPropertyInitialized(person, "Image"), Is.True);
			Assert.That(NHibernateUtil.IsPropertyInitialized(person, "Address"), Is.True);
			Assert.That(NHibernateUtil.IsPropertyInitialized(person, "Formula"), Is.True);

			Assert.That(person.Image, Has.Length.EqualTo(1));
			Assert.That(person.Address.City, Is.EqualTo("City1"));
			Assert.That(person.Address.Country, Is.EqualTo("Country1"));
			Assert.That(person.Formula, Is.EqualTo(1));
		}

		#endregion

		#region FetchAllPropertiesIndividually

		[Test]
		public void TestHqlFetchAllPropertiesIndividually()
		{
			Person person;
			using (var s = OpenSession())
			{
				person = s.CreateQuery("from Person fetch Image fetch Address fetch Formula fetch Image where Id = 1").UniqueResult<Person>();
			}

			AssertFetchAllProperties(person);
		}

		[Test]
		public void TestLinqFetchAllPropertiesIndividually()
		{
			Person person;
			using (var s = OpenSession())
			{
				person = s.Query<Person>().Fetch(o => o.Image).Fetch(o => o.Address).Fetch(o => o.Formula).FirstOrDefault(o => o.Id == 1);
			}

			AssertFetchAllProperties(person);
		}

		#endregion

		#region FetchFormulaAndManyToOneComponent

		[Test]
		public void TestHqlFetchFormulaAndManyToOneComponent()
		{
			Person person;
			using (var s = OpenSession())
			{
				person = s.CreateQuery("from Person p fetch p.Formula left join fetch p.BestFriend bf fetch bf.Address where p.Id = 1")
						  .UniqueResult<Person>();
			}

			AssertFetchFormulaAndManyToOneComponent(person);
		}

		[Test]
		public void TestLinqFetchFormulaAndManyToOneComponent()
		{
			Person person;
			using (var s = OpenSession())
			{
				person = s.Query<Person>()
						  .Fetch(o => o.Formula)
						  .Fetch(o => o.BestFriend).ThenFetch(o => o.Address)
						  .FirstOrDefault(o => o.Id == 1);
			}

			AssertFetchFormulaAndManyToOneComponent(person);
		}

		private static void AssertFetchFormulaAndManyToOneComponent(Person person)
		{
			Assert.That(person, Is.Not.Null);

			Assert.That(NHibernateUtil.IsPropertyInitialized(person, "Image"), Is.False);
			Assert.That(NHibernateUtil.IsPropertyInitialized(person, "Address"), Is.False);
			Assert.That(NHibernateUtil.IsPropertyInitialized(person, "Formula"), Is.True);

			Assert.That(NHibernateUtil.IsInitialized(person.BestFriend), Is.True);
			Assert.That(NHibernateUtil.IsPropertyInitialized(person.BestFriend, "Image"), Is.False);
			Assert.That(NHibernateUtil.IsPropertyInitialized(person.BestFriend, "Address"), Is.True);
			Assert.That(NHibernateUtil.IsPropertyInitialized(person.BestFriend, "Formula"), Is.False);

			Assert.That(person.Formula, Is.EqualTo(1));
			Assert.That(person.BestFriend.Address.City, Is.EqualTo("City2"));
			Assert.That(person.BestFriend.Address.Country, Is.EqualTo("Country2"));
		}

		#endregion

		#region FetchFormulaAndManyToOneComponentList

		[TestCase(true)]
		[TestCase(false)]
		public void TestHqlFetchFormulaAndManyToOneComponentList(bool descending)
		{
			Person person;
			using (var s = OpenSession())
			{
				person = s.CreateQuery("from Person p fetch p.Formula left join fetch p.BestFriend bf fetch bf.Address order by p.Id" + (descending ? " desc" : ""))
							.List<Person>().FirstOrDefault(o => o.Id == 1);
			}

			AssertFetchFormulaAndManyToOneComponentList(person);
		}

		[TestCase(true)]
		[TestCase(false)]
		public void TestLinqFetchFormulaAndManyToOneComponentList(bool descending)
		{
			Person person;
			using (var s = OpenSession())
			{
				IQueryable<Person> query = s.Query<Person>()
							.Fetch(o => o.Formula)
							.Fetch(o => o.BestFriend).ThenFetch(o => o.Address);
				query = descending ? query.OrderByDescending(o => o.Id) : query.OrderBy(o => o.Id);
				person = query
						.ToList()
						.FirstOrDefault(o => o.Id == 1);
			}

			AssertFetchFormulaAndManyToOneComponentList(person);
		}

		private static void AssertFetchFormulaAndManyToOneComponentList(Person person)
		{
			Assert.That(person, Is.Not.Null);

			Assert.That(NHibernateUtil.IsPropertyInitialized(person, "Image"), Is.False);
			Assert.That(NHibernateUtil.IsPropertyInitialized(person, "Address"), Is.False);
			Assert.That(NHibernateUtil.IsPropertyInitialized(person, "Formula"), Is.True);

			Assert.That(NHibernateUtil.IsInitialized(person.BestFriend), Is.True);
			Assert.That(NHibernateUtil.IsPropertyInitialized(person.BestFriend, "Image"), Is.False);
			Assert.That(NHibernateUtil.IsPropertyInitialized(person.BestFriend, "Address"), Is.True);
			Assert.That(NHibernateUtil.IsPropertyInitialized(person.BestFriend, "Formula"), Is.True);

			Assert.That(person.Formula, Is.EqualTo(1));
			Assert.That(person.BestFriend.Address.City, Is.EqualTo("City2"));
			Assert.That(person.BestFriend.Address.Country, Is.EqualTo("Country2"));
		}

		#endregion

		#region FetchManyToOneAllProperties

		[Test]
		public void TestHqlFetchManyToOneAllProperties()
		{
			Person person;
			using (var s = OpenSession())
			{
				person = s.CreateQuery("from Person p left join fetch p.BestFriend fetch all properties")
							.List<Person>().FirstOrDefault(o => o.Id == 1);
			}

			AssertFetchManyToOneAllProperties(person);
		}

		[Test]
		public void TestLinqFetchManyToOneAllProperties()
		{
			Person person;
			using (var s = OpenSession())
			{
				person = s.Query<Person>()
							.Fetch(o => o.BestFriend).FetchLazyProperties()
							.ToList()
							.FirstOrDefault(o => o.Id == 1);
			}

			AssertFetchManyToOneAllProperties(person);
		}

		private static void AssertFetchManyToOneAllProperties(Person person)
		{
			Assert.That(person, Is.Not.Null);

			Assert.That(NHibernateUtil.IsPropertyInitialized(person, "Image"), Is.False);
			Assert.That(NHibernateUtil.IsPropertyInitialized(person, "Address"), Is.False);
			Assert.That(NHibernateUtil.IsPropertyInitialized(person, "Formula"), Is.False);

			Assert.That(NHibernateUtil.IsInitialized(person.BestFriend), Is.True);
			Assert.That(NHibernateUtil.IsPropertyInitialized(person.BestFriend, "Image"), Is.True);
			Assert.That(NHibernateUtil.IsPropertyInitialized(person.BestFriend, "Address"), Is.True);
			Assert.That(NHibernateUtil.IsPropertyInitialized(person.BestFriend, "Formula"), Is.True);

			Assert.That(person.BestFriend.Formula, Is.EqualTo(2));
			Assert.That(person.BestFriend.Address.City, Is.EqualTo("City2"));
			Assert.That(person.BestFriend.Address.Country, Is.EqualTo("Country2"));
			Assert.That(person.BestFriend.Image, Has.Length.EqualTo(2));
		}

		#endregion

		#region FetchFormulaAndOneToManyComponent

		[Test]
		public void TestHqlFetchFormulaAndOneToManyComponent()
		{
			Person person;
			using (var s = OpenSession())
			{
				person = s.CreateQuery("from Person p fetch p.Formula left join fetch p.Dogs dog fetch dog.Address where p.Id = 1")
							.List<Person>()
							.FirstOrDefault();
			}

			AssertFetchFormulaAndOneToManyComponent(person);
		}

		[Test]
		public void TestLinqFetchFormulaAndOneToManyComponent()
		{
			Person person;
			using (var s = OpenSession())
			{
				person = s.Query<Person>()
							.Fetch(o => o.Formula)
							.FetchMany(o => o.Dogs).ThenFetch(o => o.Address)
							.Where(o => o.Id == 1)
							.ToList()
							.FirstOrDefault();
			}

			AssertFetchFormulaAndOneToManyComponent(person);
		}

		private static void AssertFetchFormulaAndOneToManyComponent(Person person)
		{
			Assert.That(person, Is.Not.Null);

			Assert.That(NHibernateUtil.IsPropertyInitialized(person, "Image"), Is.False);
			Assert.That(NHibernateUtil.IsPropertyInitialized(person, "Address"), Is.False);
			Assert.That(NHibernateUtil.IsPropertyInitialized(person, "Formula"), Is.True);

			Assert.That(NHibernateUtil.IsInitialized(person.Dogs), Is.True);
			Assert.That(NHibernateUtil.IsInitialized(person.BestFriend), Is.False);
			Assert.That(NHibernateUtil.IsInitialized(person.Pets), Is.False);

			Assert.That(person.Formula, Is.EqualTo(1));
			Assert.That(person.Dogs, Has.Count.EqualTo(1));
			foreach (var dog in person.Dogs)
			{
				Assert.That(NHibernateUtil.IsPropertyInitialized(dog, "Image"), Is.False);
				Assert.That(NHibernateUtil.IsPropertyInitialized(dog, "Formula"), Is.False);
				Assert.That(NHibernateUtil.IsPropertyInitialized(dog, "Address"), Is.True);

				Assert.That(dog.Address.City, Is.EqualTo("City1"));
				Assert.That(dog.Address.Country, Is.EqualTo("Country1"));
			}
		}

		#endregion

		#region FetchOneToManyProperty

		[Test]
		public void TestHqlFetchOneToManyProperty()
		{
			Person person;
			using (var s = OpenSession())
			{
				person = s.CreateQuery("from Person p left join fetch p.Cats cat fetch cat.SecondImage where p.Id = 1")
						.List<Person>()
						.FirstOrDefault();
			}

			AssertFetchOneToManyProperty(person);
		}

		[Test]
		public void TestLinqFetchOneToManyProperty()
		{
			Person person;
			using (var s = OpenSession())
			{
				person = s.Query<Person>()
						.FetchMany(o => o.Cats).ThenFetch(o => o.SecondImage)
						.Where(o => o.Id == 1)
						.ToList()
						.FirstOrDefault();
			}

			AssertFetchOneToManyProperty(person);
		}

		private static void AssertFetchOneToManyProperty(Person person)
		{
			Assert.That(person, Is.Not.Null);

			Assert.That(NHibernateUtil.IsPropertyInitialized(person, "Image"), Is.False);
			Assert.That(NHibernateUtil.IsPropertyInitialized(person, "Address"), Is.False);
			Assert.That(NHibernateUtil.IsPropertyInitialized(person, "Formula"), Is.False);

			Assert.That(NHibernateUtil.IsInitialized(person.BestFriend), Is.False);
			Assert.That(NHibernateUtil.IsInitialized(person.Pets), Is.False);
			Assert.That(NHibernateUtil.IsInitialized(person.Cats), Is.True);

			Assert.That(person.Cats, Has.Count.EqualTo(1));
			foreach (var cat in person.Cats)
			{
				Assert.That(NHibernateUtil.IsPropertyInitialized(cat, "Image"), Is.False);
				Assert.That(NHibernateUtil.IsPropertyInitialized(cat, "Formula"), Is.False);
				Assert.That(NHibernateUtil.IsPropertyInitialized(cat, "Address"), Is.False);
				Assert.That(NHibernateUtil.IsPropertyInitialized(cat, "SecondImage"), Is.True);
				Assert.That(NHibernateUtil.IsPropertyInitialized(cat, "SecondFormula"), Is.False);

				Assert.That(cat.SecondImage, Has.Length.EqualTo(6));
			}
		}

		#endregion

		#region FetchNotMappedProperty

		[Test]
		public void TestHqlFetchNotMappedProperty()
		{
			using (var s = OpenSession())
			{
				Assert.Throws<InvalidOperationException>(
					() =>
					{
						var person = s.CreateQuery("from Person p fetch p.BirthYear where p.Id = 1").UniqueResult<Person>();
					});
			}
		}

		[Test]
		public void TestLinqFetchNotMappedProperty()
		{
			using (var s = OpenSession())
			{
				Assert.Throws<QueryException>(
					() =>
					{
						var person = s.Query<Person>().Fetch(o => o.BirthYear).FirstOrDefault(o => o.Id == 1);
					});
			}
		}

		#endregion

		#region FetchComponentManyToOne

		[Test]
		public void TestHqlFetchComponentManyToOne()
		{
			Person person;
			using (var s = OpenSession())
			{
				person = s.CreateQuery("from Person p fetch p.Address left join fetch p.Address.Continent where p.Id = 1").UniqueResult<Person>();
			}

			AssertFetchComponentManyToOne(person);
		}

		[Test]
		public void TestLinqFetchComponentManyToOne()
		{
			Person person;
			using (var s = OpenSession())
			{
				person = s.Query<Person>().Fetch(o => o.Address).ThenFetch(o => o.Continent).FirstOrDefault(o => o.Id == 1);
			}

			AssertFetchComponentManyToOne(person);
		}

		private static void AssertFetchComponentManyToOne(Person person)
		{
			Assert.That(person, Is.Not.Null);

			Assert.That(NHibernateUtil.IsPropertyInitialized(person, "Image"), Is.False);
			Assert.That(NHibernateUtil.IsPropertyInitialized(person, "Address"), Is.True);
			Assert.That(NHibernateUtil.IsPropertyInitialized(person, "Formula"), Is.False);

			Assert.That(NHibernateUtil.IsInitialized(person.Address.Continent), Is.True);

			Assert.That(person.Address.City, Is.EqualTo("City1"));
			Assert.That(person.Address.Country, Is.EqualTo("Country1"));
			Assert.That(person.Address.Continent.Name, Is.EqualTo("Continent1"));
		}

		#endregion

		#region TestHqlFetchManyToOneAndComponentManyToOne

		[Test]
		public void TestHqlFetchManyToOneAndComponentManyToOne()
		{
			Person person;
			using (var s = OpenSession())
			{
				person = s.CreateQuery("from Person p fetch p.Address left join fetch p.Address.Continent left join fetch p.BestFriend where p.Id = 1").UniqueResult<Person>();
			}

			AssertFetchManyToOneAndComponentManyToOne(person);
		}

		[Test]
		public void TestLinqFetchManyToOneAndComponentManyToOne()
		{
			Person person;
			using (var s = OpenSession())
			{
				person = s.Query<Person>().Fetch(o => o.BestFriend).Fetch(o => o.Address).ThenFetch(o => o.Continent).FirstOrDefault(o => o.Id == 1);
			}

			AssertFetchManyToOneAndComponentManyToOne(person);
		}

		private static void AssertFetchManyToOneAndComponentManyToOne(Person person)
		{
			AssertFetchComponentManyToOne(person);
			Assert.That(NHibernateUtil.IsInitialized(person.BestFriend), Is.True);
		}

		#endregion

		#region FetchSubClassFormula

		[Test]
		public void TestHqlFetchSubClassFormula()
		{
			Animal animal;
			using (var s = OpenSession())
			{
				animal = s.CreateQuery("from Animal a fetch a.SecondFormula where a.Id = 1").UniqueResult<Animal>();
			}

			AssertFetchSubClassFormula(animal);
		}

		[Test]
		public void TestLinqFetchSubClassFormula()
		{
			Animal animal;
			using (var s = OpenSession())
			{
				animal = s.Query<Animal>().Fetch(o => ((Cat) o).SecondFormula).First(o => o.Id == 1);
			}

			AssertFetchSubClassFormula(animal);
		}

		private static void AssertFetchSubClassFormula(Animal animal)
		{
			Assert.That(animal, Is.AssignableTo(typeof(Cat)));
			var cat = (Cat) animal;
			Assert.That(NHibernateUtil.IsPropertyInitialized(cat, "SecondFormula"), Is.True);
			Assert.That(NHibernateUtil.IsPropertyInitialized(cat, "SecondImage"), Is.False);
			Assert.That(NHibernateUtil.IsPropertyInitialized(cat, "Address"), Is.False);
			Assert.That(NHibernateUtil.IsPropertyInitialized(cat, "Formula"), Is.False);
			Assert.That(NHibernateUtil.IsPropertyInitialized(cat, "Image"), Is.False);
		}

		#endregion

		#region FetchSubClassProperty

		[Test]
		public void TestHqlFetchSubClassProperty()
		{
			Animal animal;
			using (var s = OpenSession())
			{
				animal = s.CreateQuery("from Animal a fetch a.SecondImage where a.Id = 1").UniqueResult<Animal>();
			}

			AssertFetchSubClassProperty(animal);
		}

		[Test]
		public void TestLinqFetchSubClassProperty()
		{
			Animal animal;
			using (var s = OpenSession())
			{
				animal = s.Query<Animal>().Fetch(o => ((Cat) o).SecondImage).First(o => o.Id == 1);
			}

			AssertFetchSubClassProperty(animal);
		}

		private static void AssertFetchSubClassProperty(Animal animal)
		{
			Assert.That(animal, Is.AssignableTo(typeof(Cat)));
			var cat = (Cat) animal;
			Assert.That(NHibernateUtil.IsPropertyInitialized(cat, "SecondFormula"), Is.False);
			Assert.That(NHibernateUtil.IsPropertyInitialized(cat, "SecondImage"), Is.True);
			Assert.That(NHibernateUtil.IsPropertyInitialized(cat, "Address"), Is.False);
			Assert.That(NHibernateUtil.IsPropertyInitialized(cat, "Formula"), Is.False);
			Assert.That(NHibernateUtil.IsPropertyInitialized(cat, "Image"), Is.False);
		}

		#endregion

		#region FetchSubClassAllProperty

		[Test]
		public void TestHqlFetchSubClassAllProperties()
		{
			Animal animal;
			using (var s = OpenSession())
			{
				animal = s.CreateQuery("from Animal a fetch all properties where a.Id = 1").UniqueResult<Animal>();
			}

			AssertFetchSubClassAllProperties(animal);
		}

		[Test]
		public void TestLinqFetchSubClassAllProperties()
		{
			Animal animal;
			using (var s = OpenSession())
			{
				animal = s.Query<Animal>().FetchLazyProperties().First(o => o.Id == 1);
			}

			AssertFetchSubClassAllProperties(animal);
		}

		private static void AssertFetchSubClassAllProperties(Animal animal)
		{
			Assert.That(animal, Is.AssignableTo(typeof(Cat)));
			var cat = (Cat) animal;
			Assert.That(NHibernateUtil.IsPropertyInitialized(cat, "SecondFormula"), Is.True);
			Assert.That(NHibernateUtil.IsPropertyInitialized(cat, "SecondImage"), Is.True);
			Assert.That(NHibernateUtil.IsPropertyInitialized(cat, "Address"), Is.True);
			Assert.That(NHibernateUtil.IsPropertyInitialized(cat, "Formula"), Is.True);
			Assert.That(NHibernateUtil.IsPropertyInitialized(cat, "Image"), Is.True);
		}

		#endregion

		#region FetchAllPropertiesWithFetchProperty

		[Test]
		public void TestHqlFetchAllPropertiesWithFetchProperty()
		{
			using (var s = OpenSession())
			{
				Assert.Throws<QuerySyntaxException>(
					() =>
					{
						var person = s.CreateQuery("from Person p fetch p.Address fetch all properties where p.Id = 1").UniqueResult<Person>();
					});
				Assert.Throws<QuerySyntaxException>(
					() =>
					{
						var person = s.CreateQuery("from Person p fetch all properties fetch p.Address where p.Id = 1").UniqueResult<Person>();
					});
			}
		}

		[Test]
		public void TestLinqFetchAllPropertiesWithFetchProperty()
		{
			using (var s = OpenSession())
			{
				Assert.Throws<InvalidCastException>(
					() =>
					{
						var person = s.Query<Person>().Fetch(o => o.Address).FetchLazyProperties().FirstOrDefault(o => o.Id == 1);
					});
				Assert.Throws<QuerySyntaxException>(
					() =>
					{
						var person = s.Query<Person>().FetchLazyProperties().Fetch(o => o.Address).FirstOrDefault(o => o.Id == 1);
					});
			}
		}

		#endregion

		[Test]
		public void TestHqlFetchComponentAlias()
		{
			Person person;
			using (var s = OpenSession())
			{
				person = s.CreateQuery("from Person p fetch p.Address where p.Id = 1").UniqueResult<Person>();
			}

			AssertFetchComponent(person);
		}

		[TestCase(true)]
		[TestCase(false)]
		public void TestFetchComponentAndFormulaTwoQueryCache(bool readOnly)
		{
			TestLinqFetchComponentAndFormulaTwoQuery(readOnly);

			Person person;
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				person = s.Get<Person>(1);

				tx.Commit();
			}

			AssertFetchComponentAndFormulaTwoQuery(person);
		}

		[Test]
		public void TestFetchComponentCache()
		{
			Person person;
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				person = s.Query<Person>().Fetch(o => o.Address).FirstOrDefault(o => o.Id == 1);
				AssertFetchComponent(person);
				tx.Commit();
			}

			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				person = s.Get<Person>(1);
				AssertFetchComponent(person);
				// Will reset the cache item
				person.Name = "Test";

				tx.Commit();
			}

			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				person = s.Get<Person>(1);
				Assert.That(person, Is.Not.Null);

				Assert.That(NHibernateUtil.IsPropertyInitialized(person, "Image"), Is.False);
				Assert.That(NHibernateUtil.IsPropertyInitialized(person, "Address"), Is.False);
				Assert.That(NHibernateUtil.IsPropertyInitialized(person, "Formula"), Is.False);

				tx.Commit();
			}
		}

		[TestCase(true)]
		[TestCase(false)]
		public void TestFetchAfterPropertyIsInitialized(bool readOnly)
		{
			Person person;
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				person = s.CreateQuery("from Person fetch Address where Id = 1").SetReadOnly(readOnly).UniqueResult<Person>();
				person.Image = new byte[10];
				person = s.CreateQuery("from Person fetch Image where Id = 1").SetReadOnly(readOnly).UniqueResult<Person>();

				tx.Commit();
			}

			Assert.That(NHibernateUtil.IsPropertyInitialized(person, "Image"), Is.True);
			Assert.That(NHibernateUtil.IsPropertyInitialized(person, "Address"), Is.True);
			Assert.That(NHibernateUtil.IsPropertyInitialized(person, "Formula"), Is.False);

			Assert.That(person.Image, Has.Length.EqualTo(10));

			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				person = s.CreateQuery("from Person where Id = 1").SetReadOnly(readOnly).UniqueResult<Person>();
				person.Image = new byte[1];

				tx.Commit();
			}

			Assert.That(NHibernateUtil.IsPropertyInitialized(person, "Image"), Is.True);
			Assert.That(NHibernateUtil.IsPropertyInitialized(person, "Address"), Is.False);
			Assert.That(NHibernateUtil.IsPropertyInitialized(person, "Formula"), Is.False);
		}

		[TestCase(true)]
		[TestCase(false)]
		public void TestFetchAfterEntityIsInitialized(bool readOnly)
		{
			Person person;
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				person = s.CreateQuery("from Person where Id = 1").SetReadOnly(readOnly).UniqueResult<Person>();
				var image = person.Image;
				person = s.CreateQuery("from Person fetch Image where Id = 1").SetReadOnly(readOnly).UniqueResult<Person>();

				tx.Commit();
			}

			Assert.That(NHibernateUtil.IsPropertyInitialized(person, "Image"), Is.True);
			Assert.That(NHibernateUtil.IsPropertyInitialized(person, "Address"), Is.True);
			Assert.That(NHibernateUtil.IsPropertyInitialized(person, "Formula"), Is.True);
		}

		[TestCase(true)]
		[TestCase(false)]
		public void TestFetchAllPropertiesAfterEntityIsInitialized(bool readOnly)
		{
			Person person;
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				person = s.CreateQuery("from Person where Id = 1").SetReadOnly(readOnly).UniqueResult<Person>();
				var image = person.Image;
				person = s.CreateQuery("from Person fetch all properties where Id = 1").SetReadOnly(readOnly).UniqueResult<Person>();

				tx.Commit();
			}

			Assert.That(NHibernateUtil.IsPropertyInitialized(person, "Image"), Is.True);
			Assert.That(NHibernateUtil.IsPropertyInitialized(person, "Address"), Is.True);
			Assert.That(NHibernateUtil.IsPropertyInitialized(person, "Formula"), Is.True);
		}

		[Test]
		public void TestHqlCrossJoinFetchFormula()
		{
			if (!Dialect.SupportsCrossJoin)
			{
				Assert.Ignore("Dialect does not support cross join.");
			}

			var persons = new List<Person>();
			var bestFriends = new List<Person>();
			using (var sqlSpy = new SqlLogSpy())
			using (var s = OpenSession())
			{
				var list = s.CreateQuery("select p, bf from Person p cross join Person bf fetch bf.Formula where bf.Id = p.BestFriend.Id").List<object[]>();
				foreach (var arr in list)
				{
					persons.Add((Person) arr[0]);
					bestFriends.Add((Person) arr[1]);
				}
			}

			AssertPersons(persons, false);
			AssertPersons(bestFriends, true);

			void AssertPersons(List<Person> results, bool fetched)
			{
				foreach (var person in results)
				{
					Assert.That(person, Is.Not.Null);
					Assert.That(NHibernateUtil.IsPropertyInitialized(person, "Image"), Is.False);
					Assert.That(NHibernateUtil.IsPropertyInitialized(person, "Address"), Is.False);
					Assert.That(NHibernateUtil.IsPropertyInitialized(person, "Formula"), fetched ? Is.True : (IResolveConstraint) Is.False);
				}
			}
		}

		private static Person GeneratePerson(int i, Person bestFriend)
		{
			return new Person
			{
				Id = i,
				Name = $"Person{i}",
				Address = new Address
				{
					City = $"City{i}",
					Country = $"Country{i}",
					Continent = GenerateContinent(i)
				},
				Image = new byte[i],
				BestFriend = bestFriend
			};
		}

		private static Continent GenerateContinent(int i)
		{
			return new Continent
			{
				Id = i,
				Name = $"Continent{i}"
			};
		}

		private static Cat GenerateCat(int i, Person owner)
		{
			return new Cat
			{
				Id = i,
				Address = new Address
				{
					City = owner.Address.City,
					Country = owner.Address.Country
				},
				Image = new byte[i],
				Name = $"Cat{i}",
				SecondImage = new byte[i * 2],
				Owner = owner
			};
		}

		private static Dog GenerateDog(int i, Person owner)
		{
			return new Dog
			{
				Id = i,
				Address = new Address
				{
					City = owner.Address.City,
					Country = owner.Address.Country
				},
				Image = new byte[i * 3],
				Name = $"Dog{i}",
				Owner = owner
			};
		}
	}
}
