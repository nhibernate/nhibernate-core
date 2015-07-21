using System.Collections;
using NHibernate.Cfg;
using NHibernate.Criterion;
using NHibernate.Util;
using NUnit.Framework;

namespace NHibernate.Test.SubselectFetchTest
{
	[TestFixture]
	public class SubselectFetchFixture : TestCase
	{
		protected override void Configure(Configuration cfg)
		{
			cfg.SetProperty(Cfg.Environment.GenerateStatistics, "true");
		}

		[Test]
		public void SubselectFetchHql()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			Parent p = new Parent("foo");
			p.Children.Add(new Child("foo1"));
			p.Children.Add(new Child("foo2"));
			Parent q = new Parent("bar");
			q.Children.Add(new Child("bar1"));
			q.Children.Add(new Child("bar2"));
			ArrayHelper.AddAll(q.MoreChildren, p.Children);
			s.Save(p);
			s.Save(q);
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();

			sessions.Statistics.Clear();

			IList parents = s.CreateQuery("from Parent where name between 'bar' and 'foo' order by name desc")
				.List();
			p = (Parent) parents[0];
			q = (Parent) parents[1];

			Assert.IsFalse(NHibernateUtil.IsInitialized(p.Children));
			Assert.IsFalse(NHibernateUtil.IsInitialized(q.Children));

			Assert.AreEqual(p.Children.Count, 2);

			Assert.IsTrue(NHibernateUtil.IsInitialized(p.Children[0]));

			Assert.IsTrue(NHibernateUtil.IsInitialized(q.Children));

			Assert.AreEqual(q.Children.Count, 2);

			Assert.IsTrue(NHibernateUtil.IsInitialized(q.Children[0]));

			Assert.IsFalse(NHibernateUtil.IsInitialized(p.MoreChildren));
			Assert.IsFalse(NHibernateUtil.IsInitialized(q.MoreChildren));

			Assert.AreEqual(p.MoreChildren.Count, 0);

			Assert.IsTrue(NHibernateUtil.IsInitialized(q.MoreChildren));

			Assert.AreEqual(q.MoreChildren.Count, 2);

			Assert.IsTrue(NHibernateUtil.IsInitialized(q.MoreChildren[0]));

			Assert.AreEqual(3, sessions.Statistics.PrepareStatementCount);

			Child c = (Child) p.Children[0];
			NHibernateUtil.Initialize(c.Friends);

			s.Delete(p);
			s.Delete(q);

			t.Commit();
			s.Close();
		}

		[Test]
		public void SubselectFetchNamedParam()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			Parent p = new Parent("foo");
			p.Children.Add(new Child("foo1"));
			p.Children.Add(new Child("foo2"));
			Parent q = new Parent("bar");
			q.Children.Add(new Child("bar1"));
			q.Children.Add(new Child("bar2"));
			ArrayHelper.AddAll(q.MoreChildren, p.Children);
			s.Save(p);
			s.Save(q);
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();

			sessions.Statistics.Clear();

			IList parents = s.CreateQuery("from Parent where name between :bar and :foo order by name desc")
				.SetParameter("bar", "bar")
				.SetParameter("foo", "foo")
				.List();
			p = (Parent) parents[0];
			q = (Parent) parents[1];

			Assert.IsFalse(NHibernateUtil.IsInitialized(p.Children));
			Assert.IsFalse(NHibernateUtil.IsInitialized(q.Children));

			Assert.AreEqual(p.Children.Count, 2);

			Assert.IsTrue(NHibernateUtil.IsInitialized(p.Children[0]));

			Assert.IsTrue(NHibernateUtil.IsInitialized(q.Children));

			Assert.AreEqual(q.Children.Count, 2);

			Assert.IsTrue(NHibernateUtil.IsInitialized(q.Children[0]));

			Assert.IsFalse(NHibernateUtil.IsInitialized(p.MoreChildren));
			Assert.IsFalse(NHibernateUtil.IsInitialized(q.MoreChildren));

			Assert.AreEqual(p.MoreChildren.Count, 0);

