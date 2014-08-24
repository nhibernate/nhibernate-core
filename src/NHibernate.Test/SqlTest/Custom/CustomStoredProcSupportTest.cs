using System;
using System.Collections;
using NUnit.Framework;

namespace NHibernate.Test.SqlTest.Custom
{
	public abstract class CustomStoredProcSupportTest : CustomSQLSupportTest
	{
		[Test]
		public void ScalarStoredProcedure()
		{
			ISession s = OpenSession();
			IQuery namedQuery = s.GetNamedQuery("simpleScalar");
			namedQuery.SetInt64("number", 43L);
			IList list = namedQuery.List();
			object[] o = (object[])list[0];
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
			object[] o = (Object[])list[0];
			Assert.AreEqual(o[0], 10L);
			Assert.AreEqual(o[1], 20L);

			namedQuery = s.GetNamedQuery("paramhandling_mixed");
			namedQuery.SetInt64(0, 10L);
			namedQuery.SetInt64("second", 20L);
			list = namedQuery.List();
			o = (object[])list[0];
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