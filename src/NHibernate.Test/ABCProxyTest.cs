using System;
using NUnit.Framework;
using NHibernate;
using NHibernate.DomainModel;
using System.Collections;

namespace NHibernate.Test
{
	[TestFixture]
	public class ABCProxyTest : TestCase
	{
		[SetUp]
		public void SetUp()
		{
			ExecuteStatement("drop table D", false);
			ExecuteStatement("drop table A", false);
			ExecuteStatement("drop table map", false);
			ExportSchema(new string[] { "ABCProxy.hbm.xml"});
		}

		[Test]
		[Ignore("Test will fail because of proxy initalization problems")]
		public void Subclassing()
		{
			ISession s = sessions.OpenSession();
			ITransaction t = s.BeginTransaction();
			C1 c1 = new C1();
			D d = new D();
			d.amount =213.34f;
			c1.address = "foo bar";
			c1.count = 23432;
			c1.name = "c1";
			c1.d = d;
			s.Save(c1);
			d.id = c1.id;
			s.Save(d);
			t.Commit();
			s.Close();
		
			s = sessions.OpenSession();
			t = s.BeginTransaction();
			// Test won't run after this line because of proxy initalization problems
			A c1a = (A) s.Load(typeof(A), c1.id );
			Assert.IsFalse( NHibernate.IsInitialized(c1a) );
			Assert.IsTrue( c1a.name.Equals("c1") );
			t.Commit();
			s.Close();
		
			s = sessions.OpenSession();
			t = s.BeginTransaction();
			B c1b = (B) s.Load( typeof(B), c1.id );
			Assert.IsTrue(
				(c1b.count==23432) &&
				c1b.name.Equals("c1")
				);
			t.Commit();
			s.Close();
		
/*			s = sessions.openSession();
			t = s.beginTransaction();
			c1 = (C1) s.load( C1.class, c1.getId() );
			assertTrue(
				c1.getAddress().equals("foo bar") &&
				(c1.getCount()==23432) &&
				c1.getName().equals("c1") &&
				c1.getD().getAmount()>213.3f
				);
			t.commit();
			s.close();
		
			s = sessions.openSession();
			t = s.beginTransaction();
			c1a = (A) s.load( A.class, c1.getId() );
			assertTrue( c1a.getName().equals("c1") );
			c1 = (C1) s.load( C1.class, c1.getId() );
			assertTrue(
				c1.getAddress().equals("foo bar") &&
				(c1.getCount()==23432) &&
				c1.getName().equals("c1") &&
				c1.getD().getAmount()>213.3f
				);
			c1b = (B) s.load( B.class, c1.getId() );
			assertTrue(
				(c1b.getCount()==23432) &&
				c1b.getName().equals("c1")
				);
			assertTrue( c1a.getName().equals("c1") );
			t.commit();
			s.close();
		
			s = sessions.openSession();
			t = s.beginTransaction();
			c1a = (A) s.load( A.class, c1.getId() );
			assertTrue( c1a.getName().equals("c1") );
			c1 = (C1) s.load( C1.class, c1.getId(), LockMode.UPGRADE );
			assertTrue(
				c1.getAddress().equals("foo bar") &&
				(c1.getCount()==23432) &&
				c1.getName().equals("c1") &&
				c1.getD().getAmount()>213.3f
				);
			c1b = (B) s.load( B.class, c1.getId(), LockMode.UPGRADE );
			assertTrue(
				(c1b.getCount()==23432) &&
				c1b.getName().equals("c1")
				);
			assertTrue( c1a.getName().equals("c1") );
			t.commit();
			s.close();
		
			s = sessions.openSession();
			t = s.beginTransaction();
			c1a = (A) s.load( A.class, c1.getId() );
			c1 = (C1) s.load( C1.class, c1.getId() );
			c1b = (B) s.load( B.class, c1.getId() );
			assertTrue( c1a.getName().equals("c1") );
			assertTrue(
				c1.getAddress().equals("foo bar") &&
				(c1.getCount()==23432) &&
				c1.getName().equals("c1") &&
				c1.getD().getAmount()>213.3f
				);
			assertTrue(
				(c1b.getCount()==23432) &&
				c1b.getName().equals("c1")
				);
			System.out.println( s.delete("from a in class A") );
			t.commit();
			s.close();
		
			s = sessions.openSession();
			t = s.beginTransaction();
			s.save( new B() );
			s.save( new A() );
			assertTrue( s.find("from b in class B").size()==1 );
			assertTrue( s.find("from a in class A").size()==2 );
			s.delete("from a in class A");
			t.commit();
			s.close();
*/
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
			b.map = map;
			s.Flush();
			s.Delete(b);
			t.Commit();
		
			s = sessions.OpenSession();
			t = s.BeginTransaction();
			map = new Hashtable();
			map.Add("3", 1); 
			b = new B();
			b.map = map;
			s.Save(b);
			s.Flush();
			s.Delete(b);
			t.Commit();
		}
	}
}
