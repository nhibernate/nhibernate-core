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

		[Test]
		public void CanHandleAdd()
		{
			Assert.Multiple(() =>
			{
				CanFilter(e => decimal.Add(e.EntityValue, 2) > 3.0m);
				CanFilter(e => decimal.Add(2, e.EntityValue) > 3.0m);

				CanSelect(e => decimal.Add(e.EntityValue, 2));
				CanSelect(e => decimal.Add(2, e.EntityValue));
			});
		}

		[Test]
		public void CanHandleCeiling()
		{
			AssumeFunctionSupported("ceiling");

			Assert.Multiple(() =>
			{
				CanFilter(e => decimal.Ceiling(e.EntityValue) > 1.0m);
				CanSelect(e => decimal.Ceiling(e.EntityValue));
			});
		}

		[Test]
		public void CanHandleCompare()
		{
			AssumeFunctionSupported("sign");

			Assert.Multiple(() =>
			{
				CanFilter(e => decimal.Compare(e.EntityValue, 1.5m) < 1);
				CanFilter(e => decimal.Compare(1.0m, e.EntityValue) < 1);

				CanSelect(e => decimal.Compare(e.EntityValue, 1.5m));
				CanSelect(e => decimal.Compare(1.0m, e.EntityValue));
			});
		}

		[Test]
		public void CanHandleDivide()
		{
			Assert.Multiple(() =>
			{
				CanFilter(e => decimal.Divide(e.EntityValue, 1.25m) < 1);
				CanFilter(e => decimal.Divide(1.25m, e.EntityValue) < 1);

				CanSelect(e => decimal.Divide(e.EntityValue, 1.25m));
				CanSelect(e => decimal.Divide(1.25m, e.EntityValue));
			});
		}

		[Test]
		public void CanHandleEquals()
		{
			Assert.Multiple(() =>
			{
				CanFilter(e => decimal.Equals(e.EntityValue, 1.0m));
				CanFilter(e => decimal.Equals(1.0m, e.EntityValue));
			});
		}

		[Test]
		public void CanHandleFloor()
		{
			AssumeFunctionSupported("floor");

			Assert.Multiple(() =>
			{
				CanFilter(e => decimal.Floor(e.EntityValue) > 1.0m);
				CanSelect(e => decimal.Floor(e.EntityValue));
			});
		}
	
		[Test]
		public void CanHandleTruncate()
		{
			AssumeFunctionSupported("truncate");

			Assert.Multiple(() =>
			{
				CanFilter(e => decimal.Truncate(e.EntityValue) > 1.0m);
				CanSelect(e => decimal.Truncate(e.EntityValue));
			});
		}

		[Test]
		public void CanHandleMultiply()
		{
			Assert.Multiple(() =>
			{
				CanFilter(e => decimal.Multiply(e.EntityValue, 10m) > 10m);
				CanFilter(e => decimal.Multiply(10m, e.EntityValue) > 10m);

				CanSelect(e => decimal.Multiply(e.EntityValue, 10m));
				CanSelect(e => decimal.Multiply(10m, e.EntityValue));
			});
		}

		[Test]
		public void CanHandleNegate()
		{
			Assert.Multiple(() =>
			{
				CanFilter(e => decimal.Negate(e.EntityValue) > -1.0m);
				CanSelect(e => decimal.Negate(e.EntityValue));
			});
		}

		[Test]
		public void CanHandleRemainder()
		{
			Assume.That(TestDialect.SupportsModuloOnDecimal, Is.True);

			Assert.Multiple(() =>
			{
				CanFilter(e => decimal.Remainder(e.EntityValue, 2m) == 0);
				CanFilter(e => decimal.Remainder(2m, e.EntityValue) < 1);

				CanSelect(e => decimal.Remainder(e.EntityValue, 2m));
				CanSelect(e => decimal.Remainder(2m, e.EntityValue));
			});
		}

		[Test]
		public void CanHandleRound()
		{
			AssumeFunctionSupported("round");

			Assert.Multiple(() =>
			{
				CanFilter(e => decimal.Round(e.EntityValue) >= 2.0m);
				CanFilter(e => decimal.Round(e.EntityValue, 1) >= 1.5m);

				// SQL round() always rounds up. 
				CanSelect(e => decimal.Round(e.EntityValue), entities.Select(e => decimal.Round(e.EntityValue, MidpointRounding.AwayFromZero)));
				CanSelect(e => decimal.Round(e.EntityValue, 1), entities.Select(e => decimal.Round(e.EntityValue, 1, MidpointRounding.AwayFromZero)));
			});
		}

		[Test]
		public void CanHandleSubtract()
		{
			Assert.Multiple(() =>
			{
				CanFilter(e => decimal.Subtract(e.EntityValue, 1m) > 1m);
				CanFilter(e => decimal.Subtract(2m, e.EntityValue) > 1m);

				CanSelect(e => decimal.Subtract(e.EntityValue, 1m));
				CanSelect(e => decimal.Subtract(2m, e.EntityValue));
			});
		}

		private void CanFilter(Expression<Func<Entity, bool>> predicate)
		{
			using (ISession session = OpenSession())
			using (session.BeginTransaction())
			{
				IEnumerable<Entity> inMemory = entities.Where(predicate.Compile()).ToList();
				IEnumerable<Entity> inSession = session.Query<Entity>().Where(predicate).ToList();

				CollectionAssert.AreEquivalent(inMemory, inSession);
			}
		}

		private void CanSelect(Expression<Func<Entity, decimal>> predicate)
		{
			IEnumerable<decimal> inMemory = entities.Select(predicate.Compile()).ToList();

			CanSelect(predicate, inMemory);
		}

		private void CanSelect(Expression<Func<Entity, decimal>> predicate, IEnumerable<decimal> expected)
		{
			using (ISession session = OpenSession())
			using (session.BeginTransaction())
			{
				IEnumerable<decimal> inSession = null;
				Assert.That(() => inSession = session.Query<Entity>().Select(predicate).ToList(), Throws.Nothing);

				Assert.That(inSession, Is.EquivalentTo(expected).Using((decimal a, decimal b) => Math.Abs(a - b) < 0.0001m));
			}
		}
	}
}
