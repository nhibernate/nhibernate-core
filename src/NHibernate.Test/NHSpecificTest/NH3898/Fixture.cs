using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3898
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void Configure(Cfg.Configuration configuration)
		{
			configuration.SetProperty(Cfg.Environment.UseQueryCache, "false");
			configuration.SetProperty(Cfg.Environment.UseSecondLevelCache, "false");
		}

		protected override void OnTearDown()
		{
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				s.CreateQuery("delete from Employee").ExecuteUpdate();

				tx.Commit();
			}
		}

		[Test]
		public void GeneratedInsertUpdateTrue()
		{
			object id;
			using (var session = OpenSession())
			using (var tx = session.BeginTransaction())
			{
				var employee = new Employee
				{
					Name = "Employee 1",
					PromotionCount = 9999999
				};
				id = session.Save(employee);
				tx.Commit();
			}

			using (var session = OpenSession())
			using (var tx = session.BeginTransaction())
			{
				var employee = session.Get<Employee>(id);
				Assert.That(employee.PromotionCount, Is.EqualTo(0));
				employee.Name = "Employee 1 changed";
				employee.PromotionCount++;
				tx.Commit();
			}

			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var employee = session.Get<Employee>(id);
				Assert.That(employee.Name, Is.EqualTo("Employee 1 changed"));
				Assert.That(employee.PromotionCount, Is.EqualTo(1));
			}
		}
	}
}
