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
	}
}