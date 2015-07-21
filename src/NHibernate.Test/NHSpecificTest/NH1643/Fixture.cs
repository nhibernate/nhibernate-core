using NHibernate.Cfg;
using NUnit.Framework;
using NHibernate.Stat;

namespace NHibernate.Test.NHSpecificTest.NH1643
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		public override string BugNumber
		{
			get { return "NH1643"; }
		}

		[Test]
		public void Test()
		{
			int employeeId;
			using (ISession sess = OpenSession())
			using (ITransaction tx = sess.BeginTransaction())
			{
				Department dept = new Department();
				dept.Id = 11;
				dept.Name = "Animal Testing";

				sess.Save(dept);

				Employee emp = new Employee();
				emp.Id = 1;
				emp.FirstName = "John";
				emp.LastName = "Doe";
                emp.Departments.Add(dept);

				sess.Save(emp);

				tx.Commit();

				employeeId = emp.Id;
			}

            using (ISession sess = OpenSession())
            using (ITransaction tx = sess.BeginTransaction())
            {
                var load = sess.Load<Employee>(employeeId);
                Assert.AreEqual(1, load.Departments.Count);

                tx.Commit();
            }



			using (ISession sess = OpenSession())
			using (ITransaction tx = sess.BeginTransaction())
			{
				sess.Delete("from Employee");
				sess.Delete("from Department");
				tx.Commit();
			}
		}
	}
}
