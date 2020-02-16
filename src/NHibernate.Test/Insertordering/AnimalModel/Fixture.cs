using System.Collections;
using NHibernate.Cfg;
using NUnit.Framework;

namespace NHibernate.Test.Insertordering.AnimalModel
{
	[TestFixture]
	public class Fixture : TestCase
	{
		protected override string[] Mappings
		{
			get { return new[] { "Insertordering.AnimalModel.Mappings.hbm.xml" }; }
		}

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override void Configure(Configuration configuration)
		{
			configuration.DataBaseIntegration(x =>
			{
				x.BatchSize = 10;
				x.OrderInserts = true;
			});
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var tran = session.BeginTransaction())
			{
				session.Delete("from Animal");
				session.Delete("from Person");
				tran.Commit();
			}
		}

		[Test]
		public void ElaboratedModel()
		{
			using (var session = OpenSession())
			using (var tran = session.BeginTransaction())
			{
				var personWithAnimals = new PersonWithAnimals { Name = "fabio" };
				var personWithCats = new PersonWithCats { Name = "dario" };
				var personWithSivasKangals = new PersonWithSivasKangals { Name = "tuna" };
				var personWithDogs = new PersonWithDogs { Name = "davy" };

				var animalForAnimals = new Animal { Name = "Pasha", Owner = personWithAnimals };
				var dogForAnimals = new Dog { Name = "Efe", Country = "Turkey", Owner = personWithAnimals };
				var catForAnimals = new Cat { Name = "Tekir", EyeColor = "green", Owner = personWithAnimals };
				var sivasKangalForAnimals = new SivasKangal { Name = "Karabas", Country = "Turkey", HouseAddress = "Atakoy", Owner = personWithAnimals };

				personWithAnimals.AnimalsGeneric.Add(animalForAnimals);
				personWithAnimals.AnimalsGeneric.Add(dogForAnimals);
				personWithAnimals.AnimalsGeneric.Add(catForAnimals);
				personWithAnimals.AnimalsGeneric.Add(sivasKangalForAnimals);

				var animalForCats = new Animal { Name = "Pasha2", Owner = personWithCats };
				var catForCats = new Cat { Name = "Tekir2", EyeColor = "green", Owner = personWithCats };
				var dogForCats = new Dog { Name = "Efe2", Country = "Turkey", Owner = personWithCats };
				personWithCats.AnimalsGeneric.Add(catForCats);

				var catForDogs = new Cat { Name = "Tekir3", EyeColor = "blue", Owner = personWithDogs };
				var dogForDogs = new Dog { Name = "Efe3", Country = "Turkey", Owner = personWithDogs };
				var sivasKangalForDogs = new SivasKangal { Name = "Karabas3", Country = "Turkey", HouseAddress = "Atakoy", Owner = personWithDogs };
				personWithDogs.AnimalsGeneric.Add(dogForDogs);
				personWithDogs.AnimalsGeneric.Add(sivasKangalForDogs);

				var animalForSivasKangals = new Animal { Name = "Pasha4", Owner = personWithSivasKangals };
				var dogForSivasKangals = new Dog { Name = "Efe4", Country = "Turkey", Owner = personWithSivasKangals };
				var catForSivasKangals = new Cat { EyeColor = "red", Name = "Tekir4", Owner = personWithSivasKangals };
				var sivasKangalForSivasKangals = new SivasKangal { Name = "Karabas4", Country = "Turkey", HouseAddress = "Atakoy", Owner = personWithSivasKangals };
				personWithSivasKangals.AnimalsGeneric.Add(sivasKangalForSivasKangals);

				session.Save(animalForCats);
				session.Save(dogForCats);

				session.Save(catForDogs);

				session.Save(animalForSivasKangals);
				session.Save(dogForSivasKangals);
				session.Save(catForSivasKangals);

				session.Save(personWithAnimals);
				session.Save(personWithCats);
				session.Save(personWithDogs);
				session.Save(personWithSivasKangals);

				Assert.DoesNotThrow(() => { tran.Commit(); });
			}
		}

		// #1338
		[Test]
		public void InsertShouldNotInitializeManyToOneProxy()
		{
			var person = new Person {  Name = "AnimalOwner" };
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.Save(person);
				t.Commit();
			}
			Sfi.Evict(typeof(Person));

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var personProxy = s.Load<Person>(person.Id);
				Assert.That(NHibernateUtil.IsInitialized(personProxy), Is.False, "Person proxy already initialized after load");

				s.Save(new Cat { Name = "Felix", Owner = personProxy });
				s.Save(new Cat { Name = "Loustic", Owner = personProxy });
				Assert.That(NHibernateUtil.IsInitialized(personProxy), Is.False, "Person proxy initialized after saves");
				t.Commit();
				Assert.That(NHibernateUtil.IsInitialized(personProxy), Is.False, "Person proxy initialized after commit");
			}
		}

		[Test]
		public void InsertShouldNotInitializeOneToManyProxy()
		{
			var cat = new Cat {  Name = "Felix" };
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.Save(cat);
				t.Commit();
			}
			Sfi.Evict(typeof(Cat));

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var catProxy = s.Load<Cat>(cat.Id);
				Assert.That(NHibernateUtil.IsInitialized(catProxy), Is.False, "Cat proxy already initialized after load");

				var owner = new Person { Name = "AnimalOwner" };
				owner.AnimalsGeneric.Add(catProxy);
				// Following assert would fail if the collection was changed for a set.
				Assert.That(NHibernateUtil.IsInitialized(catProxy), Is.False, "Cat proxy initialized after collection add");
				s.Save(owner);
				Assert.That(NHibernateUtil.IsInitialized(catProxy), Is.False, "Cat proxy initialized after save");
				t.Commit();
				Assert.That(NHibernateUtil.IsInitialized(catProxy), Is.False, "Cat proxy initialized after commit");
				// The collection being inverse, the cat owner is not actually set in this test, but that is enough
				// to check the trouble. The ordering logic does not short-circuit on inverse collections. (It could
				// be an optimization, but it may cause regressions for some edge case mappings, like one having an
				// inverse one-to-many with no matching many-to-one but a basic type property for the foreign key
				// instead.)
			}
		}

		// #2141
		[Test]
		public void InsertShouldNotDependOnEntityEqualsImplementation()
		{
			var person = new PersonWithWrongEquals { Name = "AnimalOwner" };
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.Save(person);
				t.Commit();
			}
			Sfi.Evict(typeof(PersonWithWrongEquals));

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var personProxy = s.Get<PersonWithWrongEquals>(person.Id);

				s.Save(new Cat { Name = "Felix", Owner = personProxy });
				s.Save(new Cat { Name = "Loustic", Owner = personProxy });
				Assert.DoesNotThrow(() => { t.Commit(); });
			}
		}
	}
}
