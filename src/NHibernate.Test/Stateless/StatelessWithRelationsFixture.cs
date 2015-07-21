using System.Collections;
using NUnit.Framework;
using System.Collections.Generic;

namespace NHibernate.Test.Stateless
{
	[TestFixture]
	public class StatelessWithRelationsFixture : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get { return new[] { "Stateless.Naturalness.hbm.xml" }; }
		}

		[Test]
		public void ShouldWorkLoadingComplexEntities()
		{
			const string crocodileFather = "Crocodile father";
			const string crocodileMother = "Crocodile mother";

			using (ISession s = sessions.OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				var rf = new Reptile { Description = crocodileFather };
				var rm = new Reptile { Description = crocodileMother };
				var rc1 = new Reptile { Description = "Crocodile" };
				var rc2 = new Reptile { Description = "Crocodile" };
				var rfamily = new Family<Reptile>
				{
					Father = rf,
					Mother = rm,
					Childs = new HashSet<Reptile> { rc1, rc2 }
				};
				s.Save("ReptilesFamily", rfamily);
				tx.Commit();
			}

			const string humanFather = "Fred";
			const string humanMother = "Wilma";
			using (ISession s = sessions.OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				var hf = new Human { Description = "Flinstone", Name = humanFather };
				var hm = new Human { Description = "Flinstone", Name = humanMother };
				var hc1 = new Human { Description = "Flinstone", Name = "Pebbles" };
				var hfamily = new Family<Human>
				{
					Father = hf,
					Mother = hm,
					Childs = new HashSet<Human> { hc1 }
				};
				s.Save("HumanFamily", hfamily);
				tx.Commit();
			}

			using (IStatelessSession s = sessions.OpenStatelessSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				IList<Family<Human>> hf = s.CreateQuery("from HumanFamily").List<Family<Human>>();
				Assert.That(hf.Count, Is.EqualTo(1));
				Assert.That(hf[0].Father.Name, Is.EqualTo(humanFather));
				Assert.That(hf[0].Mother.Name, Is.EqualTo(humanMother));
				Assert.That(NHibernateUtil.IsInitialized(hf[0].Childs), Is.True, "No lazy collection should be initialized");
				//Assert.That(hf[0].Childs, Is.Null, "Collections should be ignored by stateless session.");

				IList<Family<Reptile>> rf = s.CreateQuery("from ReptilesFamily").List<Family<Reptile>>();
				Assert.That(rf.Count, Is.EqualTo(1));
				Assert.That(rf[0].Father.Description, Is.EqualTo(crocodileFather));
				Assert.That(rf[0].Mother.Description, Is.EqualTo(crocodileMother));
				Assert.That(NHibernateUtil.IsInitialized(hf[0].Childs), Is.True, "No lazy collection should be initialized");
				//Assert.That(rf[0].Childs, Is.Null, "Collections should be ignored by stateless session.");

				tx.Commit();
			}

			using (ISession s = sessions.OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				s.Delete("from HumanFamily");
				s.Delete("from ReptilesFamily");
				tx.Commit();
			}
		}

	}
}