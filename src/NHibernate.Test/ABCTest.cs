using System;
using System.Collections;

using NUnit.Framework;
using NHibernate;
using NHibernate.DomainModel;


namespace NHibernate.Test
{
	/// <summary>
	/// Summary description for ABCTest.
	/// </summary>
	[TestFixture]
	public class ABCTest : TestCase
	{
		
		#region NUnit.Framework.TestFixture Members

		[TestFixtureSetUp]
		public void TestFixtureSetUp() 
		{
			ExportSchema(new string[] { "ABC.hbm.xml"});
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
		public void Subclassing() 
		{
			ISession s = sessions.OpenSession();
			ITransaction t = s.BeginTransaction();
			C1 c1 = new C1();
			D d = new D();
			d.Amount =213.34f;
			// id used to be a increment
			c1.Id = 1;
			c1.Address = "foo bar";
			c1.Count = 23432;
			c1.Name ="c1";
			c1.D = d;
			s.Save(c1);
			d.Id = c1.Id;
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
				c1.D.Amount>213.3f
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
				c1.D.Amount>213.3f
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
			s.Find("from b in class B");
			t.Commit();
			s.Close();
		}
	}
}
