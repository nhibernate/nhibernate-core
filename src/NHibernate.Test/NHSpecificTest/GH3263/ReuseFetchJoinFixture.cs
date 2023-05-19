using System.Linq;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH3263
{
	[TestFixture]
	public class ReuseFetchJoinFixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			using var s = OpenSession();
			using var t = s.BeginTransaction();
			var em = new Employee() { Name = "x", OptionalInfo = new OptionalInfo() };
			em.OptionalInfo.Employee = em;
			s.Save(em);
			t.Commit();
		}
		protected override void OnTearDown()
		{
			using var session = OpenSession();
			using var transaction = session.BeginTransaction();
			session.CreateQuery("delete from System.Object").ExecuteUpdate();

			transaction.Commit();
		}

		[Test]
		public void ReuseJoinScalarSelect()
		{
			using var session = OpenSession();
			session.Query<Employee>()
				.Fetch(x => x.OptionalInfo)
				.Where(x => x.OptionalInfo != null)
				.Select(x => new { x.OptionalInfo.Age })
				.ToList();
		}

		[Test]
		public void ReuseJoinScalarSelectHql()
		{
			using var session = OpenSession();
			session.CreateQuery(
				"select x.OptionalInfo.Age " +
				"from Employee x " +
				"fetch x.OptionalInfo " +
				"where x.OptionalInfo != null ").List();

		}

		[Test]
		public void ReuseJoinScalarSelectHql2()
		{
			using var session = OpenSession();
			session.CreateQuery(
				"select x.OptionalInfo.Age " +
				"from Employee x " +
				"join fetch x.OptionalInfo o " +
				"where o != null ").List();
		}

		[Test]
		public void ReuseJoinScalarSelectHql3()
		{
			using var session = OpenSession();
			session.CreateQuery(
				"select x.OptionalInfo.Age from Employee x " +
				"join fetch x.OptionalInfo " +
				"where x.OptionalInfo != null ").List();
		}

		[Test]
		public void ReuseJoinEntityAndScalarSelect()
		{
			using var session = OpenSession();
			using var sqlLog = new SqlLogSpy();

			var x = session.Query<Employee>()
				.Fetch(x => x.OptionalInfo)
				.Where(x => x.OptionalInfo != null)
				.Select(x => new { x, x.OptionalInfo.Age })
				.First();

			Assert.That(sqlLog.Appender.GetEvents().Length, Is.EqualTo(1));
			Assert.That(NHibernateUtil.IsInitialized(x.x.OptionalInfo), Is.True);
		}
	}
}
