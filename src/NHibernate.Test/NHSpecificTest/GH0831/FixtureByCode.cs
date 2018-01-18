using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;

using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH0831
{
	public class ByCodeFixture : TestCaseMappingByCode
	{
		private readonly IList<Entity> entities = new List<Entity>
		{
			new Entity { EntityValue = 0.5m },
			new Entity { EntityValue = 1.0m },
			new Entity { EntityValue = 1.5m },
			new Entity { EntityValue = 2.0m },
			new Entity { EntityValue = 2.5m },
			new Entity { EntityValue = 3.0m }
		};

		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<Entity>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
				rc.Property(x => x.EntityValue);
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

		private void IgnoreIfNotSupported(string function)
		{
			if (!Dialect.Functions.ContainsKey(function))
			{
				Assert.Ignore("Dialect {0} does not support '{1}' function", Dialect.GetType(), function);
			}
		}

		[Test]
		public void CanHandleAdd()
		{
			CanHandle(e => decimal.Add(e.EntityValue, 2) > 3.0m);
			CanHandle(e => decimal.Add(2, e.EntityValue) > 3.0m);
		}

		[Test]
		public void CanHandleCeiling()
		{
			IgnoreIfNotSupported("ceiling");

			CanHandle(e => decimal.Ceiling(e.EntityValue) > 1.0m);
		}

		[Test]
		public void CanHandleCompare()
		{
			IgnoreIfNotSupported("sign");

			CanHandle(e => decimal.Compare(e.EntityValue, 1.5m) < 1);
			CanHandle(e => decimal.Compare(1.0m, e.EntityValue) < 1);
		}

		[Test]
		public void CanHandleDivide()
		{
			CanHandle(e => decimal.Divide(e.EntityValue, 1.25m) < 1);
			CanHandle(e => decimal.Divide(1.25m, e.EntityValue) < 1);
		}

		[Test]
		public void CanHandleEquals()
		{
			CanHandle(e => decimal.Equals(e.EntityValue, 1.0m));
			CanHandle(e => decimal.Equals(1.0m, e.EntityValue));
		}

		[Test]
		public void CanHandleFloor()
		{
			IgnoreIfNotSupported("floor");

			CanHandle(e => decimal.Floor(e.EntityValue) > 1.0m);
		}

		[Test]
		public void CanHandleMultiply()
		{
			CanHandle(e => decimal.Multiply(e.EntityValue, 10m) > 10m);
			CanHandle(e => decimal.Multiply(10m, e.EntityValue) > 10m);
		}

		[Test]
		public void CanHandleNegate()
		{
			CanHandle(e => decimal.Negate(e.EntityValue) > -1.0m);
		}

		[Test]
		public void CanHandleRemainder()
		{
			CanHandle(e => decimal.Remainder(e.EntityValue, 2) == 0);
			CanHandle(e => decimal.Remainder(2, e.EntityValue) < 1);
		}

		[Test]
		public void CanHandleRound()
		{
			IgnoreIfNotSupported("round");

			CanHandle(e => decimal.Round(e.EntityValue) >= 2.0m);
			CanHandle(e => decimal.Round(e.EntityValue, 1) >= 1.5m);
		}

		[Test]
		public void CanHandleSubtract()
		{
			CanHandle(e => decimal.Subtract(e.EntityValue, 1m) > 1m);
			CanHandle(e => decimal.Subtract(2m, e.EntityValue) > 1m);
		}

		private void CanHandle(Expression<Func<Entity, bool>> predicate)
		{
			using (ISession session = OpenSession())
			using (session.BeginTransaction())
			{
				IEnumerable<Entity> inMemory = entities.Where(predicate.Compile()).ToList();
				IEnumerable<Entity> inSession = session.Query<Entity>().Where(predicate).ToList();

				CollectionAssert.AreEquivalent(inMemory, inSession);
			}
		}
	}
}
