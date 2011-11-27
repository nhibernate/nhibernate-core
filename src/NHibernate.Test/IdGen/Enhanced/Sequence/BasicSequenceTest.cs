using System;
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
			get { return new string[] { "IdGen.Enhanced.Sequence.Basic.hbm.xml" }; }
		}

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}


		[Test]
		public void TestNormalBoundary()
		{
			var persister = sessions.GetEntityPersister(typeof(Entity).FullName);
			Assert.That(persister.IdentifierGenerator, Is.TypeOf<SequenceStyleGenerator>());
			var generator = (SequenceStyleGenerator)persister.IdentifierGenerator;

			int count = 5;
			Entity[] entities = new Entity[count];
			ISession s = OpenSession();
			s.BeginTransaction();

			for (int i = 0; i < count; i++)
			{
				entities[i] = new Entity("" + (i + 1));
				s.Save(entities[i]);
				long expectedId = i + 1;
				Assert.That(entities[i].Id, Is.EqualTo(expectedId));
				Assert.That(generator.DatabaseStructure.TimesAccessed, Is.EqualTo(expectedId));
				Assert.That(generator.Optimizer.LastSourceValue, Is.EqualTo(expectedId));
			}
			s.Transaction.Commit();

			s.BeginTransaction();
			for (int i = 0; i < count; i++)
			{
				Assert.That(entities[i].Id, Is.EqualTo(i + 1));
				s.Delete(entities[i]);
			}
			s.Transaction.Commit();
			s.Close();
		}
	}
}
