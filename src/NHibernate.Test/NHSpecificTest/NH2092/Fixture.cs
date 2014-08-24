using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2092
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		[Test]
		public void ConstrainedLazyLoadedOneToOneUsingCastleProxy()
		{
			try
			{
				using (var s = OpenSession())
				{
					var person = new Person { Id = 1, Name = "Person1" };
					var employee = new Employee { Id = 1, Name = "Emp1", Person = person };
					
					s.Save(person);
					s.Save(employee);

					s.Flush();
				}

				using (var s = OpenSession())
				{
					var employee = s.Get<Employee>(1);

					Assert.False(NHibernateUtil.IsInitialized(employee.Person));

					Assert.AreEqual(employee.Person.Name, "Person1");

					Assert.True(NHibernateUtil.IsInitialized(employee.Person));
				}
			}
			finally
			{
				using (var s = OpenSession())
				{
					s.Delete("from Employee");
					s.Delete("from Person");

					s.Flush();
				}
			}
		}
	}
}
