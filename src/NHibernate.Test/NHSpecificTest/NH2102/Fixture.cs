using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2102
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		[Test]
		public void EntityWithConstrainedLazyLoadedOneToOneShouldNotGenerateFieldInterceptingProxy()
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

					Assert.That(employee, Is.TypeOf<Employee>());
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
