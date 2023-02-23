using System.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH0829
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			using var session = OpenSession();
			using var transaction = session.BeginTransaction();

			var e1 = new Parent { Type = TestEnum.A | TestEnum.C };
			session.Save(e1);

			var e2 = new Child { Type = TestEnum.D, Parent = e1 };
			session.Save(e2);

			var e3 = new Child { Type = TestEnum.C, Parent = e1 };
			session.Save(e3);

			session.Flush();
			transaction.Commit();
		}

		[Test]
		public void SelectClass()
		{
			using var session = OpenSession();
			using var transaction = session.BeginTransaction();

			var resultFound = session.Query<Parent>().Where(x => x.Type.HasFlag(TestEnum.A)).FirstOrDefault();

			var resultNotFound = session.Query<Parent>().Where(x => x.Type.HasFlag(TestEnum.D)).FirstOrDefault();

			Assert.That(resultFound, Is.Not.Null);
			Assert.That(resultNotFound, Is.Null);
		}

		[Test]
		public void SelectChildClassContainedInParent()
		{
			using var session = OpenSession();
			using var transaction = session.BeginTransaction();

			var result = session.Query<Child>().Where(x => x.Parent.Type.HasFlag(x.Type)).FirstOrDefault();

			Assert.That(result, Is.Not.Null);
		}

		protected override void OnTearDown()
		{
			base.OnTearDown();
			using var session = OpenSession();
			foreach (var entity in new[] { nameof(Child), nameof(Parent) })
			{
				session.Delete($"from {entity}");
				session.Flush();
			}
		}
	}
}
