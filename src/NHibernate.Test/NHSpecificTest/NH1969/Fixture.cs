using System.Collections.Generic;
using System.IO;
using NHibernate.Criterion;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1969
{
	/// <summary>
	/// Author : Stephane Verlet
	/// </summary>
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			using (ISession s = OpenSession())
			{
				using (ITransaction tx = s.BeginTransaction())
				{
					var entity = new EntityWithTypeProperty {Id = 1, TypeValue = typeof (File)};
					s.Save(entity);

					entity = new EntityWithTypeProperty {Id = 2, TypeValue = typeof (DummyEntity)};
					s.Save(entity);

					tx.Commit();
				}
			}
		}

		protected override void OnTearDown()
		{
			using (ISession session = OpenSession())
			{
				using (ITransaction tx = session.BeginTransaction())
				{
					session.CreateQuery("delete from DummyEntity").ExecuteUpdate();
					session.CreateQuery("delete from EntityWithTypeProperty").ExecuteUpdate();
					tx.Commit();
				}
			}
		}

		[Test]
		public void TestMappedTypeCriteria()
		{
			using (ISession s = OpenSession())
			{
				ICriteria criteria = s.CreateCriteria(typeof (EntityWithTypeProperty));
				criteria.Add(Restrictions.Eq("TypeValue", typeof (DummyEntity)));
				IList<EntityWithTypeProperty> results = criteria.List<EntityWithTypeProperty>();
				Assert.AreEqual(1, results.Count);
				Assert.AreEqual(2, results[0].Id);
			}
		}

		[Test]
		public void TestMappedTypeHQL()
		{
			using (ISession s = OpenSession())
			{
				IQuery q = s.CreateQuery("select t from EntityWithTypeProperty as t where t.TypeValue = :type");
				q.SetParameter("type", typeof (DummyEntity));
				IList<EntityWithTypeProperty> results = q.List<EntityWithTypeProperty>();
				Assert.AreEqual(1, results.Count);
				Assert.AreEqual(2, results[0].Id);
			}
		}

		[Test]
		public void TestNonMappedTypeCriteria()
		{
			using (ISession s = OpenSession())
			{
				ICriteria criteria = s.CreateCriteria(typeof (EntityWithTypeProperty));
				criteria.Add(Restrictions.Eq("TypeValue", typeof (File)));
				IList<EntityWithTypeProperty> results = criteria.List<EntityWithTypeProperty>();
				Assert.AreEqual(1, results.Count);
				Assert.AreEqual(1, results[0].Id);
			}
		}

		[Test]
		public void TestNonMappedTypeHQL()
		{
			using (ISession s = OpenSession())
			{
				IQuery q = s.CreateQuery("select t from EntityWithTypeProperty as t where t.TypeValue = :type");
				q.SetParameter("type", typeof (File));
				IList<EntityWithTypeProperty> results = q.List<EntityWithTypeProperty>();
				Assert.AreEqual(1, results.Count);
				Assert.AreEqual(1, results[0].Id);
			}
		}
	}
}