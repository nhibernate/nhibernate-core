using System.Collections;
using NHibernate.Id.Enhanced;
using NUnit.Framework;

namespace NHibernate.Test.IdGen.Enhanced.Table
{
	[TestFixture]
	public class BasicTableTest : TestCase
	{
		protected override IList Mappings
		{
			get { return new[] { "IdGen.Enhanced.Table.Basic.hbm.xml" }; }
		}

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}


		[Test]
		public void TestNormalBoundary()
		{
			var persister = sessions.GetEntityPersister(typeof(Entity).FullName);
			Assert.That(persister.IdentifierGenerator, Is.TypeOf<TableGenerator>());

			var generator = (TableGenerator)persister.IdentifierGenerator;
			Assert.That(generator.Optimizer, Is.TypeOf<OptimizerFactory.NoopOptimizer>());

			int count = 5;
			Entity[] entities = new Entity[count];
			using (ISession s = OpenSession())
			{
				using (ITransaction transaction = s.BeginTransaction())
				{
					for (int i = 0; i < count; i++)
					{
						entities[i] = new Entity("" + (i + 1));
						s.Save(entities[i]);
						long expectedId = i + 1;
						Assert.That(entities[i].Id, Is.EqualTo(expectedId));
						Assert.That(generator.TableAccessCount, Is.EqualTo(expectedId));
						Assert.That(generator.Optimizer.LastSourceValue, Is.EqualTo(expectedId));

					}
					transaction.Commit();
				}


				using (ITransaction transaction = s.BeginTransaction())
				{
					for (int i = 0; i < count; i++)
					{
						Assert.That(entities[i].Id, Is.EqualTo(i + 1));
						s.Delete(entities[i]);
					}
					transaction.Commit();
				}
			}
		}
	}
}
