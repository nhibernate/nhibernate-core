using NHibernate.Criterion;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1324
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		public override string BugNumber
		{
			get { return "NH1324"; }
		}


		[Test]
		public void CanUseUniqueResultWithNullableType_ReturnNull_Criteria()
		{
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				Person p = new Person("a", null, 4);
				s.Save(p);
				tx.Commit();
			}

			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				int? result = s.CreateCriteria(typeof (Person))
					.SetProjection(Projections.Property("IQ"))
					.UniqueResult<int?>();
				Assert.IsNull(result);
				tx.Commit();
			}

			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				s.Delete("from Person");
				tx.Commit();
			}
		}

		[Test]
		public void CanUseUniqueResultWithNullableType_ReturnResult_Criteria()
		{
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				Person p = new Person("a", 4, 4);
				s.Save(p);
				tx.Commit();
			}

			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				int? result = s.CreateCriteria(typeof(Person))
					.SetProjection(Projections.Property("IQ"))
					.UniqueResult<int?>();
				Assert.AreEqual(4, result);
				tx.Commit();
			}

			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				s.Delete("from Person");
				tx.Commit();
			}
		}

		[Test]
		public void CanUseUniqueResultWithNullableType_ReturnNull_HQL()
		{
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				Person p = new Person("a", null, 4);
				s.Save(p);
				tx.Commit();
			}

			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				int? result = s.CreateQuery("select p.IQ from Person p")
					.UniqueResult<int?>();
				Assert.IsNull(result);
				tx.Commit();
			}

			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				s.Delete("from Person");
				tx.Commit();
			}
		}

		[Test]
		public void CanUseUniqueResultWithNullableType_ReturnResult_HQL()
		{
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				Person p = new Person("a", 4, 4);
				s.Save(p);
				tx.Commit();
			}

			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				int? result = s.CreateQuery("select p.IQ from Person p")
					.UniqueResult<int?>();
				Assert.AreEqual(4, result);
				tx.Commit();
			}

			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				s.Delete("from Person");
				tx.Commit();
			}
		}
	}
}