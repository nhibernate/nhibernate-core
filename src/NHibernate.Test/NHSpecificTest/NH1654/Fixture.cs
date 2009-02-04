using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1654
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		public override string BugNumber
		{
			get { return "NH1654"; }
		}

		[Test,Ignore]
		public void Test()
		{
			int employeeId;
			using (ISession sess = OpenSession())
			using (ITransaction tx = sess.BeginTransaction())
			{
				var emp = new Employee();
				emp.Id = 1;
				emp.FirstName = "John";

				sess.Save(emp);

				tx.Commit();

				employeeId = emp.Id;
			}

			using (ISession sess = OpenSession())
			using (ITransaction tx = sess.BeginTransaction())
			{
				var load = sess.Load<Employee>(employeeId);
				Assert.AreEqual("John", load.FirstNameFormula);

				tx.Commit();
			}

			using (ISession sess = OpenSession())
			using (ITransaction tx = sess.BeginTransaction())
			{
				sess.Delete("from Employee");
				tx.Commit();
			}
		}
	}
}