using System;
using NHibernate.Collection;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1323
{
	[Explicit("Demonstration of not viability")]
	public class CheckViability: BugTestCase
	{
		public class FullInitializedRetrievedEntity : IDisposable
		{
			private readonly ISessionFactory factory;
			private readonly MyClass entity;

			public FullInitializedRetrievedEntity(ISessionFactory factory)
			{
				this.factory = factory;
				object savedId;
				using (var session = factory.OpenSession())
				using (session.BeginTransaction())
				{
					var entity = new MyClass();
					entity.Children.Add(new MyChild { Parent = entity });
					entity.Components.Add(new MyComponent { Something = "something" });
					entity.Elements.Add("somethingelse");
					savedId = session.Save(entity);
					session.Transaction.Commit();
				}

				using (var session = factory.OpenSession())
				using (session.BeginTransaction())
				{
					entity = session.Get<MyClass>(savedId);
					NHibernateUtil.Initialize(entity.Children);
					NHibernateUtil.Initialize(entity.Components);
					NHibernateUtil.Initialize(entity.Elements);
					session.Transaction.Commit();
				}
			}

			public MyClass Entity
			{
				get { return entity; }
			}

			public void Dispose()
			{
				using (var s = factory.OpenSession())
				{
					s.Delete("from MyClass");
					s.Flush();
				}
			}
		}

		[Test]
		public void WhenReassociateCollectionUsingMergeThenReassingOwner()
		{
			using (var scenario = new FullInitializedRetrievedEntity(Sfi))
			{
				((IPersistentCollection)scenario.Entity.Children).Owner = null;
				((IPersistentCollection)scenario.Entity.Components).Owner = null;
				((IPersistentCollection)scenario.Entity.Elements).Owner = null;

				// When I reassociate the collections the Owner has value
				using (var session = OpenSession())
				using (session.BeginTransaction())
				{
					var merged = (MyClass)session.Merge(scenario.Entity);
					Assert.That(((IPersistentCollection)merged.Children).Owner, Is.Not.Null);
					Assert.That(((IPersistentCollection)merged.Components).Owner, Is.Not.Null);
					Assert.That(((IPersistentCollection)merged.Elements).Owner, Is.Not.Null);
					session.Transaction.Commit();
				}
			}
		}

		[Test]
		public void WhenReassociateCollectionUsingLockThenTheCommitNotThrows()
		{
			using (var scenario = new FullInitializedRetrievedEntity(Sfi))
			{
				((IPersistentCollection)scenario.Entity.Children).Owner = null;
				((IPersistentCollection)scenario.Entity.Components).Owner = null;
				((IPersistentCollection)scenario.Entity.Elements).Owner = null;

				using (var session = OpenSession())
				using (session.BeginTransaction())
				{
					// When I reassociate the collections the Owner is null
					session.Lock(scenario.Entity, LockMode.None);
					// If I change something in each collection, there is no problems
					scenario.Entity.Children.Add(new MyChild { Parent = scenario.Entity });
					scenario.Entity.Components.Add(new MyComponent { Something = "something" });
					scenario.Entity.Elements.Add("somethingelse");
					session.Transaction.Commit();
				}

				using (var session = OpenSession())
				using (session.BeginTransaction())
				{
					var fresh = session.Get<MyClass>(scenario.Entity.Id);
					Assert.That(fresh.Children, Has.Count.EqualTo(2));
					Assert.That(fresh.Components, Has.Count.EqualTo(2));
					Assert.That(fresh.Elements, Has.Count.EqualTo(2));
					session.Transaction.Commit();
				}
			}
		}

		[Test]
		public void WhenReassociateCollectionUsingUpdateThenTheCommitNotThrows()
		{
			using (var scenario = new FullInitializedRetrievedEntity(Sfi))
			{
				((IPersistentCollection)scenario.Entity.Children).Owner = null;
				((IPersistentCollection)scenario.Entity.Components).Owner = null;
				((IPersistentCollection)scenario.Entity.Elements).Owner = null;

				using (var session = OpenSession())
				using (session.BeginTransaction())
				{
					scenario.Entity.Children.Add(new MyChild { Parent = scenario.Entity });
					scenario.Entity.Components.Add(new MyComponent { Something = "something" });
					scenario.Entity.Elements.Add("somethingelse");
					// When I reassociate the collections the Owner is null
					session.Update(scenario.Entity);
					session.Transaction.Commit();
				}

				using (var session = OpenSession())
				using (session.BeginTransaction())
				{
					var fresh = session.Get<MyClass>(scenario.Entity.Id);
					Assert.That(fresh.Children, Has.Count.EqualTo(2));
					Assert.That(fresh.Components, Has.Count.EqualTo(2));
					Assert.That(fresh.Elements, Has.Count.EqualTo(2));
					session.Transaction.Commit();
				}
			}
		}

		[Test]
		public void WhenReassociateCollectionUsingSaveOrUpdateThenTheCommitNotThrows()
		{
			using (var scenario = new FullInitializedRetrievedEntity(Sfi))
			{
				((IPersistentCollection)scenario.Entity.Children).Owner = null;
				((IPersistentCollection)scenario.Entity.Components).Owner = null;
				((IPersistentCollection)scenario.Entity.Elements).Owner = null;

				using (var session = OpenSession())
				using (session.BeginTransaction())
				{
					scenario.Entity.Children.Add(new MyChild { Parent = scenario.Entity });
					scenario.Entity.Components.Add(new MyComponent { Something = "something" });
					scenario.Entity.Elements.Add("somethingelse");
					// When I reassociate the collections the Owner is null
					session.SaveOrUpdate(scenario.Entity);
					session.Transaction.Commit();
				}

				using (var session = OpenSession())
				using (session.BeginTransaction())
				{
					var fresh = session.Get<MyClass>(scenario.Entity.Id);
					Assert.That(fresh.Children, Has.Count.EqualTo(2));
					Assert.That(fresh.Components, Has.Count.EqualTo(2));
					Assert.That(fresh.Elements, Has.Count.EqualTo(2));
					session.Transaction.Commit();
				}
			}
		}

		[Test]
		public void WhenReassociateCollectionUsingDeleteThenTheCommitNotThrows()
		{
			using (var scenario = new FullInitializedRetrievedEntity(Sfi))
			{
				((IPersistentCollection)scenario.Entity.Children).Owner = null;
				((IPersistentCollection)scenario.Entity.Components).Owner = null;
				((IPersistentCollection)scenario.Entity.Elements).Owner = null;

				using (var session = OpenSession())
				using (session.BeginTransaction())
				{
					session.Delete(scenario.Entity);
					session.Transaction.Commit();
				}

				using (var session = OpenSession())
				using (session.BeginTransaction())
				{
					var fresh = session.Get<MyClass>(scenario.Entity.Id);
					Assert.That(fresh, Is.Null);
					session.Transaction.Commit();
				}
			}
		}
	}
}