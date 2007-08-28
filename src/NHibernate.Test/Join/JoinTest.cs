using log4net;
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

		ISession s;
		ITransaction t;

		protected override void OnSetUp()
		{
			s = OpenSession();
			//t = s.BeginTransaction();

			objectsNeedDeleting.Clear();
		}

		protected override void OnTearDown()
		{
			s.Flush();
			s.Clear();
			try
			{
				foreach (object obj in objectsNeedDeleting)
				{
					s.Delete(obj);
				}
				s.Flush();
			}
			finally
			{
				//t.Commit();
				s.Close();
			}

			t = null;
			s = null;
		}

		private IList objectsNeedDeleting = new ArrayList();

		[Test]
		public void TestSequentialSelects()
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

			objectsNeedDeleting.Add(yomomma);
			objectsNeedDeleting.Add(mark);
			objectsNeedDeleting.Add(joe);

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

			mark = (Employee)s.Get(typeof(Employee), mark.Id);
			joe = (Customer)s.Get(typeof(Customer), joe.Id);

			mark.Zip = "30306";
			s.Flush();
			s.Clear();
			Assert.AreEqual(1, s.CreateQuery("from Person p where p.Zip = '30306'").List().Count);

			// Clean up done in TearDown
		}

		[Test]
		public void TestSequentialSelectsOptionalData()
		{
			// The "optional" attribute on <join/> does not yet work

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
			IDbCommand cmd = s.Connection.CreateCommand();
			cmd.CommandText = "delete from t_user";
			cmd.CommandType = CommandType.Text;
			cmd.ExecuteNonQuery();
			s.Clear();

			// Clean up the test data
			s.Delete(jesus);
			s.Flush();

			Assert.AreEqual(0, s.CreateQuery("from Person").List().Count);
		}

		[Test]
		public void TestOptional()
		{
			Person p = CreatePerson("A guy");
			p.HomePhone = null;
			p.BusinessPhone = null;

			s.Save(p);
			objectsNeedDeleting.Add(p);
			s.Flush();
			s.Clear();

			IDbCommand cmd = s.Connection.CreateCommand();
			cmd.CommandText = "select count(*) from phone where phone_id = " + p.Id.ToString();
			cmd.CommandType = CommandType.Text;
			Int64 count = Convert.ToInt64(cmd.ExecuteScalar());

			Assert.AreEqual(0, count);
		}

		private Person PreparePersonWithInverseJoin(string name, string stuffName)
		{
			Person p = CreatePerson(name);

			s.Save(p);
			objectsNeedDeleting.Add(p);
			s.Flush();
			s.Clear();

			if (stuffName != null)
			{
				IDbCommand cmd = s.Connection.CreateCommand();
				cmd.CommandText =
					string.Format("insert into inversed_stuff (stuff_id, StuffName) values ({0}, '{1}')",
						p.Id, stuffName);
				cmd.CommandType = CommandType.Text;
				int count = cmd.ExecuteNonQuery();
				Assert.AreEqual(1, count, "Insert statement failed.");
			}

			return p;
		}

		[Test]
		public void TestInverseJoinSelected()
		{
			string stuffName = "name of the stuff";
			Person p = PreparePersonWithInverseJoin("John", stuffName);

			Person result = (Person)s.Get(typeof(Person), p.Id);
			Assert.IsNotNull(result);
			Assert.AreEqual(stuffName, result.StuffName);
		}

		[Test]
		public void TestInverseJoinNotUpdated()
		{
			string stuffName = "name of the stuff";
			Person p = PreparePersonWithInverseJoin("John", stuffName);

			Person personToUpdate = (Person)s.Get(typeof(Person), p.Id);
			Assert.IsNotNull(personToUpdate);

			personToUpdate.StuffName = "new stuff name";
			s.Flush();
			s.Clear();

			Person loaded = (Person)s.Get(typeof(Person), p.Id);
			Assert.AreEqual(stuffName, loaded.StuffName, "StuffName should not have been updated");
		}

		[Test]
		public void TestInverseJoinNotInserted()
		{
			Person p = CreatePerson("John");
			p.StuffName = "stuff name in TestInverse_Select";

			s.Save(p);
			objectsNeedDeleting.Add(p);
			s.Flush();
			s.Clear();

			Person result = (Person)s.Get(typeof(Person), p.Id);
			Assert.IsNotNull(result);
			Assert.IsNull(result.StuffName);
		}

		[Test]
		public void TestInverseJoinNotDeleted()
		{
			string stuffName = "stuff not deleted";
			Person p = PreparePersonWithInverseJoin("John", stuffName);
			objectsNeedDeleting.Remove(p);

			long personId = p.Id;
			s.Delete(p);

			IDbCommand cmd = s.Connection.CreateCommand();
			cmd.CommandText = string.Format(
				"select count(stuff_id) from inversed_stuff where stuff_id = {0}",
				personId);
			cmd.CommandType = CommandType.Text;
			Int64 count = Convert.ToInt64(cmd.ExecuteScalar());
			Assert.AreEqual(1, count, "Row from an inverse <join> was deleted.");

			IDbCommand cmd2 = s.Connection.CreateCommand();
			cmd2.CommandText = string.Format(
				"select StuffName from inversed_stuff where stuff_id = {0}",
				personId);
			cmd2.CommandType = CommandType.Text;
			string retrievedStuffName = (string)cmd2.ExecuteScalar();
			Assert.AreEqual(stuffName, retrievedStuffName, "Retrieved inverse <join> does not match");
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

			return true;
		}

		private Person[] CreateAndInsertPersons(int count)
		{
			Person[] result = new Person[count];

			for (int i = 0; i < count; i++)
			{
				result[i] = CreatePerson("Person " + i.ToString());
				s.Save(result[i]);
				objectsNeedDeleting.Add(result[i]);
			}

			s.Flush();
			s.Clear();

			return result;
		}

		[Test]
		public void TestRetrieveUsingGet()
		{
			// Create a new person John
			Person john = CreatePerson("John");

			s.Save(john);
			objectsNeedDeleting.Add(john);
			s.Flush();
			s.Clear();

			Person p = (Person)s.Get(typeof(Person), john.Id);
			Assert.IsTrue(PersonsAreEqual(john, p));
		}

		[Test]
		public void TestRetrieveUsingCriteriaInterface()
		{
			Person[] people = CreateAndInsertPersons(3);

			ICriteria criteria = s.CreateCriteria(typeof(Person))
				.Add(Expression.Expression.Eq("Name", people[1].Name));
			IList list = criteria.List();

			Assert.AreEqual(1, list.Count);
			Assert.IsTrue(PersonsAreEqual(people[1], (Person)list[0]));
		}

		[Test]
		public void TestRetrieveUsingHql()
		{
			Person[] people = CreateAndInsertPersons(3);

			IQuery query = s.CreateQuery("from Person p where p.Name = :name")
				.SetParameter("name", people[1].Name);
			IList list = query.List();

			Assert.AreEqual(1, list.Count);
			Assert.IsTrue(PersonsAreEqual(people[1], (Person)list[0]));
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

			return p;
		}

		private Employee[] CreateAndInsertEmployees(int count)
		{
			Employee[] result = new Employee[count];

			for (int i = 0; i < count; i++)
			{
				result[i] = CreateEmployee("Employee " + i.ToString(), "Title " + i.ToString());
				s.Save(result[i]);
				objectsNeedDeleting.Add(result[i]);
			}

			s.Flush();
			s.Clear();

			return result;
		}


		[Test]
		public void TestSimpleInsertAndRetrieveEmployee()
		{
			// Create a new employee Jack
			Employee jack = CreateEmployee("Jack", "Boss");

			s.Save(jack);
			objectsNeedDeleting.Add(jack);
			s.Flush();
			s.Clear();

			IList list = s.CreateQuery("from Employee p where p.Id = :id")
				.SetParameter("id", jack.Id)
				.List();
			Assert.AreEqual(1, list.Count);
			Assert.IsTrue(list[0] is Employee);
			Assert.IsTrue(EmployeesAreEqual(jack, (Employee)list[0]));
		}

		[Test]
		public void TestDeleteUsingHql()
		{
			Person[] people = new Person[3];
			for (int i = 0; i < people.Length; i++)
			{
				people[i] = CreatePerson(string.Format("Person {0}", i + 1));
				s.Save(people[i]);
				objectsNeedDeleting.Add(people[i]);
			}

			s.Flush();
			s.Clear();

			s.Delete("from Person");
			s.Flush();

			IList list = s.CreateQuery("from Person").List();
			objectsNeedDeleting = list;
			Assert.AreEqual(0, list.Count);
		}

		private bool EmployeesAreEqual(Employee x, Employee y)
		{
			if (!PersonsAreEqual(x, y)) return false;
			if (!string.Equals(x.Title, y.Title)) return false;
			if (x.Salary != y.Salary) return false;

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
			Employee[] employees = CreateAndInsertEmployees(3);

			Employee emp0 = (Employee)s.Get(typeof(Employee), employees[0].Id);
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
			// Not updating emp0.Sex because it is marked update=false in the mapping file.

			s.Flush();
			s.Clear();

			Employee emp0updated = (Employee)s.Get(typeof(Employee), employees[0].Id);
			Assert.IsTrue(EmployeesAreEqual(emp0, emp0updated));
		}

		[Test]
		public void Learn_SubclassBehavior()
		{
			SubclassOne one = new SubclassOne();
			one.TestDateTime = DateTime.Now;

			s.Save(one);
			s.Flush();
			s.Clear();

			SubclassOne result = (SubclassOne)s.Get(typeof(SubclassBase), one.Id);
			Assert.IsNotNull(result);
			Assert.IsTrue(result is SubclassOne);

			s.Delete(result);
		}

		[Test]
		public void PolymorphicGetByTypeofSuperclass()
		{
			Employee[] employees = CreateAndInsertEmployees(1);
			Employee emp0 = (Employee)s.Get(typeof(Person), employees[0].Id);
			Assert.IsNotNull(emp0);
			Assert.IsTrue(emp0 is Employee);
		}
	}
}