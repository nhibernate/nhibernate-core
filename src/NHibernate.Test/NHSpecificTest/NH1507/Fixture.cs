using System;
using System.Collections;
using NHibernate.Driver;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1507
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override bool AppliesTo(Engine.ISessionFactoryImplementor factory)
		{
			return !(factory.ConnectionProvider.Driver is OracleManagedDataClientDriver);
		}

		protected override void OnSetUp()
		{
			CreateData();
		}

		protected override void OnTearDown()
		{
			CleanupData();
		}

		private void CreateData()
		{
			//Employee
			var emp = new Employee
						{
							Address = "Zombie street",
							City = "Bitonto",
							PostalCode = "66666",
							FirstName = "tomb",
							LastName = "mutilated"
						};

			//and his related orders
			var order = new Order
							{OrderDate = DateTime.Now, Employee = emp, ShipAddress = "dead zone 1", ShipCountry = "Deadville"};

			var order2 = new Order
							{OrderDate = DateTime.Now, Employee = emp, ShipAddress = "dead zone 2", ShipCountry = "Deadville"};

			//Employee with no related orders but with same PostalCode
			var emp2 = new Employee
						{
							Address = "Gut street",
							City = "Mariotto",
							Country = "Arised",
							PostalCode = "66666",
							FirstName = "carcass",
							LastName = "purulent"
						};

			//Order with no related employee but with same ShipCountry
			var order3 = new Order {OrderDate = DateTime.Now, ShipAddress = "dead zone 2", ShipCountry = "Deadville"};

			using (ISession session = OpenSession())
			{
				using (ITransaction tx = session.BeginTransaction())
				{
					session.Save(emp);
					session.Save(emp2);
					session.Save(order);
					session.Save(order2);
					session.Save(order3);

					tx.Commit();
				}
			}
		}

		private void CleanupData()
		{
			using (ISession session = OpenSession())
			{
				using (ITransaction tx = session.BeginTransaction())
				{
					//delete empolyee and related orders
					session.Delete("from Employee ee where ee.PostalCode = '66666'");

					//delete order not related to employee
					session.Delete("from Order oo where oo.ShipCountry = 'Deadville'");

					tx.Commit();
				}
			}
		}

		[Test]
		public void ExplicitJoin()
		{
			using (ISession session = OpenSession())
			{
				//explicit join
				IList results =
					session.CreateQuery("select count(*) from Order as entity join entity.Employee ee "
										+ "where ee.PostalCode='66666' or entity.ShipCountry='Deadville'").List();

				//Debug.Assert(list[0].Equals(191), "Wrong number of orders, returned: " + list[0].ToString());
				Assert.AreEqual(2, results[0]);
			}
		}

		[Test]
		public void ImplicitJoinFailingTest()
		{
			using (ISession session = OpenSession())
			{
				//implicit join
				IList results =
					session.CreateQuery("select count(*) from Order as entity "
										+ "where entity.Employee.PostalCode='66666' or entity.ShipCountry='Deadville'").List();

				Assert.AreEqual(2, results[0]);
			}
		}
	}
}