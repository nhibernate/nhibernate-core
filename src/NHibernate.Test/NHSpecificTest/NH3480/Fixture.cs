using System;
using System.Linq;
using NHibernate.Collection;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3480
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			using (ISession session = OpenSession())
			{
				using (ITransaction transaction = session.BeginTransaction())
				{
					var parent1 = new Entity
					{
						Id = new Entity.Key { Id = Guid.NewGuid() },
						Name = "Bob",
						OtherId = 20,
						YetAnotherOtherId = 21
					};
					parent1.Elements.Add(1);
					parent1.Elements.Add(2);
					session.Save(parent1);

					var child1 = new Child { Name = "Bob1", Parent = parent1 };
					session.Save(child1);

					var child2 = new Child { Name = "Bob2", Parent = parent1 };
					session.Save(child2);

					session.Flush();
					transaction.Commit();
				}
			}
		}

		protected override void OnTearDown()
		{
			using (ISession session = OpenSession())
			{
				using (ITransaction transaction = session.BeginTransaction())
				{
					session.Delete("from System.Object");

					session.Flush();
					transaction.Commit();
				}
			}
		}

		[Test]
		public void Test()
		{
			using (ISession session = OpenSession())
			{
				using (session.BeginTransaction())
				{
					var result = from e in session.Query<Entity>()
								 where e.Name == "Bob"
								 select e;
					var entity = result.Single();

					NHibernateUtil.Initialize(entity.Children);
					Assert.That(entity.Children, Has.Count.GreaterThan(0));
				}
			}
		}

		[Test]
		public void TestOwner()
		{
			using (var session = OpenSession())
			using (var t = session.BeginTransaction())
			{
				var entity = session.Query<Entity>().Single(e => e.Name == "Bob");

				// The Elements collection is mapped with a custom type which assert the owner
				// is not null.
				NHibernateUtil.Initialize(entity.Elements);
				Assert.That(entity.Elements, Has.Count.GreaterThan(0));
				t.Commit();
			}
		}
	}
}
