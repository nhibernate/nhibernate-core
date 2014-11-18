using System;
using NHibernate.Cfg;
using NUnit.Framework;
using NHibernate.Cfg.Loquacious;

namespace NHibernate.Test.NHSpecificTest.NH2228
{
	public class Fixture : BugTestCase
	{
		public class ParentWithTwoChildrenScenario : IDisposable
		{
			private readonly ISessionFactory factory;
			private readonly int parentId;

			public ParentWithTwoChildrenScenario(ISessionFactory factory)
			{
				this.factory = factory;
				var parent = new Parent();
				parent.Children.Add(new Child {Description = "Child1", Parent = parent});
				parent.Children.Add(new Child { Description = "Child2", Parent = parent });
				using (var s = factory.OpenSession())
				{
					parentId = (int)s.Save(parent);
					s.Flush();
				}
			}

			public int ParentId
			{
				get { return parentId; }
			}

			public void Dispose()
			{
				using (var s = factory.OpenSession())
				{
					s.Delete("from Parent");
					s.Flush();
				}
			}
		}

		protected override void Configure(NHibernate.Cfg.Configuration configuration)
		{
			// needed to be sure of StaleStateException that the user has reported in the issue
			configuration.DataBaseIntegration(x => x.BatchSize = 1);
		}

		[Test]
		public void WhenStaleObjectStateThenMessageContainsEntity()
		{
			using (var scenario = new ParentWithTwoChildrenScenario(Sfi))
			{
				using (var client1 = OpenSession())
				{
					var parentFromClient1 = client1.Get<Parent>(scenario.ParentId);
					NHibernateUtil.Initialize(parentFromClient1.Children);
					var firstChildId = parentFromClient1.Children[0].Id;

					DeleteChildUsingAnotherSession(firstChildId);

					using (var tx1 = client1.BeginTransaction())
					{
						parentFromClient1.Children[0].Description = "Modified info";
						var expectedException = Assert.Throws<StaleObjectStateException>(() => tx1.Commit());
						Assert.That(expectedException.EntityName, Is.EqualTo(typeof (Child).FullName));
						Assert.That(expectedException.Identifier, Is.EqualTo(firstChildId));
					}
				}
			}
		}

		private void DeleteChildUsingAnotherSession(int childIdToDelete)
		{
			using (var client2 = Sfi.OpenStatelessSession())
			using (var tx2 = client2.BeginTransaction())
			{
				client2.CreateQuery("delete from Child c where c.Id = :pChildId").SetInt32("pChildId", childIdToDelete).ExecuteUpdate();
				tx2.Commit();
			}
		}
	}
}