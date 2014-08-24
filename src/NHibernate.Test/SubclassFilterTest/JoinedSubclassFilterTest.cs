using System;
using System.Collections;
using NUnit.Framework;
using System.Linq;

namespace NHibernate.Test.SubclassFilterTest
{
	[TestFixture]
	public class JoinedSubclassFilterTest : TestCase
	{
		protected override IList Mappings
		{
			get { return new string[] {"SubclassFilterTest.joined-subclass.hbm.xml"}; }
		}

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		[Test]
		public void FiltersWithSubclass()
		{
			ISession s = OpenSession();
			s.EnableFilter("region").SetParameter("userRegion", "US");
			ITransaction t = s.BeginTransaction();

			PrepareTestData(s);
			s.Clear();

			IList results;

			results = s.CreateQuery("from Person").List();
			Assert.AreEqual(4, results.Count, "Incorrect qry result count");
			s.Clear();

			results = s.CreateQuery("from Employee").List();
			Assert.AreEqual(2, results.Count, "Incorrect qry result count");

			foreach (Person p in  results)
			{
				// find john
				if (p.Name.Equals("John Doe"))
				{
					Employee john = (Employee) p;
					Assert.AreEqual(2, john.Minions.Count, "Incorrect fecthed minions count");
					break;
				}
			}
			s.Clear();

			// TODO : currently impossible to define a collection-level filter w/
			// joined-subclass elements that will filter based on a superclass
			// column and function correctly in (theta only?) outer joins;
			// this is consistent with the behaviour of a collection-level where.
			// this might be one argument for "pulling" the attached class-level
			// filters into collection assocations,
			// although we'd need some way to apply the appropriate alias in that
			// scenario.
			results = s.CreateQuery("from Person as p left join fetch p.Minions").List<Person>().Distinct().ToList();
			Assert.AreEqual(4, results.Count, "Incorrect qry result count");
			foreach (Person p in results)
			{
				if (p.Name.Equals("John Doe"))
				{
					Employee john = (Employee) p;
					Assert.AreEqual(2, john.Minions.Count, "Incorrect fecthed minions count");
					break;
				}
			}

			s.Clear();

			results = s.CreateQuery("from Employee as p left join fetch p.Minions").List<Employee>().Distinct().ToList();
			Assert.AreEqual(2, results.Count, "Incorrect qry result count");
			foreach (Person p in results)
			{
				if (p.Name.Equals("John Doe"))
				{
					Employee john = (Employee) p;
					Assert.AreEqual(2, john.Minions.Count, "Incorrect fecthed minions count");
					break;
				}
			}

			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			s.Delete("from Customer c where c.ContactOwner is not null");
			s.Delete("from Employee e where e.Manager is not null");
			s.Delete("from Person");
			t.Commit();
			s.Close();
		}

		private static void PrepareTestData(ISession s)
		{
			Employee john = new Employee("John Doe");
			john.Company = ("JBoss");
			john.Department = ("hr");
			john.Title = ("hr guru");
			john.Region = ("US");

			Employee polli = new Employee("Polli Wog");
			polli.Company = ("JBoss");
			polli.Department = ("hr");
			polli.Title = ("hr novice");
			polli.Region = ("US");
			polli.Manager = (john);
			john.Minions.Add(polli);

			Employee suzie = new Employee("Suzie Q");
			suzie.Company = ("JBoss");
			suzie.Department = ("hr");
			suzie.Title = ("hr novice");
			suzie.Region = ("EMEA");
			suzie.Manager = (john);
			john.Minions.Add(suzie);

			Customer cust = new Customer("John Q Public");
			cust.Company = ("Acme");
			cust.Region = ("US");
			cust.ContactOwner = (john);

			Person ups = new Person("UPS guy");
			ups.Company = ("UPS");
			ups.Region = ("US");

			s.Save(john);
			s.Save(cust);
			s.Save(ups);

			s.Flush();
		}
	}
}