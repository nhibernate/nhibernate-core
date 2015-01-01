using NHibernate.Dialect;
using NHibernate.DomainModel;
using NHibernate.Hql.Ast.ANTLR;
using NHibernate.Id;
using NHibernate.Linq;
using NHibernate.Persister.Entity;
using NHibernate.Test.NHSpecificTest.NH3488.Domain;
using NUnit.Framework;
using System;
using System.Collections;
using System.Linq;
using System.Threading;

namespace NHibernate.Test.NHSpecificTest.NH3488
{
	[TestFixture]
	public class LinqBulkManipulationFixture : BaseFixture
	{
		public ISession OpenNewSession()
		{
			return OpenSession();
		}

	
		#region INSERTS

		[Test]
		public void SimpleInsert()
		{
			var data = new TestData(this);
			data.Prepare();

			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();

			s.Query<Car>().Insert().As(x=>new Pickup{Id=x.Id,Vin=x.Vin,Owner=x.Owner});

			t.Commit();
			t = s.BeginTransaction();

			s.CreateQuery("delete Vehicle").ExecuteUpdate();

			t.Commit();
			s.Close();

			data.Cleanup();
		}

		[Test]
		public void SimpleInsertFromAggregate()
		{
			var data = new TestData(this);
			data.Prepare();

			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();

			s.Query<Car>()
				.GroupBy(x => x.Id)
				.Select(x => new { Id = x.Key, Vin = x.Max(y => y.Vin), Owner = x.Max(y => y.Owner) })
				.Insert().As(x => new Pickup { Id = x.Id, Vin = x.Vin, Owner = x.Owner });

			t.Commit();
			t = s.BeginTransaction();

			s.CreateQuery("delete Vehicle").ExecuteUpdate();

			t.Commit();
			s.Close();

			data.Cleanup();
		}

		[Test]
		public void SimpleInsertFromLimited()
		{
			var data = new TestData(this);
			data.Prepare();

			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();

			s.Query<Car>()
				.Skip(1)
				.Take(1)
				.Insert().As(x => new Pickup { Id = x.Id, Vin = x.Vin, Owner = x.Owner });

			t.Commit();
			t = s.BeginTransaction();

			s.CreateQuery("delete Vehicle").ExecuteUpdate();

			t.Commit();
			s.Close();

			data.Cleanup();
		}

		[Test]
		public void SimpleInsertWithConstants()
		{
			var data = new TestData(this);
			data.Prepare();

			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();

			s.Query<Car>()
				.Insert().Into<Pickup>(x => x.Set(y=>y.Id,y=>y.Id).Set(y=>y.Vin,y=>y.Vin).Set(y=>y.Owner,"The owner"));

			t.Commit();
			t = s.BeginTransaction();

			s.CreateQuery("delete Vehicle").ExecuteUpdate();

			t.Commit();
			s.Close();

			data.Cleanup();
		}

		[Test]
		public void SimpleInsertFromProjection()
		{
			var data = new TestData(this);
			data.Prepare();

			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();

			s.Query<Car>()
				.Select(x=>new {x.Id,x.Owner,UpperOwner=x.Owner.ToUpper()})
				.Insert().Into<Pickup>(x => x.Set(y => y.Id, y => y.Id).Set(y => y.Vin, y => y.UpperOwner));

			t.Commit();
			t = s.BeginTransaction();

			s.CreateQuery("delete Vehicle").ExecuteUpdate();

			t.Commit();
			s.Close();

			data.Cleanup();
		}

		[Test]
		public void InsertWithClientSideRequirementsThrowsException()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();

			Assert.Throws<NotSupportedException>(() => 
				s.Query<Car>()
			   .Insert().As(x => new Pickup {Id = x.Id, Vin = x.Vin, Owner = x.Owner.PadRight(200)}));
			
			t.Commit();
			t = s.BeginTransaction();

			s.CreateQuery("delete Vehicle").ExecuteUpdate();

			t.Commit();
			s.Close();
		}

		
		[Test]
		public void InsertWithManyToOne()
		{
			var data = new TestData(this);
			data.Prepare();

			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();

			s.Query<Human>()
			 .Insert().As(x => new Animal {Description = x.Description, BodyWeight = x.BodyWeight, Mother = x.Mother});
			
			t.Commit();
			t = s.BeginTransaction();

			t.Commit();
			s.Close();

			data.Cleanup();
		}


