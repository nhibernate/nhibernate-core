using System.Linq;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH3288
{
	[TestFixture]
	public class FetchAndCollectionJoinFixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			using var session = OpenSession();
			using var transaction = session.BeginTransaction();
			var middleEntity = new MiddleEntity();
			middleEntity.Components.Add(new Component { MiddleEntity = middleEntity, Value = 1 });
			var te = new TopEntity
			{
				MiddleEntity = middleEntity
			};
			session.Save(middleEntity);
			session.Save(te);

			transaction.Commit();
		}

		protected override void OnTearDown()
		{
			using var session = OpenSession();
			using var transaction = session.BeginTransaction();
			session.Delete("from System.Object");

			transaction.Commit();
		}

		[Test]
		public void ReuseEntityJoinWithCollectionJoin()
		{
			using var session = OpenSession();

			var entities = session.Query<TopEntity>()
				.Fetch(e => e.MiddleEntity)
				.Where(e => e.MiddleEntity.Components.Any(e => e.Value != 0))
				.ToList();
			Assert.That(entities.Count, Is.EqualTo(1));
		}
	}
}
