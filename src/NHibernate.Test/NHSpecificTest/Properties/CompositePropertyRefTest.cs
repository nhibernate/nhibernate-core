using System;
using System.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.Properties
{
	[TestFixture]
	public class CompositePropertyRefTest : BugTestCase
	{
		private long p_id;
		private long p2_id;

		protected override void OnSetUp()
		{
			using (var s = OpenSession())
			{
				using (var tx = s.BeginTransaction())
				{
					var p = new Person { Name = "Steve", UserId = "steve" };
					var a = new Address { Addr = "Texas", Country = "USA", Person = p };
					var p2 = new Person { Name = "Max", UserId = "max" };
					var act = new Account { Type = Convert.ToChar("c"), User = p2 };
					p2.Accounts.Add(act);
					p_id = (long)s.Save(p);
					s.Save(a);
					p2_id = (long)s.Save(p2);
					s.Save(act);
					tx.Commit();
				}
			}
		}

		protected override void OnTearDown()
		{
			using (var s = OpenSession())
			{
				using (var tx = s.BeginTransaction())
				{
					s.Delete("from Account");
					s.Delete("from Address");
					s.Delete("from Person");
					tx.Commit();
				}
			}
		}

		[Test]
		public void MappingOuterJoin()
		{
			using(var s = OpenSession())
			{
				using (s.BeginTransaction())
				{
					var	p = s.Get<Person>(p_id); //get address reference by outer join
					var p2 = s.Get<Person>(p2_id); //get null address reference by outer join
					Assert.IsNull(p2.Address);
					Assert.IsNotNull(p.Address);
					var l = s.CreateQuery("from Person").List(); //pull address references for cache
					Assert.AreEqual(l.Count, 2);
					Assert.IsTrue(l.Contains(p) && l.Contains(p2));
				}
			}
		}

		[Test]
		public void AddressBySequentialSelect()
		{
			using (var s = OpenSession())
			{
				using (s.BeginTransaction())
				{
					var l = s.CreateQuery("from Person p order by p.Name").List<Person>(); 
					Assert.AreEqual(l.Count, 2);
					Assert.IsNull(l[0].Address);
					Assert.IsNotNull(l[1].Address);
				}
			}		
		}

		[Test]
		public void AddressOuterJoin()
		{
			using (var s = OpenSession())
			{
				using (s.BeginTransaction())
				{
					var l = s.CreateQuery("from Person p left join fetch p.Address a order by a.Country").List<Person>();
					Assert.AreEqual(l.Count, 2);
					if (l[0].Name.Equals("Max"))
					{
						Assert.IsNull(l[0].Address);
						Assert.IsNotNull(l[1].Address);
					}
					else
					{
						Assert.IsNull(l[1].Address);
						Assert.IsNotNull(l[0].Address);
					}
				}
			}		
		}

		[Test]
		public void AccountsOuterJoin()
		{
			using (var s = OpenSession())
			{
				using (s.BeginTransaction())
				{
					var l = s.CreateQuery("from Person p left join p.Accounts").List();
					for (var i = 0; i < 2; i++)
					{
						var row = (object[])l[i];
						var px = (Person)row[0];
						var accounts = px.Accounts;
						Assert.IsFalse(NHibernateUtil.IsInitialized(accounts));
						Assert.IsTrue(px.Accounts.Count > 0 || row[1] == null);
					}
				}
			}
		}

		[Test]
		public void AccountsOuterJoinVerifyInitialization()
		{
			using (var s = OpenSession())
			{
				using (s.BeginTransaction())
				{
					var l = s.CreateQuery("from Person p left join fetch p.Accounts a order by p.Name").List<Person>();
					var p0 = l[0];
					Assert.IsTrue(NHibernateUtil.IsInitialized(p0.Accounts));
					Assert.AreEqual(p0.Accounts.Count, 1);
					Assert.AreSame(p0.Accounts.First().User, p0);
					var p1 = l[1];
					Assert.IsTrue(NHibernateUtil.IsInitialized(p1.Accounts));
					Assert.AreEqual(p1.Accounts.Count, 0);
				}
			}
		}
	}
}