		[Test]
		public void InsertWithManyToOneAsParameter()
		{
			var data = new TestData(this);
			data.Prepare();

			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();

			s.Query<Human>()
			 .Insert().As(x => new Animal { Description = x.Description, BodyWeight = x.BodyWeight, Mother = data.Butterfly });

			t.Commit();
			s.Close();

			data.Cleanup();
		}

		[Test]
		public void InsertWithManyToOneWithCompositeKey()
		{
		

			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();

			var parent = new EntityWithCrazyCompositeKey {Id = new CrazyCompositeKey {Id=1, OtherId=1}, Name = "Parent"};

			s.Save(parent);

			t.Commit();
			t = s.BeginTransaction();

			s.Query<EntityWithCrazyCompositeKey>()
			 .Insert().As(x => new EntityReferencingEntityWithCrazyCompositeKey { Name = "Child", Parent = x });

			t.Commit();
			t = s.BeginTransaction();

			s.CreateQuery("delete EntityReferencingEntityWithCrazyCompositeKey").ExecuteUpdate();
			s.CreateQuery("delete EntityWithCrazyCompositeKey").ExecuteUpdate();

			t.Commit();
			s.Close();

			
		}

		

		[Test]
		public void InsertIntoSuperclassPropertiesFails()
		{
			var data = new TestData(this);
			data.Prepare();

			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();

			Assert.Throws<QueryException>(
				() => s.Query<Lizard>().Insert().As(x=>new Human{Id=x.Id,BodyWeight = x.BodyWeight}),
				"superclass prop insertion did not error");

			t.Commit();
			t = s.BeginTransaction();

			s.CreateQuery("delete Animal where Mother is not null").ExecuteUpdate();
			s.CreateQuery("delete Animal where Father is not null").ExecuteUpdate();
			s.CreateQuery("delete Animal").ExecuteUpdate();

			t.Commit();
			s.Close();

			data.Cleanup();
		}

		[Test]
		public void InsertAcrossMappedJoinFails()
		{
			var data = new TestData(this);
			data.Prepare();

			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();

			Assert.Throws<QueryException>(
				() => s.Query<Car>().Insert().As(x=>new Joiner{Name = x.Vin,JoinedName = x.Owner}),
				"mapped-join insertion did not error");

			t.Commit();
			t = s.BeginTransaction();

			s.CreateQuery("delete Joiner").ExecuteUpdate();
			s.CreateQuery("delete Vehicle").ExecuteUpdate();

			t.Commit();
			s.Close();

			data.Cleanup();
		}

		public void InsertWithGeneratedId()
		{
			// Make sure the env supports bulk inserts with generated ids...
			IEntityPersister persister = Sfi.GetEntityPersister(typeof(PettingZoo).FullName);
			IIdentifierGenerator generator = persister.IdentifierGenerator;
			if (!HqlSqlWalker.SupportsIdGenWithBulkInsertion(generator))
			{
				return;
			}

			// create a Zoo
			var zoo = new Zoo { Name = "zoo" };

			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			s.Save(zoo);
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			int count = s.Query<Zoo>().Insert().As(x=>new PettingZoo{Name=x.Name});
			t.Commit();
			s.Close();
			Assert.That(count, Is.EqualTo(1), "unexpected insertion count");

			s = OpenSession();
			t = s.BeginTransaction();
			var pz = (PettingZoo)s.CreateQuery("from PettingZoo").UniqueResult();
			t.Commit();
			s.Close();

			Assert.That(zoo.Name, Is.EqualTo(pz.Name));
			Assert.That(zoo.Id != pz.Id);

			s = OpenSession();
			t = s.BeginTransaction();
			s.CreateQuery("delete Zoo").ExecuteUpdate();
			t.Commit();
			s.Close();
		}

