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
		[SetUp]
		public void SetUp()
		{
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
			Assert.IsFalse( NHibernate.IsInitialized(c1a) );
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
	}
}
