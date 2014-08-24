using System.Collections;
using NHibernate.Dialect;
using NUnit.Framework;

namespace NHibernate.Test.SqlTest.Custom.Oracle
{
	[TestFixture]
	public class OracleCustomSQLFixture : CustomStoredProcSupportTest
	{
		protected override IList Mappings
		{
			get { return new[] { "SqlTest.Custom.Oracle.Mappings.hbm.xml", "SqlTest.Custom.Oracle.StoredProcedures.hbm.xml" }; }
		}

		protected override bool AppliesTo(NHibernate.Engine.ISessionFactoryImplementor factory)
		{
			return factory.ConnectionProvider.Driver is Driver.OracleDataClientDriver;
		}

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return dialect is Oracle8iDialect;
		}

		[Test]
		public void RefCursorOutStoredProcedure()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();

			Organization ifa = new Organization("IFA");
			Organization jboss = new Organization("JBoss");
			Person gavin = new Person("Gavin");
			Person kevin = new Person("Kevin");
			Employment emp = new Employment(gavin, jboss, "AU");
			Employment emp2 = new Employment(kevin, ifa, "EU");
			s.Save(ifa);
			s.Save(jboss);
			s.Save(gavin);
			s.Save(kevin);
			s.Save(emp);
			s.Save(emp2);

			IQuery namedQuery = s.GetNamedQuery("selectEmploymentsForRegion");
			namedQuery.SetString("regionCode", "EU");
			IList list = namedQuery.List();
			Assert.That(list.Count, Is.EqualTo(1));
			s.Delete(emp2);
			s.Delete(emp);
			s.Delete(ifa);
			s.Delete(jboss);
			s.Delete(kevin);
			s.Delete(gavin);

			t.Commit();
			s.Close();
		}

	}
}