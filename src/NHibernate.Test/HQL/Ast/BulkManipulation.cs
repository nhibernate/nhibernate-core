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

		[Test, Ignore("Not supported")]
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
				Frog.Offspring.Add(Polliwog);

				Butterfly = new Animal { BodyWeight = 9, Description = "Butterfly" };

				Catepillar.Mother = Butterfly;
				Butterfly.Offspring.Add(Catepillar);

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