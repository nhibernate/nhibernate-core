using System.Collections;
using NHibernate.Cfg;
using NHibernate.Stat;
using NUnit.Framework;
using System.Collections.Generic;

namespace NHibernate.Test.Ondelete
{
	[TestFixture]
	public class JoinedSubclassFixture : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get { return new string[] { "Ondelete.EFGJoinedSubclass.hbm.xml" }; }
		}

		protected override void Configure(Configuration cfg)
		{
			cfg.SetProperty(Environment.GenerateStatistics, "true");
		}

		[Test]
		public void JoinedSubclassCascade()
		{
			G g1 = new G("thing", "white", "10x10");
			F f1 = new F("thing2", "blue");
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			s.Save(g1);
			s.Save(f1);
			t.Commit();
			s.Close();

			IStatistics statistics = Sfi.Statistics;
			statistics.Clear();

			s = OpenSession();
			t = s.BeginTransaction();
			IList<E> l = s.CreateQuery("from E").List<E>();
			statistics.Clear();

			s.Delete(l[0]);
			s.Delete(l[1]);
			t.Commit();
			s.Close();

			Assert.AreEqual(2, statistics.EntityDeleteCount);

			// In this case the batcher reuse the same command because have same SQL and same parametersTypes
			Assert.AreEqual(1, statistics.PrepareStatementCount);
		}
	}
}
