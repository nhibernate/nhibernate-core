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
			c1.Address = "foo bar";
			c1.Count = 23432;
			c1.Name ="c1";
			c1.D = d;
			s.Save(c1);
			d.id = c1.Id;
			s.Save(d);
		
			Assert.IsTrue( s.Find("from c in class C2 where 1=1 or 1=1").Count ==0 );
		
			t.Commit();
			s.Close();

			s = sessions.OpenSession();
			t = s.BeginTransaction();
			c1 = (C1) s.Load( typeof(A), c1.Id );
			Assert.IsTrue(
				c1.Address.Equals("foo bar") &&
				(c1.Count==23432) &&
				c1.Name.Equals("c1") &&
				c1.D.amount>213.3f
				);
			t.Commit();
			s.Close();
		
			s = sessions.OpenSession();
			t = s.BeginTransaction();
			c1 = (C1) s.Load( typeof(B), c1.Id );
			Assert.IsTrue(
				c1.Address.Equals("foo bar") &&
				(c1.Count==23432) &&
				c1.Name.Equals("c1") &&
				c1.D.amount>213.3f
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
				c1.D.amount>213.3f
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