		[Test]
		public void InsertWithGeneratedVersionAndId()
		{
			// Make sure the env supports bulk inserts with generated ids...
			IEntityPersister persister = Sfi.GetEntityPersister(typeof(IntegerVersioned).FullName);
			IIdentifierGenerator generator = persister.IdentifierGenerator;
			if (!HqlSqlWalker.SupportsIdGenWithBulkInsertion(generator))
			{
				return;
			}

			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();

			var entity = new IntegerVersioned { Name = "int-vers" };
			s.Save(entity);
			s.CreateQuery("select Id, Name, Version from IntegerVersioned").List();
			t.Commit();
			s.Close();

			long initialId = entity.Id;
			int initialVersion = entity.Version;

			s = OpenSession();
			t = s.BeginTransaction();
			int count =
				s.Query<IntegerVersioned>()
				 .Where(x => x.Id == entity.Id)
				 .Insert().As(x => new IntegerVersioned {Name = x.Name, Data = x.Data});
			t.Commit();
			s.Close();

			Assert.That(count, Is.EqualTo(1), "unexpected insertion count");

			s = OpenSession();
			t = s.BeginTransaction();
			var created =
				(IntegerVersioned)
				s.CreateQuery("from IntegerVersioned where Id <> :initialId").SetInt64("initialId", initialId).UniqueResult();
			t.Commit();
			s.Close();

			Assert.That(created.Version, Is.EqualTo(initialVersion), "version was not seeded");

			s = OpenSession();
			t = s.BeginTransaction();
			s.CreateQuery("delete IntegerVersioned").ExecuteUpdate();
			t.Commit();
			s.Close();
		}

		[Test]
		public void InsertWithGeneratedTimestampVersion()
		{
			// Make sure the env supports bulk inserts with generated ids...
			IEntityPersister persister = Sfi.GetEntityPersister(typeof(TimestampVersioned).FullName);
			IIdentifierGenerator generator = persister.IdentifierGenerator;
			if (!HqlSqlWalker.SupportsIdGenWithBulkInsertion(generator))
			{
				return;
			}

			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();

			var entity = new TimestampVersioned { Name = "int-vers" };
			s.Save(entity);
			s.CreateQuery("select Id, Name, Version from TimestampVersioned").List();
			t.Commit();
			s.Close();

			long initialId = entity.Id;
			//Date initialVersion = entity.getVersion();

			s = OpenSession();
			t = s.BeginTransaction();
			int count =
				s.Query<TimestampVersioned>()
				 .Where(x => x.Id == entity.Id)
				 .Insert().As(x => new TimestampVersioned {Name = x.Name, Data = x.Data});

			t.Commit();
			s.Close();

			Assert.That(count, Is.EqualTo(1), "unexpected insertion count");

			s = OpenSession();
			t = s.BeginTransaction();
			var created =
				(TimestampVersioned)
				s.CreateQuery("from TimestampVersioned where Id <> :initialId").SetInt64("initialId", initialId).UniqueResult();
			t.Commit();
			s.Close();

			Assert.That(created.Version, Is.GreaterThan(DateTime.Today));

			s = OpenSession();
			t = s.BeginTransaction();
			s.CreateQuery("delete TimestampVersioned").ExecuteUpdate();
			t.Commit();
			s.Close();
		}

		[Test]
		public void InsertWithSelectListUsingJoins()
		{
			// this is just checking parsing and syntax...
			ISession s = OpenSession();
			s.BeginTransaction();

			s.Query<Human>().Where(x=>x.Mother.Mother!=null)
			 .Insert().As(x => new Animal { Description = x.Description, BodyWeight = x.BodyWeight });
			
			s.CreateQuery("delete from Animal").ExecuteUpdate();
			s.Transaction.Commit();
			s.Close();
		}

		[Test]
		public void InsertToComponent()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();

			var human = new SimpleClassWithComponent { Name = new Name { First = "Stevee", Initial = 'X', Last = "Ebersole" } };

			s.Save(human);
			t.Commit();

			string correctName = "Steve";

			t = s.BeginTransaction();
			int count =
				s.Query<SimpleClassWithComponent>().Insert().Into<SimpleClassWithComponent>(x => x.Set(y => y.Name.First, y => correctName));
			Assert.That(count, Is.EqualTo(1), "incorrect insert count");

			count =
				s.Query<SimpleClassWithComponent>()
				 .Where(x=>x.Name.First==correctName)
				 .Insert().As(x => new SimpleClassWithComponent {Name = new Name {First = x.Name.First,Last=x.Name.Last,Initial = 'Z'}});
			Assert.That(count, Is.EqualTo(1), "incorrect insert count");
			t.Commit();

			t = s.BeginTransaction();
			
