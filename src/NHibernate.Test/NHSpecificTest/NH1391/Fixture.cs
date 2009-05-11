using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
namespace NHibernate.Test.NHSpecificTest.NH1391
{
	[TestFixture,Ignore]
	public class Fixture:BugTestCase
	{
		protected override void OnSetUp()
		{
			using(var session=OpenSession())
			using(var tran=session.BeginTransaction())
			{
				PersonWithAnimals personWithAnimals = new PersonWithAnimals {Name = "fabio"};
				PersonWithCats personWithCats = new PersonWithCats {Name = "dario"};
				PersonWithSivasKangals personWithSivasKangals = new PersonWithSivasKangals {Name = "tuna"};
				PersonWithDogs personWithDogs = new PersonWithDogs {Name = "davy"};

				var animalForAnimals = new Animal {Name = "Pasha",Owner=personWithAnimals};
				var dogForAnimals = new Dog { Name = "Efe", Country = "Turkey", Owner = personWithAnimals };
				var catForAnimals = new Cat { Name = "Tekir", EyeColor = "green", Owner = personWithAnimals };
				var sivasKangalForAnimals = new SivasKangal { Name = "Karabas", Country = "Turkey", HouseAddress = "Atakoy", Owner = personWithAnimals };

				personWithAnimals.AnimalsGeneric.Add(animalForAnimals);
				personWithAnimals.AnimalsGeneric.Add(dogForAnimals);
				personWithAnimals.AnimalsGeneric.Add(catForAnimals);
				personWithAnimals.AnimalsGeneric.Add(sivasKangalForAnimals);

				var animalForCats = new Animal {Name = "Pasha2", Owner = personWithCats};
				var catForCats = new Cat { Name = "Tekir2", EyeColor = "green", Owner = personWithCats };
				var dogForCats = new Dog { Name = "Efe2", Country = "Turkey", Owner = personWithCats };
				personWithCats.AnimalsGeneric.Add(catForCats);

				var catForDogs = new Cat {Name = "Tekir3", EyeColor = "blue", Owner = personWithDogs};
				var dogForDogs = new Dog { Name = "Efe3", Country = "Turkey", Owner = personWithDogs };
				var sivasKangalForDogs = new SivasKangal { Name = "Karabas3", Country = "Turkey", HouseAddress = "Atakoy", Owner = personWithDogs };
				personWithDogs.AnimalsGeneric.Add(dogForDogs);
				personWithDogs.AnimalsGeneric.Add(sivasKangalForDogs);

				var animalForSivasKangals = new Animal {Name = "Pasha4", Owner = personWithSivasKangals};
				var dogForSivasKangals = new Dog {Name = "Efe4", Country = "Turkey", Owner = personWithSivasKangals};
				var catForSivasKangals = new Cat {EyeColor = "red", Name = "Tekir4", Owner = personWithSivasKangals};
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
		public void Can_discriminate_subclass_on_list_with_lazy_loading_when_used_get()
		{
			using (var session = OpenSession())
			using (var tran = session.BeginTransaction())
			{
				var personWithAnimals = session.Get<PersonWithAnimals>(1);
				var personWithCats = session.Get<PersonWithCats>(2);
				var personWithDogs = session.Get<PersonWithDogs>(3);
				var personWithSivasKangals = session.Get<PersonWithSivasKangals>(4);

				Assert.That(personWithAnimals.AnimalsGeneric,Has.Count.EqualTo(4));
				Assert.That(personWithAnimals.AnimalsNonGeneric, Has.Count.EqualTo(4));

				Assert.That(personWithCats.CatsGeneric, Has.Count.EqualTo(1));
				Assert.That(personWithCats.CatsNonGeneric, Has.Count.EqualTo(1));

				Assert.That(personWithDogs.DogsGeneric, Has.Count.EqualTo(2));
				Assert.That(personWithDogs.DogsNonGeneric, Has.Count.EqualTo(2));

				Assert.That(personWithSivasKangals.SivasKangalsGeneric, Has.Count.EqualTo(1));
				Assert.That(personWithSivasKangals.SivasKangalsNonGeneric, Has.Count.EqualTo(1));
			}
		}
	}
}