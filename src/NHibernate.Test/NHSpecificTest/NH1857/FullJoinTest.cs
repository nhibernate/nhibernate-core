using System;
using System.Collections;
using NHibernate.Dialect;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1857
{
	[TestFixture]
	public class FullJoinTest : BugTestCase
	{
		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var e1 = new Employee(1, "Employee1", new DateTime(1995, 1, 1));
				var e2 = new Employee(2, "Employee2", new DateTime(2007, 8, 1));
				var e3 = new Employee(3, "Employee3", new DateTime(2009, 5, 1));

				var d1 = new Department(1, "Department S");

				d1.AddEmployee(e1);
				d1.AddEmployee(e2);

				session.SaveOrUpdate(d1);
				session.SaveOrUpdate(e1);
				session.SaveOrUpdate(e2);
				session.SaveOrUpdate(e3);

				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.CreateQuery("delete from Employee").ExecuteUpdate();
				session.CreateQuery("delete from Department").ExecuteUpdate();
				transaction.Commit();
			}
		}

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			if (!TestDialect.SupportsFullJoin)
				return false;
			if (dialect is MySQLDialect)
				return false;
			if (dialect is InformixDialect)
				return false;

			return true;
		}

		[Test]
		public void TestFullJoin()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var q = session.CreateQuery("from Employee as e full join e.Department");

				var result = q.List();

				Assert.AreEqual(3, result.Count);
			}
		}
	}
}
