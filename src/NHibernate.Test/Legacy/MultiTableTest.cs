using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate.Dialect;
using NHibernate.DomainModel;
using NUnit.Framework;

namespace NHibernate.Test.Legacy
{
	/// <summary>
	/// Summary description for MultiTableTest.
	/// </summary>
	[TestFixture]
	public class MultiTableTest : TestCase
	{
		protected override IList Mappings
		{
			get { return new string[] {"Multi.hbm.xml", "MultiExtends.hbm.xml"}; }
		}

		[Test]
		public void FetchManyToOne()
		{
			ISession s = OpenSession();
			s.CreateCriteria(typeof(Po)).SetFetchMode("Set", FetchMode.Eager).List();
			s.CreateCriteria(typeof(Po)).SetFetchMode("List", FetchMode.Eager).List();
			s.Close();
		}


		[Test]
		public void Joins()
		{
			ISession s = OpenSession();
			s.CreateQuery("from Lower l join l.YetAnother l2 where lower(l2.Name) > 'a'").List();
			s.CreateQuery("from SubMulti sm join sm.Children smc where smc.Name > 'a'").List();
			s.CreateQuery("select s, ya from Lower s join s.YetAnother ya").List();
			s.CreateQuery("from Lower s1 join s1.Bag s2").List();
			s.CreateQuery("from Lower s1 left join s1.Bag s2").List();
			s.CreateQuery("select s, a from Lower s join s.Another a").List();
			s.CreateQuery("select s, a from Lower s left join s.Another a").List();
			s.CreateQuery("from Top s, Lower ls").List();
			s.CreateQuery("from Lower ls join ls.Set s where s.Name > 'a'").List();
			s.CreateQuery("from Po po join po.List sm where sm.Name > 'a'").List();
			s.CreateQuery("from Lower ls inner join ls.Another s where s.Name is not null").List();
			s.CreateQuery("from Lower ls where ls.Other.Another.Name is not null").List();
			s.CreateQuery("from Multi m where m.Derived like 'F%'").List();
			s.CreateQuery("from SubMulti m where m.Derived like 'F%'").List();
			s.Close();
		}


		[Test]
		public void JoinOpenBug()
		{
			// Known bug in H2.1, fixed in H3
			/*
			ISession s = OpenSession();
			s.CreateQuery("from Lower l where lower(l.YetAnother.Top.Name) > 'a'").List();
			s.Close();
			*/
		}

		[Test]
		public void SubclassCollection()
		{
			ISession s = OpenSession();
			SubMulti sm = new SubMulti();
			SubMulti sm1 = new SubMulti();
			SubMulti sm2 = new SubMulti();
			sm.Children = new List<SubMulti> {sm1, sm2};
			sm.MoreChildren = new List<SubMulti> {sm1, sm2};
			sm.ExtraProp = "foo";
			sm1.Parent = sm;
			sm2.Parent = sm;
			object id = s.Save(sm);
			s.Save(sm1);
			s.Save(sm2);
			s.Flush();
			s.Close();

			sessions.Evict(typeof(SubMulti));

			s = OpenSession();
			// TODO: I don't understand why h2.0.3/h2.1 issues a select statement here

			Assert.AreEqual(2,
							s.CreateQuery(
								"select s from SubMulti as sm join sm.Children as s where s.Amount>-1 and s.Name is null").List().
								Count);
			Assert.AreEqual(2, s.CreateQuery("select elements(sm.Children) from SubMulti as sm").List().Count);
			Assert.AreEqual(1,
							s.CreateQuery(
								"select distinct sm from SubMulti as sm join sm.Children as s where s.Amount>-1 and s.Name is null")
								.List().Count);
			sm = (SubMulti) s.Load(typeof(SubMulti), id);
			Assert.AreEqual(2, sm.Children.Count);

			ICollection filterColl =
				s.CreateFilter(sm.MoreChildren, "select count(*) where this.Amount>-1 and this.Name is null").List();
			foreach (object obj in filterColl)
			{
				Assert.AreEqual(2, obj);
				// only want the first one
				break;
			}
			Assert.AreEqual("FOO", sm.Derived, "should have uppercased the column in a formula");

			IEnumerator enumer =
				s.CreateQuery("select distinct s from s in class SubMulti where s.MoreChildren[1].Amount < 1.0").Enumerable().
					GetEnumerator();
			Assert.IsTrue(enumer.MoveNext());
			Assert.AreSame(sm, enumer.Current);
			Assert.AreEqual(2, sm.MoreChildren.Count);
			s.Delete(sm);

			foreach (object obj in sm.Children)
			{
				s.Delete(obj);
			}
			s.Flush();
			s.Close();
		}

