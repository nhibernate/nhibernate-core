using System;
using System.Collections;

using NUnit.Framework;

namespace NHibernate.Test.JoinedSubclass
{
	/// <summary>
	/// Test the use of <c>&lt;class&gt;</c> and <c>&lt;joined-subclass&gt;</c> mappings.
	/// </summary>
	[TestFixture]
	public class JoinedSubclassFixture : TestCase 
	{

		private DateTime testDateTime = new DateTime(2003, 8, 16);
		private DateTime updateDateTime = new DateTime(2003, 8, 17);

		[SetUp]
		public virtual void SetUp() 
		{
			ExportSchema( new string[] { "JoinedSubclass.JoinedSubclass.hbm.xml" }, true, "NHibernate.Test" );
		}

		[Test]
		public void TestJoinedSubclass() 
		{
			ISession s = sessions.OpenSession();
			
			Employee mark = new Employee();
			mark.Name = "Mark";
			mark.Title = "internal sales";
			mark.Sex = 'M';
			mark.Address.Street = "buckhead";
			mark.Address.Zip = "30305";
			mark.Address.Country = "USA";

			Customer joe = new Customer();
			joe.Name = "Joe";
			joe.Address.Street = "San Francisco";
			joe.Address.Zip = "54353";
			joe.Address.Country = "USA";
			joe.Comments = "very demanding";
			joe.Sex = 'M';
			joe.Salesperson = mark;

			Person mom = new Person();
			mom.Name = "mom";
			mom.Sex = 'F';

			s.Save( mom );
			s.Save( mark );
			s.Save( joe );

			s.Flush();

			Assert.AreEqual( 3, s.CreateQuery( "from Person" ).List().Count );
			IQuery query = s.CreateQuery( "from Customer" );
			IList results = query.List();
			Assert.AreEqual( 1, results.Count );
			Assert.IsTrue( results[0] is Customer, "should be a customer" );

			// in later versions of hibernate a Clear method was added to session
			s.Close();
			s = sessions.OpenSession();
			
			IList customers = s.CreateQuery( "from Customer c left join fetch c.Salesperson" ).List();
			foreach( Customer c in customers ) 
			{
				// when proxies is working this is important
				Assert.IsTrue( NHibernate.IsInitialized( c.Salesperson ) );
				Assert.AreEqual( "Mark", c.Salesperson.Name );
			}
			Assert.AreEqual( 1, customers.Count );
			s.Close();
			s = sessions.OpenSession();

			customers = s.CreateQuery( "from Customer" ).List();
			foreach( Customer c in customers ) 
			{
				//TODO: proxies make this work
				// Assert.IsFalse( NHibernate.IsInitialized( c.Salesperson ) );
				Assert.AreEqual( "Mark", c.Salesperson.Name );
			}
			Assert.AreEqual( 1, customers.Count );

			s.Close();
			s = sessions.OpenSession();

			mark = (Employee)s.Load( typeof(Employee), mark.Id );
			joe = (Customer)s.Load( typeof(Customer), joe.Id );

			mark.Address.Zip = "30306" ;
			Assert.AreEqual( 1, s.CreateQuery( "from Person p where p.Address.Zip = '30306'" ).List().Count );

			s.Delete( mom );
			s.Delete( joe );
			s.Delete( mark );

			Assert.AreEqual( 0, s.CreateQuery( "from Person" ).List().Count );
			s.Close();
										 
		}

		/// <summary>
		/// Test the ability to insert a new row with a User Assigned Key
		/// Right now - the only way to verify this is to watch SQL Profiler
		/// </summary>
		[Test]
		public void TestCRUD() {
			
			// test the Save
			ISession s = sessions.OpenSession();
			ITransaction t = s.BeginTransaction();

			Employee emp = new Employee();
			emp.Name = "test one";
			emp.Title = "office clown";

			s.Save(emp);

			Person person = new Person();
			person.Name = "the test string";
			
			s.Save(person);

			t.Commit();
			s.Close();

			int empId = emp.Id;
			int personId = person.Id;

			// lets verify the correct classes were saved
			s = sessions.OpenSession();
			t = s.BeginTransaction();
			
			// perform a load based on the base class
			Person empAsPerson = (Person)s.Load( typeof(Person), empId );
			person = (Person)s.Load( typeof(Person), personId );

			// the object with id=2 was loaded using the base class - lets make sure it actually loaded
			// the sublcass
			emp = empAsPerson as Employee;
			Assert.IsNotNull(emp);

			// lets update the objects
			person.Name = "Did it get updated";
			person.Sex = 'M';

			// update the properties from the subclass and base class
			emp.Name = "Updated Employee String";
			emp.Title = "office dunce";
			
			// save it through the base class reference and make sure that the
			// subclass properties get updated.
			s.Update( empAsPerson );
			s.Update( person );
			
			t.Commit();
			s.Close();

			// lets test the Criteria interface for subclassing
			s = sessions.OpenSession();
			t = s.BeginTransaction();

			IList results = s.CreateCriteria( typeof(Person) )
				.Add(Expression.Expression.In( "Name", new string[] {"Did it get updated", "Updated Employee String" }))
				.List();

			Assert.AreEqual( 2, results.Count);

			person = null;
			emp = null;

			foreach(Person obj in results) 
			{
				if(obj is Employee) 
					emp = (Employee)obj;
				else 
					person = obj;
			}

			// verify the properties got updated
			Assert.AreEqual( 'M', person.Sex );
			Assert.AreEqual( "office dunce", emp.Title );

			s.Delete( emp );
			s.Delete( person );

			t.Commit();
			s.Close();
		}
	}
}
