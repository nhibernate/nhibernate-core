using System;
using System.Collections;
using NHibernate.Cfg;
using NUnit.Framework;

namespace NHibernate.Test.SqlTest
{
	public abstract class HandSQLTest : TestCase
	{
		private static object GetFirstItem(IEnumerable it)
		{
			IEnumerator en = it.GetEnumerator();
			return en.MoveNext() ? en.Current : null;
		}

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected abstract System.Type GetDialect();

		private void CheckDialect()
		{
			if (!GetDialect().IsInstanceOfType(Dialect.Dialect.GetDialect()))
				Assert.Ignore("This test is specific for " + GetDialect().ToString());
		}

		protected override void Configure(Configuration cfg)
		{
			CheckDialect();
			base.Configure(cfg);
		}

		[Test]
		public void HandSQL()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			Organization ifa = new Organization("IFA");
			Organization jboss = new Organization("JBoss");
			Person gavin = new Person("Gavin");
			Employment emp = new Employment(gavin, jboss, "AU");
			object orgId = s.Save(jboss);
			s.Save(ifa);
			s.Save(gavin);
			s.Save(emp);
			t.Commit();

			t = s.BeginTransaction();
			Person christian = new Person("Christian");
			s.Save(christian);
			Employment emp2 = new Employment(christian, jboss, "EU");
			s.Save(emp2);
			t.Commit();
			s.Close();

			sessions.Evict(typeof(Organization));
			sessions.Evict(typeof(Person));
			sessions.Evict(typeof(Employment));

			s = OpenSession();
			t = s.BeginTransaction();
			jboss = (Organization) s.Get(typeof(Organization), orgId);
			Assert.AreEqual(jboss.Employments.Count, 2);
			emp = (Employment) GetFirstItem(jboss.Employments);
			gavin = emp.Employee;
			Assert.AreEqual(gavin.Name, "GAVIN");
			Assert.AreEqual(s.GetCurrentLockMode(gavin), LockMode.Upgrade);
			emp.EndDate = DateTime.Today;
			Employment emp3 = new Employment(gavin, jboss, "US");
			s.Save(emp3);
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			IEnumerator iter = s.GetNamedQuery("allOrganizationsWithEmployees").List().GetEnumerator();
			Assert.IsTrue(iter.MoveNext());
			Organization o = (Organization) iter.Current;
			Assert.AreEqual(o.Employments.Count, 3);

			foreach (Employment e in o.Employments)
			{
				s.Delete(e);
			}

			foreach (Employment e in o.Employments)
			{
				s.Delete(e.Employee);
			}
			s.Delete(o);
			Assert.IsFalse(iter.MoveNext());
			s.Delete(ifa);
			t.Commit();
			s.Close();
		}

		[Test]
		public void ScalarStoredProcedure()
		{
			ISession s = OpenSession();
			IQuery namedQuery = s.GetNamedQuery("simpleScalar");
			namedQuery.SetInt64("number", 43L);
			IList list = namedQuery.List();
			object[] o = (object[]) list[0];
			Assert.AreEqual(o[0], "getAll");
			Assert.AreEqual(o[1], 43L);
			s.Close();
		}

		[Test]
		public void ParameterHandling()
		{
			ISession s = OpenSession();

			IQuery namedQuery = s.GetNamedQuery("paramhandling");
			namedQuery.SetInt64(0, 10L);
			namedQuery.SetInt64(1, 20L);
			IList list = namedQuery.List();
			object[] o = (Object[]) list[0];
			Assert.AreEqual(o[0], 10L);
			Assert.AreEqual(o[1], 20L);

			namedQuery = s.GetNamedQuery("paramhandling_mixed");
			namedQuery.SetInt64(0, 10L);
			namedQuery.SetInt64("second", 20L);
			list = namedQuery.List();
			o = (object[]) list[0];
			Assert.AreEqual(o[0], 10L);
			Assert.AreEqual(o[1], 20L);
			s.Close();
		}

		[Test]
		public void EntityStoredProcedure()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();

			Organization ifa = new Organization("IFA");
			Organization jboss = new Organization("JBoss");
			Person gavin = new Person("Gavin");
			Employment emp = new Employment(gavin, jboss, "AU");
			s.Save(ifa);
			s.Save(jboss);
			s.Save(gavin);
			s.Save(emp);

			IQuery namedQuery = s.GetNamedQuery("selectAllEmployments");
			IList list = namedQuery.List();
			Assert.IsTrue(list[0] is Employment);
			s.Delete(emp);
			s.Delete(ifa);
			s.Delete(jboss);
			s.Delete(gavin);

			t.Commit();
			s.Close();
		}
	}
}