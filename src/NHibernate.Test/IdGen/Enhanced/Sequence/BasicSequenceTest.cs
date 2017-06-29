using NUnit.Framework;
using NHibernate.Id.Enhanced;
using System.Collections;

namespace NHibernate.Test.IdGen.Enhanced.Sequence
{
	[TestFixture]
	public class BasicSequenceTest : TestCase
	{
		protected override IList Mappings
		{
			get { return new[] { "IdGen.Enhanced.Sequence.Basic.hbm.xml" }; }
		}

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		[Test]
		public void TestNormalBoundary()
		{
			var persister = Sfi.GetEntityPersister(typeof(Entity).FullName);
			Assert.That(persister.IdentifierGenerator, Is.TypeOf<SequenceStyleGenerator>());

			var generator = (SequenceStyleGenerator)persister.IdentifierGenerator;

			const int count = 5;
			var entities = new Entity[count];

			using (ISession session = OpenSession())
			{
				using (ITransaction transaction = session.BeginTransaction())
				{
					for (int i = 0; i < count; i++)
					{
						entities[i] = new Entity("" + (i + 1));
						session.Save(entities[i]);
						long expectedId = i + 1;
						Assert.That(entities[i].Id, Is.EqualTo(expectedId));
						Assert.That(generator.DatabaseStructure.TimesAccessed, Is.EqualTo(expectedId));
						Assert.That(generator.Optimizer.LastSourceValue, Is.EqualTo(expectedId));
					}
					transaction.Commit();
				}

				using (ITransaction transaction = session.BeginTransaction())
				{
					for (int i = 0; i < count; i++)
					{
						Assert.That(entities[i].Id, Is.EqualTo(i + 1));
						session.Delete(entities[i]);
					}
					transaction.Commit();
				}

				session.Close();
			}
		}
	}
}