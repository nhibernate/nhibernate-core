using System.Collections;
using System.Collections.Generic;
using NHibernate.Criterion;
using NUnit.Framework;

namespace NHibernate.Test.Unionsubclass2
{
	[TestFixture]
	public class UnionSubclassFixture : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get { return new string[] { "Unionsubclass2.Person.hbm.xml" }; }
		}

		[Test]
		public void UnionSubclass()
		{
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				Employee mark = new Employee();
				mark.Name = "Mark";
				mark.Title = "internal sales";
				mark.Sex = 'M';
				mark.Address.address = "buckhead";
				mark.Address.zip = "30305";
				mark.Address.country = "USA";

				Customer joe = new Customer();
				joe.Name = "Joe";
				joe.Address.address = "San Francisco";
				joe.Address.zip = "XXXXX";
				joe.Address.country = "USA";
				joe.Comments = "Very demanding";
				joe.Sex = 'M';
				joe.Salesperson = mark;

				Person yomomma = new Person();
				yomomma.Name = "mum";
				yomomma.Sex = 'F';

				s.Save(yomomma);
				s.Save(mark);
				s.Save(joe);

				// TODO NH : This line is present in H3.2.5 test; ReadCommitted ?
				//Assert.AreEqual(0, s.CreateQuery("from System.Object").List().Count);

				Assert.AreEqual(3, s.CreateQuery("from Person").List().Count);
				Assert.AreEqual(1, s.CreateQuery("from Person p where p.class = Customer").List().Count);
				Assert.AreEqual(1, s.CreateQuery("from Person p where p.class = Person").List().Count);
				s.Clear();

				IList<Customer> customers = s.CreateQuery("from Customer c left join fetch c.salesperson").List<Customer>();
				foreach (Customer c in customers)
				{
					Assert.IsTrue(NHibernateUtil.IsInitialized(c.Salesperson));
					Assert.AreEqual("Mark", c.Salesperson.Name);
				}
				Assert.AreEqual(1, customers.Count);
				s.Clear();

				customers = s.CreateQuery("from Customer").List<Customer>();
				foreach (Customer c in customers)
				{
					Assert.IsFalse(NHibernateUtil.IsInitialized(c.Salesperson));
					Assert.AreEqual("Mark", c.Salesperson.Name);
				}
				Assert.AreEqual(1, customers.Count);
				s.Clear();

				mark = s.Get<Employee>(mark.Id);
				joe = s.Get<Customer>(joe.Id);

				mark.Address.zip = "30306";
				Assert.AreEqual(1, s.CreateQuery("from Person p where p.address.zip = '30306'").List().Count);

				s.Delete(mark);
				s.Delete(joe);
				s.Delete(yomomma);
				Assert.AreEqual(0, s.CreateQuery("from Person").List().Count);
				t.Commit();
				s.Close();
			}
		}

		[Test]
		public void QuerySubclassAttribute()
		{
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				Person p = new Person();
				p.Name = "Emmanuel";
				p.Sex = 'M';
				s.Persist(p);
				Employee q = new Employee();
				q.Name = "Steve";
				q.Sex = 'M';
				q.Title = "Mr";
				q.Salary = 1000m;
				s.Persist(q);

				IList result = s.CreateQuery("from Person p where p.salary > 100").List();
				Assert.AreEqual(1, result.Count);
				Assert.AreSame(q, result[0]);

				result = s.CreateQuery("from Person p where p.salary > 100 or p.name like 'E%'").List();
				Assert.AreEqual(2, result.Count);

                if (!TestDialect.HasBrokenDecimalType)
                {
                    result = s.CreateCriteria(typeof (Person)).Add(Property.ForName("salary").Gt(100m)).List();
                    Assert.AreEqual(1, result.Count);
                    Assert.AreSame(q, result[0]);
                }

			    result = s.CreateQuery("select p.salary from Person p where p.salary > 100").List();
				Assert.AreEqual(1, result.Count);
				Assert.AreEqual(1000m, (decimal)result[0]);

				s.Delete(p);
				s.Delete(q);
				t.Commit();
				s.Close();
			}
		}
	}
}
