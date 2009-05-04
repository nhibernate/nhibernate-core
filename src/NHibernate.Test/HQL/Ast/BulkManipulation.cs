using System.Collections;
using System.Diagnostics;
using NUnit.Framework;
using NHibernate.Hql.Ast.ANTLR;

namespace NHibernate.Test.HQL.Ast
{
	[TestFixture]
	public class BulkManipulation: BaseFixture
	{
		public ISession OpenNewSession()
		{
			return OpenSession();
		}

		#region Non-exists

		[Test]
		public void DeleteNonExistentEntity()
		{
			using (ISession s = OpenSession())
			{
				Assert.Throws<QuerySyntaxException>(() => s.CreateQuery("delete NonExistentEntity").ExecuteUpdate());
			}
		}

		[Test]
		public void UpdateNonExistentEntity()
		{
			using (ISession s = OpenSession())
			{
				Assert.Throws<QuerySyntaxException>(() => s.CreateQuery("update NonExistentEntity e set e.someProp = ?").ExecuteUpdate());
			}
		}

		#endregion

		#region DELETES

		[Test]
		public void DeleteWithSubquery()
		{
			// setup the test data...
			ISession s = OpenSession();
			s.BeginTransaction();
			var owner = new SimpleEntityWithAssociation { Name = "myEntity-1" };
			owner.AddAssociation("assoc-1");
			owner.AddAssociation("assoc-2");
			owner.AddAssociation("assoc-3");
			s.Save(owner);
			var owner2 = new SimpleEntityWithAssociation { Name = "myEntity-2" };
			owner2.AddAssociation("assoc-1");
			owner2.AddAssociation("assoc-2");
			owner2.AddAssociation("assoc-3");
			owner2.AddAssociation("assoc-4");
			s.Save(owner2);
			var owner3 = new SimpleEntityWithAssociation { Name = "myEntity-3" };
			s.Save(owner3);
			s.Transaction.Commit();
			s.Close();

			// now try the bulk delete
			s = OpenSession();
			s.BeginTransaction();
			int count = s.CreateQuery("delete SimpleEntityWithAssociation e where size(e.AssociatedEntities ) = 0 and e.Name like '%'")
				.ExecuteUpdate();
			Assert.That(count, Is.EqualTo(1), "Incorrect delete count");
			s.Transaction.Commit();
			s.Close();

			// finally, clean up
			s = OpenSession();
			s.BeginTransaction();
			s.CreateQuery("delete SimpleAssociatedEntity").ExecuteUpdate();
			s.CreateQuery("delete SimpleEntityWithAssociation").ExecuteUpdate();
			s.Transaction.Commit();
			s.Close();
		}

		[Test]
		public void SimpleDeleteOnAnimal()
		{
			if (Dialect.HasSelfReferentialForeignKeyBug)
			{
				Assert.Ignore("self referential FK bug", "HQL delete testing");
				return;
			}

			var data = new TestData(this);
			data.Prepare();

			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();

			int count = s.CreateQuery("delete from Animal as a where a.id = :id")
					.SetInt64("id", data.Polliwog.Id)
					.ExecuteUpdate();
			Assert.That(count, Is.EqualTo(1), "Incorrect delete count");

			count = s.CreateQuery("delete Animal where id = :id")
					.SetInt64("id", data.Catepillar.Id)
					.ExecuteUpdate();
			Assert.That(count, Is.EqualTo(1), "Incorrect delete count");

			// HHH-873...
			if (Dialect.SupportsSubqueryOnMutatingTable)
			{
				count = s.CreateQuery("delete from User u where u not in (select u from User u)").ExecuteUpdate();
				Assert.That(count, Is.EqualTo(0));
			}

			count = s.CreateQuery("delete Animal a").ExecuteUpdate();
			Assert.That(count, Is.EqualTo(4), "Incorrect delete count");

			IList list = s.CreateQuery("select a from Animal as a").List();
			Assert.That(list, Is.Empty, "table not empty");

			t.Commit();
			s.Close();
			data.Cleanup();
		}

