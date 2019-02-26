using System;
using System.Collections;
using System.Linq;
using System.Threading;
using NHibernate.Dialect;
using NHibernate.DomainModel;
using NHibernate.Hql.Ast.ANTLR;
using NHibernate.Linq;
using NHibernate.Test.LinqBulkManipulation.Domain;
using NUnit.Framework;

namespace NHibernate.Test.LinqBulkManipulation
{
	[TestFixture]
	public class Fixture : TestCase
	{
		protected override IList Mappings => Array.Empty<string>();

		protected override void Configure(Cfg.Configuration configuration)
		{
			var type = typeof(Fixture);
			var assembly = type.Assembly;
			var mappingNamespace = type.Namespace;
			foreach (var resource in assembly.GetManifestResourceNames())
			{
				if (resource.StartsWith(mappingNamespace) && resource.EndsWith(".hbm.xml"))
				{
					configuration.AddResource(resource, assembly);
				}
			}
		}

		private Animal _polliwog;
		private Animal _catepillar;
		private Animal _frog;
		private Animal _butterfly;
		private Zoo _zoo;
		private Zoo _pettingZoo;
		private Human _joe;
		private Human _doll;
		private Human _stevee;
		private IntegerVersioned _intVersioned;
		private TimestampVersioned _timeVersioned;

		protected override void OnSetUp()
		{
			using (var s = OpenSession())
			using (var txn = s.BeginTransaction())
			{
				_polliwog = new Animal { BodyWeight = 12, Description = "Polliwog" };
				_catepillar = new Animal { BodyWeight = 10, Description = "Catepillar" };
				_frog = new Animal { BodyWeight = 34, Description = "Frog" };
				_butterfly = new Animal { BodyWeight = 9, Description = "Butterfly" };

				_polliwog.Father = _frog;
				_frog.AddOffspring(_polliwog);
				_catepillar.Mother = _butterfly;
				_butterfly.AddOffspring(_catepillar);

				s.Save(_frog);
				s.Save(_polliwog);
				s.Save(_butterfly);
				s.Save(_catepillar);

				var dog = new Dog { BodyWeight = 200, Description = "dog" };
				s.Save(dog);
				var cat = new Cat { BodyWeight = 100, Description = "cat" };
				s.Save(cat);

				var dragon = new Dragon();
				dragon.SetFireTemperature(200);
				s.Save(dragon);

				_zoo = new Zoo { Name = "Zoo" };
				var add = new Address { City = "MEL", Country = "AU", Street = "Main st", PostalCode = "3000" };
				_zoo.Address = add;

				_pettingZoo = new PettingZoo { Name = "Petting Zoo" };
				var addr = new Address { City = "Sydney", Country = "AU", Street = "High st", PostalCode = "2000" };
				_pettingZoo.Address = addr;

				s.Save(_zoo);
				s.Save(_pettingZoo);

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

				var entCompKey = new EntityWithCrazyCompositeKey { Id = new CrazyCompositeKey { Id = 1, OtherId = 1 }, Name = "Parent" };
				s.Save(entCompKey);

				_joe = new Human { Name = new Name { First = "Joe", Initial = 'Q', Last = "Public" } };
				_doll = new Human { Name = new Name { First = "Kyu", Initial = 'P', Last = "Doll" }, Friends = new[] { _joe } };
				_stevee = new Human { Name = new Name { First = "Stevee", Initial = 'X', Last = "Ebersole" } };
				s.Save(_joe);
				s.Save(_doll);
				s.Save(_stevee);

				_intVersioned = new IntegerVersioned { Name = "int-vers", Data = "foo" };
				s.Save(_intVersioned);

				_timeVersioned = new TimestampVersioned { Name = "ts-vers", Data = "foo" };
				s.Save(_timeVersioned);

				var scwc = new SimpleClassWithComponent { Name = new Name { First = "Stevee", Initial = 'X', Last = "Ebersole" } };
				s.Save(scwc);

				var mainEntWithAssoc = new SimpleEntityWithAssociation() { Name = "main" };
				var otherEntWithAssoc = new SimpleEntityWithAssociation() { Name = "many-to-many-association" };
				mainEntWithAssoc.ManyToManyAssociatedEntities.Add(otherEntWithAssoc);
				mainEntWithAssoc.AddAssociation("one-to-many-association");
				s.Save(mainEntWithAssoc);

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

				txn.Commit();
			}
		}

