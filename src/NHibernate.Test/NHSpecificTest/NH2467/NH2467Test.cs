using System.Linq;
using NHibernate.Criterion;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2467
{
	[TestFixture]
	public class NH2467Test : BugTestCase
	{
		protected override void OnSetUp()
		{
			base.OnSetUp();
			using (var session = OpenSession())
			{
				var entity = new DomainClass {Id = 1, Data = "Test"};
				session.Save(entity);
				session.Flush();
			}
		}

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return dialect.SupportsLimit;
		}

		protected override void OnTearDown()
		{
			base.OnTearDown();
			using (var session = OpenSession())
			{
				session.Delete("from System.Object");
				session.Flush();
			}
		}
		
		[Test]
		public void ShouldNotThrowOnFuturePaging()
		{
			using (var session = OpenSession())
			{
			
				var contentQuery = session
					.CreateCriteria<DomainClass>()
					.Add(Restrictions.Eq("Data", "Test"));
				contentQuery.SetMaxResults(2);
				contentQuery.SetFirstResult(0);
				var content = contentQuery.Future<DomainClass>();
					
				var countQuery = session
					.CreateCriteria<DomainClass>()
					.Add(Restrictions.Eq("Data", "Test"));
				countQuery.SetProjection(Projections.RowCount());
				var count = countQuery.FutureValue<int>();

				// triggers batch operation, should not throw
				var result = content.ToList();
			}
		}		
		
		[Test]
		public void ShouldNotThrowOnReversedFuturePaging()
		{
			using (var session = OpenSession())
			{
			
				var countQuery = session
					.CreateCriteria<DomainClass>()
					.Add(Restrictions.Eq("Data", "Test"));
				countQuery.SetProjection(Projections.RowCount());
				var count = countQuery.FutureValue<int>();

				var contentQuery = session
					.CreateCriteria<DomainClass>()
					.Add(Restrictions.Eq("Data", "Test"));
				contentQuery.SetMaxResults(2);
				contentQuery.SetFirstResult(0);
				var content = contentQuery.Future<DomainClass>();

				// triggers batch operation, should not throw
				var result = content.ToList();
			}
		}		
		
		[Test]
		public void ShouldNotThrowOnFuturePagingUsingHql()
		{
			using (var session = OpenSession())
			{

				var contentQuery = session.CreateQuery("from DomainClass as d where d.Data = ?");
				contentQuery.SetString(0, "Test");
				contentQuery.SetMaxResults(2);
				contentQuery.SetFirstResult(0);
				var content = contentQuery.Future<DomainClass>();

				var countQuery = session.CreateQuery("select count(d) from DomainClass as d where d.Data = ?");
				countQuery.SetString(0, "Test");
				var count = countQuery.FutureValue<long>();
				
				Assert.AreEqual(1, content.ToList().Count);
				Assert.AreEqual(1, count.Value);
			}
		}

		[Test]
		public void ShouldNotThrowOnReversedFuturePagingUsingHql()
		{
			using (var session = OpenSession())
			{

				var contentQuery = session.CreateQuery("from DomainClass as d where d.Data = ?");
				contentQuery.SetString(0, "Test");
				contentQuery.SetMaxResults(2);
				contentQuery.SetFirstResult(0);
				var content = contentQuery.Future<DomainClass>();

				var countQuery = session.CreateQuery("select count(d) from DomainClass as d where d.Data = ?");
				countQuery.SetString(0, "Test");
				var count = countQuery.FutureValue<long>();
				
				Assert.AreEqual(1, content.ToList().Count);
				Assert.AreEqual(1, count.Value);
			}
		}
	}
}