		[Test]
		public void CollectionOnly()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			NotMono m = new NotMono();
			long id = (long) s.Save(m);
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			s.Update(m, id);
			s.Flush();
			m.Address = "foo bar";
			s.Flush();
			s.Delete(m);
			t.Commit();
			s.Close();
		}

		[Test]
		public void Queries()
		{
			ISession s = OpenSession();
			long id = 1L;

			if (Dialect is MsSql2000Dialect)
			{
				id = (long) s.Save(new TrivialClass());
			}
			else
			{
				s.Save(new TrivialClass(), id);
			}

			s.Flush();
			s.Close();

			s = OpenSession();
			TrivialClass tc = (TrivialClass) s.Load(typeof(TrivialClass), id);
			s.CreateQuery("from s in class TrivialClass where s.id = 2").List();
			s.CreateQuery("select s.Count from s in class Top").List();
			s.CreateQuery("from s in class Lower where s.Another.Name='name'").List();
			s.CreateQuery("from s in class Lower where s.YetAnother.Name='name'").List();
			s.CreateQuery("from s in class Lower where s.YetAnother.Name='name' and s.YetAnother.Foo is null").List();
			s.CreateQuery("from s in class Top where s.Count=1").List();
			s.CreateQuery("select s.Count from s in class Top, ls in class Lower where ls.Another=s").List();
			s.CreateQuery("select elements(ls.Bag), elements(ls.Set) from ls in class Lower").List();
			s.CreateQuery("from s in class Lower").Enumerable();
			s.CreateQuery("from s in class Top").Enumerable();
			s.Delete(tc);
			s.Flush();
			s.Close();
		}

		[Test]
		public void Constraints()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			SubMulti sm = new SubMulti();
			sm.Amount = 66.5f;
			if (Dialect is MsSql2000Dialect)
			{
				s.Save(sm);
			}
			else
			{
				s.Save(sm, (long) 2);
			}
			t.Commit();
			s.Close();