		protected override void OnTearDown()
		{
			if (!Dialect.SupportsTemporaryTables)
			{
				// Give-up usual cleanup due to TPC: cannot perform multi-table deletes using dialect not supporting temp tables
				DropSchema();
				CreateSchema();
				return;
			}

			using (var s = OpenSession())
			using (var txn = s.BeginTransaction())
			{
				// workaround FK
				var doll = s.Query<Human>().SingleOrDefault(h => h.Id == _doll.Id);
				if (doll != null)
					s.Delete(doll);
				var entity = s.Query<SimpleEntityWithAssociation>().SingleOrDefault(e => e.ManyToManyAssociatedEntities.Any());
				if (entity != null)
				{
					s.Delete(entity.ManyToManyAssociatedEntities.First());
					s.Delete(entity);
				}
				s.Flush();
				s.CreateQuery("delete from Animal where Mother is not null or Father is not null").ExecuteUpdate();

				s.CreateQuery("delete from Animal").ExecuteUpdate();
				s.CreateQuery("delete from Zoo").ExecuteUpdate();
				s.CreateQuery("delete from Joiner").ExecuteUpdate();
				s.CreateQuery("delete from Vehicle").ExecuteUpdate();
				s.CreateQuery("delete EntityReferencingEntityWithCrazyCompositeKey").ExecuteUpdate();
				s.CreateQuery("delete EntityWithCrazyCompositeKey").ExecuteUpdate();
				s.CreateQuery("delete IntegerVersioned").ExecuteUpdate();
				s.CreateQuery("delete TimestampVersioned").ExecuteUpdate();
				s.CreateQuery("delete SimpleClassWithComponent").ExecuteUpdate();
				s.CreateQuery("delete SimpleAssociatedEntity").ExecuteUpdate();
				s.CreateQuery("delete SimpleEntityWithAssociation").ExecuteUpdate();

				txn.Commit();
			}
		}

		#region INSERTS

		[Test]
		public void SimpleInsert()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var count = s.Query<Car>().InsertInto(x => new Pickup { Id = -x.Id, Vin = x.Vin, Owner = x.Owner });
				Assert.AreEqual(1, count);

