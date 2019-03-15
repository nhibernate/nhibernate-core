using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace NHibernate.Test.Extralazy
{
	[TestFixture]
	public class ExtraLazyFixture : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override string[] Mappings
		{
			get { return new[] {"Extralazy.UserGroup.hbm.xml"}; }
		}

		protected override string CacheConcurrencyStrategy
		{
			get { return null; }
		}

		protected override void OnTearDown()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.Delete("from System.Object");
				t.Commit();
			}
		}

		[Test]
		public void ExtraLazyWithWhereClause()
		{
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.CreateSQLQuery("insert into Users (Name,Password) values('gavin','secret')")
					.UniqueResult();
				s.CreateSQLQuery("insert into Photos (Title,Owner) values('PRVaaa','gavin')")
					.UniqueResult();
				s.CreateSQLQuery("insert into Photos (Title,Owner) values('PUBbbb','gavin')")
					.UniqueResult();
				t.Commit();
			}

			using (ISession s = OpenSession())
			{
				var gavin = s.Get<User>("gavin");
				Assert.AreEqual(1, gavin.Photos.Count);
				Assert.IsFalse(NHibernateUtil.IsInitialized(gavin.Documents));
			}

			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.CreateSQLQuery("delete from Photos")
					.UniqueResult();
				s.CreateSQLQuery("delete from Users")
					.UniqueResult();

				t.Commit();
			}
			Sfi.Evict(typeof (User));
			Sfi.Evict(typeof (Photo));
		}

		[Test]
		public void OrphanDelete()
		{
			User gavin = null;
			Document hia = null;
			Document hia2 = null;

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = new User("gavin", "secret");
				hia = new Document("HiA", "blah blah blah", gavin);
				hia2 = new Document("HiA2", "blah blah blah blah", gavin);
				gavin.Documents.Add(hia); // NH: added ; I don't understand how can work in H3.2.5 without add
				gavin.Documents.Add(hia2); // NH: added 
				s.Persist(gavin);
				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = s.Get<User>("gavin");
				Assert.AreEqual(2, gavin.Documents.Count);
				gavin.Documents.Remove(hia2);
				// Do not convert Documents.Contains to Does.Contain/Does.Not.Contain: NUnit constraints will trigger
				// initialization of the collection. Moreover, with extra-lazy, collection.Contains works even if
				// the entity does not properly overrides Equals and GetHashCode and a detached instance is used,
				// provided the collection is not yet initialized, which is the case here.
				Assert.IsFalse(gavin.Documents.Contains(hia2));
				Assert.IsTrue(gavin.Documents.Contains(hia));
				Assert.AreEqual(1, gavin.Documents.Count);
				Assert.IsFalse(NHibernateUtil.IsInitialized(gavin.Documents));
				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = s.Get<User>("gavin");
				Assert.AreEqual(1, gavin.Documents.Count);
				Assert.IsFalse(gavin.Documents.Contains(hia2));
				Assert.IsTrue(gavin.Documents.Contains(hia));
				Assert.IsFalse(NHibernateUtil.IsInitialized(gavin.Documents));
				Assert.That(s.Get<Document>("HiA2"), Is.Null);
				gavin.Documents.Clear();
				Assert.IsTrue(NHibernateUtil.IsInitialized(gavin.Documents));
				s.Delete(gavin);
				t.Commit();
			}
		}

		[Test]
		public void OrphanDeleteWithEnumeration()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var gavin = new User("gavin", "secret");
				gavin.Documents.Add(new Document("HiA", "blah blah blah", gavin));
				gavin.Documents.Add(new Document("HiA2", "blah blah blah blah", gavin));
				s.Persist(gavin);
				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var hia2 = s.Get<Document>("HiA2");
				var hia = s.Get<Document>("HiA");
				var gavin = s.Get<User>("gavin");
				Assert.That(gavin.Documents, Has.Count.EqualTo(2));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Documents), Is.False, "Expecting non initialized collection");
				gavin.Documents.Remove(hia2);
				// Force an enumeration
				using (var e = gavin.Documents.GetEnumerator())
					e.MoveNext();
				Assert.That(NHibernateUtil.IsInitialized(gavin.Documents), Is.True, "Expecting initialized collection");
				Assert.That(gavin.Documents, Does.Not.Contain(hia2).And.Contain(hia).And.Count.EqualTo(1));
				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var hia = s.Get<Document>("HiA");
				var gavin = s.Get<User>("gavin");
				Assert.That(gavin.Documents, Has.Count.EqualTo(1).And.Contain(hia));
				Assert.That(s.Get<Document>("HiA2"), Is.Null);
				t.Commit();
			}
		}

		[Test]
		public void Get()
		{
			User gavin = null;
			User turin = null;
			Group g = null;

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = new User("gavin", "secret");
				turin = new User("turin", "tiger");
				g = new Group("developers");
				g.Users.Add("gavin", gavin);
				g.Users.Add("turin", turin);
				s.Persist(g);
				gavin.Session.Add("foo", new SessionAttribute("foo", "foo bar baz"));
				gavin.Session.Add("bar", new SessionAttribute("bar", "foo bar baz 2"));
				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				g = s.Get<Group>("developers");
				gavin = (User) g.Users["gavin"];
				turin = (User) g.Users["turin"];
				Assert.That(gavin, Is.Not.Null);
				Assert.That(turin, Is.Not.Null);
				Assert.That(g.Users.ContainsKey("emmanuel"), Is.False);
				Assert.IsFalse(NHibernateUtil.IsInitialized(g.Users));
				Assert.That(gavin.Session["foo"], Is.Not.Null);
				Assert.That(turin.Session.ContainsKey("foo"), Is.False);
				Assert.IsFalse(NHibernateUtil.IsInitialized(gavin.Session));
				Assert.IsFalse(NHibernateUtil.IsInitialized(turin.Session));
				s.Delete(gavin);
				s.Delete(turin);
				s.Delete(g);
				t.Commit();
			}
		}

		[Test]
		public void RemoveClear()
		{
			User gavin = null;
			User turin = null;
			Group g = null;

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = new User("gavin", "secret");
				turin = new User("turin", "tiger");
				g = new Group("developers");
				g.Users.Add("gavin", gavin);
				g.Users.Add("turin", turin);
				s.Persist(g);
				gavin.Session.Add("foo", new SessionAttribute("foo", "foo bar baz"));
				gavin.Session.Add("bar", new SessionAttribute("bar", "foo bar baz 2"));
				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				g = s.Get<Group>("developers");
				gavin = (User) g.Users["gavin"];
				turin = (User) g.Users["turin"];
				Assert.IsFalse(NHibernateUtil.IsInitialized(g.Users));
				g.Users.Clear();
				gavin.Session.Remove("foo");
				Assert.IsTrue(NHibernateUtil.IsInitialized(g.Users));
				Assert.IsTrue(NHibernateUtil.IsInitialized(gavin.Session));
				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				g = s.Get<Group>("developers");
				//Assert.IsTrue( g.Users.IsEmpty() );
				//Assert.IsFalse( NHibernateUtil.IsInitialized( g.getUsers() ) );
				gavin = s.Get<User>("gavin");
				Assert.IsFalse(gavin.Session.ContainsKey("foo"));
				Assert.IsFalse(NHibernateUtil.IsInitialized(gavin.Session));
				s.Delete(gavin);
				s.Delete(turin);
				s.Delete(g);
				t.Commit();
			}
		}

		[Test]
		public void IndexFormulaMap()
		{
			User gavin = null;
			User turin = null;
			Group g = null;
			IDictionary<string, SessionAttribute> smap = null;

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = new User("gavin", "secret");
				turin = new User("turin", "tiger");
				g = new Group("developers");
				g.Users.Add("gavin", gavin);
				g.Users.Add("turin", turin);
				s.Persist(g);
				gavin.Session.Add("foo", new SessionAttribute("foo", "foo bar baz"));
				gavin.Session.Add("bar", new SessionAttribute("bar", "foo bar baz 2"));
				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				g = s.Get<Group>("developers");
				Assert.AreEqual(2, g.Users.Count);
				g.Users.Remove("turin");
				smap = ((User) g.Users["gavin"]).Session;
				Assert.AreEqual(2, smap.Count);
				smap.Remove("bar");
				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				g = s.Get<Group>("developers");
				Assert.AreEqual(1, g.Users.Count);
				smap = ((User) g.Users["gavin"]).Session;
				Assert.AreEqual(1, smap.Count);
				gavin = (User) g.Users["gavin"]; // NH: put in JAVA return the previous value
				g.Users["gavin"] = turin;
				s.Delete(gavin);
				Assert.AreEqual(0, s.CreateQuery("select count(*) from SessionAttribute").UniqueResult<long>());
				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				g = s.Get<Group>("developers");
				Assert.AreEqual(1, g.Users.Count);
				turin = (User) g.Users["turin"];
				smap = turin.Session;
				Assert.AreEqual(0, smap.Count);
				Assert.AreEqual(1L, s.CreateQuery("select count(*) from User").UniqueResult<long>());
				s.Delete(g);
				s.Delete(turin);
				Assert.AreEqual(0, s.CreateQuery("select count(*) from User").UniqueResult<long>());
				t.Commit();
			}
		}

		[Test]
		public void SQLQuery()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				User gavin = new User("gavin", "secret");
				User turin = new User("turin", "tiger");
				gavin.Session.Add("foo", new SessionAttribute("foo", "foo bar baz"));
				gavin.Session.Add("bar", new SessionAttribute("bar", "foo bar baz 2"));
				s.Persist(gavin);
				s.Persist(turin);
				s.Flush();
				s.Clear();

				IList results = s.GetNamedQuery("UserSessionData").SetParameter("uname", "%in").List();
				Assert.AreEqual(2, results.Count);
				// NH Different behavior : NH1612, HHH-2831
				gavin = (User) results[0];
				Assert.AreEqual("gavin", gavin.Name);
				Assert.AreEqual(2, gavin.Session.Count);
				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.Delete("from SessionAttribute");
				s.Delete("from User");
				t.Commit();
			}
		}

		[Test]
		public void AddToUninitializedSetWithLaterLazyLoad()
		{
			User gavin;

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = new User("gavin", "secret");
				var hia = new Document("HiA", "blah blah blah", gavin);
				gavin.Documents.Add(hia);
				s.Persist(gavin);
				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = s.Get<User>("gavin");
				var hia2 = new Document("HiA2", "blah blah blah blah", gavin);
				gavin.Documents.Add(hia2);

				foreach (var _ in gavin.Documents)
				{
					//Force Iteration
				}

				Assert.That(gavin.Documents.Contains(hia2));
				s.Delete(gavin);
				t.Commit();
			}
		}
	}
}
