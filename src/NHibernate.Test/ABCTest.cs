using System;
using NUnit.Framework;
using NHibernate;
using NHibernate.DomainModel;
using System.Collections;

namespace NHibernate.Test
{
	/// <summary>
	/// Summary description for ABCTest.
	/// </summary>
	[TestFixture]
	public class ABCTest : TestCase
	{
		[SetUp]
		public void SetUp()
		{
			ExportSchema(new string[] { "ABC.hbm.xml"});
		}

		[Test]
		public void Subclassing() 
		{
			ISession s = sessions.OpenSession();
			ITransaction t = s.BeginTransaction();
			C1 c1 = new C1();
			D d = new D();
			d.amount =213.34f;
			c1.address = "foo bar";
			c1.count = 23432;
			c1.name ="c1";
			c1.d = d;
			s.Save(c1);
			d.id = c1.id;
			s.Save(d);
		
			Assert.IsTrue( s.Find("from c in class C2 where 1=1 or 1=1").Count ==0 );
		
			t.Commit();
			s.Close();

			s = sessions.OpenSession();
			t = s.BeginTransaction();
			c1 = (C1) s.Load( typeof(A), c1.id );
			Assert.IsTrue(
				c1.address.Equals("foo bar") &&
				(c1.count==23432) &&
				c1.name.Equals("c1") &&
				c1.d.amount>213.3f
				);
			t.Commit();
			s.Close();
		
			s = sessions.OpenSession();
			t = s.BeginTransaction();
			c1 = (C1) s.Load( typeof(B), c1.id );
			Assert.IsTrue(
				c1.address.Equals("foo bar") &&
				(c1.count==23432) &&
				c1.name.Equals("c1") &&
				c1.d.amount>213.3f
				);
			t.Commit();
			s.Close();
		
			s = sessions.OpenSession();
			t = s.BeginTransaction();
			c1 = (C1) s.Load( typeof(C1), c1.id );
			Assert.IsTrue(
				c1.address.Equals("foo bar") &&
				(c1.count==23432) &&
				c1.name.Equals("c1") &&
				c1.d.amount>213.3f
				);
			t.Commit();
			s.Close();

			s = sessions.OpenSession();
			t = s.BeginTransaction();
			s.Find("from b in class B");
			t.Commit();
			s.Close();
		}
	}
}
