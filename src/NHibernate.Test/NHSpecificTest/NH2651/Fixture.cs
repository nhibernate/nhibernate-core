using NHibernate.Cfg;
using NHibernate.Criterion;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2651
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			base.OnSetUp();
			using (ISession session = this.OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				var entity = new Model { Id = 1, SampleData = 1 };
				session.Save(entity);

				var entity2 = new Model { Id = 2, SampleData = 2 };
				session.Save(entity2);

				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			base.OnTearDown();
			using (ISession session = this.OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				session.Delete("from System.Object");
				transaction.Commit();
			}
		}

		[Test]
		public void TestConditionalProjectionWithConstantAndLikeExpression()
		{
			// Fails on Firebird since it's unable to determine the type of the
			// case expression from the parameters. See http://tracker.firebirdsql.org/browse/CORE-1821.
			// I don't want to mess up the test with cast statements that the DB really shouldn't need
			// (or the NHibernate dialect should add them just when needed).

			using (ISession session = this.OpenSession())
			{
				var projection = (Projections.Conditional(Restrictions.Eq("SampleData", 1),
														  Projections.Constant("Foo"),
														  Projections.Constant("Bar")));

				var likeExpression = new NHibernate.Criterion.LikeExpression(projection, "B", MatchMode.Start);
				var criteria1 = session.CreateCriteria<Model>()
					.Add(Restrictions.Eq("Id", 1))
					.Add(likeExpression);

				var result1 = criteria1.UniqueResult<Model>();

				var criteria2 = session.CreateCriteria<Model>()
					.Add(Restrictions.Eq("Id", 2))
					.Add(likeExpression);

				var result2 = criteria2.UniqueResult<Model>();

				Assert.IsNull(result1);
				Assert.IsNotNull(result2);
			}
		}
	}
}
