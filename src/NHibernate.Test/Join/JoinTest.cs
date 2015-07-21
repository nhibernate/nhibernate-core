using System.Collections.Generic;
using log4net;
using NHibernate.Criterion;
using NUnit.Framework;
using System;
using System.Collections;
using System.Data;

namespace NHibernate.Test.Join
{
	using NHibernate.Test.Subclass;

	[TestFixture]
	public class JoinTest : TestCase
	{
		private static ILog log = LogManager.GetLogger(typeof(JoinTest));

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get
			{
				return new string[] { 
					"Join.Person.hbm.xml",
					"Subclass.Subclass.hbm.xml"
				};
			}
		}

		protected override void OnTearDown()
		{
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				s.Delete("from Person");

				tx.Commit();
			}
		}

		[Test]
		public void TestSequentialSelects()
		{
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				Employee mark = new Employee();
				mark.Name = "Mark";
				mark.Title = "internal sales";
				mark.Sex = 'M';
				mark.Address = "buckhead";
				mark.Zip = "30305";
				mark.Country = "USA";

				Customer joe = new Customer();
				joe.Name = "Joe";
				joe.Address = "San Francisco";
				joe.Zip = "54353";
				joe.Country = "USA";
				joe.Comments = "very demanding";
				joe.Sex = 'M';
				joe.Salesperson = mark;

				Person yomomma = new Person();
				yomomma.Name = "mom";
				yomomma.Sex = 'F';

				s.Save(yomomma);
				s.Save(mark);
				s.Save(joe);

				s.Flush();
				s.Clear();

				Person p = s.Get<Person>(yomomma.Id);
				Assert.AreEqual(yomomma.Name, p.Name);
				Assert.AreEqual(yomomma.Sex, p.Sex);
				s.Clear();

				// Copied from H3.  Don't really know what it is testing
				//Assert.AreEqual(0, s.CreateQuery("from System.Serializable").List().Count);

				Assert.AreEqual(3, s.CreateQuery("from Person").List().Count);
				Assert.AreEqual(1, s.CreateQuery("from Person p where p.class is null").List().Count);
				Assert.AreEqual(1, s.CreateQuery("from Person p where p.class = Customer").List().Count);
				Assert.AreEqual(1, s.CreateQuery("from Customer c").List().Count);
				s.Clear();

				IList customers = s.CreateQuery("from Customer c left join fetch c.Salesperson").List();
				foreach (Customer c in customers)
				{
					Assert.IsTrue(NHibernateUtil.IsInitialized(c.Salesperson));
					Assert.AreEqual("Mark", c.Salesperson.Name);
				}
				Assert.AreEqual(1, customers.Count);
				s.Clear();

				mark = (Employee) s.Get(typeof (Employee), mark.Id);
				joe = (Customer) s.Get(typeof (Customer), joe.Id);

				mark.Zip = "30306";
				s.Flush();
				s.Clear();
				Assert.AreEqual(1, s.CreateQuery("from Person p where p.Zip = '30306'").List().Count);

				tx.Commit();
			}
		}

		[Test]
		public void TestSequentialSelectsOptionalData()
		{
			// The "optional" attribute on <join/> does not yet work

			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				User jesus = new User();
				jesus.Name = "Jesus Olvera y Martinez";
				jesus.Sex = 'M';

				s.Save(jesus);
				//objectsNeedDeleting.Add(jesus);

				Assert.AreEqual(1, s.CreateQuery("from Person").List().Count);
				Assert.AreEqual(0, s.CreateQuery("from Person p where p.class is null").List().Count);
				Assert.AreEqual(1, s.CreateQuery("from Person p where p.class = User").List().Count);
				Assert.AreEqual(1, s.CreateQuery("from User u").List().Count);
				s.Clear();

				// Remove the optional row from the join table and requery the User obj
				ExecuteStatement(s, tx, "delete from t_user");
				s.Clear();

				// Clean up the test data
				s.Delete(jesus);
				s.Flush();

				Assert.AreEqual(0, s.CreateQuery("from Person").List().Count);

				tx.Commit();
			}
		}

		[Test]
		public void TestOptional()
		{
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				Person p = CreatePerson("A guy");
				p.HomePhone = null;
				p.BusinessPhone = null;
				p.OthersPhones = null;
				s.Save(p);
				s.Flush();
				s.Clear();

				IDbCommand cmd = s.Connection.CreateCommand();
				tx.Enlist(cmd);
				cmd.CommandText = "select count(*) from phone where phone_id = " + p.Id.ToString();
				cmd.CommandType = CommandType.Text;
				Int64 count = Convert.ToInt64(cmd.ExecuteScalar());

				Assert.AreEqual(0, count);
				tx.Commit();
			}
		}

		private Person PreparePersonWithInverseJoin(ISession s, ITransaction tx, string name, string stuffName)
		{
			Person p = CreatePerson(name);

			s.Save(p);
			s.Flush();
			s.Clear();

			if (stuffName != null)
			{
				int count = ExecuteStatement(s, tx,
					string.Format("insert into inversed_stuff (stuff_id, StuffName) values ({0}, '{1}')",
					              p.Id, stuffName));
				Assert.AreEqual(1, count, "Insert statement failed.");
			}

			return p;
		}

		[Test]
		public void TestInverseJoinSelected()
		{
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				string stuffName = "name of the stuff";
				Person p = PreparePersonWithInverseJoin(s, tx, "John", stuffName);

				Person result = (Person) s.Get(typeof (Person), p.Id);
				Assert.IsNotNull(result);
				Assert.AreEqual(stuffName, result.StuffName);

				ExecuteStatement(s, tx, "delete from inversed_stuff");

				tx.Commit();
			}
		}

		[Test]
		public void TestInverseJoinNotUpdated()
		{
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				string stuffName = "name of the stuff";

				Person p = PreparePersonWithInverseJoin(s, tx, "John", stuffName);

				Person personToUpdate = (Person) s.Get(typeof (Person), p.Id);
				Assert.IsNotNull(personToUpdate);

				personToUpdate.StuffName = "new stuff name";
				s.Flush();
				s.Clear();

				Person loaded = (Person) s.Get(typeof (Person), p.Id);
				Assert.AreEqual(stuffName, loaded.StuffName, "StuffName should not have been updated");

				ExecuteStatement(s, tx, "delete from inversed_stuff");

				tx.Commit();
			}
		}

		[Test]
		public void TestInverseJoinNotInserted()
		{
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				Person p = CreatePerson("John");
				p.StuffName = "stuff name in TestInverse_Select";

				s.Save(p);
				s.Flush();
				s.Clear();

				Person result = (Person) s.Get(typeof (Person), p.Id);
				Assert.IsNotNull(result);
				Assert.IsNull(result.StuffName);

				tx.Commit();
			}
		}

		[Test]
		public void TestInverseJoinNotDeleted()
		{
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				string stuffName = "stuff not deleted";
				Person p = PreparePersonWithInverseJoin(s, tx, "John", stuffName);

				long personId = p.Id;
				s.Delete(p);

				IDbCommand cmd = s.Connection.CreateCommand();
				tx.Enlist(cmd);
				cmd.CommandText = string.Format(
					"select count(stuff_id) from inversed_stuff where stuff_id = {0}",
					personId);
				cmd.CommandType = CommandType.Text;
				Int64 count = Convert.ToInt64(cmd.ExecuteScalar());
				Assert.AreEqual(1, count, "Row from an inverse <join> was deleted.");

				IDbCommand cmd2 = s.Connection.CreateCommand();
				tx.Enlist(cmd2);
				cmd2.CommandText = string.Format(
					"select StuffName from inversed_stuff where stuff_id = {0}",
					personId);
				cmd2.CommandType = CommandType.Text;
				string retrievedStuffName = (string) cmd2.ExecuteScalar();
				Assert.AreEqual(stuffName, retrievedStuffName, "Retrieved inverse <join> does not match");

				ExecuteStatement(s, tx, "delete from inversed_stuff");

				tx.Commit();
			}
		}

		private Person CreatePerson(string name)
		{
			Person p = new Person();
			p.Name = name;
			p.Sex = 'M';
			p.Address = "123 Some Street";
			p.Zip = "12345";
			p.Country = "Canada";
			p.HomePhone = "555-1234";
			p.BusinessPhone = "555-4321";
			p.OthersPhones = new HashSet<string> {"555-9876", "555-6789"};
			return p;
		}

		protected bool PersonsAreEqual(Person x, Person y)
		{
			if (!string.Equals(x.Name, y.Name)) return false;
			if (x.Sex != y.Sex) return false;
			if (!string.Equals(x.Address, y.Address)) return false;
			if (!string.Equals(x.Zip, y.Zip)) return false;
			if (!string.Equals(x.Country, y.Country)) return false;
			if (!string.Equals(x.HomePhone, y.HomePhone)) return false;
			if (!string.Equals(x.BusinessPhone, y.BusinessPhone)) return false;
			if(x.OthersPhones.Count != y.OthersPhones.Count)
			{
				return false;
			}
			return true;
		}

		private Person[] CreateAndInsertPersons(ISession s, int count)
		{
			Person[] result = new Person[count];

			for (int i = 0; i < count; i++)
			{
				result[i] = CreatePerson("Person " + i.ToString());
				s.Save(result[i]);
			}

			s.Flush();
			s.Clear();

			return result;
		}

		[Test]
		public void TestRetrieveUsingGet()
		{
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				// Create a new person John
				Person john = CreatePerson("John");

				s.Save(john);
				s.Flush();
				s.Clear();

				Person p = (Person) s.Get(typeof (Person), john.Id);
				Assert.IsTrue(PersonsAreEqual(john, p));

				tx.Commit();
			}
		}

		[Test]
		public void TestRetrieveUsingCriteriaInterface()
		{
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				Person[] people = CreateAndInsertPersons(s, 3);

				ICriteria criteria = s.CreateCriteria(typeof (Person))
					.Add(Expression.Eq("Name", people[1].Name));
				IList list = criteria.List();

				Assert.AreEqual(1, list.Count);
				Assert.IsTrue(PersonsAreEqual(people[1], (Person) list[0]));

				tx.Commit();
			}
		}

		[Test]
		public void TestRetrieveUsingHql()
		{
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				Person[] people = CreateAndInsertPersons(s, 3);

				IQuery query = s.CreateQuery("from Person p where p.Name = :name")
					.SetParameter("name", people[1].Name);
				IList list = query.List();

				Assert.AreEqual(1, list.Count);
				Assert.IsTrue(PersonsAreEqual(people[1], (Person) list[0]));

				tx.Commit();
			}
		}

		private Employee CreateEmployee(string name, string title)
		{
			Employee p = new Employee();
			p.Name = name;
			p.Sex = 'M';
			p.Address = "123 Some Street";
			p.Zip = "12345";
			p.Country = "Canada";
			p.HomePhone = "555-1234";
			p.BusinessPhone = "555-4321";

			p.Title = title;
			p.Salary = 100;
			p.Meetings.Add(new Meeting {Employee = p, Description = "salary definition"});
			p.Meetings.Add(new Meeting { Employee = p, Description = "targets definition" });
			return p;
		}

		private Employee[] CreateAndInsertEmployees(ISession s, int count)
		{
			Employee[] result = new Employee[count];

			for (int i = 0; i < count; i++)
			{
				result[i] = CreateEmployee("Employee " + i.ToString(), "Title " + i.ToString());
				s.Save(result[i]);
			}

			s.Flush();
			s.Clear();

			return result;
		}


		[Test]
		public void TestSimpleInsertAndRetrieveEmployee()
		{
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				// Create a new employee Jack
				Employee jack = CreateEmployee("Jack", "Boss");

				s.Save(jack);
				s.Flush();
				s.Clear();

				IList list = s.CreateQuery("from Employee p where p.Id = :id")
					.SetParameter("id", jack.Id)
					.List();
				Assert.AreEqual(1, list.Count);
				Assert.IsTrue(list[0] is Employee);
				Assert.IsTrue(EmployeesAreEqual(jack, (Employee) list[0]));

				tx.Commit();
			}
		}

		[Test]
		public void TestDeleteUsingHql()
		{
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				Person[] people = new Person[3];
				for (int i = 0; i < people.Length; i++)
				{
					people[i] = CreatePerson(string.Format("Person {0}", i + 1));
					s.Save(people[i]);
				}

				s.Flush();
				s.Clear();

				s.Delete("from Person");
				s.Flush();

				IList list = s.CreateQuery("from Person").List();
				Assert.AreEqual(0, list.Count);

				tx.Commit();
			}
		}

		private bool EmployeesAreEqual(Employee x, Employee y)
		{
			if (!PersonsAreEqual(x, y)) return false;
			if (!string.Equals(x.Title, y.Title)) return false;
			if (x.Salary != y.Salary) return false;
			if (x.Meetings.Count != y.Meetings.Count) return false;
			if (x.Manager != null && y.Manager != null)
			{
				return x.Manager.Id == y.Manager.Id;
			}
			else if (x.Manager != null || y.Manager != null)
			{
				return false;
			}
			else // x.Manager and y.Manager are both null
			{
				return true;
			}
		}

		[Test]
		public void TestUpdateEmployee()
		{
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				Employee[] employees = CreateAndInsertEmployees(s, 3);

				Employee emp0 = (Employee) s.Get(typeof (Employee), employees[0].Id);
				Assert.IsNotNull(emp0);
				emp0.Address = "Address";
				emp0.BusinessPhone = "BusinessPhone";
				emp0.Country = "Country";
				emp0.HomePhone = "HomePhone";
				emp0.Manager = employees[2];
				emp0.Name = "Name";
				emp0.Salary = 20000;
				emp0.Title = "Title";
				emp0.Zip = "Zip";
				NHibernateUtil.Initialize(emp0.Meetings);
				NHibernateUtil.Initialize(emp0.OthersPhones);
				emp0.Meetings.Add(new Meeting { Employee = emp0, Description = "vacation def" });
				// Not updating emp0.Sex because it is marked update=false in the mapping file.

				s.Flush();
				s.Clear();

				Employee emp0updated = (Employee) s.Get(typeof (Employee), employees[0].Id);
				Assert.IsTrue(EmployeesAreEqual(emp0, emp0updated));

				tx.Commit();
			}
		}

		[Test]
		public void Learn_SubclassBehavior()
		{
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				SubclassOne one = new SubclassOne();
				one.TestDateTime = DateTime.Now;

				s.Save(one);
				s.Flush();
				s.Clear();

				SubclassOne result = (SubclassOne) s.Get(typeof (SubclassBase), one.Id);
				Assert.IsNotNull(result);
				Assert.IsTrue(result is SubclassOne);

				s.Delete(result);

				tx.Commit();
			}
		}

		[Test]
		public void PolymorphicGetByTypeofSuperclass()
		{
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				Employee[] employees = CreateAndInsertEmployees(s, 1);
				Employee emp0 = (Employee) s.Get(typeof (Person), employees[0].Id);
				Assert.IsNotNull(emp0);
				Assert.IsTrue(emp0 is Employee);

				tx.Commit();
			}
		}
	}
}