			Assert.IsTrue(NHibernateUtil.IsInitialized(q.MoreChildren));

			Assert.AreEqual(q.MoreChildren.Count, 2);

			Assert.IsTrue(NHibernateUtil.IsInitialized(q.MoreChildren[0]));

			Assert.AreEqual(3, sessions.Statistics.PrepareStatementCount);

			Child c = (Child) p.Children[0];
			NHibernateUtil.Initialize(c.Friends);

			s.Delete(p);
			s.Delete(q);

			t.Commit();
			s.Close();
		}

		[Test]
		public void SubselectFetchPosParam()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			Parent p = new Parent("foo");
			p.Children.Add(new Child("foo1"));
			p.Children.Add(new Child("foo2"));
			Parent q = new Parent("bar");
			q.Children.Add(new Child("bar1"));
			q.Children.Add(new Child("bar2"));
			ArrayHelper.AddAll(q.MoreChildren, p.Children);
			s.Save(p);
			s.Save(q);
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();

			sessions.Statistics.Clear();

			IList parents = s.CreateQuery("from Parent where name between ? and ? order by name desc")
				.SetParameter(0, "bar")
				.SetParameter(1, "foo")
				.List();
			p = (Parent) parents[0];
			q = (Parent) parents[1];

			Assert.IsFalse(NHibernateUtil.IsInitialized(p.Children));
			Assert.IsFalse(NHibernateUtil.IsInitialized(q.Children));

			Assert.AreEqual(p.Children.Count, 2);

			Assert.IsTrue(NHibernateUtil.IsInitialized(p.Children[0]));

			Assert.IsTrue(NHibernateUtil.IsInitialized(q.Children));

			Assert.AreEqual(q.Children.Count, 2);

			Assert.IsTrue(NHibernateUtil.IsInitialized(q.Children[0]));

			Assert.IsFalse(NHibernateUtil.IsInitialized(p.MoreChildren));
			Assert.IsFalse(NHibernateUtil.IsInitialized(q.MoreChildren));

			Assert.AreEqual(p.MoreChildren.Count, 0);

			Assert.IsTrue(NHibernateUtil.IsInitialized(q.MoreChildren));

			Assert.AreEqual(q.MoreChildren.Count, 2);

			Assert.IsTrue(NHibernateUtil.IsInitialized(q.MoreChildren[0]));

			Assert.AreEqual(3, sessions.Statistics.PrepareStatementCount);

			Child c = (Child) p.Children[0];
			NHibernateUtil.Initialize(c.Friends);

			s.Delete(p);
			s.Delete(q);

			t.Commit();
			s.Close();
		}

		[Test]
		public void SubselectFetchWithLimit()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			Parent p = new Parent("foo");
			p.Children.Add(new Child("foo1"));
			p.Children.Add(new Child("foo2"));
			Parent q = new Parent("bar");
			q.Children.Add(new Child("bar1"));
			q.Children.Add(new Child("bar2"));
			Parent r = new Parent("aaa");
			r.Children.Add(new Child("aaa1"));
			s.Save(p);
			s.Save(q);
			s.Save(r);
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();

			sessions.Statistics.Clear();

			IList parents = s.CreateQuery("from Parent order by name desc")
				.SetMaxResults(2)
				.List();
			p = (Parent) parents[0];
			q = (Parent) parents[1];
			Assert.IsFalse(NHibernateUtil.IsInitialized(p.Children));
			Assert.IsFalse(NHibernateUtil.IsInitialized(p.MoreChildren));
			Assert.IsFalse(NHibernateUtil.IsInitialized(q.Children));
			Assert.IsFalse(NHibernateUtil.IsInitialized(q.MoreChildren));
			Assert.AreEqual(p.MoreChildren.Count, 0);
			Assert.AreEqual(p.Children.Count, 2);
			Assert.IsTrue(NHibernateUtil.IsInitialized(q.Children));
			Assert.IsTrue(NHibernateUtil.IsInitialized(q.MoreChildren));

			Assert.AreEqual(3, sessions.Statistics.PrepareStatementCount);

			r = (Parent) s.Get(typeof(Parent), r.Name);
			Assert.IsTrue(NHibernateUtil.IsInitialized(r.Children)); // The test for True is the test of H3.2
			Assert.IsFalse(NHibernateUtil.IsInitialized(r.MoreChildren));
			Assert.AreEqual(r.Children.Count, 1);
			Assert.AreEqual(r.MoreChildren.Count, 0);

			s.Delete(p);
			s.Delete(q);
			s.Delete(r);

			t.Commit();
			s.Close();
		}

		[Test]
		public void ManyToManyCriteriaJoin()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			Parent p = new Parent("foo");
			p.Children.Add(new Child("foo1"));
			p.Children.Add(new Child("foo2"));
			Parent q = new Parent("bar");
			q.Children.Add(new Child("bar1"));
			q.Children.Add(new Child("bar2"));
			ArrayHelper.AddAll(q.MoreChildren, p.Children);
			s.Save(p);
			s.Save(q);
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();

			s.CreateCriteria(typeof(Parent))
				.AddOrder(Order.Desc("Name"))
				// H3 has this after CreateCriteria("Friends"), but it's not yet supported in NH
				.CreateCriteria("MoreChildren")
				.CreateCriteria("Friends")
				.List();

			IList parents = s.CreateCriteria(typeof(Parent))
				.SetFetchMode("MoreChildren", FetchMode.Join)
				.SetFetchMode("MoreChildren.Friends", FetchMode.Join)
				.AddOrder(Order.Desc("Name"))
				.List();

			s.Delete(parents[0]);
			s.Delete(parents[1]);

			t.Commit();
			s.Close();
		}

		[Test]
		public void SubselectFetchCriteria()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			Parent p = new Parent("foo");
			p.Children.Add(new Child("foo1"));
			p.Children.Add(new Child("foo2"));
			Parent q = new Parent("bar");
			q.Children.Add(new Child("bar1"));
			q.Children.Add(new Child("bar2"));
			ArrayHelper.AddAll(q.MoreChildren, p.Children);
			s.Save(p);
			s.Save(q);
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();

			sessions.Statistics.Clear();

			IList parents = s.CreateCriteria(typeof(Parent))
				.Add(Expression.Between("Name", "bar", "foo"))
				.AddOrder(Order.Desc("Name"))
				.List();
			p = (Parent) parents[0];
			q = (Parent) parents[1];

			Assert.IsFalse(NHibernateUtil.IsInitialized(p.Children));
			Assert.IsFalse(NHibernateUtil.IsInitialized(q.Children));

			Assert.AreEqual(p.Children.Count, 2);

			Assert.IsTrue(NHibernateUtil.IsInitialized(p.Children[0]));

			Assert.IsTrue(NHibernateUtil.IsInitialized(q.Children));

			Assert.AreEqual(q.Children.Count, 2);

			Assert.IsTrue(NHibernateUtil.IsInitialized(q.Children[0]));

			Assert.IsFalse(NHibernateUtil.IsInitialized(p.MoreChildren));
			Assert.IsFalse(NHibernateUtil.IsInitialized(q.MoreChildren));

			Assert.AreEqual(p.MoreChildren.Count, 0);

			Assert.IsTrue(NHibernateUtil.IsInitialized(q.MoreChildren));

			Assert.AreEqual(q.MoreChildren.Count, 2);

			Assert.IsTrue(NHibernateUtil.IsInitialized(q.MoreChildren[0]));

			Assert.AreEqual(3, sessions.Statistics.PrepareStatementCount);

			Child c = (Child) p.Children[0];
			NHibernateUtil.Initialize(c.Friends);

			s.Delete(p);
			s.Delete(q);

			t.Commit();
			s.Close();
		}

		protected override IList Mappings
		{
			get { return new string[] {"SubselectFetchTest.ParentChild.hbm.xml"}; }
		}

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		//public override string CacheConcurrencyStrategy
		//{
		//    get { return null; }
		//}
	}
}
