using NHibernate.AdoNet;
using NHibernate.Cfg;
using NHibernate.Cfg.Loquacious;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2527
{
	public class FixtureWithNoBatcher : BugTestCase
	{

		protected override void Configure(Cfg.Configuration configuration)
		{
			configuration.DataBaseIntegration(x =>
																				{
																					x.BatchSize = 0;
																					x.Batcher<NonBatchingBatcherFactory>();
																				});
		}

		protected override void OnSetUp()
		{
			base.OnSetUp();

			using (ISession session = OpenSession())
			{
				using (ITransaction tx = session.BeginTransaction())
				{
					var parent = new Parent();
					var childOne = new Child();
					parent.Childs.Add(childOne);
					session.Save(parent);

					tx.Commit();
				}
			}
		}

		protected override void OnTearDown()
		{
			base.OnTearDown();

			using (ISession session = OpenSession())
			{
				using (ITransaction tx = session.BeginTransaction())
				{
					session.Delete("from Child");
					session.Delete("from Parent");
					tx.Commit();
				}
			}
		}

		[Test]
		public void DisposedCommandShouldNotBeReused()
		{
			using (ISession session = OpenSession())
			{
				using (ITransaction tx = session.BeginTransaction())
				{
					var parent = session.CreateCriteria<Parent>().UniqueResult<Parent>();

					Child childOne = parent.Childs[0];

					var childTwo = new Child();
					parent.Childs.Add(childTwo);

					Child childToMove = parent.Childs[1];
					parent.Childs.RemoveAt(1);
					parent.Childs.Insert(0, childToMove);

					Assert.DoesNotThrow(() => { tx.Commit(); });

					Assert.AreEqual(childTwo.Id, parent.Childs[0].Id);
					Assert.AreEqual(childOne.Id, parent.Childs[1].Id);
				}
			}
		}
	}
}