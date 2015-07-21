using System;
using System.Collections;

using log4net;
using NHibernate.Criterion;
using NUnit.Framework;

namespace NHibernate.Test.Unconstrained
{
	[TestFixture]
	public class UnconstrainedNoLazyTest : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get { return new string[] {"Unconstrained.PersonNoLazy.hbm.xml"}; }
		}

		[Test]
		public void UnconstrainedNoCache()
		{
			ISession session = OpenSession();
			ITransaction tx = session.BeginTransaction();
			Person p = new Person("gavin");
			p.EmployeeId = "123456";
			session.Save(p);
			tx.Commit();
			session.Close();

			sessions.Evict(typeof(Person));

			session = OpenSession();
			tx = session.BeginTransaction();
			p = (Person) session.Get(typeof(Person), "gavin");
			Assert.IsNull(p.Employee);
			p.Employee = new Employee("123456");
			tx.Commit();
			session.Close();

			sessions.Evict(typeof(Person));

			session = OpenSession();
			tx = session.BeginTransaction();
			p = (Person) session.Get(typeof(Person), "gavin");
			Assert.IsNotNull(p.Employee);
			Assert.IsTrue(NHibernateUtil.IsInitialized(p.Employee));
			session.Delete(p);
			tx.Commit();
			session.Close();
		}

		[Test]
		public void UnconstrainedOuterJoinFetch()
		{
			ISession session = OpenSession();
			ITransaction tx = session.BeginTransaction();
			Person p = new Person("gavin");
			p.EmployeeId = "123456";
			session.Save(p);
			tx.Commit();
			session.Close();

			sessions.Evict(typeof(Person));

			session = OpenSession();
			tx = session.BeginTransaction();
			p = (Person) session.CreateCriteria(typeof(Person))
			             	.SetFetchMode("Employee", FetchMode.Join)
			             	.Add(Expression.Eq("Name", "gavin"))
			             	.UniqueResult();
			Assert.IsNull(p.Employee);
			p.Employee = new Employee("123456");
			tx.Commit();
			session.Close();

			sessions.Evict(typeof(Person));

			session = OpenSession();
			tx = session.BeginTransaction();
			p = (Person) session.CreateCriteria(typeof(Person))
			             	.SetFetchMode("Employee", FetchMode.Join)
			             	.Add(Expression.Eq("Name", "gavin"))
			             	.UniqueResult();
			Assert.IsTrue(NHibernateUtil.IsInitialized(p.Employee));
			Assert.IsNotNull(p.Employee);
			session.Delete(p);
			tx.Commit();
			session.Close();
		}

		[Test]
		public void Unconstrained()
		{
			ILog log = LogManager.GetLogger(GetType());

			log.Info("Unconstrained - BEGIN");
			
			log.Info("Creating Person#gavin with EmployeeId = 123456 (non-existent)");

			ISession session = OpenSession();
			ITransaction tx = session.BeginTransaction();
			Person p = new Person("gavin");
			p.EmployeeId = "123456";
			session.Save(p);
			tx.Commit();
			session.Close();

			log.Info("Loading Person#gavin and associating it with a new Employee#123456");

			session = OpenSession();
			tx = session.BeginTransaction();
			p = (Person) session.Get(typeof(Person), "gavin");
			Assert.IsNull(p.Employee);
			p.Employee = new Employee("123456");
			tx.Commit();
			session.Close();

			log.Info("Reloading Person#gavin and checking that its Employee is not null");

			session = OpenSession();
			tx = session.BeginTransaction();
			p = (Person) session.Get(typeof(Person), "gavin");
			Assert.IsNotNull(p.Employee);
			Assert.IsTrue(NHibernateUtil.IsInitialized(p.Employee));
			Assert.IsNotNull(p.Employee.Id);
			session.Delete(p);
			tx.Commit();
			session.Close();
		}

		[Test]
		public void ManyToOneUpdateFalse()
		{
			ISession session = OpenSession();
			ITransaction tx = session.BeginTransaction();
			Person p = new Person("gavin");
			p.EmployeeId = "123456";
			p.Unrelated = 10;
			session.Save(p);
			tx.Commit();

			session.BeginTransaction();
			p.Employee = new Employee("456123");
			p.Unrelated = 235; // Force update of the object
			session.Save(p.Employee);
			session.Transaction.Commit();
			session.Close();

			session = OpenSession();
			session.BeginTransaction();
			p = (Person) session.Load(typeof (Person), "gavin");
			// Should be null, not Employee#456123
			Assert.IsNull(p.Employee);
			session.Delete(p);
			session.Delete("from Employee");
			session.Transaction.Commit();
			session.Close();
		}
	}
}