using System;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH467
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		public override string BugNumber
		{
			get { return "NH467"; }
		}

		[Test]
		public void WhereClauseInManyToOneNavigation()
		{
			User inactive = new User();
			inactive.Id = 10;
			inactive.Name = "inactive";
			inactive.IsActive = 0;

			Employee employee = new Employee();
			employee.Id = 20;
			employee.User = inactive;

			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.Save(inactive);
				s.Save(employee);
				t.Commit();
			}

			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				Employee loaded = (Employee) s.Get(typeof(Employee), employee.Id);
				Assert.IsNotNull(loaded.User);

				try
				{
					NHibernateUtil.Initialize(loaded.User);
					Assert.Fail("Should not have initialized");
				}
				catch (ObjectNotFoundException)
				{
					// Correct
				}

				s.Delete("from Employee");
				s.Delete("from User");
				t.Commit();
			}
		}
	}
}