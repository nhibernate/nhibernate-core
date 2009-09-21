using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1969
{
	using Criterion;

	/// <summary>
	/// Author : Stephane Verlet
	/// </summary>
	[TestFixture]
	public class Fixture : BugTestCase
	{

		protected override void OnSetUp()
		{
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{

				EntityWithTypeProperty entity = new EntityWithTypeProperty();
				entity.Id = 1;
				entity.TypeValue = typeof(System.IO.File);  //A random not mapped type
				s.Save(entity);

				entity = new EntityWithTypeProperty();
				entity.Id = 2;
				entity.TypeValue = typeof(DummyEntity); // A mapped entity
				s.Save(entity);

				tx.Commit();
			}

		}

		protected override void OnTearDown()
		{
			using (ISession session = this.OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				string hql = "from System.Object";
				session.Delete(hql);
				tx.Commit();
			}
		}

		[Test,Ignore]
		public void TestMappedTypeCriteria()
		{
			using (ISession s = OpenSession())
			{
				ICriteria criteria = s.CreateCriteria(typeof(EntityWithTypeProperty));
				criteria.Add(Restrictions.Eq("TypeValue", typeof(DummyEntity)));
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
				q.SetParameter("type", typeof(DummyEntity));
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
				ICriteria criteria = s.CreateCriteria(typeof(EntityWithTypeProperty));
				criteria.Add(Restrictions.Eq("TypeValue", typeof(System.IO.File)));
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
				q.SetParameter("type", typeof(System.IO.File));
				IList<EntityWithTypeProperty> results = q.List<EntityWithTypeProperty>();
				Assert.AreEqual(1, results.Count);
				Assert.AreEqual(1, results[0].Id);

			}
		}

	}
}