		[Test]
		public void DeleteOnDiscriminatorSubclass()
		{
			var data = new TestData(this);
			data.Prepare();

			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();

			int count = s.CreateQuery("delete PettingZoo").ExecuteUpdate();
			Assert.That(count, Is.EqualTo(1), "Incorrect discrim subclass delete count");

			count = s.CreateQuery("delete Zoo").ExecuteUpdate();
			Assert.That(count, Is.EqualTo(1), "Incorrect discrim subclass delete count");

			t.Commit();
			s.Close();

			data.Cleanup();
		}

		[Test]
		public void DeleteOnJoinedSubclass()
		{
			var data = new TestData(this);
			data.Prepare();

			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();

			int count = s.CreateQuery("delete Mammal where bodyWeight > 150").ExecuteUpdate();
			Assert.That(count, Is.EqualTo(1), "Incorrect deletion count on joined subclass");

			count = s.CreateQuery("delete Mammal").ExecuteUpdate();
			Assert.That(count, Is.EqualTo(1), "Incorrect deletion count on joined subclass");

			count = s.CreateQuery("delete SubMulti").ExecuteUpdate();
			Assert.That(count, Is.EqualTo(0), "Incorrect deletion count on joined subclass");

			t.Commit();
			s.Close();

			data.Cleanup();
		}

		[Test]
		public void DeleteOnMappedJoin()
		{
			var data = new TestData(this);
			data.Prepare();

			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();

			int count = s.CreateQuery("delete Joiner where joinedName = :joinedName")
				.SetString("joinedName", "joined-name").ExecuteUpdate();
			Assert.That(count, Is.EqualTo(1), "Incorrect deletion count on joined class");

			t.Commit();
			s.Close();

			data.Cleanup();
		}

		[Test]
		public void DeleteUnionSubclassAbstractRoot()
		{
			var data = new TestData(this);
			data.Prepare();

			// These should reach out into *all* subclass tables...
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();

			int count = s.CreateQuery("delete Vehicle where Owner = :owner")
				.SetString("owner", "Steve").ExecuteUpdate();
			Assert.That(count, Is.EqualTo(1), "incorrect restricted update count");

			count = s.CreateQuery("delete Vehicle").ExecuteUpdate();
			Assert.That(count, Is.EqualTo(3), "incorrect update count");

			t.Commit();
			s.Close();

			data.Cleanup();
		}

		[Test]
		public void DeleteUnionSubclassConcreteSubclass()
		{
			var data = new TestData(this);
			data.Prepare();

			// These should only affect the given table
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();

			int count = s.CreateQuery("delete Truck where Owner = :owner")
				.SetString("owner", "Steve")
				.ExecuteUpdate();
			Assert.That(count, Is.EqualTo(1), "incorrect restricted update count");

			count = s.CreateQuery("delete Truck").ExecuteUpdate();
			Assert.That(count, Is.EqualTo(2), "incorrect update count");
			t.Commit();
			s.Close();

			data.Cleanup();
		}

		[Test]
		public void DeleteUnionSubclassLeafSubclass()
		{
			var data = new TestData(this);
			data.Prepare();

			// These should only affect the given table
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();

			int count = s.CreateQuery("delete Car where Owner = :owner")
				.SetString("owner", "Kirsten")
				.ExecuteUpdate();
			Assert.That(count, Is.EqualTo(1), "incorrect restricted update count");

			count = s.CreateQuery("delete Car").ExecuteUpdate();
			Assert.That(count, Is.EqualTo(0), "incorrect update count");

			t.Commit();
			s.Close();

			data.Cleanup();
		}

		[Test]
		public void DeleteRestrictedOnManyToOne()
		{
			var data = new TestData(this);
			data.Prepare();

			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();

			int count = s.CreateQuery("delete Animal where mother = :mother")
					.SetEntity("mother", data.Butterfly)
					.ExecuteUpdate();
			Assert.That(count, Is.EqualTo(1));

			t.Commit();
			s.Close();

			data.Cleanup();
		}

