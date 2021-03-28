using System.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH2465
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return dialect.SupportsScalarSubSelects;
		}

		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var applicant = new Entity {IdentityNames = {"name1", "name2"}};
				session.Save(applicant);

				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.Delete("from System.Object");
				transaction.Commit();
			}
		}

		[Test]
		public void ContainsInsideValueCollection()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var identityNames = new[] {"name1", "x"};
				session
					.Query<Entity>()
					.Where(a => a.IdentityNames.Any(n => identityNames.Contains(n)))
					.ToList();
				session
					.Query<Entity>()
					.Where(a => a.IdentityNames.All(n => identityNames.Contains(n)))
					.ToList();
				session
					.Query<Entity>()
					.Where(a => a.IdentityNames.FirstOrDefault(n => identityNames.Contains(n)) == "test")
					.ToList();

				transaction.Commit();
			}
		}

		[Test]
		public void EqualsInsideValueCollection()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var value = "test";
				session
					.Query<Entity>()
					.Where(a => a.IdentityNames.Any(n => n == value))
					.ToList();
				session
					.Query<Entity>()
					.Where(a => a.IdentityNames.Any(n => (string) n == value))
					.ToList();
				session
					.Query<Entity>()
					.Where(a => a.IdentityNames.All(n => n == value))
					.ToList();
				session
					.Query<Entity>()
					.Where(a => a.IdentityNames.FirstOrDefault(n => n == "test") == "test")
					.ToList();

				transaction.Commit();
			}
		}
	}
}
