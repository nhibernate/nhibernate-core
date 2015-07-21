using System;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2976
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		private readonly Guid _employerId = Guid.NewGuid();
		private Guid _employeeId1;
		private Guid _employeeId2;

		protected override void OnSetUp()
		{
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				var employer = new Employer { Id = _employerId };
				var employee1 = new Employee("Carl", employer);
				var employee2 = new Employee("Philip", employer);

				employer.AddEmployee1(employee1);
				employer.AddEmployee1(employee2);

				_employeeId1 = employee1.Id;
				_employeeId2 = employee2.Id;
				
				session.Save(employer);
				session.Flush();
				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				session.Delete("from System.Object");
				session.Flush();
				transaction.Commit();
			}
		}

		[Test]
		public void ShouldRemoveItemFromUninitializedGenericDictionary()
		{
			using (ISession session = OpenSession())
			using (session.BeginTransaction())
			{
				var employer = session.Get<Employer>(_employerId);

				Assert.IsFalse(NHibernateUtil.IsInitialized(employer.Employees1)); // Just make sure the dictionary really is not initialized

				// First call to PersistentGenericMap.Remove will initialize the dictionary and then enqueue a delayed operation.
				// The item will not be removed from the dictionary. Enqueued operation will never be executed since
				// AbstractPersistentCollection.PerformQueuedOperations is executed on AfterInitialize - and dictionary was already
				// initialized before the operation was enqueued
				employer.Employees1.Remove(_employeeId1);
				employer.Employees1.Remove(_employeeId2); // The item will be removed as normal

				Assert.That(employer.Employees1.Values.Count, Is.EqualTo(0));
			}
		}
	}
}