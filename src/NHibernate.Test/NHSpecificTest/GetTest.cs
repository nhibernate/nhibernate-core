using System.Collections;
using NHibernate.DomainModel;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest
{
	[TestFixture]
	public class GetTest : TestCase
	{
		protected override IList Mappings
		{
			get
			{
				// have to use classes with proxies to test difference
				// between Get() and Load()
				return new string[] {"ABCProxy.hbm.xml"};
			}
		}

		protected override void OnTearDown()
		{
			using (ISession s = Sfi.OpenSession())
			{
				s.Delete("from A");
				s.Flush();
			}
		}

		[Test]
		public void GetVsLoad()
		{
			A a = new A("name");

			using (ISession s = OpenSession())
			{
				s.Save(a);
			}

			using (ISession s = OpenSession())
			{
				A loadedA = (A) s.Load(typeof(A), a.Id);
				Assert.IsFalse(NHibernateUtil.IsInitialized(loadedA),
				               "Load should not initialize the object");

				Assert.IsNotNull(s.Load(typeof(A), (a.Id + 1)),
				                 "Loading non-existent object should not return null");
			}

			using (ISession s = OpenSession())
			{
				A gotA = (A) s.Get(typeof(A), a.Id);
				Assert.IsTrue(NHibernateUtil.IsInitialized(gotA),
				              "Get should initialize the object");

				Assert.IsNull(s.Get(typeof(A), (a.Id + 1)),
				              "Getting non-existent object should return null");
			}
		}

		[Test]
		public void GetAndModify()
		{
			A a = new A("name");
			using (ISession s = OpenSession())
			{
				s.Save(a);
				s.Flush();
			}

			using (ISession s = OpenSession())
			{
				a = s.Get(typeof(A), a.Id) as A;
				a.Name = "modified";
				s.Flush();
			}

			using (ISession s = OpenSession())
			{
				a = s.Get(typeof(A), a.Id) as A;
				Assert.AreEqual("modified", a.Name, "the name was modified");
			}
		}

		[Test]
		public void GetAfterLoad()
		{
			long id;

			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				A a = new A("name");
				id = (long) s.Save(a);
				tx.Commit();
			}

			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				A loadedA = (A) s.Load(typeof(A), id);
				Assert.IsFalse(NHibernateUtil.IsInitialized(loadedA));
				A gotA = (A) s.Get(typeof(A), id);
				Assert.IsTrue(NHibernateUtil.IsInitialized(gotA));
				Assert.AreSame(loadedA, gotA);
				tx.Commit();
			}

			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				A loadedNonExistentA = (A) s.Load(typeof(A), -id);
				Assert.IsFalse(NHibernateUtil.IsInitialized(loadedNonExistentA));
				// changed behavior because NH-1252
				Assert.IsNull(s.Get(typeof(A), -id));
				tx.Commit();
			}
		}
	}
}
