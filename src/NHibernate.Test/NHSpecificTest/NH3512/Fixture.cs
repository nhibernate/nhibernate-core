using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3512
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				var employee = new Employee {Id = 1, Name = "Bob", Age = 33, Salary = 100};
				session.Save(employee);

				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				session.Delete("from System.Object");

				transaction.Commit();
			}
		}

		[Test]
		public void ShouldChangeVersionWhenBasePropertyChanged()
		{
			using (ISession session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var person = session.Get<Person>(1);

				var before = person.Version;

				person.Age++;

				transaction.Commit();

				CollectionAssert.AreNotEqual(before, person.Version);
			}
		}

		[Test]
		public void ShouldChangeVersionWhenDerivedPropertyChanged()
		{
			using (ISession session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var employee = session.Get<Employee>(1);

				var before = employee.Version;

				employee.Salary += 10;

				transaction.Commit();

				CollectionAssert.AreNotEqual(before, employee.Version);
			}
		}
	}
}