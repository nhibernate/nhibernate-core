using System.Linq;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH3263
{
	[TestFixture]
	public class ReuseFetchJoinFixture : BugTestCase
	{
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
			session.Query<Employee>()
				.Fetch(x => x.OptionalInfo)
				.Where(x => x.OptionalInfo != null)
				.Select(x => new { x, x.OptionalInfo.Age })
				.ToList();
		}
	}
}
