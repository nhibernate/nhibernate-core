using System;
using System.Collections;
using NUnit.Framework;

namespace NHibernate.Test.SqlTest.Custom
{
	public abstract class CustomSQLSupportTest: TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected static object GetFirstItem(IEnumerable it)
		{
			IEnumerator en = it.GetEnumerator();
			return en.MoveNext() ? en.Current : null;
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
			jboss = (Organization)s.Get(typeof(Organization), orgId);
			Assert.AreEqual(jboss.Employments.Count, 2);
			emp = (Employment)GetFirstItem(jboss.Employments);
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
			Organization o = (Organization)iter.Current;
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

	}
}