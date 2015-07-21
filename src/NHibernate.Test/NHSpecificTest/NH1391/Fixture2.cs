using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1391
{
	[TestFixture]
	public class Fixture2:BugTestCase
	{
		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var tran = session.BeginTransaction())
			{
				PersonWithAllTypes personWithAllTypes = new PersonWithAllTypes();
				Animal animal = new Animal { Name = "Pasha", Owner = personWithAllTypes };
				Dog dog = new Dog { Country = "Turkey", Name = "Kral", Owner = personWithAllTypes };
				SivasKangal sivasKangal = new SivasKangal
				                          	{
				                          		Name = "Karabas",
				                          		Country = "Turkey",
				                          		HouseAddress = "Address",
				                          		Owner = personWithAllTypes
				                          	};
				Cat cat = new Cat { Name = "Tekir", EyeColor = "Red", Owner = personWithAllTypes };
				personWithAllTypes.AnimalsGeneric.Add(animal);
				personWithAllTypes.AnimalsGeneric.Add(cat);
				personWithAllTypes.AnimalsGeneric.Add(dog);
				personWithAllTypes.AnimalsGeneric.Add(sivasKangal);
				session.Save(personWithAllTypes);
				tran.Commit();
			}
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
		public void Can_discriminate_subclass_on_list_with_lazy_loading_when_used_and_person_had_multiple_list()
		{
			using (var session = OpenSession())
			using (var tran = session.BeginTransaction())
			{
				var personWithAnimals = session.Get<PersonWithAllTypes>(1);
				Assert.That(personWithAnimals.AnimalsGeneric, Has.Count.EqualTo(4));
				Assert.That(personWithAnimals.CatsGeneric,Has.Count.EqualTo(1));
				Assert.That(personWithAnimals.DogsGeneric,Has.Count.EqualTo(2));
				Assert.That(personWithAnimals.SivasKangalsGeneric, Has.Count.EqualTo(1));
			}
		}
	}
}
