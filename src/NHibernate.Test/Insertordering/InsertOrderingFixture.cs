using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using NHibernate.AdoNet;
using NHibernate.Cfg;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;
using NUnit.Framework;

namespace NHibernate.Test.Insertordering
{
	public class InsertOrderingFixture : TestCase
	{
		const int batchSize = 10;
		const int instancesPerEach = 12;
		const int typesOfEntities = 3;

		protected override IList Mappings
		{
			get { return new[] { "Insertordering.Mapping.hbm.xml" }; }
		}

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return dialect.SupportsSqlBatches;
		}

		protected override void Configure(Configuration configuration)
		{
			configuration.DataBaseIntegration(x =>
			{
				x.BatchSize = batchSize;
				x.OrderInserts = true;
				x.Batcher<StatsBatcherFactory>();
			});
		}

		protected override void OnSetUp()
		{
			StatsBatcher.Reset();
			StatsBatcher.StatsEnabled = true;
		}

		protected override void OnTearDown()
		{
			StatsBatcher.StatsEnabled = false;

			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				session.Delete("from System.Object");

				session.Flush();
				transaction.Commit();
			}
		}

		[Test]
		public void BatchOrdering()
		{
			using (ISession s = OpenSession())
			using (s.BeginTransaction())
			{
				for (int i = 0; i < instancesPerEach; i++)
				{
					var user = new User { UserName = "user-" + i };
					var group = new Group { Name = "group-" + i };
					s.Save(user);
					s.Save(group);
					user.AddMembership(group);
				}
				StatsBatcher.Reset();
				s.Transaction.Commit();
			}

			int expectedBatchesPerEntity = (instancesPerEach / batchSize) + ((instancesPerEach % batchSize) == 0 ? 0 : 1);
			Assert.That(StatsBatcher.BatchSizes.Count, Is.EqualTo(expectedBatchesPerEntity * typesOfEntities));
		}

		// Following tests have been added for NH-3931

		#region Bidirectional many-to-many

		// Adapted from https://github.com/hibernate/hibernate-orm/blob/0c8261b0ae499d8ecc4001892b4cb43539de195a/hibernate-core/src/test/java/org/hibernate/test/insertordering/InsertOrderingWithBidirectionalManyToMany.java

		// Non-reg test case.
		[Test]
		public void WithBidiManyToMany()
		{
			using (ISession session = OpenSession())
			using (var trx = session.BeginTransaction())
			{
				var father = new PersonM2M();
				var mother = new PersonM2M();
				var son = new PersonM2M();
				var daughter = new PersonM2M();

				var home = new AddressM2M();
				var office = new AddressM2M();

				home.AddPerson(father);
				home.AddPerson(mother);
				home.AddPerson(son);
				home.AddPerson(daughter);

				office.AddPerson(father);
				office.AddPerson(mother);

				session.Save(home);
				session.Save(office);

				Assert.DoesNotThrow(() => { trx.Commit(); });
			}

			// 4 Person inserts, 2 Address inserts, 6 PersonAddresses inserts
			Assert.AreEqual(3, StatsBatcher.BatchSizes.Count, "Unexpected batches count");
		}

		#endregion

		#region Bidirectional many-to-one

		// Adapted from https://github.com/hibernate/hibernate-orm/blob/0c8261b0ae499d8ecc4001892b4cb43539de195a/hibernate-core/src/test/java/org/hibernate/test/insertordering/InsertOrderingWithBidirectionalOneToMany.java

		// Non-reg test case.
		[Test]
		public void WithBidiManyToOne()
		{
			using (ISession session = OpenSession())
			using (var trx = session.BeginTransaction())
			{
				var father = new PersonM2O();
				var mother = new PersonM2O();
				var son = new PersonM2O();
				var daughter = new PersonM2O();

				var home = new AddressM2O();
				var office = new AddressM2O();

				home.AddPerson(father);
				home.AddPerson(mother);
				home.AddPerson(son);
				home.AddPerson(daughter);

				office.AddPerson(father);
				office.AddPerson(mother);

				session.Save(home);
				session.Save(office);

				Assert.DoesNotThrow(() => { trx.Commit(); });
			}

			// 2 Address inserts, 4 Person inserts, 2 Person.Address updates
			Assert.AreEqual(3, StatsBatcher.BatchSizes.Count, "Unexpected batches count");
			Assert.AreEqual(8, StatsBatcher.BatchSizes.Sum(), "Unexpected batched queries count");
		}

		#endregion

		#region Bidirectional one-to-one (simulated with non pk fk)

		// Adapted from https://github.com/hibernate/hibernate-orm/blob/f90845c30c2a6d5e14eeafd32a4c9d321d3a55ef/hibernate-core/src/test/java/org/hibernate/test/insertordering/InsertOrderingWithBidirectionalOneToOne.java

		// Non-reg test case.
		[Test]
		public void WithBidiOneToOne()
		{
			using (ISession session = OpenSession())
			using (var trx = session.BeginTransaction())
			{
				var worker = new PersonO2O();
				var homestay = new PersonO2O();

				var home = new AddressO2O();
				var office = new AddressO2O();

				home.SetPerson(homestay);
				office.SetPerson(worker);

				session.Save(home);
				session.Save(office);

				Assert.DoesNotThrow(() => { trx.Commit(); });
			}

			// 2 Person inserts, 2 Address inserts, 2 Person updates (because mapped through foreign key)
			Assert.AreEqual(3, StatsBatcher.BatchSizes.Count, "Unexpected batches count");
			Assert.AreEqual(6, StatsBatcher.BatchSizes.Sum(), "Unexpected batched queries count");
		}

		#endregion

		#region Bidirectional actual one-to-one (pk being fk)

		// Non-reg test case.
		[Test]
		public void WithBidiTrueOneToOne()
		{
			using (ISession session = OpenSession())
			using (var trx = session.BeginTransaction())
			{
				var worker = new PersonTrueO2O();
				var homestay = new PersonTrueO2O();

				var home = new AddressTrueO2O();
				var office = new AddressTrueO2O();

				home.SetPerson(homestay);
				office.SetPerson(worker);

				session.Save(home);
				session.Save(office);

				Assert.DoesNotThrow(() => { trx.Commit(); });
			}

			// 2 Person inserts, 2 Address inserts
			Assert.AreEqual(2, StatsBatcher.BatchSizes.Count, "Unexpected batches count");
			Assert.AreEqual(4, StatsBatcher.BatchSizes.Sum(), "Unexpected batched queries count");
		}

		#endregion

		#region Element collection

		// Adapted from https://github.com/hibernate/hibernate-orm/blob/aa5f89326705fdb64ca8de2478c08006d11a974b/hibernate-core/src/test/java/org/hibernate/test/insertordering/ElementCollectionTest.java

		// Non-reg test case.
		[Test]
		public void WithElementCollection()
		{
			using (ISession session = OpenSession())
			using (var trx = session.BeginTransaction())
			{
				var task = new Task();
				task.Categories.Add(Category.A);
				session.Save(task);

				var task1 = new Task();
				task1.Categories.Add(Category.A);
				session.Save(task1);

				Assert.DoesNotThrow(() => { trx.Commit(); });
			}

			// 2 Task inserts, 2 Category inserts
			Assert.AreEqual(2, StatsBatcher.BatchSizes.Count, "Unexpected batches count");
			Assert.AreEqual(4, StatsBatcher.BatchSizes.Sum(), "Unexpected batched queries count");
		}

		#endregion

		#region Joined table inheritance

		// Adapted from https://github.com/hibernate/hibernate-orm/blob/0c8261b0ae499d8ecc4001892b4cb43539de195a/hibernate-core/src/test/java/org/hibernate/test/insertordering/InsertOrderingWithJoinedTableInheritance.java

		// Failing test case till NH-3931 is fixed.
		[Test]
		public void WithJoinedTableInheritance()
		{
			using (ISession session = OpenSession())
			using (var trx = session.BeginTransaction())
			{
				var person = new PersonJI();
				person.AddAddress(new AddressJI());
				session.Save(person);

				// Derived Object with dependent object (address)
				var specialPerson = new SpecialPersonJI();
				specialPerson.AddAddress(new AddressJI());
				session.Save(specialPerson);

				Assert.DoesNotThrow(() => { trx.Commit(); });
			}

			// 2 Person inserts, 1 SpecialPerson insert (out of any batch currently), 2 Address inserts
			Assert.AreEqual(2, StatsBatcher.BatchSizes.Count, "Unexpected batches count");
			Assert.AreEqual(4, StatsBatcher.BatchSizes.Sum(), "Unexpected batched queries count");
		}

		// Failing test case till NH-3931 is fixed.
		[Test]
		public void WithJoinedTableInheritance_Bigger()
		{
			using (ISession session = OpenSession())
			using (var trx = session.BeginTransaction())
			{
				for (int i = 0; i < 12; i++)
				{
					var person = new PersonJI();
					person.AddAddress(new AddressJI());
					session.Save(person);

					// Derived Object with dependent object (address)
					var specialPerson = new SpecialPersonJI();
					specialPerson.AddAddress(new AddressJI());
					session.Save(specialPerson);
				}

				Assert.DoesNotThrow(() => { trx.Commit(); });
			}

			// 24 Person inserts (2 batches + 12 due to SpecialPerson inserts which fragment batches, minus maybe 1 depending on execution order),
			// 12 SpecialPerson inserts (out of any batch currently), 24 Address inserts (3 batches)
			Assert.That(StatsBatcher.BatchSizes.Count, Is.InRange(16, 17), "Unexpected batches count");
			Assert.AreEqual(48, StatsBatcher.BatchSizes.Sum(), "Unexpected batched queries count");
		}

		#endregion

		#region Joined table multi-level inheritance

		// Adapted from https://github.com/hibernate/hibernate-orm/blob/0c8261b0ae499d8ecc4001892b4cb43539de195a/hibernate-core/src/test/java/org/hibernate/test/insertordering/InsertOrderingWithJoinedTableMultiLevelInheritance.java

		// Failing test case till NH-3931 is fixed.
		[Test]
		public void WithJoinedTableInheritance_MultiLevel()
		{
			using (ISession session = OpenSession())
			using (var trx = session.BeginTransaction())
			{
				for (int i = 0; i < 2; i++)
				{
					var president = new PresidentJim();
					president.AddAddress(new AddressJim());
					session.Save(president);

					var anotherPerson = new AnotherPersonJim();
					var office = new OfficeJim();
					session.Save(office);
					anotherPerson.Office = office;
					session.Save(anotherPerson);

					var person = new PersonJim();
					session.Save(person);

					var specialPerson = new SpecialPersonJim();
					specialPerson.AddAddress(new AddressJim());
					session.Save(specialPerson);
				}

				Assert.DoesNotThrow(() => { trx.Commit(); });
			}

			// 8 Person inserts (1 batch + 2 due to SpecialPerson inserts which fragment batches + 2 due to President inserts (frag too)
			//   + 2 due to AnotherPerson inserts (frag too), minus maybe 1 depending on execution order),
			// 4 SpecialPerson inserts (out of any batch currently), 2 President inserts (out of any batch currently),
			// 2 AnotherPerson inserts (out of any batch currently), 2 Office inserts, 4 Address inserts
			Assert.That(StatsBatcher.BatchSizes.Count, Is.InRange(8, 9), "Unexpected batches count");
			Assert.AreEqual(14, StatsBatcher.BatchSizes.Sum(), "Unexpected batched queries count");
		}

		#endregion

		#region Single table inheritance

		// Adapted from https://github.com/hibernate/hibernate-orm/blob/0c8261b0ae499d8ecc4001892b4cb43539de195a/hibernate-core/src/test/java/org/hibernate/test/insertordering/InsertOrderingWithSingleTableInheritance.java

		// Failing test case till NH-3931 is fixed.
		[Test]
		public void WithSingleTableInheritance()
		{
			using (ISession session = OpenSession())
			using (var trx = session.BeginTransaction())
			{
				var person = new PersonSti();
				person.AddAddress(new AddressSti());
				session.Save(person);

				// Derived Object with dependent object (address)
				var specialPerson = new SpecialPersonSti();
				specialPerson.AddAddress(new AddressSti());
				session.Save(specialPerson);

				Assert.DoesNotThrow(() => { trx.Commit(); });
			}

			// 1 Person insert, 1 SpecialPerson insert (into Person but with different columns), 2 Address inserts
			Assert.AreEqual(3, StatsBatcher.BatchSizes.Count, "Unexpected batches count");
			Assert.AreEqual(4, StatsBatcher.BatchSizes.Sum(), "Unexpected batched queries count");
		}

		// Failing test case till NH-3931 is fixed.
		[Test]
		public void WithSingleTableInheritance_Bigger()
		{
			using (ISession session = OpenSession())
			using (var trx = session.BeginTransaction())
			{
				for (int i = 0; i < 12; i++)
				{
					var person = new PersonSti();
					person.AddAddress(new AddressSti());
					session.Save(person);

					// Derived Object with dependent object (address)
					var specialPerson = new SpecialPersonSti();
					specialPerson.AddAddress(new AddressSti());
					session.Save(specialPerson);
				}

				Assert.DoesNotThrow(() => { trx.Commit(); });
			}

			// 12 Person inserts (2 batches), 12 SpecialPerson inserts (into Person but with different columns, 2 batches), 24 Address inserts (3 batches)
			Assert.AreEqual(7, StatsBatcher.BatchSizes.Count, "Unexpected batches count");
			Assert.AreEqual(48, StatsBatcher.BatchSizes.Sum(), "Unexpected batched queries count");
		}

		#endregion

		#region Table per concrete class inheritance

		// Adapted from https://github.com/hibernate/hibernate-orm/blob/0c8261b0ae499d8ecc4001892b4cb43539de195a/hibernate-core/src/test/java/org/hibernate/test/insertordering/InsertOrderingWithTablePerClassInheritance.java

		// Non-reg test case
		[Test]
		public void WithTablePerConcreteInheritance()
		{
			using (ISession session = OpenSession())
			using (var trx = session.BeginTransaction())
			{
				var person = new PersonTpc();
				person.AddAddress(new AddressTpc());
				session.Save(person);

				// Derived Object with dependent object (address)
				var specialPerson = new SpecialPersonTpc();
				specialPerson.AddAddress(new AddressTpc());
				session.Save(specialPerson);

				Assert.DoesNotThrow(() => { trx.Commit(); });
			}

			// 1 Person insert, 1 SpecialPerson insert, 2 Address inserts
			Assert.AreEqual(3, StatsBatcher.BatchSizes.Count, "Unexpected batches count");
			Assert.AreEqual(4, StatsBatcher.BatchSizes.Sum(), "Unexpected batched queries count");
		}

		// Non-reg test case
		[Test]
		public void WithTablePerConcreteInheritance_Bigger()
		{
			using (ISession session = OpenSession())
			using (var trx = session.BeginTransaction())
			{
				for (int i = 0; i < 12; i++)
				{
					var person = new PersonTpc();
					person.AddAddress(new AddressTpc());
					session.Save(person);

					// Derived Object with dependent object (address)
					var specialPerson = new SpecialPersonTpc();
					specialPerson.AddAddress(new AddressTpc());
					session.Save(specialPerson);
				}

				Assert.DoesNotThrow(() => { trx.Commit(); });
			}

			// 12 Person inserts (2 batches), 12 SpecialPerson inserts (2 batches), 24 Address inserts (3 batches)
			Assert.AreEqual(7, StatsBatcher.BatchSizes.Count, "Unexpected batches count");
			Assert.AreEqual(48, StatsBatcher.BatchSizes.Sum(), "Unexpected batched queries count");
		}

		#endregion

		#region Unidirectional one-to-one

		// Adapted from https://github.com/hibernate/hibernate-orm/blob/f90845c30c2a6d5e14eeafd32a4c9d321d3a55ef/hibernate-core/src/test/java/org/hibernate/test/insertordering/InsertOrderingWithUnidirectionalOneToOne.java

		// Non-reg test case.
		[Test]
		public void WithUnidiOneToOne()
		{
			using (ISession session = OpenSession())
			using (var trx = session.BeginTransaction())
			{
				var worker = new PersonUO2O();
				var homestay = new PersonUO2O();

				var home = new AddressUO2O();
				var office = new AddressUO2O();

				home.SetPerson(homestay);
				office.SetPerson(worker);

				session.Save(home);
				session.Save(office);

				Assert.DoesNotThrow(() => { trx.Commit(); });
			}

			// 2 Person inserts, 2 Address inserts
			Assert.AreEqual(2, StatsBatcher.BatchSizes.Count, "Unexpected batches count");
			Assert.AreEqual(4, StatsBatcher.BatchSizes.Sum(), "Unexpected batched queries count");
		}

		#endregion

		#region Nested type: StatsBatcher

		public class StatsBatcher : SqlClientBatchingBatcher
		{
			private static string batchSQL;
			private static IList<int> batchSizes = new List<int>();
			private static int currentBatch = -1;

			public static bool StatsEnabled { get; set; }

			public StatsBatcher(ConnectionManager connectionManager, IInterceptor interceptor)
				: base(connectionManager, interceptor) { }

			public static IList<int> BatchSizes
			{
				get { return batchSizes; }
			}

			public static void Reset()
			{
				batchSizes = new List<int>();
				currentBatch = -1;
				batchSQL = null;
			}

			public override DbCommand PrepareBatchCommand(CommandType type, SqlString sql, SqlType[] parameterTypes)
			{
				var result = base.PrepareBatchCommand(type, sql, parameterTypes);

				if (!StatsEnabled)
					return result;

				string sqlstring = sql.ToString();
				if (batchSQL == null || !sqlstring.Equals(batchSQL))
				{
					currentBatch++;
					batchSQL = sqlstring;
					batchSizes.Insert(currentBatch, 0);
					Console.WriteLine("--------------------------------------------------------");
					Console.WriteLine("Preparing statement [" + batchSQL + "]");
				}
				return result;
			}

			public override void AddToBatch(IExpectation expectation)
			{
				if (StatsEnabled)
				{
					batchSizes[currentBatch]++;
					Console.WriteLine("Adding to batch [" + batchSQL + "]");
				}
				base.AddToBatch(expectation);
			}

			protected override void DoExecuteBatch(DbCommand ps)
			{
				if (StatsEnabled)
				{
					Console.WriteLine("executing batch [" + batchSQL + "]");
					Console.WriteLine("--------------------------------------------------------");
				}
				batchSQL = null;
				base.DoExecuteBatch(ps);
			}
		}

		#endregion

		#region Nested type: StatsBatcherFactory

		public class StatsBatcherFactory : IBatcherFactory
		{
			#region IBatcherFactory Members

			public IBatcher CreateBatcher(ConnectionManager connectionManager, IInterceptor interceptor)
			{
				return new StatsBatcher(connectionManager, interceptor);
			}

			#endregion
		}

		#endregion
	}
}