			s = OpenSession();
			s.Delete("from sm in class SubMulti");
			t = s.BeginTransaction();
			t.Commit();
			s.Close();
		}

		[Test]
		public void MultiTable()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			Multi multi = new Multi();
			multi.ExtraProp = "extra";
			multi.Name = "name";
			Top simp = new Top();
			simp.Date = DateTime.Now;
			simp.Name = "simp";
			object mid;
			object sid;
			if (Dialect is MsSql2000Dialect)
			{
				mid = s.Save(multi);
				sid = s.Save(simp);
			}
			else
			{
				mid = 123L;
				sid = 1234L;
				s.Save(multi, mid);
				s.Save(simp, sid);
			}
			SubMulti sm = new SubMulti();
			sm.Amount = 66.5f;
			object smid;
			if (Dialect is MsSql2000Dialect)
			{
				smid = s.Save(sm);
			}
			else
			{
				smid = 2L;
				s.Save(sm, smid);
			}
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			multi.ExtraProp = multi.ExtraProp + "2";
			multi.Name = "new name";
			s.Update(multi, mid);
			simp.Name = "new name";
			s.Update(simp, sid);
			sm.Amount = 456.7f;
			s.Update(sm, smid);
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			multi = (Multi) s.Load(typeof(Multi), mid);
			Assert.AreEqual("extra2", multi.ExtraProp);
			multi.ExtraProp = multi.ExtraProp + "3";
			Assert.AreEqual("new name", multi.Name);
			multi.Name = "newer name";
			sm = (SubMulti) s.Load(typeof(SubMulti), smid);
			Assert.AreEqual(456.7f, sm.Amount);
			sm.Amount = 23423f;
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			multi = (Multi) s.Load(typeof(Top), mid);
			simp = (Top) s.Load(typeof(Top), sid);
			Assert.IsFalse(simp is Multi);
			Assert.AreEqual("extra23", multi.ExtraProp);
			Assert.AreEqual("newer name", multi.Name);
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			IEnumerator enumer = s.CreateQuery("select\n\ns from s in class Top where s.Count>0").Enumerable().GetEnumerator();
			bool foundSimp = false;
			bool foundMulti = false;
			bool foundSubMulti = false;
			while (enumer.MoveNext())
			{
				object o = enumer.Current;
				if ((o is Top) && !(o is Multi)) foundSimp = true;
				if ((o is Multi) && !(o is SubMulti)) foundMulti = true;
				if (o is SubMulti) foundSubMulti = true;
			}
			Assert.IsTrue(foundSimp);
			Assert.IsTrue(foundMulti);
			Assert.IsTrue(foundSubMulti);

			s.CreateQuery("from m in class Multi where m.Count>0 and m.ExtraProp is not null").List();
			s.CreateQuery("from m in class Top where m.Count>0 and m.Name is not null").List();
			s.CreateQuery("from m in class Lower where m.Other is not null").List();
			s.CreateQuery("from m in class Multi where m.Other.id = 1").List();
			s.CreateQuery("from m in class SubMulti where m.Amount > 0.0").List();

			Assert.AreEqual(2, s.CreateQuery("from m in class Multi").List().Count);

			//if( !(dialect is Dialect.HSQLDialect) ) 
			//{
			Assert.AreEqual(1, s.CreateQuery("from m in class Multi where m.class = SubMulti").List().Count);
			Assert.AreEqual(1, s.CreateQuery("from m in class Top where m.class = Multi").List().Count);
			//}

			Assert.AreEqual(3, s.CreateQuery("from s in class Top").List().Count);
			Assert.AreEqual(0, s.CreateQuery("from ls in class Lower").List().Count);
			Assert.AreEqual(1, s.CreateQuery("from sm in class SubMulti").List().Count);

			s.CreateQuery("from ls in class Lower, s in elements(ls.Bag) where s.id is not null").List();
			s.CreateQuery("from ls in class Lower, s in elements(ls.Set) where s.id is not null").List();
			s.CreateQuery("from sm in class SubMulti where exists elements(sm.Children)").List();

			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			if (TestDialect.SupportsSelectForUpdateOnOuterJoin)
				multi = (Multi)s.Load(typeof(Top), mid, LockMode.Upgrade);
			simp = (Top) s.Load(typeof(Top), sid);
			s.Lock(simp, LockMode.UpgradeNoWait);
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			s.Update(multi, mid);
			s.Delete(multi);
			Assert.AreEqual(2, s.Delete("from s in class Top"));
			t.Commit();
			s.Close();
		}

		[Test]
		public void MultiTableGeneratedId()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			Multi multi = new Multi();
			multi.ExtraProp = "extra";
			multi.Name = "name";
			Top simp = new Top();
			simp.Date = DateTime.Now;
			simp.Name = "simp";
			object multiId = s.Save(multi);
			object simpId = s.Save(simp);
			SubMulti sm = new SubMulti();
			sm.Amount = 66.5f;
			object smId = s.Save(sm);
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			multi.ExtraProp += "2";
			multi.Name = "new name";
			s.Update(multi, multiId);
			simp.Name = "new name";
			s.Update(simp, simpId);
			sm.Amount = 456.7f;
			s.Update(sm, smId);
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			multi = (Multi) s.Load(typeof(Multi), multiId);
			Assert.AreEqual("extra2", multi.ExtraProp);
			multi.ExtraProp += "3";
			Assert.AreEqual("new name", multi.Name);
			multi.Name = "newer name";
			sm = (SubMulti) s.Load(typeof(SubMulti), smId);
			Assert.AreEqual(456.7f, sm.Amount);
			sm.Amount = 23423f;
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			multi = (Multi) s.Load(typeof(Top), multiId);
			simp = (Top) s.Load(typeof(Top), simpId);
			Assert.IsFalse(simp is Multi);
			// Can't see the point of this test since the variable is declared as Multi!
			//Assert.IsTrue( multi is Multi );
			Assert.AreEqual("extra23", multi.ExtraProp);
			Assert.AreEqual("newer name", multi.Name);
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			IEnumerable enumer = s.CreateQuery("select\n\ns from s in class Top where s.Count>0").Enumerable();
			bool foundSimp = false;
			bool foundMulti = false;
			bool foundSubMulti = false;

			foreach (object obj in enumer)
			{
				if ((obj is Top) && !(obj is Multi)) foundSimp = true;
				if ((obj is Multi) && !(obj is SubMulti)) foundMulti = true;
				if (obj is SubMulti) foundSubMulti = true;
			}
			Assert.IsTrue(foundSimp);
			Assert.IsTrue(foundMulti);
			Assert.IsTrue(foundSubMulti);

			s.CreateQuery("from m in class Multi where m.Count>0 and m.ExtraProp is not null").List();
			s.CreateQuery("from m in class Top where m.Count>0 and m.Name is not null").List();
			s.CreateQuery("from m in class Lower where m.Other is not null").List();
			s.CreateQuery("from m in class Multi where m.Other.id = 1").List();
			s.CreateQuery("from m in class SubMulti where m.Amount > 0.0").List();

			Assert.AreEqual(2, s.CreateQuery("from m in class Multi").List().Count);
			Assert.AreEqual(3, s.CreateQuery("from s in class Top").List().Count);
			Assert.AreEqual(0, s.CreateQuery("from s in class Lower").List().Count);
			Assert.AreEqual(1, s.CreateQuery("from sm in class SubMulti").List().Count);

			s.CreateQuery("from ls in class Lower, s in elements(ls.Bag) where s.id is not null").List();
			s.CreateQuery("from sm in class SubMulti where exists elements(sm.Children)").List();
			
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			if (TestDialect.SupportsSelectForUpdateOnOuterJoin)
				multi = (Multi) s.Load(typeof(Top), multiId, LockMode.Upgrade);
			simp = (Top) s.Load(typeof(Top), simpId);
			s.Lock(simp, LockMode.UpgradeNoWait);
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			s.Update(multi, multiId);
			s.Delete(multi);
			Assert.AreEqual(2, s.Delete("from s in class Top"));
			t.Commit();
			s.Close();
		}

		[Test]
		public void MultiTableCollections()
		{
			if (Dialect is MySQLDialect)
			{
				return;
			}

			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			Assert.AreEqual(0, s.CreateQuery("from s in class Top").List().Count);
			Multi multi = new Multi();
			multi.ExtraProp = "extra";
			multi.Name = "name";
			Top simp = new Top();
			simp.Date = DateTime.Now;
			simp.Name = "simp";
			object mid;
			object sid;
			if (Dialect is MsSql2000Dialect)
			{
				mid = s.Save(multi);
				sid = s.Save(simp);
			}
			else
			{
				mid = 123L;
				sid = 1234L;
				s.Save(multi, mid);
				s.Save(simp, sid);
			}
			Lower ls = new Lower();
			ls.Other = ls;
			ls.Another = ls;
			ls.YetAnother = ls;
			ls.Name = "Less Simple";
			ls.Set = new HashSet<Top> { multi, simp };

			object id;
			if (Dialect is MsSql2000Dialect)
			{
				id = s.Save(ls);
			}
			else
			{
				id = 2L;
				s.Save(ls, id);
			}
			t.Commit();
			s.Close();
			Assert.AreSame(ls, ls.Other);
			Assert.AreSame(ls, ls.Another);
			Assert.AreSame(ls, ls.YetAnother);

			s = OpenSession();
			t = s.BeginTransaction();
			ls = (Lower) s.Load(typeof(Lower), id);
			Assert.AreSame(ls, ls.Other);
			Assert.AreSame(ls, ls.Another);
			Assert.AreSame(ls, ls.YetAnother);
			Assert.AreEqual(2, ls.Set.Count);

			int foundMulti = 0;
			int foundSimple = 0;

			foreach (object obj in ls.Set)
			{
				if (obj is Top) foundSimple++;
				if (obj is Multi) foundMulti++;
			}
			Assert.AreEqual(2, foundSimple);
			Assert.AreEqual(1, foundMulti);
			Assert.AreEqual(3, s.Delete("from s in class Top"));
			t.Commit();
			s.Close();
		}

		[Test]
		public void MultiTableManyToOne()
		{
			if (Dialect is MySQLDialect)
			{
				return;
			}

			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			Assert.AreEqual(0, s.CreateQuery("from s in class Top").List().Count);
			Multi multi = new Multi();
			multi.ExtraProp = "extra";
			multi.Name = "name";
			Top simp = new Top();
			simp.Date = DateTime.Now;
			simp.Name = "simp";
			object mid;

			if (Dialect is MsSql2000Dialect)
			{
				mid = s.Save(multi);
			}
			else
			{
				mid = 123L;
				s.Save(multi, mid);
			}
			Lower ls = new Lower();
			ls.Other = ls;
			ls.Another = multi;
			ls.YetAnother = ls;
			ls.Name = "Less Simple";
			object id;
			if (Dialect is MsSql2000Dialect)
			{
				id = s.Save(ls);
			}
			else
			{
				id = 2L;
				s.Save(ls, id);
			}
			t.Commit();
			s.Close();

			Assert.AreSame(ls, ls.Other);
			Assert.AreSame(multi, ls.Another);
			Assert.AreSame(ls, ls.YetAnother);

			s = OpenSession();
			t = s.BeginTransaction();
			ls = (Lower) s.Load(typeof(Lower), id);
			Assert.AreSame(ls, ls.Other);
			Assert.AreSame(ls, ls.YetAnother);
			Assert.AreEqual("name", ls.Another.Name);
			Assert.IsTrue(ls.Another is Multi);
			s.Delete(ls);
			s.Delete(ls.Another);
			t.Commit();
			s.Close();
		}

		[Test]
		public void MultiTableNativeId()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			Multi multi = new Multi();
			multi.ExtraProp = "extra";
			object id = s.Save(multi);
			Assert.IsNotNull(id);
			s.Delete(multi);
			t.Commit();
			s.Close();
		}

		[Test]
		public void Collection()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			Multi multi1 = new Multi();
			multi1.ExtraProp = "extra1";
			Multi multi2 = new Multi();
			multi2.ExtraProp = "extra2";
			Po po = new Po();
			multi1.Po = po;
			multi2.Po = po;
			po.Set = new HashSet<Multi> {multi1, multi2};
			po.List = new List<SubMulti> {new SubMulti()};
			object id = s.Save(po);
			Assert.IsNotNull(id);
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			po = (Po) s.Load(typeof(Po), id);
			Assert.AreEqual(2, po.Set.Count);
			Assert.AreEqual(1, po.List.Count);
			s.Delete(po);
			Assert.AreEqual(0, s.CreateQuery("from s in class Top").List().Count);
			t.Commit();
			s.Close();
		}

		[Test]
		public void OneToOne()
		{
			ISession s = OpenSession();
			Lower ls = new Lower();
			object id = s.Save(ls);
			s.Flush();
			s.Close();

			s = OpenSession();
			s.Load(typeof(Lower), id);
			s.Close();

			s = OpenSession();
			s.Delete(s.Load(typeof(Lower), id));
			s.Flush();
			s.Close();
		}

		[Test]
		public void CollectionPointer()
		{
			ISession s = OpenSession();
			Lower ls = new Lower();
			IList<Top> list = new List<Top>();
			ls.Bag = list;
			Top simple = new Top();
			object id = s.Save(ls);
			s.Save(simple);
			s.Flush();
			list.Add(simple);
			s.Flush();
			s.Close();

			s = OpenSession();
			ls = (Lower) s.Load(typeof(Lower), id);
			Assert.AreEqual(1, ls.Bag.Count);
			s.Delete("from o in class System.Object");
			s.Flush();
			s.Close();
		}

		[Test]
		public void DynamicUpdate()
		{
			object id;
			Top simple = new Top();

			simple.Name = "saved";

			using (ISession s = OpenSession())
			{
				id = s.Save(simple);
				s.Flush();

				simple.Name = "updated";
				s.Flush();
			}

			using (ISession s = OpenSession())
			{
				simple = (Top) s.Load(typeof(Top), id);
				Assert.AreEqual("updated", simple.Name, "name should have been updated");
			}

			using (ISession s = OpenSession())
			{
				s.Delete("from Top");
				s.Flush();
			}
		}
	}
}