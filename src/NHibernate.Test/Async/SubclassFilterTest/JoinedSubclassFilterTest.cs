﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System.Collections;
using NUnit.Framework;
using System.Linq;
using NHibernate.Linq;

namespace NHibernate.Test.SubclassFilterTest
{
	using System.Threading.Tasks;
	[TestFixture]
	public class JoinedSubclassFilterTestAsync : TestCase
	{
		protected override string[] Mappings => new[] {"SubclassFilterTest.joined-subclass.hbm.xml"};

		protected override string MappingsAssembly => "NHibernate.Test";

		protected override void OnSetUp()
		{
			using var s = OpenSession();
			using var t = s.BeginTransaction();
			PrepareTestData(s);
			t.Commit();
		}

		protected override void OnTearDown()
		{
			using var s = OpenSession();
			using var t = s.BeginTransaction();
			s.Delete("from Customer c where c.ContactOwner is not null");
			s.Delete("from Employee e where e.Manager is not null");
			s.Delete("from Person");
			t.Commit();
		}

		[Test]
		public async Task FiltersWithSubclassAsync()
		{
			using var s = OpenSession();
			s.EnableFilter("region").SetParameter("userRegion", "US");

			IList results;

			results = await (s.CreateQuery("from Person").ListAsync());
			Assert.AreEqual(4, results.Count, "Incorrect qry result count");
			s.Clear();

			results = await (s.CreateQuery("from Employee").ListAsync());
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

			results = (await (s.CreateQuery("from Person as p left join fetch p.Minions").ListAsync<Person>())).Distinct().ToList();
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

			results = (await (s.CreateQuery("from Employee as p left join fetch p.Minions").ListAsync<Employee>())).Distinct().ToList();
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
		}

		[Test]
		public async Task FilterCollectionWithSubclass1Async()
		{
			using var s = OpenSession();

			s.EnableFilter("minionsWithManager");

			var employees = await (s.Query<Employee>().Where(x => x.Minions.Any()).ToListAsync());
			Assert.That(employees.Count, Is.EqualTo(1));
			Assert.That(employees[0].Minions.Count, Is.EqualTo(2));
		}

		[Test(Description = "GH-3079: Collection filter on subclass columns")]
		public async Task FilterCollectionWithSubclass2Async()
		{
			using var s = OpenSession();
			s.EnableFilter("minionsRegion").SetParameter("userRegion", "US");

			var employees = await (s.Query<Employee>().Where(x => x.Minions.Any()).ToListAsync());
			Assert.That(employees.Count, Is.EqualTo(1));
			Assert.That(employees[0].Minions.Count, Is.EqualTo(1));
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
		}
	}
}
