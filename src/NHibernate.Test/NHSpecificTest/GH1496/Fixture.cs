using System;
using System.Linq;
using NUnit.Framework;
using NHibernate.Cfg;
using NHibernate.Event;

namespace NHibernate.Test.NHSpecificTest.GH1496
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		private readonly AuditEventListener _auditEventListener = new AuditEventListener();
		private const string WORK_TYPENAME = "WORK";
		private const string HOME_TYPENAME = "HOME";
		private Person testPerson;
		private Employee testEmployee;
		
		protected override void Configure(Configuration configuration)
		{
			base.Configure(configuration);
			configuration.AppendListeners(ListenerType.PostUpdate, new[] { _auditEventListener });
		}

		protected override void OnSetUp()
		{
			SetupPerson();
			SetupEmployee();
		}

		private void SetupPerson()
		{
			using (var session = OpenSession())
			{
				using (var transaction = session.BeginTransaction())
				{
					var address = new Address("Postal A", "State A", "Street A");
					var person = new Person(1, "Alex", address);
					session.Save(address);
					person.Address = address;
					session.Save(person);
					testPerson = person;


					address = new Address("Postal T", "State T", "Street T");
					person = new Person(2, "Tom", address);
					session.Save(address);
					person.Address = address;
					session.Save(person);

					session.Flush();
					transaction.Commit();
				}
			}
		}

		private void SetupEmployee()
		{
			using (var session = OpenSession())
			{
				using (var transaction = session.BeginTransaction())
				{
					var employee = new Employee(1, "Alex");
					var contact = new Contact { Phone = "111-111-1111", ContactIdentifier = new ContactIdentifier(HOME_TYPENAME, "1") };
					session.Save(contact);
					employee.Contact = contact;
					session.Save(employee);
					testEmployee = employee;


					employee = new Employee(2, "Tom");
					contact = new Contact { Phone = "666-666-6666", ContactIdentifier = new ContactIdentifier(WORK_TYPENAME, "2") };
					session.Save(contact);
					employee.Contact = contact;
					session.Save(employee);

					session.Flush();
					transaction.Commit();
				}
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.Delete("from System.Object");
				session.Flush();
				transaction.Commit();
			}
		}

		[Test]
		public void EventListener_Entity_NoChange()
		{

			using (var session = OpenSession())
			{
				using (var transaction = session.BeginTransaction())
				{
					_auditEventListener.Start();
					var person = session.Get<Person>(2);
					session.Update(person);
					session.Flush();
					transaction.Commit();
					Assert.That(_auditEventListener.ModifiedItems.Count, Is.EqualTo(0), "Total 0 item changed");
					_auditEventListener.Stop();
				}
			}
		}

		[Test]
		public void SelectBeforeUpdate_Entity_NoChange()
		{
			_auditEventListener.Stop();
			using (var session = OpenSession())
			{
				using (var transaction = session.BeginTransaction())
				{
					session.Update(testPerson);
					Assert.DoesNotThrow(() =>
					{
						session.Flush();
						transaction.Commit();
					},  "Saving data failed.");
				}
			}
		}

		[Test]
		public void EventListener_Entity_ChangeProperty()
		{
			using (var session = OpenSession())
			{
				using (var transaction = session.BeginTransaction())
				{
					_auditEventListener.Start();
					var person = session.Get<Person>(2);
					person.Name = "Steve";
					session.Update(person);
					session.Flush();
					transaction.Commit();
					Assert.That(_auditEventListener.ModifiedItems.Count, Is.EqualTo(1), "Total 1 item changed");
					Assert.That(_auditEventListener.ModifiedItems.Count(x => x.State.ToString() == "Steve"), Is.EqualTo(1), "Includes Modified Item 'Steve'");
					_auditEventListener.Stop();
				}
			}
		}

		[Test]
		public void SelectBeforeUpdate_Entity_ChangeProperty()
		{
			_auditEventListener.Stop();
			using (var session = OpenSession())
			{
				using (var transaction = session.BeginTransaction())
				{
					testPerson.Name = "Mike";
					session.Update(testPerson);
					Assert.DoesNotThrow(() =>
					{
						session.Flush();
						transaction.Commit();
					}, "Saving data failed.");
				}
			}
		}



		[Test]
		public void EventListener_EntityWithCompositeId_ChangeProperty()
		{
			using (var session = OpenSession())
			{
				using (var transaction = session.BeginTransaction())
				{
					_auditEventListener.Start();
					var employee = session.Get<Employee>(2);
					employee.Name = "Steve";
					session.Update(employee);
					session.Flush();
					transaction.Commit();
					Assert.That(_auditEventListener.ModifiedItems.Count, Is.EqualTo(1), "Total 1 item changed");
					Assert.That(_auditEventListener.ModifiedItems.Count(x => x.State.ToString() == "Steve"), Is.EqualTo(1), "Includes Modified Item 'Steve'");
					_auditEventListener.Stop();
				}
			}
		}

		[Test]
		public void SelectBeforeUpdate_EntityWithCompositeId_ChangeProperty()
		{
			_auditEventListener.Stop();
			using (var session = OpenSession())
			{
				using (var transaction = session.BeginTransaction())
				{
					testEmployee.Name = "Mike";
					session.Update(testEmployee);
					Assert.DoesNotThrow(() =>
					{
						session.Flush();
						transaction.Commit();
					}, "Saving data failed.");
				}
			}
		}


		[Test]
		public void EventListener_ManyToOne_ChangeProperty()
		{

			using (var session = OpenSession())
			{
				using (var transaction = session.BeginTransaction())
				{
					_auditEventListener.Start();
					var person = session.Get<Person>(2);
					person.Address.Street = "Street B";
					session.Update(person);
					session.Flush();
					transaction.Commit();
					Assert.That(_auditEventListener.ModifiedItems.Count, Is.EqualTo(1), "Total 1 item changed");
					Assert.That(_auditEventListener.ModifiedItems.Count(x => x.State.ToString() == "Street B"), Is.EqualTo(1), "Includes Modified Item 'Street B'");
					_auditEventListener.Stop();
				}
			}
		}

		[Test]
		public void SelectBeforeUpdate_ManyToOne_ChangeProperty()
		{
			_auditEventListener.Stop();
			using (var session = OpenSession())
			{
				using (var transaction = session.BeginTransaction())
				{
					testPerson.Address.Street = "Street E";
					session.Update(testPerson);
					Assert.DoesNotThrow(() =>
					{
						session.Flush();
						transaction.Commit();
					}, "Saving data failed.");
				}
			}
		}

		[Test]
		public void EventListener_Entity_SetNewManyToOne()
		{

			using (var session = OpenSession())
			{
				using (var transaction = session.BeginTransaction())
				{
					_auditEventListener.Start();
					var person = session.Get<Person>(2);
					var address = new Address("Postal B", "State B", "Street B");
					session.Save(address);
					person.Address = address;
					session.Update(person);
					session.Flush();
					transaction.Commit();
					Assert.That(_auditEventListener.ModifiedItems.Count, Is.EqualTo(1), "Total 1 item changed");
					Assert.That(_auditEventListener.ModifiedItems.Count(x => x.State.GetType().Name.Contains("Address")), Is.EqualTo(1), "Includes Modified Item type of Address'");
					_auditEventListener.Stop();
				}
			}
		}

		[Test]
		public void SelectBeforeUpdate_Entity_SetNewManyToOne()
		{
			_auditEventListener.Stop();
			using (var session = OpenSession())
			{
				using (var transaction = session.BeginTransaction())
				{
					var address = new Address("Postal D", "State D", "Street D");
					session.Save(address);
					testPerson.Address = address;
					session.Update(testPerson);
					Assert.DoesNotThrow(() =>
					{
						session.Flush();
						transaction.Commit();
					}, "Saving data failed.");
				}
			}
		}

		[Test]
		public void EventListener_ManyToOneWithCompositeId_ChangeProperty()
		{

			using (var session = OpenSession())
			{
				using (var transaction = session.BeginTransaction())
				{
					_auditEventListener.Start();
					var employee = session.Get<Employee>(2);
					employee.Contact.Phone = "333-333-3333";
					session.Update(employee);
					session.Flush();
					transaction.Commit();
					Assert.That(_auditEventListener.ModifiedItems.Count, Is.EqualTo(1), "Total 1 item changed");
					Assert.That(_auditEventListener.ModifiedItems.Count(x => x.State.ToString() == "333-333-3333"), Is.EqualTo(1), "Includes Modified Item '333-333-3333'");
					_auditEventListener.Stop();
				}
			}
		}

		[Test]
		public void SelectBeforeUpdate_ManyToOneWithCompositeId_ChangeProperty()
		{
			_auditEventListener.Stop();
			using (var session = OpenSession())
			{
				using (var transaction = session.BeginTransaction())
				{
					testEmployee.Contact.Phone = "666-666-6666";
					session.Update(testEmployee);
					Assert.DoesNotThrow(() =>
					{
						session.Flush();
						transaction.Commit();
					}, "Saving data failed.");
				}
			}
		}

		[Test]
		public void EventListener_Entity_SetNewManyToOneWithCompositeId()
		{

			using (var session = OpenSession())
			{
				using (var transaction = session.BeginTransaction())
				{
					_auditEventListener.Start();
					var employee = session.Get<Employee>(2);
					var contact = employee.Contact;
					employee.Contact = null;
					session.Delete(contact);

					session.Update(employee);
					session.Flush();
					transaction.Commit();
					Assert.That(_auditEventListener.ModifiedItems.Count, Is.EqualTo(1), "Total 1 item changed");
					Assert.That(_auditEventListener.ModifiedItems.Count(x => x.OldState.GetType().Name.Contains("Contact")), Is.EqualTo(1), "Includes Modified Item type of Contact'");
					_auditEventListener.Stop();
				}
			}


			using (var session = OpenSession())
			{
				using (var transaction = session.BeginTransaction())
				{
					_auditEventListener.Start();
					var employee = session.Get<Employee>(2);
					var contact = new Contact { Phone = "444-444-4444", ContactIdentifier = new ContactIdentifier(HOME_TYPENAME, "2") };
					session.Save(contact);
					employee.Contact = contact;
					session.Update(employee);
					session.Flush();
					transaction.Commit();
					Assert.That(_auditEventListener.ModifiedItems.Count, Is.EqualTo(1), "Total 1 item changed");
					Assert.That(_auditEventListener.ModifiedItems.Count(x => x.State.GetType().Name.Contains("Contact")), Is.EqualTo(1), "Includes Modified Item type of Contact'");
					_auditEventListener.Stop();
				}
			}
		}

		[Test]
		public void SelectBeforeUpdate_Entity_SetNewManyToOneWithCompositeId()
		{
			_auditEventListener.Stop();
			using (var session = OpenSession())
			{
				using (var transaction = session.BeginTransaction())
				{
					var contact = testEmployee.Contact;
					testEmployee.Contact = null;
					session.Delete(contact);
					session.Update(testEmployee);

					Assert.DoesNotThrow(() =>
					{
						session.Flush();
						transaction.Commit();
					}, "Saving data failed.");
				}
			}


			using (var session = OpenSession())
			{
				using (var transaction = session.BeginTransaction())
				{
					var contact = new Contact { Phone = "555-555-5555", ContactIdentifier = new ContactIdentifier(HOME_TYPENAME, "1") };
					session.Save(contact);
					testEmployee.Contact = contact;
					session.Update(testEmployee);
					Assert.DoesNotThrow(() =>
					{
						session.Flush();
						transaction.Commit();
					}, "Saving data failed.");
				}
			}
		}
	}
}
