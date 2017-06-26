using System;
using System.Collections;
using System.Threading;
using NHibernate.Dialect;
using NHibernate.Hql.Ast.ANTLR;
using NHibernate.Id;
using NHibernate.Persister.Entity;
using NUnit.Framework;

namespace NHibernate.Test.Hql.Ast
{
	[TestFixture]
	public class BulkManipulation : BaseFixture
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
				Assert.Throws<QuerySyntaxException>(
					() => s.CreateQuery("update NonExistentEntity e set e.someProp = ?").ExecuteUpdate());
			}
		}

		#endregion

		#region INSERTS

		[Test]
		public void SimpleInsert()
		{
			var data = new TestData(this);
			data.Prepare();

			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();

			s.CreateQuery("insert into Pickup (id, Vin, Owner) select id, Vin, Owner from Car").ExecuteUpdate();

			t.Commit();
			t = s.BeginTransaction();

			s.CreateQuery("delete Vehicle").ExecuteUpdate();

			t.Commit();
			s.Close();

			data.Cleanup();
		}

		[Test]
		public void InsertWithManyToOne()
		{
			var data = new TestData(this);
			data.Prepare();

			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();

			s.CreateQuery(
				"insert into Animal (description, bodyWeight, mother) select description, bodyWeight, mother from Human").
				ExecuteUpdate();

			t.Commit();
			t = s.BeginTransaction();

			t.Commit();
			s.Close();

			data.Cleanup();
		}

		[Test]
		public void InsertWithMismatchedTypes()
		{
			var data = new TestData(this);
			data.Prepare();

			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			Assert.Throws<QueryException>(
			  () => s.CreateQuery("insert into Pickup (Owner, Vin, id) select id, Vin, Owner from Car").ExecuteUpdate(),
				"mismatched types did not error");

			t.Commit();
			t = s.BeginTransaction();

			s.CreateQuery("delete Vehicle").ExecuteUpdate();

			t.Commit();
			s.Close();

			data.Cleanup();
		}

		[Test]
		public void InsertIntoSuperclassPropertiesFails()
		{
			var data = new TestData(this);
			data.Prepare();

			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();

			Assert.Throws<QueryException>(
				() => s.CreateQuery("insert into Human (id, bodyWeight) select id, bodyWeight from Lizard").ExecuteUpdate(),
				"superclass prop insertion did not error");

			t.Commit();
			t = s.BeginTransaction();

			s.CreateQuery("delete Animal where mother is not null").ExecuteUpdate();
			s.CreateQuery("delete Animal where father is not null").ExecuteUpdate();
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
				() => s.CreateQuery("insert into Joiner (name, joinedName) select vin, owner from Car").ExecuteUpdate(),
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
			IEntityPersister persister = Sfi.GetEntityPersister(typeof (PettingZoo).FullName);
			IIdentifierGenerator generator = persister.IdentifierGenerator;
			if (!HqlSqlWalker.SupportsIdGenWithBulkInsertion(generator))
			{
				return;
			}

			// create a Zoo
			var zoo = new Zoo {Name = "zoo"};

			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			s.Save(zoo);
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			int count = s.CreateQuery("insert into PettingZoo (name) select name from Zoo").ExecuteUpdate();
			t.Commit();
			s.Close();
			Assert.That(count, Is.EqualTo(1), "unexpected insertion count");

			s = OpenSession();
			t = s.BeginTransaction();
			var pz = (PettingZoo) s.CreateQuery("from PettingZoo").UniqueResult();
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
			IEntityPersister persister = Sfi.GetEntityPersister(typeof (IntegerVersioned).FullName);
			IIdentifierGenerator generator = persister.IdentifierGenerator;
			if (!HqlSqlWalker.SupportsIdGenWithBulkInsertion(generator))
			{
				return;
			}

			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();

			var entity = new IntegerVersioned {Name = "int-vers"};
			s.Save(entity);
			s.CreateQuery("select id, name, version from IntegerVersioned").List();
			t.Commit();
			s.Close();

			long initialId = entity.Id;
			int initialVersion = entity.Version;

			s = OpenSession();
			t = s.BeginTransaction();
			int count =
				s.CreateQuery("insert into IntegerVersioned ( name, Data ) select name, Data from IntegerVersioned where id = :id")
					.SetInt64("id", entity.Id)
					.ExecuteUpdate();
			t.Commit();
			s.Close();

			Assert.That(count, Is.EqualTo(1), "unexpected insertion count");

			s = OpenSession();
			t = s.BeginTransaction();
			var created =
				(IntegerVersioned)
				s.CreateQuery("from IntegerVersioned where id <> :initialId").SetInt64("initialId", initialId).UniqueResult();
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
			IEntityPersister persister = Sfi.GetEntityPersister(typeof (TimestampVersioned).FullName);
			IIdentifierGenerator generator = persister.IdentifierGenerator;
			if (!HqlSqlWalker.SupportsIdGenWithBulkInsertion(generator))
			{
				return;
			}

			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();

			var entity = new TimestampVersioned {Name = "int-vers"};
			s.Save(entity);
			s.CreateQuery("select id, name, version from TimestampVersioned").List();
			t.Commit();
			s.Close();

			long initialId = entity.Id;
			//Date initialVersion = entity.getVersion();

			s = OpenSession();
			t = s.BeginTransaction();
			int count =
				s.CreateQuery(
					"insert into TimestampVersioned ( name, Data ) select name, Data from TimestampVersioned where id = :id")
					.SetInt64("id", entity.Id)
					.ExecuteUpdate();
			t.Commit();
			s.Close();

			Assert.That(count, Is.EqualTo(1), "unexpected insertion count");

			s = OpenSession();
			t = s.BeginTransaction();
			var created =
				(TimestampVersioned)
				s.CreateQuery("from TimestampVersioned where id <> :initialId").SetInt64("initialId", initialId).UniqueResult();
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
			s.CreateQuery(
				"insert into Animal (description, bodyWeight) select h.description, h.bodyWeight from Human h where h.mother.mother is not null")
				.ExecuteUpdate();
			s.CreateQuery("delete from Animal").ExecuteUpdate();
			s.Transaction.Commit();
			s.Close();
		}

		#endregion

		#region UPDATES

		[Test]
		public void IncorrectSyntax()
		{
			using (ISession s = OpenSession())
			{
				Assert.Throws<QueryException>(
					() => s.CreateQuery("update Human set Human.description = 'xyz' where Human.id = 1 and Human.description is null"));
			}
		}

		[Test]
		public void UpdateWithWhereExistsSubquery()
		{
			// multi-table ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			var joe = new Human {Name = new Name {First = "Joe", Initial = 'Q', Last = "Public"}};
			s.Save(joe);
			var doll = new Human {Name = new Name {First = "Kyu", Initial = 'P', Last = "Doll"}, Friends = new[] {joe}};
			s.Save(doll);
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			string updateQryString = "update Human h " + "set h.description = 'updated' " + "where exists ("
									 + "      select f.id " + "      from h.friends f " + "      where f.name.last = 'Public' "
									 + ")";
			int count = s.CreateQuery(updateQryString).ExecuteUpdate();
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
			updateQryString = "update SimpleEntityWithAssociation e set e.Name = 'updated' where "
							  + "exists(select a.id from e.AssociatedEntities a " + "where a.Name = 'one-to-many-association')";
			count = s.CreateQuery(updateQryString).ExecuteUpdate();
			Assert.That(count, Is.EqualTo(1));
			// many-to-many test
			if (Dialect.SupportsSubqueryOnMutatingTable)
			{
				updateQryString = "update SimpleEntityWithAssociation e set e.Name = 'updated' where "
								  + "exists(select a.id from e.ManyToManyAssociatedEntities a "
								  + "where a.Name = 'many-to-many-association')";
				count = s.CreateQuery(updateQryString).ExecuteUpdate();
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
				entity = new IntegerVersioned {Name = "int-vers", Data = "foo"};
				s.Save(entity);
				t.Commit();
			}

			int initialVersion = entity.Version;

			using (ISession s = OpenSession())
			{
				using (ITransaction t = s.BeginTransaction())
				{
					// Note: Update more than one column to showcase NH-3624, which involved losing some columns. /2014-07-26
					int count = s.CreateQuery("update versioned IntegerVersioned set name = concat(name, 'upd'), Data = concat(Data, 'upd')")
						.ExecuteUpdate();
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
			using(ITransaction t = s.BeginTransaction())
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
					int count = s.CreateQuery("update versioned TimestampVersioned set name = concat(name, 'upd'), Data = concat(Data, 'upd')")
						.ExecuteUpdate();
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

			var human = new Human {Name = new Name {First = "Stevee", Initial = 'X', Last = "Ebersole"}};

			s.Save(human);
			t.Commit();

			string correctName = "Steve";

			t = s.BeginTransaction();
			int count =
				s.CreateQuery("update Human set name.first = :correction where id = :id").SetString("correction", correctName).
					SetInt64("id", human.Id).ExecuteUpdate();
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
		public void UpdateOnManyToOne()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();

			s.CreateQuery("update Animal a set a.mother = null where a.id = 2").ExecuteUpdate();
			if (Dialect.SupportsSubqueryOnMutatingTable)
			{
				s.CreateQuery("update Animal a set a.mother = (from Animal where id = 1) where a.id = 2").ExecuteUpdate();
			}

			t.Commit();
			s.Close();
		}

		[Test]
		public void UpdateOnImplicitJoinFails()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();

			var human = new Human {Name = new Name {First = "Steve", Initial = 'E', Last = null}};

			var mother = new Human {Name = new Name {First = "Jane", Initial = 'E', Last = null}};
			human.Mother = (mother);

			s.Save(human);
			s.Save(mother);
			s.Flush();

			t.Commit();

			t = s.BeginTransaction();
			var e =
				Assert.Throws<QueryException>(
					() => s.CreateQuery("update Human set mother.name.initial = :initial").SetString("initial", "F").ExecuteUpdate());
			Assert.That(e.Message, Does.StartWith("Implied join paths are not assignable in update"));

			s.CreateQuery("delete Human where mother is not null").ExecuteUpdate();
			s.CreateQuery("delete Human").ExecuteUpdate();
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

			int count = s.CreateQuery("update PettingZoo set name = name").ExecuteUpdate();
			Assert.That(count, Is.EqualTo(1), "Incorrect discrim subclass update count");

			t.Rollback();
			t = s.BeginTransaction();

			count =
				s.CreateQuery("update PettingZoo pz set pz.name = pz.name where pz.id = :id").SetInt64("id", data.PettingZoo.Id).
					ExecuteUpdate();
			Assert.That(count, Is.EqualTo(1), "Incorrect discrim subclass update count");

			t.Rollback();
			t = s.BeginTransaction();

			count = s.CreateQuery("update Zoo as z set z.name = z.name").ExecuteUpdate();
			Assert.That(count, Is.EqualTo(2), "Incorrect discrim subclass update count");

			t.Rollback();
			t = s.BeginTransaction();

			// TODO : not so sure this should be allowed.  Seems to me that if they specify an alias,
			// property-refs should be required to be qualified.
			count = s.CreateQuery("update Zoo as z set name = name where id = :id").SetInt64("id", data.Zoo.Id).ExecuteUpdate();
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
			int count =
				s.CreateQuery("update Animal set description = description where description = :desc")
				.SetString("desc", data.Frog.Description)
				.ExecuteUpdate();
			Assert.That(count, Is.EqualTo(1), "Incorrect entity-updated count");

			count =
				s.CreateQuery("update Animal set description = :newDesc where description = :desc")
				.SetString("desc",data.Polliwog.Description)
				.SetString("newDesc", "Tadpole")
				.ExecuteUpdate();
			Assert.That(count, Is.EqualTo(1), "Incorrect entity-updated count");

			var tadpole = s.Load<Animal>(data.Polliwog.Id);

			Assert.That(tadpole.Description, Is.EqualTo("Tadpole"), "Update did not take effect");

			count =
				s.CreateQuery("update Animal set bodyWeight = bodyWeight + :w1 + :w2")
				.SetSingle("w1", 1)
				.SetSingle("w2", 2)
				.ExecuteUpdate();
			Assert.That(count, Is.EqualTo(6), "incorrect count on 'complex' update assignment");

			if (Dialect.SupportsSubqueryOnMutatingTable)
			{
				s.CreateQuery("update Animal set bodyWeight = ( select max(bodyWeight) from Animal )").ExecuteUpdate();
			}

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
					s.CreateQuery("update Animal set description = :newDesc, bodyWeight = :w1 where description = :desc")
						.SetString("desc", data.Polliwog.Description)
						.SetString("newDesc", "Tadpole")
						.SetSingle("w1", 3)
						.ExecuteUpdate();
				
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

			int count = s.CreateQuery("update Mammal set description = description").ExecuteUpdate();
			Assert.That(count, Is.EqualTo(2), "incorrect update count against 'middle' of joined-subclass hierarchy");

			count = s.CreateQuery("update Mammal set bodyWeight = 25").ExecuteUpdate();
			Assert.That(count, Is.EqualTo(2), "incorrect update count against 'middle' of joined-subclass hierarchy");

			if (Dialect.SupportsSubqueryOnMutatingTable)
			{
				count = s.CreateQuery("update Mammal set bodyWeight = ( select max(bodyWeight) from Animal )").ExecuteUpdate();
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

			int count = s.CreateQuery("update Vehicle set Owner = 'Steve'").ExecuteUpdate();
			Assert.That(count, Is.EqualTo(4), "incorrect restricted update count");
			count = s.CreateQuery("update Vehicle set Owner = null where Owner = 'Steve'").ExecuteUpdate();
			Assert.That(count, Is.EqualTo(4), "incorrect restricted update count");

			count = s.CreateQuery("delete Vehicle where Owner is null").ExecuteUpdate();
			Assert.That(count, Is.EqualTo(4), "incorrect restricted update count");

			t.Commit();
			s.Close();

			data.Cleanup();
		}

		[Test]
		public void WrongPropertyNameThrowQueryException()
		{
			using (ISession s = OpenSession())
			{
				var e = Assert.Throws<QueryException>(() => s.CreateQuery("update Vehicle set owner = null where owner = 'Steve'").ExecuteUpdate());
				Assert.That(e.Message, Does.StartWith("Left side of assigment should be a case sensitive property or a field"));
			}
		}

		[Test]
		public void UpdateSetNullOnDiscriminatorSubclass()
		{
			var data = new TestData(this);
			data.Prepare();

			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();

			int count = s.CreateQuery("update PettingZoo set address.city = null").ExecuteUpdate();
			Assert.That(count, Is.EqualTo(1), "Incorrect discrim subclass delete count");
			count = s.CreateQuery("delete Zoo where address.city is null").ExecuteUpdate();
			Assert.That(count, Is.EqualTo(1), "Incorrect discrim subclass delete count");

			count = s.CreateQuery("update Zoo set address.city = null").ExecuteUpdate();
			Assert.That(count, Is.EqualTo(1), "Incorrect discrim subclass delete count");
			count = s.CreateQuery("delete Zoo where address.city is null").ExecuteUpdate();
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

			int count = s.CreateQuery("update Mammal set bodyWeight = null").ExecuteUpdate();
			Assert.That(count, Is.EqualTo(2), "Incorrect deletion count on joined subclass");

			count = s.CreateQuery("delete Animal where bodyWeight = null").ExecuteUpdate();
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
			int count =
				s.CreateQuery("delete SimpleEntityWithAssociation e where size(e.AssociatedEntities ) = 0 and e.Name like '%'").
					ExecuteUpdate();
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

			int count =
				s.CreateQuery("delete from Animal as a where a.id = :id").SetInt64("id", data.Polliwog.Id).ExecuteUpdate();
			Assert.That(count, Is.EqualTo(1), "Incorrect delete count");

			count = s.CreateQuery("delete Animal where id = :id").SetInt64("id", data.Catepillar.Id).ExecuteUpdate();
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

			int count =
				s.CreateQuery("delete Joiner where joinedName = :joinedName").SetString("joinedName", "joined-name").ExecuteUpdate();
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

			int count = s.CreateQuery("delete Vehicle where Owner = :owner").SetString("owner", "Steve").ExecuteUpdate();
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

			int count = s.CreateQuery("delete Truck where Owner = :owner").SetString("owner", "Steve").ExecuteUpdate();
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

			int count = s.CreateQuery("delete Car where Owner = :owner").SetString("owner", "Kirsten").ExecuteUpdate();
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

			int count = s.CreateQuery("delete Animal where mother = :mother").SetEntity("mother", data.Butterfly).ExecuteUpdate();
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
