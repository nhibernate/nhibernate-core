using NHibernate.Cfg;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3512
{
	public class Fixture : BugTestCase
	{
		private int _id;

		protected override void OnSetUp()
		{
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				var employee = new Employee {Name = "Bob", Age = 33, Salary = 100};
				session.Save(employee);

				transaction.Commit();

				_id = employee.Id;
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

		protected void UpdateBaseEntity()
		{
			using (ISession session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var person = session.Get<Person>(_id);

				var before = person.Version;

				person.Age++;

				transaction.Commit();

				Assert.That(person.Version, Is.GreaterThan(before));
			}
		}

		protected void UpdateDerivedEntity()
		{
			using (ISession session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var employee = session.Get<Employee>(_id);

				var before = employee.Version;

				employee.Salary += 10;

				transaction.Commit();

				Assert.That(employee.Version, Is.GreaterThan(before));
			}
		}
	}

	[TestFixture]
	public class DynamicUpdateOn : Fixture
	{
		protected override void Configure(Configuration configuration)
		{
			foreach (var mapping in configuration.ClassMappings)
			{
				mapping.DynamicUpdate = true;
			}
		}

		[Test]
		public void ShouldChangeVersionWhenBasePropertyChanged()
		{
			UpdateBaseEntity();
		}

		[Test]
		public void ShouldChangeVersionWhenDerivedPropertyChanged()
		{
			UpdateDerivedEntity();
		}
	}

	[TestFixture]
	public class DynamicUpdateOff : Fixture
	{
		[Test]
		public void ShouldChangeVersionWhenBasePropertyChanged()
		{
			UpdateBaseEntity();
		}

		[Test]
		public void ShouldChangeVersionWhenDerivedPropertyChanged()
		{
			UpdateDerivedEntity();
		}
	}
}