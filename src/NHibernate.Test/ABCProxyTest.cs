using System;
using System.Collections;

using NUnit.Framework;

using NHibernate;
using NHibernate.DomainModel;

namespace NHibernate.Test
{
	[TestFixture]
	public class ABCProxyTest : TestCase
	{
		
		#region NUnit.Framework.TestFixture Members

		[TestFixtureSetUp]
		public void TestFixtureSetUp() 
		{
			ExportSchema(new string[] { "ABCProxy.hbm.xml"});
		}

		[SetUp]
		public void SetUp() 
		{
			// there are test in here where we don't need to resetup the 
			// tables - so only set the tables up once
		}

		[TearDown]
		public override void TearDown() 
		{
			// do nothing except not let the base TearDown get called
		}

		[TestFixtureTearDown]
		public void TestFixtureTearDown() 
		{
			base.TearDown();
		}

		#endregion

		[Test]
		//[Ignore("Proxies Required - http://jira.nhibernate.org:8080/browse/NH-41")]
		public void Subclassing()
		{
			ISession s = sessions.OpenSession();
			ITransaction t = s.BeginTransaction();
			C1 c1 = new C1();
			D d = new D();
			d.Amount =213.34f;
			c1.Address = "foo bar";
			c1.Count = 23432;
			c1.Name = "c1";
			c1.D = d;
			s.Save(c1);
			d.Id = c1.Id;
			s.Save(d);
			t.Commit();
			s.Close();
		
			s = sessions.OpenSession();
			t = s.BeginTransaction();
			// Test won't run after this line because of proxy initalization problems
			A c1a = (A) s.Load(typeof(A), c1.Id );
			Assert.IsFalse( NHibernateUtil.IsInitialized(c1a) );
			Assert.IsTrue( c1a.Name.Equals("c1") );
			t.Commit();
			s.Close();
		
			s = sessions.OpenSession();
			t = s.BeginTransaction();
			B c1b = (B) s.Load( typeof(B), c1.Id );
			Assert.IsTrue(
				(c1b.Count==23432) &&
				c1b.Name.Equals("c1")
				);
			t.Commit();
			s.Close();
		
			s = sessions.OpenSession();
			t = s.BeginTransaction();
			c1 = (C1) s.Load( typeof(C1), c1.Id );
			Assert.IsTrue(
				c1.Address.Equals("foo bar") &&
				(c1.Count==23432) &&
				c1.Name.Equals("c1") &&
				c1.D.Amount>213.3f
				);
			t.Commit();
			s.Close();
		
			s = sessions.OpenSession();
			t = s.BeginTransaction();
			c1a = (A) s.Load( typeof(A), c1.Id );
			Assert.IsTrue( c1a.Name.Equals("c1") );
			c1 = (C1) s.Load( typeof(C1), c1.Id );
			Assert.IsTrue(
				c1.Address.Equals("foo bar") &&
				(c1.Count==23432) &&
				c1.Name.Equals("c1") &&
				c1.D.Amount>213.3f
				);
			c1b = (B) s.Load( typeof(B), c1.Id );
			Assert.IsTrue(
				(c1b.Count==23432) &&
				c1b.Name.Equals("c1")
				);
			Assert.IsTrue( c1a.Name.Equals("c1") );
			t.Commit();
			s.Close();
		
			s = sessions.OpenSession();
			t = s.BeginTransaction();
			c1a = (A) s.Load( typeof(A), c1.Id );
			Assert.IsTrue( c1a.Name.Equals("c1") );
			c1 = (C1) s.Load( typeof(C1), c1.Id, LockMode.Upgrade );
			Assert.IsTrue(
				c1.Address.Equals("foo bar") &&
				(c1.Count==23432) &&
				c1.Name.Equals("c1") &&
				c1.D.Amount>213.3f
				);
			c1b = (B) s.Load( typeof(B), c1.Id, LockMode.Upgrade);
			Assert.IsTrue(
				(c1b.Count==23432) &&
				c1b.Name.Equals("c1")
				);
			Assert.IsTrue( c1a.Name.Equals("c1") );
			t.Commit();
			s.Close();
		
			s = sessions.OpenSession();
			t = s.BeginTransaction();
			c1a = (A) s.Load( typeof(A), c1.Id );
			c1 = (C1) s.Load( typeof(C1), c1.Id );
			c1b = (B) s.Load( typeof(B), c1.Id );
			Assert.IsTrue( c1a.Name.Equals("c1") );
			Assert.IsTrue(
				c1.Address.Equals("foo bar") &&
				(c1.Count==23432) &&
				c1.Name.Equals("c1") &&
				c1.D.Amount>213.3f
				);
			Assert.IsTrue(
				(c1b.Count==23432) &&
				c1b.Name.Equals("c1")
				);
			System.Console.Out.WriteLine( s.Delete("from a in class A") );
			t.Commit();
			s.Close();
		
			s = sessions.OpenSession();
			t = s.BeginTransaction();
			s.Save( new B() );
			s.Save( new A() );
			Assert.IsTrue( s.Find("from b in class B").Count==1 );
			Assert.IsTrue( s.Find("from a in class A").Count==2 );
			s.Delete("from a in class A");
			t.Commit();
			s.Close();

		}