				t.Commit();
			}
		}

		[Test]
		public void SimpleAnonymousInsert()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var count = s.Query<Car>().InsertInto<Car, Pickup>(x => new { Id = -x.Id, x.Vin, x.Owner });
				Assert.AreEqual(1, count);

				t.Commit();
			}
		}

		[Test]
		public void SimpleInsertFromAggregate()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var count = s
					.Query<Car>()
					.GroupBy(x => x.Id)
					.Select(x => new { Id = x.Key, Vin = x.Max(y => y.Vin), Owner = x.Max(y => y.Owner) })
					.InsertInto(x => new Pickup { Id = -x.Id, Vin = x.Vin, Owner = x.Owner });
				Assert.AreEqual(1, count);

				t.Commit();
			}
		}

		[Test]
		public void SimpleInsertFromLimited()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var count = s
					.Query<Vehicle>()
					.Skip(1)
					.Take(1)
					.InsertInto(x => new Pickup { Id = -x.Id, Vin = x.Vin, Owner = x.Owner });
				Assert.AreEqual(1, count);

				t.Commit();
			}
		}

		[Test]
		public void SimpleInsertWithConstants()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var count = s
					.Query<Car>()
					.InsertBuilder().Into<Pickup>().Value(y => y.Id, y => -y.Id).Value(y => y.Vin, y => y.Vin).Value(y => y.Owner, "The owner")
					.Insert();
				Assert.AreEqual(1, count);

				t.Commit();
			}
		}

		[Test]
		public void SimpleInsertFromProjection()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var count = s
					.Query<Car>()
					.Select(x => new { x.Id, x.Owner, UpperOwner = x.Owner.ToUpper() })
					.InsertBuilder().Into<Pickup>().Value(y => y.Id, y => -y.Id).Value(y => y.Vin, y => y.UpperOwner)
					.Insert();
				Assert.AreEqual(1, count);

				t.Commit();
			}
		}

		[Test]
		public void InsertWithClientSideRequirementsThrowsException()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				Assert.Throws<NotSupportedException>(
					() => s
						.Query<Car>()
						.InsertInto(x => new Pickup { Id = -x.Id, Vin = x.Vin, Owner = x.Owner.PadRight(200) }));

				t.Commit();
			}
		}

		[Test]
		public void InsertWithManyToOne()
		{
			CheckSupportOfBulkInsertionWithGeneratedId<Animal>();

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var count = s
					.Query<Human>()
					.InsertInto(x => new Animal { Description = x.Description, BodyWeight = x.BodyWeight, Mother = x.Mother });
				Assert.AreEqual(3, count);

				t.Commit();
			}
		}

		[Test]
		public void InsertWithManyToOneAsParameter()
		{
			CheckSupportOfBulkInsertionWithGeneratedId<Animal>();

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var count = s
					.Query<Human>()
					.InsertInto(x => new Animal { Description = x.Description, BodyWeight = x.BodyWeight, Mother = _butterfly });
				Assert.AreEqual(3, count);

				t.Commit();
			}
		}

		[Test]
		public void InsertWithManyToOneWithCompositeKey()
		{
			CheckSupportOfBulkInsertionWithGeneratedId<EntityWithCrazyCompositeKey>();

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var count = s
					.Query<EntityWithCrazyCompositeKey>()
					.InsertInto(x => new EntityReferencingEntityWithCrazyCompositeKey { Name = "Child", Parent = x });
				Assert.AreEqual(1, count);

				t.Commit();
			}
		}

		[Test]
		public void InsertIntoSuperclassPropertiesFails()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				Assert.Throws<QueryException>(
					() => s.Query<Lizard>().InsertInto(x => new Human { Id = -x.Id, BodyWeight = x.BodyWeight }),
					"superclass prop insertion did not error");

				t.Commit();
			}
		}

		[Test]
		public void InsertAcrossMappedJoinFails()
		{
			CheckSupportOfBulkInsertionWithGeneratedId<Joiner>();

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				Assert.Throws<QueryException>(
					() => s.Query<Car>().InsertInto(x => new Joiner { Name = x.Vin, JoinedName = x.Owner }),
					"mapped-join insertion did not error");

				t.Commit();
			}
		}

		[Test]
		public void InsertWithGeneratedId()
		{
			CheckSupportOfBulkInsertionWithGeneratedId<PettingZoo>();

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var count = s.Query<Zoo>().Where(z => z.Id == _zoo.Id).InsertInto(x => new PettingZoo { Name = x.Name });
				Assert.That(count, Is.EqualTo(1), "unexpected insertion count");
				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var pz = s.Query<PettingZoo>().Single(z => z.Name == _zoo.Name);
				t.Commit();

				Assert.That(_zoo.Id != pz.Id);
			}
		}

		[Test]
		public void InsertWithGeneratedVersionAndId()
		{
			CheckSupportOfBulkInsertionWithGeneratedId<IntegerVersioned>();

			var initialId = _intVersioned.Id;
			var initialVersion = _intVersioned.Version;

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var count = s
					.Query<IntegerVersioned>()
					.Where(x => x.Id == initialId)
					.InsertInto(x => new IntegerVersioned { Name = x.Name, Data = x.Data });
				Assert.That(count, Is.EqualTo(1), "unexpected insertion count");
				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var created = s.Query<IntegerVersioned>().Single(iv => iv.Id != initialId);
				Assert.That(created.Version, Is.EqualTo(initialVersion), "version was not seeded");
				t.Commit();
			}
		}

		[Test]
		public void InsertWithGeneratedTimestampVersion()
		{
			CheckSupportOfBulkInsertionWithGeneratedId<TimestampVersioned>();

			var initialId = _timeVersioned.Id;

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var count = s
					.Query<TimestampVersioned>()
					.Where(x => x.Id == initialId)
					.InsertInto(x => new TimestampVersioned { Name = x.Name, Data = x.Data });
				Assert.That(count, Is.EqualTo(1), "unexpected insertion count");

				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var created = s.Query<TimestampVersioned>().Single(tv => tv.Id != initialId);
				Assert.That(created.Version, Is.GreaterThan(DateTime.Today));
				t.Commit();
			}
		}

		[Test]
		public void InsertWithSelectListUsingJoins()
		{
			CheckSupportOfBulkInsertionWithGeneratedId<Animal>();

			// this is just checking parsing and syntax...
			using (var s = OpenSession())
			{
				s.BeginTransaction();

				Assert.DoesNotThrow(() =>
				{
					s
						.Query<Human>().Where(x => x.Mother.Mother != null)
						.InsertInto(x => new Animal { Description = x.Description, BodyWeight = x.BodyWeight });
				});

				s.Transaction.Commit();
			}
		}

		[Test]
		public void InsertToComponent()
		{
			CheckSupportOfBulkInsertionWithGeneratedId<SimpleClassWithComponent>();

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				const string correctName = "Steve";

				var count = s
					.Query<SimpleClassWithComponent>()
					// Avoid Firebird unstable cursor bug by filtering.
					// https://firebirdsql.org/file/documentation/reference_manuals/fblangref25-en/html/fblangref25-dml-insert.html#fblangref25-dml-insert-select-unstable
					.Where(sc => sc.Name.First != correctName)
					.InsertBuilder().Into<SimpleClassWithComponent>().Value(y => y.Name.First, y => correctName)
					.Insert();
				Assert.That(count, Is.EqualTo(1), "incorrect insert count from individual setters");

				count = s
					.Query<SimpleClassWithComponent>()
					.Where(x => x.Name.First == correctName && x.Name.Initial != 'Z')
					.InsertInto(x => new SimpleClassWithComponent { Name = new Name { First = x.Name.First, Last = x.Name.Last, Initial = 'Z' } });
				Assert.That(count, Is.EqualTo(1), "incorrect insert from non anonymous selector");

				count = s
					.Query<SimpleClassWithComponent>()
					.Where(x => x.Name.First == correctName && x.Name.Initial == 'Z')
					.InsertInto<SimpleClassWithComponent, SimpleClassWithComponent>(x => new { Name = new { x.Name.First, x.Name.Last, Initial = 'W' } });
				Assert.That(count, Is.EqualTo(1), "incorrect insert from anonymous selector");

				count = s
					.Query<SimpleClassWithComponent>()
					.Where(x => x.Name.First == correctName && x.Name.Initial == 'Z')
					.InsertInto<SimpleClassWithComponent, SimpleClassWithComponent>(x => new { Name = new Name { First = x.Name.First, Last = x.Name.Last, Initial = 'V' } });
				Assert.That(count, Is.EqualTo(1), "incorrect insert from hybrid selector");
				t.Commit();
			}
		}

		private void CheckSupportOfBulkInsertionWithGeneratedId<T>()
		{
			// Make sure the env supports bulk inserts with generated ids...
			var persister = Sfi.GetEntityPersister(typeof(T).FullName);
			var generator = persister.IdentifierGenerator;
			if (!HqlSqlWalker.SupportsIdGenWithBulkInsertion(generator))
			{
				Assert.Ignore($"Identifier generator {generator.GetType().Name} for entity {typeof(T).FullName} does not support bulk insertions.");
			}
		}

		#endregion

		#region UPDATES

		[Test]
		public void SimpleUpdate()
		{
			using (var s = OpenSession())
			using (s.BeginTransaction())
			{
				var count = s
					.Query<Car>()
					.Update(a => new Car { Owner = a.Owner + " a" });
				Assert.AreEqual(1, count);
			}
		}

		[Test]
		public void SimpleAnonymousUpdate()
		{
			using (var s = OpenSession())
			using (s.BeginTransaction())
			{
				var count = s
					.Query<Car>()
					.Update(a => new { Owner = a.Owner + " a" });
				Assert.AreEqual(1, count);
			}
		}

		[Test]
		public void UpdateWithWhereExistsSubquery()
		{
			if (!Dialect.SupportsTemporaryTables)
			{
				Assert.Ignore("Cannot perform multi-table updates using dialect not supporting temp tables.");
			}

			// multi-table ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var count = s
					.Query<Human>()
					.Where(x => x.Friends.OfType<Human>().Any(f => f.Name.Last == "Public"))
					.UpdateBuilder().Set(y => y.Description, "updated")
					.Update();
				Assert.That(count, Is.EqualTo(1));
				t.Commit();
			}

			// single-table (one-to-many & many-to-many) ~~~~~~~~~~~~~~~~~~~~~~~~~~
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				// one-to-many test
				var count = s
					.Query<SimpleEntityWithAssociation>()
					.Where(x => x.AssociatedEntities.Any(a => a.Name == "one-to-many-association"))
					.UpdateBuilder().Set(y => y.Name, "updated")
					.Update();
				Assert.That(count, Is.EqualTo(1));
				// many-to-many test
				if (Dialect.SupportsSubqueryOnMutatingTable)
				{
					count = s
						.Query<SimpleEntityWithAssociation>()
						.Where(x => x.ManyToManyAssociatedEntities.Any(a => a.Name == "many-to-many-association"))
						.UpdateBuilder().Set(y => y.Name, "updated")
						.Update();

					Assert.That(count, Is.EqualTo(1));
				}
				t.Commit();
			}
		}

		[Test]
		public void IncrementCounterVersion()
		{
			var initialVersion = _intVersioned.Version;

			using (var s = OpenSession())
			{
				using (var t = s.BeginTransaction())
				{
					// Note: Update more than one column to showcase NH-3624, which involved losing some columns. /2014-07-26
					var count = s
						.Query<IntegerVersioned>()
						.UpdateBuilder().Set(y => y.Name, y => y.Name + "upd").Set(y => y.Data, y => y.Data + "upd")
						.UpdateVersioned();
					Assert.That(count, Is.EqualTo(1), "incorrect exec count");
					t.Commit();
				}

				using (var t = s.BeginTransaction())
				{
					var entity = s.Get<IntegerVersioned>(_intVersioned.Id);
					Assert.That(entity.Version, Is.EqualTo(initialVersion + 1), "version not incremented");
					Assert.That(entity.Name, Is.EqualTo("int-versupd"));
					Assert.That(entity.Data, Is.EqualTo("fooupd"));
					t.Commit();
				}
			}
		}

		[Test]
		public void IncrementTimestampVersion()
		{
			var initialVersion = _timeVersioned.Version;

			Thread.Sleep(1300);

			using (var s = OpenSession())
			{
				using (var t = s.BeginTransaction())
				{
					// Note: Update more than one column to showcase NH-3624, which involved losing some columns. /2014-07-26
					var count = s
						.Query<TimestampVersioned>()
						.UpdateBuilder().Set(y => y.Name, y => y.Name + "upd").Set(y => y.Data, y => y.Data + "upd")
						.UpdateVersioned();
					Assert.That(count, Is.EqualTo(1), "incorrect exec count");
					t.Commit();
				}

				using (var t = s.BeginTransaction())
				{
					var entity = s.Load<TimestampVersioned>(_timeVersioned.Id);
					Assert.That(entity.Version, Is.GreaterThan(initialVersion), "version not incremented");
					Assert.That(entity.Name, Is.EqualTo("ts-versupd"));
					Assert.That(entity.Data, Is.EqualTo("fooupd"));
					t.Commit();
				}
			}
		}

		[Test]
		public void UpdateOnComponent()
		{
			if (!Dialect.SupportsTemporaryTables)
			{
				Assert.Ignore("Cannot perform multi-table updates using dialect not supporting temp tables.");
			}

			using (var s = OpenSession())
			{
				const string correctName = "Steve";

				using (var t = s.BeginTransaction())
				{
					var count =
						s.Query<Human>().Where(x => x.Id == _stevee.Id).Update(x => new Human { Name = { First = correctName } });

					Assert.That(count, Is.EqualTo(1), "incorrect update count");
					t.Commit();
				}

				using (var t = s.BeginTransaction())
				{
					s.Refresh(_stevee);

					Assert.That(_stevee.Name.First, Is.EqualTo(correctName), "Update did not execute properly");

					t.Commit();
				}
			}
		}

		[Test]
		public void UpdateWithClientSideRequirementsThrowsException()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				Assert.Throws<NotSupportedException>(
					() => s.Query<Human>().Where(x => x.Id == _stevee.Id).Update(x => new Human { Name = { First = x.Name.First.PadLeft(200) } })
				);

				t.Commit();
			}
		}

		[Test]
		public void UpdateOnManyToOne()
		{
			if (!Dialect.SupportsTemporaryTables)
			{
				Assert.Ignore("Cannot perform multi-table updates using dialect not supporting temp tables.");
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				Assert.DoesNotThrow(() => { s.Query<Animal>().Where(x => x.Id == 2).UpdateBuilder().Set(y => y.Mother, y => null).Update(); });

				if (Dialect.SupportsSubqueryOnMutatingTable)
				{
					Assert.DoesNotThrow(
						() => { s.Query<Animal>().Where(x => x.Id == 2).UpdateBuilder().Set(y => y.Mother, y => s.Query<Animal>().First(z => z.Id == 1)).Update(); });
				}

				t.Commit();
			}
		}

		[Test]
		public void UpdateOnDiscriminatorSubclass()
		{
			using (var s = OpenSession())
			{
				using (var t = s.BeginTransaction())
				{
					var count = s.Query<PettingZoo>().UpdateBuilder().Set(y => y.Name, y => y.Name).Update();
					Assert.That(count, Is.EqualTo(1), "Incorrect discrim subclass update count");

					t.Rollback();
				}
				using (var t = s.BeginTransaction())
				{
					var count = s.Query<PettingZoo>().Where(x => x.Id == _pettingZoo.Id).UpdateBuilder().Set(y => y.Name, y => y.Name).Update();
					Assert.That(count, Is.EqualTo(1), "Incorrect discrim subclass update count");

					t.Rollback();
				}
				using (var t = s.BeginTransaction())
				{

					var count = s.Query<Zoo>().UpdateBuilder().Set(y => y.Name, y => y.Name).Update();
					Assert.That(count, Is.EqualTo(2), "Incorrect discrim subclass update count");

					t.Rollback();
				}
				using (var t = s.BeginTransaction())
				{
					// TODO : not so sure this should be allowed.  Seems to me that if they specify an alias,
					// property-refs should be required to be qualified.
					var count = s.Query<Zoo>().Where(x => x.Id == _zoo.Id).UpdateBuilder().Set(y => y.Name, y => y.Name).Update();
					Assert.That(count, Is.EqualTo(1), "Incorrect discrim subclass update count");

					t.Commit();
				}
			}
		}

		[Test]
		public void UpdateOnAnimal()
		{
			if (!Dialect.SupportsTemporaryTables)
			{
				Assert.Ignore("Cannot perform multi-table updates using dialect not supporting temp tables.");
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				//var count = s.Query<Animal>().Where(x => x.Description == data.Frog.Description).Update().Set(y => y.Description, y => y.Description));
				//Assert.That(count, Is.EqualTo(1), "Incorrect entity-updated count");

				var count =
					s.Query<Animal>().Where(x => x.Description == _polliwog.Description).UpdateBuilder().Set(y => y.Description, y => "Tadpole").Update();
				Assert.That(count, Is.EqualTo(1), "Incorrect entity-updated count");

				var tadpole = s.Load<Animal>(_polliwog.Id);

				Assert.That(tadpole.Description, Is.EqualTo("Tadpole"), "Update did not take effect");

				count =
					s.Query<Dragon>().UpdateBuilder().Set(y => y.FireTemperature, 300).Update();
				Assert.That(count, Is.EqualTo(1), "Incorrect entity-updated count");


				count =
					s.Query<Animal>().UpdateBuilder().Set(y => y.BodyWeight, y => y.BodyWeight + 1 + 1).Update();
				Assert.That(count, Is.EqualTo(10), "incorrect count on 'complex' update assignment");

				if (Dialect.SupportsSubqueryOnMutatingTable)
				{
					Assert.DoesNotThrow(() => { s.Query<Animal>().UpdateBuilder().Set(y => y.BodyWeight, y => s.Query<Animal>().Max(z => z.BodyWeight)).Update(); });
				}

				t.Commit();
			}
		}

		[Test]
		public void UpdateOnDragonWithProtectedProperty()
		{
			if (!Dialect.SupportsTemporaryTables)
			{
				Assert.Ignore("Cannot perform multi-table updates using dialect not supporting temp tables.");
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var count =
					s.Query<Dragon>().UpdateBuilder().Set(y => y.FireTemperature, 300).Update();
				Assert.That(count, Is.EqualTo(1), "Incorrect entity-updated count");

				t.Commit();
			}
		}

		[Test]
		public void UpdateMultiplePropertyOnAnimal()
		{
			if (!Dialect.SupportsTemporaryTables)
			{
				Assert.Ignore("Cannot perform multi-table updates using dialect not supporting temp tables.");
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var count =
					s.Query<Animal>()
					 .Where(x => x.Description == _polliwog.Description)
					 .UpdateBuilder().Set(y => y.Description, y => "Tadpole").Set(y => y.BodyWeight, 3).Update();

				Assert.That(count, Is.EqualTo(1));
				t.Commit();
			}

			using (var s = OpenSession())
			using (s.BeginTransaction())
			{
				var tadpole = s.Get<Animal>(_polliwog.Id);
				Assert.That(tadpole.Description, Is.EqualTo("Tadpole"));
				Assert.That(tadpole.BodyWeight, Is.EqualTo(3));
			}
		}

		[Test]
		public void UpdateOnMammal()
		{
			if (!Dialect.SupportsTemporaryTables)
			{
				Assert.Ignore("Cannot perform multi-table updates using dialect not supporting temp tables.");
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var count = s.Query<Mammal>().UpdateBuilder().Set(y => y.Description, y => y.Description).Update();

				Assert.That(count, Is.EqualTo(5), "incorrect update count against 'middle' of joined-subclass hierarchy");

				count = s.Query<Mammal>().UpdateBuilder().Set(y => y.BodyWeight, 25).Update();
				Assert.That(count, Is.EqualTo(5), "incorrect update count against 'middle' of joined-subclass hierarchy");

				if (Dialect.SupportsSubqueryOnMutatingTable)
				{
					count = s.Query<Mammal>().UpdateBuilder().Set(y => y.BodyWeight, y => s.Query<Animal>().Max(z => z.BodyWeight)).Update();
					Assert.That(count, Is.EqualTo(5), "incorrect update count against 'middle' of joined-subclass hierarchy");
				}

				t.Commit();
			}
		}

		[Test]
		public void UpdateSetNullUnionSubclass()
		{
			if (!Dialect.SupportsTemporaryTables)
			{
				Assert.Ignore("Cannot perform multi-table updates using dialect not supporting temp tables.");
			}

			// These should reach out into *all* subclass tables...
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var count = s.Query<Vehicle>().UpdateBuilder().Set(y => y.Owner, "Steve").Update();
				Assert.That(count, Is.EqualTo(4), "incorrect restricted update count");
				count = s.Query<Vehicle>().Where(x => x.Owner == "Steve").UpdateBuilder().Set(y => y.Owner, default(string)).Update();
				Assert.That(count, Is.EqualTo(4), "incorrect restricted update count");

				count = s.CreateQuery("delete Vehicle where Owner is null").ExecuteUpdate();
				Assert.That(count, Is.EqualTo(4), "incorrect restricted update count");

				t.Commit();
			}
		}

		[Test]
		public void UpdateSetNullOnDiscriminatorSubclass()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var count = s.Query<PettingZoo>().UpdateBuilder().Set(y => y.Address.City, default(string)).Update();

				Assert.That(count, Is.EqualTo(1), "Incorrect discrim subclass delete count");
				count = s.CreateQuery("delete Zoo where Address.City is null").ExecuteUpdate();
				Assert.That(count, Is.EqualTo(1), "Incorrect discrim subclass delete count");

				count = s.Query<Zoo>().UpdateBuilder().Set(y => y.Address.City, default(string)).Update();
				Assert.That(count, Is.EqualTo(1), "Incorrect discrim subclass delete count");
				count = s.CreateQuery("delete Zoo where Address.City is null").ExecuteUpdate();
				Assert.That(count, Is.EqualTo(1), "Incorrect discrim subclass delete count");

				t.Commit();
			}
		}

		[Test]
		public void UpdateSetNullOnJoinedSubclass()
		{
			if (!Dialect.SupportsTemporaryTables)
			{
				Assert.Ignore("Cannot perform multi-table updates using dialect not supporting temp tables.");
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var count = s.Query<Mammal>().UpdateBuilder().Set(y => y.BodyWeight, -1).Update();
				Assert.That(count, Is.EqualTo(5), "Incorrect update count on joined subclass");

				count = s.Query<Mammal>().Count(m => m.BodyWeight > -1.0001 && m.BodyWeight < -0.9999);
				Assert.That(count, Is.EqualTo(5), "Incorrect body weight count");

				t.Commit();
			}
		}

		[Test]
		public void UpdateOnOtherClassThrows()
		{
			using (var s = OpenSession())
			using (s.BeginTransaction())
			{
				var query = s
					.Query<Animal>().Where(x => x.Mother == _butterfly);
				Assert.That(() => query.Update(a => new Human { Description = a.Description + " humanized" }), Throws.TypeOf<TypeMismatchException>());
			}
		}

		#endregion

		#region DELETES

		[Test]
		public void DeleteWithSubquery()
		{
			if (Dialect is MsSqlCeDialect)
			{
				Assert.Ignore("Test failing on Ms SQL CE.");
			}

			using (var s = OpenSession())
			{
				s.BeginTransaction();
				var count = s.Query<SimpleEntityWithAssociation>().Where(x => x.AssociatedEntities.Count == 0 && x.Name.Contains("myEntity")).Delete();
				Assert.That(count, Is.EqualTo(1), "Incorrect delete count");
				s.Transaction.Commit();
			}
		}

		[Test]
		public void SimpleDeleteOnAnimal()
		{
			if (Dialect.HasSelfReferentialForeignKeyBug)
			{
				Assert.Ignore("Self referential FK bug");
			}
			if (!Dialect.SupportsTemporaryTables)
			{
				Assert.Ignore("Cannot perform multi-table deletes using dialect not supporting temp tables.");
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				// Get rid of FK which may fail the test
				_doll.Friends = Array.Empty<Human>();
				s.Update(_doll);
				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{

				var count = s.Query<Animal>().Where(x => x.Id == _polliwog.Id).Delete();
				Assert.That(count, Is.EqualTo(1), "Incorrect delete count");

				count = s.Query<Animal>().Where(x => x.Id == _catepillar.Id).Delete();
				Assert.That(count, Is.EqualTo(1), "Incorrect delete count");

				if (Dialect.SupportsSubqueryOnMutatingTable)
				{
					count = s.Query<User>().Where(x => s.Query<User>().Contains(x)).Delete();
					Assert.That(count, Is.EqualTo(0));
				}

				count = s.Query<Animal>().Delete();
				Assert.That(count, Is.EqualTo(8), "Incorrect delete count");

				IList list = s.Query<Animal>().ToList();
				Assert.That(list, Is.Empty, "table not empty");

				t.Commit();
			}
		}

		[Test]
		public void DeleteOnDiscriminatorSubclass()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var count = s.Query<PettingZoo>().Delete();
				Assert.That(count, Is.EqualTo(1), "Incorrect discrim subclass delete count");

				count = s.Query<Zoo>().Delete();
				Assert.That(count, Is.EqualTo(1), "Incorrect discrim subclass delete count");

				t.Commit();
			}
		}

		[Test]
		public void DeleteOnJoinedSubclass()
		{
			if (!Dialect.SupportsTemporaryTables)
			{
				Assert.Ignore("Cannot perform multi-table deletes using dialect not supporting temp tables.");
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				// Get rid of FK which may fail the test
				_doll.Friends = Array.Empty<Human>();
				s.Update(_doll);
				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var count = s.Query<Mammal>().Where(x => x.BodyWeight > 150).Delete();
				Assert.That(count, Is.EqualTo(1), "Incorrect deletion count on joined subclass");

				count = s.Query<Mammal>().Delete();
				Assert.That(count, Is.EqualTo(4), "Incorrect deletion count on joined subclass");

				count = s.Query<SubMulti>().Delete();
				Assert.That(count, Is.EqualTo(0), "Incorrect deletion count on joined subclass");

				t.Commit();
			}
		}

		[Test]
		public void DeleteOnMappedJoin()
		{
			if (!Dialect.SupportsTemporaryTables)
			{
				Assert.Ignore("Cannot perform multi-table deletes using dialect not supporting temp tables.");
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var count = s.Query<Joiner>().Where(x => x.JoinedName == "joined-name").Delete();
				Assert.That(count, Is.EqualTo(1), "Incorrect deletion count on joined class");

				t.Commit();
			}
		}

		[Test]
		public void DeleteUnionSubclassAbstractRoot()
		{
			if (!Dialect.SupportsTemporaryTables)
			{
				Assert.Ignore("Cannot perform multi-table deletes using dialect not supporting temp tables.");
			}

			// These should reach out into *all* subclass tables...
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var count = s.Query<Vehicle>().Where(x => x.Owner == "Steve").Delete();
				Assert.That(count, Is.EqualTo(1), "incorrect restricted update count");

				count = s.Query<Vehicle>().Delete();
				Assert.That(count, Is.EqualTo(3), "incorrect update count");

				t.Commit();
			}
		}

		[Test]
		public void DeleteUnionSubclassConcreteSubclass()
		{
			if (!Dialect.SupportsTemporaryTables)
			{
				Assert.Ignore("Cannot perform multi-table deletes using dialect not supporting temp tables.");
			}

			// These should only affect the given table
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var count = s.Query<Truck>().Where(x => x.Owner == "Steve").Delete();
				Assert.That(count, Is.EqualTo(1), "incorrect restricted update count");

				count = s.Query<Truck>().Delete();
				Assert.That(count, Is.EqualTo(2), "incorrect update count");
				t.Commit();
			}
		}

		[Test]
		public void DeleteUnionSubclassLeafSubclass()
		{
			// These should only affect the given table
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var count = s.Query<Car>().Where(x => x.Owner == "Kirsten").Delete();
				Assert.That(count, Is.EqualTo(1), "incorrect restricted update count");

				count = s.Query<Car>().Delete();
				Assert.That(count, Is.EqualTo(0), "incorrect update count");

				t.Commit();
			}
		}

		[Test]
		public void DeleteRestrictedOnManyToOne()
		{
			if (!Dialect.SupportsTemporaryTables)
			{
				Assert.Ignore("Cannot perform multi-table deletes using dialect not supporting temp tables.");
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var count = s.Query<Animal>().Where(x => x.Mother == _butterfly).Delete();
				Assert.That(count, Is.EqualTo(1));

				t.Commit();
			}
		}

		[Test]
		public void DeleteSyntaxWithCompositeId()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.Query<EntityWithCrazyCompositeKey>().Where(x => x.Id.Id == 1 && x.Id.OtherId == 2).Delete();

				t.Commit();
			}
		}

		[Test]
		public void DeleteOnProjectionThrows()
		{
			using (var s = OpenSession())
			using (s.BeginTransaction())
			{
				var query = s
					.Query<Animal>().Where(x => x.Mother == _butterfly)
					.Select(x => new Car { Id = x.Id });
				Assert.That(() => query.Delete(), Throws.InvalidOperationException);
			}
		}

		[Test]
		public void DeleteOnFilterThrows()
		{
			using (var s = OpenSession())
			using (s.BeginTransaction())
			{
				var a = s.Query<SimpleEntityWithAssociation>().Take(1).SingleOrDefault();
				var query = a.AssociatedEntities.AsQueryable();
				Assert.That(() => query.Delete(), Throws.InstanceOf<NotSupportedException>());
			}
		}

		#endregion
	}
}
