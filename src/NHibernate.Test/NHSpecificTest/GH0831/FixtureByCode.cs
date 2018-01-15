using System;
using System.Collections.Generic;
using System.Linq;

using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;

using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH0831
{
	public class ByCodeFixture : TestCaseMappingByCode
	{
		private readonly IList<Entity> entities = new List<Entity>
		{
			new Entity { Value = 0.5m },
			new Entity { Value = 1.0m },
			new Entity { Value = 1.5m },
			new Entity { Value = 2.0m },
			new Entity { Value = 2.5m },
			new Entity { Value = 3.0m }
		};

		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<Entity>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
				rc.Property(x => x.Value);
			});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override void OnSetUp()
		{
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				foreach (Entity entity in entities)
				{
					session.Save(entity);
				}

				session.Flush();
				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				session.Delete("from System.Object");

				session.Flush();
				transaction.Commit();
			}
		}

		[Test]
		public void CanHandleAdd()
		{
			CanHandle(e => decimal.Add(e.Value, 2) > 3.0m);
			CanHandle(e => decimal.Add(2, e.Value) > 3.0m);
		}

		[Test]
		public void CanHandleCeiling()
		{
			CanHandle(e => decimal.Ceiling(e.Value) > 1.0m);
		}

		[Test]
		public void CanHandleCompare()
		{
			CanHandle(e => decimal.Compare(e.Value, 1.0m) < 1);
			CanHandle(e => decimal.Compare(1.0m, e.Value) < 1);
		}

		[Test]
		public void CanHandleDivide()
		{
			CanHandle(e => decimal.Divide(e.Value, 1.25m) < 1);
			CanHandle(e => decimal.Divide(1.25m, e.Value) < 1);
		}

		[Test]
		public void CanHandleEquals()
		{
			CanHandle(e => decimal.Equals(e.Value, 1.0m));
			CanHandle(e => decimal.Equals(1.0m, e.Value));
		}

		[Test]
		public void CanHandleFloor()
		{
			CanHandle(e => decimal.Floor(e.Value) > 1.0m);
		}

		[Test]
		public void CanHandleMultiply()
		{
			CanHandle(e => decimal.Multiply(e.Value, 10m) > 10m);
			CanHandle(e => decimal.Multiply(10m, e.Value) > 10m);
		}

		[Test]
		public void CanHandleNegate()
		{
			CanHandle(e => decimal.Negate(e.Value) > -1.0m);
		}

		[Test]
		public void CanHandleRemainder()
		{
			CanHandle(e => decimal.Remainder(e.Value, 2m) >= 0.5m);
			CanHandle(e => decimal.Remainder(2m, e.Value) >= 0.5m);
		}

		[Test]
		public void CanHandleRound()
		{
			CanHandle(e => decimal.Round(e.Value) >= 2.0m);
			CanHandle(e => decimal.Round(e.Value, 1) >= 1.5m);
			CanHandle(e => decimal.Round(e.Value, MidpointRounding.AwayFromZero) >= 2.0m);
			CanHandle(e => decimal.Round(e.Value, 1, MidpointRounding.AwayFromZero) >= 2.0m);
		}

		[Test]
		public void CanHandleSubtract()
		{
			CanHandle(e => decimal.Subtract(e.Value, 1m) > 1m);
			CanHandle(e => decimal.Subtract(2m, e.Value) > 1m);
		}

		private void CanHandle(Expression<Func<Entity, bool>> predicate)
		{
			using (ISession session = OpenSession())
			using (session.BeginTransaction())
			{
				IEnumerable<Entity> inMemory = entities.Where(predicate.Compile()).ToList();
				IEnumerable<Entity> inSession = session.Query<Entity>().Where(predicate).ToList();

				Assume.That(inMemory.Any());

				CollectionAssert.AreEquivalent(inMemory, inSession);
			}
		}
	}
}