		[Test]
		public void SubclassMap()
		{
			//Test is converted, but the original didn't check anything
			ISession s = sessions.OpenSession();
			ITransaction t = s.BeginTransaction();
			B b = new B();
			s.Save(b);
			Hashtable map = new Hashtable();
			map.Add("3", 1 ); 
			b.Map = map;
			s.Flush();
			s.Delete(b);
			t.Commit();
		
			s = sessions.OpenSession();
			t = s.BeginTransaction();
			map = new Hashtable();
			map.Add("3", 1); 
			b = new B();
			b.Map = map;
			s.Save(b);
			s.Flush();
			s.Delete(b);
			t.Commit();
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

			using( ISession s = sessions.OpenSession() )
			using( ITransaction t = s.BeginTransaction() )
			{
				aid = s.Save(a);
				d2id = s.Save(d2);
				t.Commit();
			}

			using( ISession s = sessions.OpenSession() )
			using( ITransaction t = s.BeginTransaction() )
			{
				IList l = s.Find( "from E e, A a where e.Reverse = a.Forward and a = ?", a, NHibernateUtil.Entity (typeof(A)) );
				Assert.AreEqual( 1, l.Count );
				l = s.Find( "from E e join fetch e.Reverse" );
				Assert.AreEqual( 2, l.Count );
				t.Commit();
			}

			using( ISession s = sessions.OpenSession() )
			using( ITransaction t = s.BeginTransaction() )
			{
				IList l = s.Find( "from E e" );
				Assert.AreEqual( 2, l.Count );
				E e = (E) l[ 0 ];
				Assert.AreSame( e, e.Reverse.Forward );
				e = (E) l[ 1 ];
				Assert.AreSame( e, e.Reverse.Forward );
				t.Commit();
			}

			using( ISession s = sessions.OpenSession() )
			using( ITransaction t = s.BeginTransaction() )
			{
				a = (A) s.Load( typeof(A), aid );
				d2 = (E) s.Load( typeof(E), d2id );
				Assert.AreSame( a, a.Forward.Reverse );
				Assert.AreSame( d2, d2.Reverse.Forward );
				s.Delete( a );
				s.Delete( a.Forward );
				s.Delete( d2 );
				s.Delete( d2.Reverse );
				t.Commit();
			}

			using( ISession s = sessions.OpenSession() )
			using( ITransaction t = s.BeginTransaction() )
			{
				IList l = s.Find( "from E e" );
				Assert.AreEqual( 0, l.Count ); 
				t.Commit();
			}
		}
	}
}
