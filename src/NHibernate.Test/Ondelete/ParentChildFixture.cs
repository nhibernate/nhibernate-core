using System.Collections;
using System.Collections.Generic;
using NHibernate.Cfg;
using NHibernate.Stat;
using NUnit.Framework;

namespace NHibernate.Test.Ondelete
{
	[TestFixture]
	public class ParentChildFixture : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get { return new string[] { "Ondelete.ParentChild.hbm.xml" }; }
		}

		protected override void Configure(Configuration cfg)
		{
			cfg.SetProperty(Environment.GenerateStatistics, "true");
		}

		[Test]
		public void ParentChildCascade()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			Parent p = new Parent("foo");
			p.Children.Add(new Child(p, "foo1"));
			p.Children.Add(new Child(p, "foo2"));
			Parent q = new Parent("bar");
			q.Children.Add(new Child(q, "bar1"));
			q.Children.Add(new Child(q, "bar2"));
			s.Save(p);
			s.Save(q);
			t.Commit();
			s.Close();

			IStatistics statistics = Sfi.Statistics;
			statistics.Clear();

			s = OpenSession();
			t = s.BeginTransaction();
			IList<Parent> l = s.CreateQuery("from Parent").List<Parent>();
			p = l[0];
			q = l[1];
			statistics.Clear();

			s.Delete(p);
			s.Delete(q);
			t.Commit();

			Assert.AreEqual(2, statistics.PrepareStatementCount);
			Assert.AreEqual(6, statistics.EntityDeleteCount);

			t = s.BeginTransaction();
			IList names = s.CreateQuery("from Parent p").List();
			Assert.AreEqual(0, names.Count);

			t.Commit();
			s.Close();
		}
	}
}
