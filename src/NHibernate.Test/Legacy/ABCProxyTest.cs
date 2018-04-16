using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate.DomainModel;
using NUnit.Framework;

namespace NHibernate.Test.Legacy
{
	[TestFixture]
	public class ABCProxyTest : TestCase
	{
		protected override IList Mappings
		{
			get { return new string[] {"ABCProxy.hbm.xml"}; }
		}

		[Test]
		public void OptionalOneToOneInCollection()
		{
			C2 c2;

			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				C1 c1 = new C1();
				c2 = new C2();
				c1.C2 = c2;
				c2.C1 = c1;
				c2.C1s = new List<C1>();
				c2.C1s.Add(c1);
				c1.C2 = c2;
				s.Save(c2);
				s.Save(c1);
				t.Commit();
			}

			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				c2 = (C2) s.Get(typeof(C2), c2.Id);
				Assert.IsTrue(c2.C1s.Count == 1);
				s.Delete(c2.C1s[0]);
				s.Delete(c2);
				t.Commit();
			}
		}

		[Test]
		public void SharedColumn()
		{
			C1 c1;
			C2 c2;

			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				c1 = new C1();
				c2 = new C2();
				c1.C2 = c2;
				c2.C1 = c1;
				s.Save(c1);
				s.Save(c2);
				t.Commit();
			}

			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				IList list = s.CreateQuery("from B").List();
				Assert.AreEqual(2, list.Count);
				t.Commit();
			}

			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				c1 = (C1) s.CreateQuery("from C1").UniqueResult();
				c2 = (C2) s.CreateQuery("from C2").UniqueResult();
				Assert.AreSame(c2, c1.C2);
				Assert.AreSame(c1, c2.C1);
				Assert.IsTrue(c1.C2s.Contains(c2));
				Assert.IsTrue(c2.C1s.Contains(c1));
				t.Commit();
			}

			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				c1 = (C1) s.Get(typeof(A), c1.Id);
				c2 = (C2) s.Get(typeof(A), c2.Id);
				Assert.AreSame(c2, c1.C2);
				Assert.AreSame(c1, c2.C1);
				Assert.IsTrue(c1.C2s.Contains(c2));
				Assert.IsTrue(c2.C1s.Contains(c1));
				t.Commit();
			}

			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.Delete(c1);
				s.Delete(c2);
				t.Commit();
			}
		}

		[Test]
		public void Subclassing()
		{
			C1 c1;

			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				c1 = new C1();
				D d = new D();
				d.Amount = 213.34f;
				c1.Address = "foo bar";
				c1.Count = 23432;
				c1.Name = "c1";
				c1.D = d;
				s.Save(c1);
				d.Id = c1.Id;
				s.Save(d);
				t.Commit();
			}

			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				// Test won't run after this line because of proxy initalization problems
				A c1a = (A) s.Load(typeof(A), c1.Id);
				Assert.IsFalse(NHibernateUtil.IsInitialized(c1a));
				Assert.IsTrue(c1a.Name.Equals("c1"));
				t.Commit();
			}

			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				B c1b = (B) s.Load(typeof(B), c1.Id);
				Assert.IsTrue(
					(c1b.Count == 23432) &&
					c1b.Name.Equals("c1")
					);
				t.Commit();
			}

			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				c1 = (C1) s.Load(typeof(C1), c1.Id);
				Assert.IsTrue(
					c1.Address.Equals("foo bar") &&
					(c1.Count == 23432) &&
					c1.Name.Equals("c1") &&
					c1.D.Amount > 213.3f
					);
				t.Commit();
			}

			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				A c1a = (A) s.Load(typeof(A), c1.Id);
				Assert.IsTrue(c1a.Name.Equals("c1"));
				c1 = (C1) s.Load(typeof(C1), c1.Id);
				Assert.IsTrue(
					c1.Address.Equals("foo bar") &&
					(c1.Count == 23432) &&
					c1.Name.Equals("c1") &&
					c1.D.Amount > 213.3f
					);
				B c1b = (B) s.Load(typeof(B), c1.Id);
				Assert.IsTrue(
					(c1b.Count == 23432) &&
					c1b.Name.Equals("c1")
					);
				Assert.IsTrue(c1a.Name.Equals("c1"));
				t.Commit();
			}

			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				A c1a = (A) s.Load(typeof(A), c1.Id);
				Assert.IsTrue(c1a.Name.Equals("c1"));
				c1 = (C1) s.Load(typeof(C1), c1.Id, LockMode.Upgrade);
				Assert.IsTrue(
					c1.Address.Equals("foo bar") &&
					(c1.Count == 23432) &&
					c1.Name.Equals("c1") &&
					c1.D.Amount > 213.3f
					);
				B c1b = (B) s.Load(typeof(B), c1.Id, LockMode.Upgrade);
				Assert.IsTrue(
					(c1b.Count == 23432) &&
					c1b.Name.Equals("c1")
					);
				Assert.IsTrue(c1a.Name.Equals("c1"));
				t.Commit();
			}

			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				A c1a = (A) s.Load(typeof(A), c1.Id);
				c1 = (C1) s.Load(typeof(C1), c1.Id);
				B c1b = (B) s.Load(typeof(B), c1.Id);
				Assert.IsTrue(c1a.Name.Equals("c1"));
				Assert.IsTrue(
					c1.Address.Equals("foo bar") &&
					(c1.Count == 23432) &&
					c1.Name.Equals("c1") &&
					c1.D.Amount > 213.3f
					);
				Assert.IsTrue(
					(c1b.Count == 23432) &&
					c1b.Name.Equals("c1")
					);
				Console.Out.WriteLine(s.Delete("from a in class A"));
				t.Commit();
			}

			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.Save(new B());
				s.Save(new A());
				Assert.IsTrue(s.CreateQuery("from b in class B").List().Count == 1);
				Assert.IsTrue(s.CreateQuery("from a in class A").List().Count == 2);
				s.Delete("from a in class A");
				s.Delete(c1.D);
				t.Commit();
			}
		}

		[Test]
		public void SubclassMap()
		{
			//Test is converted, but the original didn't check anything
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			B b = new B();
			s.Save(b);
			IDictionary<string, string> map = new Dictionary<string, string>();
			map.Add("3", "1");
			b.Map = map;
			s.Flush();
			s.Delete(b);
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			map = new Dictionary<string, string>();
			map.Add("3", "1");
			b = new B();
			b.Map = map;
			s.Save(b);
			s.Flush();
			s.Delete(b);
			t.Commit();
			s.Close();
		}

		[Test, Ignore("ANTLR parser : Not supported ")]
		public void OnoToOneComparing()
		{
			A a = new A();
			E d1 = new E();
			C1 c = new C1();
			E d2 = new E();
			a.Forward = d1;
			d1.Reverse = a;
			c.Forward = d2;
			d2.Reverse = c;

			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.Save(a);
				s.Save(d2);
				t.Commit();
			}
			using (ISession s = OpenSession())
			{
				IList l = s.CreateQuery("from E e, A a where e.Reverse = a.Forward and a = ?").SetEntity(0, a).List();
				Assert.AreEqual(1, l.Count);
			}
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.Delete("from A");
				s.Delete("from E");
				t.Commit();
			}
		}

		[Test]
		public void OneToOne()
		{
			A a = new A();
			E d1 = new E();
			C1 c = new C1();
			E d2 = new E();
			a.Forward = d1;
			d1.Reverse = a;
			c.Forward = d2;
			d2.Reverse = c;

			object aid;
			object d2id;

			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				aid = s.Save(a);
				d2id = s.Save(d2);
				t.Commit();
			}

			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				IList l;
				l = s.CreateQuery("from E e join fetch e.Reverse").List();
				Assert.AreEqual(2, l.Count);
				t.Commit();
			}

			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				IList l = s.CreateQuery("from E e").List();
				Assert.AreEqual(2, l.Count);
				E e = (E) l[0];
				Assert.AreSame(e, e.Reverse.Forward);
				e = (E) l[1];
				Assert.AreSame(e, e.Reverse.Forward);
				t.Commit();
			}

			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				a = (A) s.Load(typeof(A), aid);
				d2 = (E) s.Load(typeof(E), d2id);
				Assert.AreSame(a, a.Forward.Reverse);
				Assert.AreSame(d2, d2.Reverse.Forward);
				s.Delete(a);
				s.Delete(a.Forward);
				s.Delete(d2);
				s.Delete(d2.Reverse);
				t.Commit();
			}

			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				IList l = s.CreateQuery("from E e").List();
				Assert.AreEqual(0, l.Count);
				t.Commit();
			}
		}
	}
}