			s.CreateQuery("delete SimpleClassWithComponent").ExecuteUpdate();
			t.Commit();

			s.Close();
		}

		#endregion


		#region UPDATES

		
		[Test]
		public void UpdateWithWhereExistsSubquery()
		{
			// multi-table ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			var joe = new Human { Name = new Name { First = "Joe", Initial = 'Q', Last = "Public" } };
			s.Save(joe);
			var doll = new Human { Name = new Name { First = "Kyu", Initial = 'P', Last = "Doll" }, Friends = new[] { joe } };
			s.Save(doll);
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();

			int count = s.Query<Human>()
			 .Where(x => x.Friends.OfType<Human>().Any(f => f.Name.Last == "Public"))
			 .Update().Assign(x => x.Set(y => y.Description, "updated"));
			Assert.That(count, Is.EqualTo(1));
			s.Delete(doll);
			s.Delete(joe);
			t.Commit();
			s.Close();

			// single-table (one-to-many & many-to-many) ~~~~~~~~~~~~~~~~~~~~~~~~~~
			s = OpenSession();
			t = s.BeginTransaction();
			var entity = new SimpleEntityWithAssociation();
			var other = new SimpleEntityWithAssociation();
			entity.Name = "main";
			other.Name = "many-to-many-association";
			entity.ManyToManyAssociatedEntities.Add(other);
			entity.AddAssociation("one-to-many-association");
			s.Save(entity);
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			// one-to-many test

			count = s.Query<SimpleEntityWithAssociation>()
			 .Where(x => x.AssociatedEntities.Any(a => a.Name == "one-to-many-association"))
			 .Update().Assign(x => x.Set(y => y.Name, "updated"));
			Assert.That(count, Is.EqualTo(1));
			// many-to-many test
			if (Dialect.SupportsSubqueryOnMutatingTable)
			{
				count = s.Query<SimpleEntityWithAssociation>()
				 .Where(x => x.ManyToManyAssociatedEntities.Any(a => a.Name == "many-to-many-association"))
				 .Update().Assign(x => x.Set(y => y.Name, "updated"));
				
				Assert.That(count, Is.EqualTo(1));
			}
			IEnumerator mtm = entity.ManyToManyAssociatedEntities.GetEnumerator();
			mtm.MoveNext();
			s.Delete(mtm.Current);
			s.Delete(entity);
			t.Commit();
			s.Close();
		}

		[Test]
		public void IncrementCounterVersion()
		{
			IntegerVersioned entity;

			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				entity = new IntegerVersioned { Name = "int-vers", Data = "foo" };
				s.Save(entity);
				t.Commit();
			}

			int initialVersion = entity.Version;

			using (ISession s = OpenSession())
			{
				using (ITransaction t = s.BeginTransaction())
				{
					// Note: Update more than one column to showcase NH-3624, which involved losing some columns. /2014-07-26
					int count =
						s.Query<IntegerVersioned>()
						 .Update().Assign(x => x.Set(y => y.Name, y => y.Name + "upd").Set(y => y.Data, y => y.Data + "upd"), true);
					Assert.That(count, Is.EqualTo(1), "incorrect exec count");
					t.Commit();
				}

				using (ITransaction t = s.BeginTransaction())
				{
					entity = s.Get<IntegerVersioned>(entity.Id);
					s.Delete(entity);
					t.Commit();
				}
			}

			Assert.That(entity.Version, Is.EqualTo(initialVersion + 1), "version not incremented");
			Assert.That(entity.Name, Is.EqualTo("int-versupd"));
			Assert.That(entity.Data, Is.EqualTo("fooupd"));
		}

		[Test]
		public void IncrementTimestampVersion()
		{
			TimestampVersioned entity;

			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				entity = new TimestampVersioned { Name = "ts-vers", Data = "foo" };
				s.Save(entity);
				t.Commit();
			}

			DateTime initialVersion = entity.Version;

			Thread.Sleep(1300);

			using (ISession s = OpenSession())
			{
				using (ITransaction t = s.BeginTransaction())
				{
					// Note: Update more than one column to showcase NH-3624, which involved losing some columns. /2014-07-26
					int count = s.Query<TimestampVersioned>().
									  Update().Assign(x => x.Set(y => y.Name, y => y.Name + "upd").Set(y => y.Data, y => y.Data + "upd"), true);
					Assert.That(count, Is.EqualTo(1), "incorrect exec count");
					t.Commit();
				}

				using (ITransaction t = s.BeginTransaction())
				{
					entity = s.Load<TimestampVersioned>(entity.Id);
					s.Delete(entity);
					t.Commit();
				}
			}

			Assert.That(entity.Version, Is.GreaterThan(initialVersion), "version not incremented");
			Assert.That(entity.Name, Is.EqualTo("ts-versupd"));
			Assert.That(entity.Data, Is.EqualTo("fooupd"));
		}

		[Test]
		public void UpdateOnComponent()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();

			var human = new Human { Name = new Name { First = "Stevee", Initial = 'X', Last = "Ebersole" } };

			s.Save(human);
			t.Commit();

			string correctName = "Steve";

			t = s.BeginTransaction();
			int count =
				s.Query<Human>().Where(x => x.Id == human.Id).Update().As(x => new Human{Name={First = correctName}});
			
			Assert.That(count, Is.EqualTo(1), "incorrect update count");
			t.Commit();

			t = s.BeginTransaction();
			s.Refresh(human);

			Assert.That(human.Name.First, Is.EqualTo(correctName), "Update did not execute properly");

			s.CreateQuery("delete Human").ExecuteUpdate();
			t.Commit();

			s.Close();
		}

		[Test]
		public void UpdateWithClientSideRequirementsThrowsException()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();

			var human = new Human { Name = new Name { First = "Stevee", Initial = 'X', Last = "Ebersole" } };

			s.Save(human);
			t.Commit();

			t = s.BeginTransaction();
			
			Assert.Throws<NotSupportedException>(()=>
				s.Query<Human>().Where(x => x.Id == human.Id).Update().As(x => new Human { Name = { First = x.Name.First.PadLeft(200) } })
			);

			t.Commit();

			t = s.BeginTransaction();
			
			s.CreateQuery("delete Human").ExecuteUpdate();
			t.Commit();

			s.Close();
		}

		[Test]
		public void UpdateOnManyToOne()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			s.Query<Animal>().Where(x => x.Id == 2).Update().Assign(x => x.Set(y => y.Mother, y => null));
			
			if (!(Dialect is MySQLDialect))
			{
				// MySQL does not support (even un-correlated) subqueries against the update-mutating table
				s.Query<Animal>().Where(x => x.Id == 2).Update().Assign(x => x.Set(y => y.Mother, y => s.Query<Animal>().First(z => z.Id == 1)));
			}

			t.Commit();
			s.Close();
		}

		

		[Test]
		public void UpdateOnDiscriminatorSubclass()
		{
			var data = new TestData(this);
			data.Prepare();

			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();

			int count = s.Query<PettingZoo>().Update().Assign(x => x.Set(y => y.Name, y => y.Name));
			Assert.That(count, Is.EqualTo(1), "Incorrect discrim subclass update count");

			t.Rollback();
			t = s.BeginTransaction();

			count = s.Query<PettingZoo>().Where(x => x.Id == data.PettingZoo.Id).Update().Assign(x => x.Set(y => y.Name, y => y.Name));
			Assert.That(count, Is.EqualTo(1), "Incorrect discrim subclass update count");

			t.Rollback();
			t = s.BeginTransaction();

			count = s.Query<Zoo>().Update().Assign(x => x.Set(y => y.Name, y => y.Name));
			Assert.That(count, Is.EqualTo(2), "Incorrect discrim subclass update count");

			t.Rollback();
			t = s.BeginTransaction();

			// TODO : not so sure this should be allowed.  Seems to me that if they specify an alias,
			// property-refs should be required to be qualified.
			count = s.Query<Zoo>().Where(x => x.Id == data.Zoo.Id).Update().Assign(x => x.Set(y => y.Name, y => y.Name));
			Assert.That(count, Is.EqualTo(1), "Incorrect discrim subclass update count");

			t.Commit();
			s.Close();

			data.Cleanup();
		}

		[Test]
		public void UpdateOnAnimal()
		{
			var data = new TestData(this);
			data.Prepare();

			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();

			//int count = s.Query<Animal>().Where(x => x.Description == data.Frog.Description).Update(x => x.Set(y => y.Description, y => y.Description));
			//Assert.That(count, Is.EqualTo(1), "Incorrect entity-updated count");

			int count =
				s.Query<Animal>().Where(x => x.Description == data.Polliwog.Description).Update().Assign(x => x.Set(y => y.Description, y => "Tadpole"));
			Assert.That(count, Is.EqualTo(1), "Incorrect entity-updated count");

			var tadpole = s.Load<Animal>(data.Polliwog.Id);

			Assert.That(tadpole.Description, Is.EqualTo("Tadpole"), "Update did not take effect");

			count =
				s.Query<Dragon>().Update().Assign(x => x.Set(y => y.FireTemperature, 300));
			Assert.That(count, Is.EqualTo(1), "Incorrect entity-updated count");


			count =
				s.Query<Animal>().Update().Assign(x => x.Set(y => y.BodyWeight, y => y.BodyWeight + 1 + 1));
			Assert.That(count, Is.EqualTo(7), "incorrect count on 'complex' update assignment");

			if (!(Dialect is MySQLDialect))
			{
				// MySQL does not support (even un-correlated) subqueries against the update-mutating table
				s.Query<Animal>().Update().Assign(x => x.Set(y => y.BodyWeight, y => s.Query<Animal>().Max(z => z.BodyWeight)));
			}

			t.Commit();
			s.Close();

			data.Cleanup();
		}

		[Test]
		public void UpdateOnDragonWithProtectedProperty()
		{
			var data = new TestData(this);
			data.Prepare();

			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();

			int count =
				s.Query<Dragon>().Update().Assign(x => x.Set(y => y.FireTemperature, 300));
			Assert.That(count, Is.EqualTo(1), "Incorrect entity-updated count");

			t.Commit();
			s.Close();

			data.Cleanup();
		}

		[Test]
		public void UpdateMultiplePropertyOnAnimal()
		{
			var data = new TestData(this);
			data.Prepare();

			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				int count =

					s.Query<Animal>()
					 .Where(x => x.Description == data.Polliwog.Description)
					 .Update().Assign(x => x.Set(y => y.Description, y => "Tadpole").Set(y => y.BodyWeight, 3));

					

				Assert.That(count, Is.EqualTo(1));
				t.Commit();
			}

			using (ISession s = OpenSession())
			using (s.BeginTransaction())
			{
				var tadpole = s.Get<Animal>(data.Polliwog.Id);
				Assert.That(tadpole.Description, Is.EqualTo("Tadpole"));
				Assert.That(tadpole.BodyWeight, Is.EqualTo(3));
			}

			data.Cleanup();
		}

		[Test]
		public void UpdateOnMammal()
		{
			var data = new TestData(this);
			data.Prepare();

			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();

			int count = s.Query<Mammal>().Update().Assign(x=>x.Set(y=>y.Description,y=>y.Description));


			Assert.That(count, Is.EqualTo(2), "incorrect update count against 'middle' of joined-subclass hierarchy");

			count = s.Query<Mammal>().Update().Assign(x => x.Set(y => y.BodyWeight, 25));
			Assert.That(count, Is.EqualTo(2), "incorrect update count against 'middle' of joined-subclass hierarchy");

			if (!(Dialect is MySQLDialect))
			{
				// MySQL does not support (even un-correlated) subqueries against the update-mutating table
				count = s.Query<Mammal>().Update().Assign(x => x.Set(y => y.BodyWeight, y => s.Query<Animal>().Max(z => z.BodyWeight)));
				Assert.That(count, Is.EqualTo(2), "incorrect update count against 'middle' of joined-subclass hierarchy");
			}

			t.Commit();
			s.Close();

			data.Cleanup();
		}

		[Test]
		public void UpdateSetNullUnionSubclass()
		{
			var data = new TestData(this);
			data.Prepare();

			// These should reach out into *all* subclass tables...
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();

			int count = s.Query<Vehicle>().Update().Assign(x => x.Set(y => y.Owner, "Steve"));
			Assert.That(count, Is.EqualTo(4), "incorrect restricted update count");
			count = s.Query<Vehicle>().Where(x => x.Owner == "Steve").Update().Assign(x => x.Set(y => y.Owner, (string)null));
			Assert.That(count, Is.EqualTo(4), "incorrect restricted update count");

			count = s.CreateQuery("delete Vehicle where Owner is null").ExecuteUpdate();
			Assert.That(count, Is.EqualTo(4), "incorrect restricted update count");

			t.Commit();
			s.Close();

			data.Cleanup();
		}

		[Test]
		public void UpdateSetNullOnDiscriminatorSubclass()
		{
			var data = new TestData(this);
			data.Prepare();

			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();

			int count = s.Query<PettingZoo>().Update().Assign(x => x.Set(y => y.Address.City, (string)null));
				
			Assert.That(count, Is.EqualTo(1), "Incorrect discrim subclass delete count");
			count = s.CreateQuery("delete Zoo where Address.City is null").ExecuteUpdate();
			Assert.That(count, Is.EqualTo(1), "Incorrect discrim subclass delete count");

			count = s.Query<Zoo>().Update().Assign(x => x.Set(y => y.Address.City, (string)null));
			Assert.That(count, Is.EqualTo(1), "Incorrect discrim subclass delete count");
			count = s.CreateQuery("delete Zoo where Address.City is null").ExecuteUpdate();
			Assert.That(count, Is.EqualTo(1), "Incorrect discrim subclass delete count");

			t.Commit();
			s.Close();

			data.Cleanup();
		}

		[Test]
		public void UpdateSetNullOnJoinedSubclass()
		{
			var data = new TestData(this);
			data.Prepare();

			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();

			int count = s.Query<Mammal>().Update().Assign(x => x.Set(y => y.BodyWeight, -1));
			Assert.That(count, Is.EqualTo(2), "Incorrect deletion count on joined subclass");

			count = s.CreateQuery("delete Animal where BodyWeight = -1").ExecuteUpdate();
			Assert.That(count, Is.EqualTo(2), "Incorrect deletion count on joined subclass");

			t.Commit();
			s.Close();

			data.Cleanup();
		}

		#endregion


		#region DELETES

		[Test]
		public void DeleteWithSubquery()
		{
			// setup the test data...
			ISession s = OpenSession();
			s.BeginTransaction();
			var owner = new SimpleEntityWithAssociation {Name = "myEntity-1"};
			owner.AddAssociation("assoc-1");
			owner.AddAssociation("assoc-2");
			owner.AddAssociation("assoc-3");
			s.Save(owner);
			var owner2 = new SimpleEntityWithAssociation {Name = "myEntity-2"};
			owner2.AddAssociation("assoc-1");
			owner2.AddAssociation("assoc-2");
			owner2.AddAssociation("assoc-3");
			owner2.AddAssociation("assoc-4");
			s.Save(owner2);
			var owner3 = new SimpleEntityWithAssociation {Name = "myEntity-3"};
			s.Save(owner3);
			s.Transaction.Commit();
			s.Close();

			// now try the bulk delete
			s = OpenSession();
			s.BeginTransaction();
			int count = s.Query<SimpleEntityWithAssociation>().Where(x=>x.AssociatedEntities.Count==0 && x.Name.Contains("")).Delete();
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

			int count = s.Query<Animal>().Where(x=>x.Id==data.Polliwog.Id).Delete();
			Assert.That(count, Is.EqualTo(1), "Incorrect delete count");

			count = s.Query<Animal>().Where(x => x.Id == data.Catepillar.Id).Delete();
			Assert.That(count, Is.EqualTo(1), "Incorrect delete count");

			// HHH-873...
			if (Dialect.SupportsSubqueryOnMutatingTable)
			{
				count = s.Query<User>().Where(x=>s.Query<User>().Contains(x)).Delete();
				Assert.That(count, Is.EqualTo(0));
			}

			count = s.Query<Animal>().Delete();
			Assert.That(count, Is.EqualTo(5), "Incorrect delete count");

			IList list = s.Query<Animal>().ToList();
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

			int count = s.Query<PettingZoo>().Delete();
			Assert.That(count, Is.EqualTo(1), "Incorrect discrim subclass delete count");

			count = s.Query<Zoo>().Delete();
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

			int count = s.Query<Mammal>().Where(x=>x.BodyWeight>150).Delete();
			Assert.That(count, Is.EqualTo(1), "Incorrect deletion count on joined subclass");

			count = s.Query<Mammal>().Delete();
			Assert.That(count, Is.EqualTo(1), "Incorrect deletion count on joined subclass");

			count = s.Query<SubMulti>().Delete();
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

			int count = s.Query<Joiner>().Where(x=>x.JoinedName == "joined-name").Delete();
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

			int count = s.Query<Vehicle>().Where(x => x.Owner == "Steve").Delete();
			Assert.That(count, Is.EqualTo(1), "incorrect restricted update count");

			count = s.Query<Vehicle>().Delete();
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

			int count = s.Query<Truck>().Where(x=>x.Owner =="Steve").Delete();
			Assert.That(count, Is.EqualTo(1), "incorrect restricted update count");

			count = s.Query<Truck>().Delete();
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

			int count = s.Query<Car>().Where(x=>x.Owner == "Kirsten").Delete();
			Assert.That(count, Is.EqualTo(1), "incorrect restricted update count");

			count = s.Query<Car>().Delete();
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

			int count = s.Query<Animal>().Where(x=>x.Mother == data.Butterfly).Delete();
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

			s.Query<EntityWithCrazyCompositeKey>().Where(x=>x.Id.Id == 1 && x.Id.OtherId == 2).Delete();
			
			t.Commit();
			s.Close();
		}

		#endregion

		private class TestData
		{
			private readonly LinqBulkManipulationFixture tc;
			public Animal Polliwog;
			public Animal Catepillar;
			public Animal Frog;
			public Animal Butterfly;

			public Zoo Zoo;
			public Zoo PettingZoo;

			public TestData(LinqBulkManipulationFixture tc)
			{
				this.tc = tc;
			}

			public void Prepare()
			{
				ISession s = tc.OpenNewSession();
				ITransaction txn = s.BeginTransaction();

				Polliwog = new Animal {BodyWeight = 12, Description = "Polliwog"};

				Catepillar = new Animal {BodyWeight = 10, Description = "Catepillar"};

				Frog = new Animal {BodyWeight = 34, Description = "Frog"};

				Polliwog.Father = Frog;
				Frog.AddOffspring(Polliwog);

				Butterfly = new Animal {BodyWeight = 9, Description = "Butterfly"};

				Catepillar.Mother = Butterfly;
				Butterfly.AddOffspring(Catepillar);

				s.Save(Frog);
				s.Save(Polliwog);
				s.Save(Butterfly);
				s.Save(Catepillar);

				var dog = new Dog {BodyWeight = 200, Description = "dog"};
				s.Save(dog);

				var cat = new Cat {BodyWeight = 100, Description = "cat"};
				s.Save(cat);

				var dragon = new Dragon();
				dragon.SetFireTemperature(200);
				s.Save(dragon);

				Zoo = new Zoo {Name = "Zoo"};
				var add = new Address {City = "MEL", Country = "AU", Street = "Main st", PostalCode = "3000"};
				Zoo.Address = add;

				PettingZoo = new PettingZoo {Name = "Petting Zoo"};
				var addr = new Address {City = "Sydney", Country = "AU", Street = "High st", PostalCode = "2000"};
				PettingZoo.Address = addr;

				s.Save(Zoo);
				s.Save(PettingZoo);

				var joiner = new Joiner {JoinedName = "joined-name", Name = "name"};
				s.Save(joiner);

				var car = new Car {Vin = "123c", Owner = "Kirsten"};
				s.Save(car);

				var truck = new Truck {Vin = "123t", Owner = "Steve"};
				s.Save(truck);

				var suv = new SUV {Vin = "123s", Owner = "Joe"};
				s.Save(suv);

				var pickup = new Pickup {Vin = "123p", Owner = "Cecelia"};
				s.Save(pickup);

				txn.Commit();
				s.Close();
			}

			public void Cleanup()
			{
				ISession s = tc.OpenNewSession();
				ITransaction txn = s.BeginTransaction();

				// workaround awesome HSQLDB "feature"
				s.CreateQuery("delete from Animal where Mother is not null or Father is not null").ExecuteUpdate();
				s.CreateQuery("delete from Animal").ExecuteUpdate();
				s.CreateQuery("delete from Zoo").ExecuteUpdate();
				s.CreateQuery("delete from Joiner").ExecuteUpdate();
				s.CreateQuery("delete from Vehicle").ExecuteUpdate();
				
				txn.Commit();
				s.Close();
			}
		}
	}
}