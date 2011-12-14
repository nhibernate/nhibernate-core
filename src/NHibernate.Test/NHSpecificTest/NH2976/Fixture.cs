using System;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2976
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
	    private readonly Guid _employerId = Guid.NewGuid();
	    private Employee _employee1;
	    private Employee _employee2;

	    protected override void OnSetUp()
		{
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
                var emp = new Employer() { Id = _employerId };
			    _employee1 = new Employee("Carl", emp);
			    _employee2 = new Employee("Philip", emp);
			    emp.AddEmployee(_employee1);
			    emp.AddEmployee(_employee2);
				session.Save(emp);

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
		public void Should_remove_item_from_uninitialized_dictionary()
		{
			using (ISession session = OpenSession())
			using (session.BeginTransaction())
			{
			    var emp = session.Get<Employer>(_employerId);
                Assert.IsFalse(NHibernateUtil.IsInitialized(emp.Employees)); // Just make sure the dictionary really is not initialized

                // First call to PersistentGenericMap.Remove will initialize the dictionary and then enqueue a delayed operation. 
                // The item will not be removed from the dictionary. Enqueued operation will never be executed since AbstractPersistentCollection.PerformQueuedOperations 
                // is executed on AfterInitialize (and dictionary was already initialized before the operation was enqueued
                emp.Employees.Remove(_employee1.Id); 
                emp.Employees.Remove(_employee2.Id); // The item will be removed as normal
                
				Assert.AreEqual(0, emp.Employees.Values.Count);
			}
		}
	}
}