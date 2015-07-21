using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2969
{ 
	[TestFixture, Ignore("Not fixed yet.")]
	public class Fixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var john = new Person {ID = 1, Name = "John"};
				var garfield = new DomesticCat {ID = 2, Name = "Garfield", Owner = john};
				session.Save(john);
				session.Save(garfield);

				var alice = new Person {ID = 3, Name = "Alice"};
				var bubbles = new Goldfish {ID = 4, Name = "Bubbles", Owner = alice};
				session.Save(alice);
				session.Save(bubbles);

				var pirate = new Person {ID = 5, Name = "Pirate"};
				var parrot = new Parrot {ID = 6, Name = "Parrot", Pirate = pirate};
				session.Save(pirate);
				session.Save(parrot);

				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.Delete("from System.Object");
				transaction.Commit();
			}
		}

		[Test]
		public void CanGetDomesticCat()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var domesticCat = session.Get<DomesticCat>(2);

				Assert.IsNotNull(domesticCat);
				Assert.AreEqual("Garfield", domesticCat.Name);
				Assert.IsNotNull(domesticCat.Owner);
				Assert.AreEqual("John", domesticCat.Owner.Name);
			}
		}

		[Test]
		public void CanGetDomesticCatAsCat()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var cat = session.Get<Cat>(2);
				Assert.IsNotNull(cat);
				Assert.AreEqual("Garfield", cat.Name);

				var domesticCat = cat as DomesticCat;
				Assert.IsNotNull(domesticCat);
				Assert.IsNotNull(domesticCat.Owner);
				Assert.AreEqual("John", domesticCat.Owner.Name);
			}
		}

		[Test]
		public void CanGetGoldfish()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var goldfish = session.Get<Goldfish>(4);

				Assert.IsNotNull(goldfish);
				Assert.AreEqual("Bubbles", goldfish.Name);
				Assert.IsNotNull(goldfish.Owner);
				Assert.AreEqual("Alice", goldfish.Owner.Name);
			}
		}

		[Test]
		public void CanGetGoldfishAsFish()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var fish = session.Get<Fish>(4);
				Assert.IsNotNull(fish);
				Assert.AreEqual("Bubbles", fish.Name);

				var goldfish = fish as Goldfish;
				Assert.IsNotNull(goldfish);
				Assert.IsNotNull(goldfish.Owner);
				Assert.AreEqual("Alice", goldfish.Owner.Name);
			}
		}


		[Test]
		public void CanGetParrot()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var parrot = session.Get<Parrot>(6);

				Assert.IsNotNull(parrot, "Parrot");
				Assert.AreEqual("Parrot", parrot.Name, "Parrot Name");
				Assert.IsNotNull(parrot.Pirate, "Pirate");
				Assert.AreEqual("Pirate", parrot.Pirate.Name, "Pirate Name");
			}
		}

		[Test]
		public void CanGetParrotAsBird()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var bird = session.Get<Bird>(6);
				Assert.IsNotNull(bird, "Bird");
				Assert.AreEqual("Parrot", bird.Name, "Bird Name");

				var parrot = bird as Parrot;
				Assert.IsNotNull(parrot, "Parrot");
				Assert.IsNotNull(parrot.Pirate, "Pirate");
				Assert.AreEqual("Pirate", parrot.Pirate.Name, "Pirate Name");
			}
		}
	}
}