		[Test]
		public void DeleteSyntaxWithCompositeId()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();

			s.CreateQuery("delete EntityWithCrazyCompositeKey where Id.Id = 1 and Id.OtherId = 2").ExecuteUpdate();
			s.CreateQuery("delete from EntityWithCrazyCompositeKey where Id.Id = 1 and Id.OtherId = 2").ExecuteUpdate();
			s.CreateQuery("delete from EntityWithCrazyCompositeKey e where e.Id.Id = 1 and e.Id.OtherId = 2").ExecuteUpdate();

			t.Commit();
			s.Close();
		}

		#endregion

		private class TestData
		{
			private readonly BulkManipulation tc;
			public Animal Polliwog;
			public Animal Catepillar;
			public Animal Frog;
			public Animal Butterfly;

			public Zoo Zoo;
			public Zoo PettingZoo;

			public TestData(BulkManipulation tc)
			{
				this.tc = tc;
			}

			public void Prepare()
			{
				ISession s = tc.OpenNewSession();
				ITransaction txn = s.BeginTransaction();

				Polliwog = new Animal { BodyWeight = 12, Description = "Polliwog" };

				Catepillar = new Animal { BodyWeight = 10, Description = "Catepillar" };

				Frog = new Animal { BodyWeight = 34, Description = "Frog" };

				Polliwog.Father = Frog;
				Frog.AddOffspring(Polliwog);

				Butterfly = new Animal { BodyWeight = 9, Description = "Butterfly" };

				Catepillar.Mother = Butterfly;
				Butterfly.AddOffspring(Catepillar);

				s.Save(Frog);
				s.Save(Polliwog);
				s.Save(Butterfly);
				s.Save(Catepillar);

				var dog = new Dog { BodyWeight = 200, Description = "dog" };
				s.Save(dog);

				var cat = new Cat { BodyWeight = 100, Description = "cat" };
				s.Save(cat);

				Zoo = new Zoo { Name = "Zoo" };
				var add = new Address { City = "MEL", Country = "AU", Street = "Main st", PostalCode = "3000" };
				Zoo.Address = add;

				PettingZoo = new PettingZoo { Name = "Petting Zoo" };
				var addr = new Address { City = "Sydney", Country = "AU", Street = "High st", PostalCode = "2000" };
				PettingZoo.Address = addr;

				s.Save(Zoo);
				s.Save(PettingZoo);

				var joiner = new Joiner { JoinedName = "joined-name", Name = "name" };
				s.Save(joiner);

				var car = new Car { Vin = "123c", Owner = "Kirsten" };
				s.Save(car);

				var truck = new Truck { Vin = "123t", Owner = "Steve" };
				s.Save(truck);

				var suv = new SUV { Vin = "123s", Owner = "Joe" };
				s.Save(suv);

				var pickup = new Pickup { Vin = "123p", Owner = "Cecelia" };
				s.Save(pickup);

				var b = new BooleanLiteralEntity();
				s.Save(b);

				txn.Commit();
				s.Close();
			}

			public void Cleanup()
			{
				ISession s = tc.OpenNewSession();
				ITransaction txn = s.BeginTransaction();

				// workaround awesome HSQLDB "feature"
				s.CreateQuery("delete from Animal where mother is not null or father is not null").ExecuteUpdate();
				s.CreateQuery("delete from Animal").ExecuteUpdate();
				s.CreateQuery("delete from Zoo").ExecuteUpdate();
				s.CreateQuery("delete from Joiner").ExecuteUpdate();
				s.CreateQuery("delete from Vehicle").ExecuteUpdate();
				s.CreateQuery("delete from BooleanLiteralEntity").ExecuteUpdate();

				txn.Commit();
				s.Close();
			}
		}
	}
}