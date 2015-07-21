using System;
using System.Collections;
using NUnit.Framework;

namespace NHibernate.Test.VersionTest
{
	[TestFixture]
	public class VersionFixture : TestCase
	{
		protected override IList Mappings
		{
			get { return new string[] {"VersionTest.PersonThing.hbm.xml"}; }
		}

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		[Test]
		public void VersionShortCircuitFlush()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			Person gavin = new Person("Gavin");
			new Thing("Passport", gavin);
			s.Save(gavin);
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			Thing passp = (Thing) s.Get(typeof(Thing), "Passport");
			passp.LongDescription = "blah blah blah";
			s.CreateQuery("from Person").List();
			s.CreateQuery("from Person").List();
			s.CreateQuery("from Person").List();
			t.Commit();
			s.Close();

			Assert.AreEqual(passp.Version, 2);

			s = OpenSession();
			t = s.BeginTransaction();
			s.Delete("from Thing");
			s.Delete("from Person");
			t.Commit();
			s.Close();
		}

		[Test]
		public void CollectionVersion()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			Person gavin = new Person("Gavin");
			new Thing("Passport", gavin);
			s.Save(gavin);
			t.Commit();
			s.Close();

			Assert.AreEqual(1, gavin.Version);

			s = OpenSession();
			t = s.BeginTransaction();
			gavin = (Person) s.CreateCriteria(typeof(Person)).UniqueResult();
			new Thing("Laptop", gavin);
			t.Commit();
			s.Close();

			Assert.AreEqual(2, gavin.Version);
			Assert.IsFalse(NHibernateUtil.IsInitialized(gavin.Things));

			s = OpenSession();
			t = s.BeginTransaction();
			gavin = (Person) s.CreateCriteria(typeof(Person)).UniqueResult();
			gavin.Things.Clear();
			t.Commit();
			s.Close();

			Assert.AreEqual(3, gavin.Version);
			Assert.IsTrue(NHibernateUtil.IsInitialized(gavin.Things));

			s = OpenSession();
			t = s.BeginTransaction();
			s.Delete(gavin);
			t.Commit();
			s.Close();
		}

		[Test]
		public void CollectionNoVersion()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			Person gavin = new Person("Gavin");
			new Task("Code", gavin);
			s.Save(gavin);
			t.Commit();
			s.Close();

			Assert.AreEqual(1, gavin.Version);

			s = OpenSession();
			t = s.BeginTransaction();
			gavin = (Person) s.CreateCriteria(typeof(Person)).UniqueResult();
			new Task("Document", gavin);
			t.Commit();
			s.Close();

			Assert.AreEqual(1, gavin.Version);
			Assert.IsFalse(NHibernateUtil.IsInitialized(gavin.Tasks));

			s = OpenSession();
			t = s.BeginTransaction();
			gavin = (Person) s.CreateCriteria(typeof(Person)).UniqueResult();
			gavin.Tasks.Clear();
			t.Commit();
			s.Close();

			Assert.AreEqual(1, gavin.Version);
			Assert.IsTrue(NHibernateUtil.IsInitialized(gavin.Tasks));

			s = OpenSession();
			t = s.BeginTransaction();
			s.Delete(gavin);
			t.Commit();
			s.Close();
		}
	}
}