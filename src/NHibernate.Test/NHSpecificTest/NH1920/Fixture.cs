using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1920
{
	[TestFixture]
	public class Fixture : BugTestCase
	{

		[Test] 
		public void Can_Query_Without_Collection_Size_Condition() 
		{ 
			using (ISession sess = OpenSession()) 
			using (ITransaction tx = sess.BeginTransaction()) 
			{ 
				sess.SaveOrUpdate(new Customer() { IsDeleted = false }); 
				tx.Commit(); 
			} 
			using (ISession sess = OpenSession()) 
			using (ITransaction tx = sess.BeginTransaction()) 
			{ 
				sess.EnableFilter("state").SetParameter("deleted", false); 
				var result = sess 
					.CreateQuery("from Customer c join c.Orders o where c.id > :cid") 
					.SetParameter("cid", 0) 
					.List(); 
				Assert.That(result.Count == 0); 
				tx.Commit(); 
			} 
			using (ISession sess = OpenSession()) 
			using (ITransaction tx = sess.BeginTransaction()) 
			{ 
				sess.Delete("from System.Object"); 
				tx.Commit(); 
			} 
		} 

		[Test] 
		public void Can_Query_With_Collection_Size_Condition() 
		{ 
			using (ISession sess = OpenSession()) 
			using (ITransaction tx = sess.BeginTransaction()) 
			{ 
				sess.SaveOrUpdate(new Customer() { IsDeleted = false }); 
				tx.Commit(); 
			} 
			using (ISession sess = OpenSession()) 
			using (ITransaction tx = sess.BeginTransaction()) 
			{ 
				sess.EnableFilter("state").SetParameter("deleted", false); 
				var result = sess 
					.CreateQuery("from Customer c join c.Orders o where c.id > :cid and c.Orders.size > 0") 
					.SetParameter("cid", 0) 
					.List(); 
				Assert.That(result.Count == 0); 
				tx.Commit(); 
			} 
			using (ISession sess = OpenSession()) 
			using (ITransaction tx = sess.BeginTransaction()) 
			{ 
				sess.Delete("from System.Object"); 
				tx.Commit(); 
			} 
		} 

	}
}
