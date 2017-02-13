using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3372
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return dialect is NHibernate.Dialect.MsSql2000Dialect;
		}

		[Test]
		public void CanGeneratePropertyOnInsertOfEntityWithCustomLoader()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var entity = new Entity { Content = "Some text" };
				session.Save(entity);
				session.Flush();

				Assert.That(entity.ShardId, Is.Not.Null & Has.Length.GreaterThan(0));
			}
		}

		[Test]
		public void CanGeneratePropertyOnUpdateOfEntityWithCustomLoader()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var entity = new Entity { Content = "Some text" };
				session.Save(entity);
				session.Flush();

				entity.ShardId = null;
				entity.Content = "Some other text";
				session.Update(entity);
				session.Flush();
				
				Assert.That(entity.ShardId, Is.Not.Null & Has.Length.GreaterThan(0));
			}
		}

	}
}