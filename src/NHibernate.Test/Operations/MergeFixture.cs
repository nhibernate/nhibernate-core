using System.Collections.Generic;
using NHibernate.Criterion;
using NUnit.Framework;

namespace NHibernate.Test.Operations
{
	[TestFixture]
	public class MergeFixture : AbstractOperationTestCase
	{
		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return !(dialect is Dialect.FirebirdDialect); // Firebird has no CommandTimeout, and locks up during the tear-down of this fixture
		}

		protected override void OnTearDown()
		{
			Cleanup();
		}

		private void Cleanup()
		{
			using (ISession s = OpenSession())
			{
				using (ITransaction tx = s.BeginTransaction())
				{
					s.Delete("from NumberedNode where parent is not null");
					s.Delete("from NumberedNode");

					s.Delete("from Node where parent is not null");
					s.Delete("from Node");

					s.Delete("from VersionedEntity where parent is not null");
					s.Delete("from VersionedEntity");
					s.Delete("from TimestampedEntity");

					s.Delete("from Competitor");
					s.Delete("from Competition");

					s.Delete("from Employer");

					tx.Commit();
				}
			}
		}

		[Test]
		public void DeleteAndMerge()
		{
			using (ISession s = OpenSession())
			{
				s.BeginTransaction();
				var jboss = new Employer();
				s.Persist(jboss);
				s.Transaction.Commit();
				s.Clear();

				s.BeginTransaction();
				var otherJboss = s.Get<Employer>(jboss.Id);
				s.Delete(otherJboss);
				s.Transaction.Commit();
				s.Clear();
				jboss.Vers = 1;
				s.BeginTransaction();
				s.Merge(jboss);
				s.Transaction.Commit();
			}
		}

		[Test]
		public void MergeBidiForeignKeyOneToOne()
		{
			Person p;
			Address a;
			using (ISession s = OpenSession())
			{
				using (ITransaction tx = s.BeginTransaction())
				{
					p = new Person {Name = "steve"};
					a = new Address {StreetAddress = "123 Main", City = "Austin", Country = "US", Resident = p};
					s.Persist(a);
					s.Persist(p);
					tx.Commit();
				}
			}

			ClearCounts();

			p.Address.StreetAddress = "321 Main";

			using (ISession s = OpenSession())
			{
				using (s.BeginTransaction())
				{
					p = (Person) s.Merge(p);
					s.Transaction.Commit();
				}
			}

			AssertInsertCount(0);
			AssertUpdateCount(0); // no cascade
			AssertDeleteCount(0);

			using (ISession s = OpenSession())
			{
				using (ITransaction tx = s.BeginTransaction())
				{
					s.Delete(a);
					s.Delete(p);
					tx.Commit();
				}
			}
		}

		[Test, Ignore("Need some more investigation about id sync.")]
		public void MergeBidiPrimayKeyOneToOne()
		{
			Person p;
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				p = new Person {Name = "steve"};
				new PersonalDetails {SomePersonalDetail = "I have big feet", Person = p};
				s.Persist(p);
				tx.Commit();
			}

			ClearCounts();

			p.Details.SomePersonalDetail = p.Details.SomePersonalDetail + " and big hands too";
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				p = (Person) s.Merge(p);
				tx.Commit();
			}

			AssertInsertCount(0);
			AssertUpdateCount(1);
			AssertDeleteCount(0);

			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				s.Delete(p);
				tx.Commit();
			}
		}

		[Test]
		public void MergeDeepTree()
		{
			ClearCounts();

			ISession s = OpenSession();
			ITransaction tx = s.BeginTransaction();
			var root = new Node {Name = "root"};
			var child = new Node {Name = "child"};
			var grandchild = new Node {Name = "grandchild"};
			root.AddChild(child);
			child.AddChild(grandchild);
			s.Merge(root);
			tx.Commit();
			s.Close();

			AssertInsertCount(3);
			AssertUpdateCount(0);
			ClearCounts();

			grandchild.Description = "the grand child";
			var grandchild2 = new Node {Name = "grandchild2"};
			child.AddChild(grandchild2);

			s = OpenSession();
			tx = s.BeginTransaction();
			s.Merge(root);
			tx.Commit();
			s.Close();

			AssertInsertCount(1);
			AssertUpdateCount(1);
			ClearCounts();

			var child2 = new Node {Name = "child2"};
			var grandchild3 = new Node {Name = "grandchild3"};
			child2.AddChild(grandchild3);
			root.AddChild(child2);

			s = OpenSession();
			tx = s.BeginTransaction();
			s.Merge(root);
			tx.Commit();
			s.Close();

			AssertInsertCount(2);
			AssertUpdateCount(0);
			ClearCounts();

			s = OpenSession();
			tx = s.BeginTransaction();
			s.Delete(grandchild);
			s.Delete(grandchild2);
			s.Delete(grandchild3);
			s.Delete(child);
			s.Delete(child2);
			s.Delete(root);
			tx.Commit();
			s.Close();
		}

		[Test]
		public void MergeDeepTreeWithGeneratedId()
		{
			ClearCounts();

			NumberedNode root;
			NumberedNode child;
			NumberedNode grandchild;
			using (ISession s = OpenSession())
			{
				ITransaction tx = s.BeginTransaction();
				root = new NumberedNode("root");
				child = new NumberedNode("child");
				grandchild = new NumberedNode("grandchild");
				root.AddChild(child);
				child.AddChild(grandchild);
				root = (NumberedNode) s.Merge(root);
				tx.Commit();
			}

			AssertInsertCount(3);
			AssertUpdateCount(0);
			ClearCounts();

			IEnumerator<NumberedNode> rit = root.Children.GetEnumerator();
			rit.MoveNext();
			child = rit.Current;
			IEnumerator<NumberedNode> cit = child.Children.GetEnumerator();
			cit.MoveNext();
			grandchild = cit.Current;
			grandchild.Description = "the grand child";
			var grandchild2 = new NumberedNode("grandchild2");
			child.AddChild(grandchild2);

			using (ISession s = OpenSession())
			{
				ITransaction tx = s.BeginTransaction();
				root = (NumberedNode) s.Merge(root);
				tx.Commit();
			}

			AssertInsertCount(1);
			AssertUpdateCount(1);
			ClearCounts();

			sessions.Evict(typeof (NumberedNode));

			var child2 = new NumberedNode("child2");
			var grandchild3 = new NumberedNode("grandchild3");
			child2.AddChild(grandchild3);
			root.AddChild(child2);

			using (ISession s = OpenSession())
			{
				ITransaction tx = s.BeginTransaction();
				root = (NumberedNode) s.Merge(root);
				tx.Commit();
			}

			AssertInsertCount(2);
			AssertUpdateCount(0);
			ClearCounts();

			using (ISession s = OpenSession())
			{
				ITransaction tx = s.BeginTransaction();
				s.Delete("from NumberedNode where name like 'grand%'");
				s.Delete("from NumberedNode where name like 'child%'");
				s.Delete("from NumberedNode");
				tx.Commit();
			}
		}

		[Test]
		public void MergeManaged()
		{
			ISession s = OpenSession();
			ITransaction tx = s.BeginTransaction();
			var root = new NumberedNode("root");
			s.Persist(root);
			tx.Commit();

			ClearCounts();

			tx = s.BeginTransaction();
			var child = new NumberedNode("child");
			root.AddChild(child);
			Assert.That(s.Merge(root), Is.SameAs(root));
			IEnumerator<NumberedNode> rit = root.Children.GetEnumerator();
			rit.MoveNext();
			NumberedNode mergedChild = rit.Current;
			Assert.That(mergedChild, Is.Not.SameAs(child));
			Assert.That(s.Contains(mergedChild));
			Assert.That(! s.Contains(child));
			Assert.That(root.Children.Count, Is.EqualTo(1));
			Assert.That(root.Children.Contains(mergedChild));
			//assertNotSame( mergedChild, s.Merge(child) ); //yucky :(
			tx.Commit();

			AssertInsertCount(1);
			AssertUpdateCount(0);

			Assert.That(root.Children.Count, Is.EqualTo(1));
			Assert.That(root.Children.Contains(mergedChild));

			tx = s.BeginTransaction();
			Assert.That(s.CreateCriteria(typeof (NumberedNode)).SetProjection(Projections.RowCount()).UniqueResult(),
			            Is.EqualTo(2));
			tx.Rollback();
			s.Close();
		}

		[Test]
		public void MergeManyToManyWithCollectionDeference()
		{
			// setup base data...
			ISession s = OpenSession();
			ITransaction tx = s.BeginTransaction();
			var competition = new Competition();
			competition.Competitors.Add(new Competitor {Name = "Name"});
			competition.Competitors.Add(new Competitor());
			competition.Competitors.Add(new Competitor());
			s.Persist(competition);
			tx.Commit();
			s.Close();

			// the competition graph is now detached:
			//   1) create a new List reference to represent the competitors
			s = OpenSession();
			tx = s.BeginTransaction();
			var newComp = new List<Competitor>();
			Competitor originalCompetitor = competition.Competitors[0];
			originalCompetitor.Name = "Name2";
			newComp.Add(originalCompetitor);
			newComp.Add(new Competitor());
			//   2) set that new List reference unto the Competition reference
			competition.Competitors = newComp;
			//   3) attempt the merge
			var competition2 = (Competition) s.Merge(competition);
			tx.Commit();
			s.Close();

			Assert.That(!(competition == competition2));
			Assert.That(!(competition.Competitors == competition2.Competitors));
			Assert.That(competition2.Competitors.Count, Is.EqualTo(2));

			s = OpenSession();
			tx = s.BeginTransaction();
			competition = s.Get<Competition>(competition.Id);
			Assert.That(competition.Competitors.Count, Is.EqualTo(2));
			s.Delete(competition);
			tx.Commit();
			s.Close();
		}

		[Test]
		public void MergeStaleVersionFails()
		{
			ISession s = OpenSession();
			s.BeginTransaction();
			var entity = new VersionedEntity {Id = "entity", Name = "entity"};
			s.Persist(entity);
			s.Transaction.Commit();
			s.Close();

			// make the detached 'entity' reference stale...
			s = OpenSession();
			s.BeginTransaction();
			var entity2 = s.Get<VersionedEntity>(entity.Id);
			entity2.Name = "entity-name";
			s.Transaction.Commit();
			s.Close();

			// now try to reattch it
			s = OpenSession();
			s.BeginTransaction();
			try
			{
				s.Merge(entity);
				s.Transaction.Commit();
				Assert.Fail("was expecting staleness error");
			}
			catch (StaleObjectStateException)
			{
				// expected outcome...
			}
			finally
			{
				s.Transaction.Rollback();
				s.Close();
				Cleanup();
			}
		}

		[Test]
		public void MergeTree()
		{
			ClearCounts();

			ISession s = OpenSession();
			ITransaction tx = s.BeginTransaction();
			var root = new Node {Name = "root"};
			var child = new Node {Name = "child"};
			root.AddChild(child);
			s.Persist(root);
			tx.Commit();
			s.Close();

			AssertInsertCount(2);
			ClearCounts();

			root.Description = "The root node";
			child.Description = "The child node";

			var secondChild = new Node {Name = "second child"};

			root.AddChild(secondChild);

			s = OpenSession();
			tx = s.BeginTransaction();
			s.Merge(root);
			tx.Commit();
			s.Close();

			AssertInsertCount(1);
			AssertUpdateCount(2);
		}

		[Test]
		public void MergeTreeWithGeneratedId()
		{
			ClearCounts();

			ISession s = OpenSession();
			ITransaction tx = s.BeginTransaction();
			var root = new NumberedNode("root");
			var child = new NumberedNode("child");
			root.AddChild(child);
			s.Persist(root);
			tx.Commit();
			s.Close();

			AssertInsertCount(2);
			ClearCounts();

			root.Description = "The root node";
			child.Description = "The child node";

			var secondChild = new NumberedNode("second child");

			root.AddChild(secondChild);

			s = OpenSession();
			tx = s.BeginTransaction();
			s.Merge(root);
			tx.Commit();
			s.Close();

			AssertInsertCount(1);
			AssertUpdateCount(2);
		}

		[Test]
		public void NoExtraUpdatesOnMerge()
		{
			ISession s = OpenSession();
			s.BeginTransaction();
			var node = new Node {Name = "test"};
			s.Persist(node);
			s.Transaction.Commit();
			s.Close();

			ClearCounts();

			// node is now detached, but we have made no changes.  so attempt to merge it
			// into this new session; this should cause no updates...
			s = OpenSession();
			s.BeginTransaction();
			node = (Node) s.Merge(node);
			s.Transaction.Commit();
			s.Close();

			AssertUpdateCount(0);
			AssertInsertCount(0);

			///////////////////////////////////////////////////////////////////////
			// as a control measure, now update the node while it is detached and
			// make sure we get an update as a result...
			node.Description = "new description";
			s = OpenSession();
			s.BeginTransaction();
			node = (Node) s.Merge(node);
			s.Transaction.Commit();
			s.Close();
			AssertUpdateCount(1);
			AssertInsertCount(0);
			///////////////////////////////////////////////////////////////////////
		}

		[Test]
		public void NoExtraUpdatesOnMergeVersioned()
		{
			ISession s = OpenSession();
			s.BeginTransaction();
			var entity = new VersionedEntity {Id = "entity", Name = "entity"};
			s.Persist(entity);
			s.Transaction.Commit();
			s.Close();

			ClearCounts();

			// entity is now detached, but we have made no changes.  so attempt to merge it
			// into this new session; this should cause no updates...
			s = OpenSession();
			s.BeginTransaction();
			var mergedEntity = (VersionedEntity) s.Merge(entity);
			s.Transaction.Commit();
			s.Close();

			AssertUpdateCount(0);
			AssertInsertCount(0);
			Assert.That(entity.Version, Is.EqualTo(mergedEntity.Version), "unexpected version increment");

			///////////////////////////////////////////////////////////////////////
			// as a control measure, now update the node while it is detached and
			// make sure we get an update as a result...
			entity.Name = "new name";
			s = OpenSession();
			s.BeginTransaction();
			entity = (VersionedEntity) s.Merge(entity);
			s.Transaction.Commit();
			s.Close();
			AssertUpdateCount(1);
			AssertInsertCount(0);
			///////////////////////////////////////////////////////////////////////
		}

		[Test]
		public void NoExtraUpdatesOnMergeVersionedWithCollection()
		{
			ISession s = OpenSession();
			s.BeginTransaction();
			var parent = new VersionedEntity {Id = "parent", Name = "parent"};
			var child = new VersionedEntity {Id = "child", Name = "child"};
			parent.Children.Add(child);
			child.Parent = parent;
			s.Persist(parent);
			s.Transaction.Commit();
			s.Close();

			ClearCounts();

			// parent is now detached, but we have made no changes.  so attempt to merge it
			// into this new session; this should cause no updates...
			s = OpenSession();
			s.BeginTransaction();
			var mergedParent = (VersionedEntity) s.Merge(parent);
			s.Transaction.Commit();
			s.Close();

			AssertUpdateCount(0);
			AssertInsertCount(0);
			Assert.That(parent.Version, Is.EqualTo(mergedParent.Version), "unexpected parent version increment");
			IEnumerator<VersionedEntity> it = mergedParent.Children.GetEnumerator();
			it.MoveNext();
			VersionedEntity mergedChild = it.Current;
			Assert.That(child.Version, Is.EqualTo(mergedChild.Version), "unexpected child version increment");

			///////////////////////////////////////////////////////////////////////
			// as a control measure, now update the node while it is detached and
			// make sure we get an update as a result...
			mergedParent.Name = "new name";
			mergedParent.Children.Add(new VersionedEntity {Id = "child2", Name = "new child"});
			s = OpenSession();
			s.BeginTransaction();
			parent = (VersionedEntity) s.Merge(mergedParent);
			s.Transaction.Commit();
			s.Close();
			AssertUpdateCount(1);
			AssertInsertCount(1);
			///////////////////////////////////////////////////////////////////////
		}

		[Test]
		public void NoExtraUpdatesOnMergeWithCollection()
		{
			ISession s = OpenSession();
			s.BeginTransaction();
			var parent = new Node {Name = "parent"};
			var child = new Node {Name = "child"};
			parent.Children.Add(child);
			child.Parent = parent;
			s.Persist(parent);
			s.Transaction.Commit();
			s.Close();

			ClearCounts();

			// parent is now detached, but we have made no changes.  so attempt to merge it
			// into this new session; this should cause no updates...
			s = OpenSession();
			s.BeginTransaction();
			parent = (Node) s.Merge(parent);
			s.Transaction.Commit();
			s.Close();

			AssertUpdateCount(0);
			AssertInsertCount(0);

			///////////////////////////////////////////////////////////////////////
			// as a control measure, now update the node while it is detached and
			// make sure we get an update as a result...
			IEnumerator<Node> it = parent.Children.GetEnumerator();
			it.MoveNext();
			it.Current.Description = "child's new description";
			parent.Children.Add(new Node {Name = "second child"});
			s = OpenSession();
			s.BeginTransaction();
			parent = (Node) s.Merge(parent);
			s.Transaction.Commit();
			s.Close();
			AssertUpdateCount(1);
			AssertInsertCount(1);
			///////////////////////////////////////////////////////////////////////
		}

		[Test]
		public void PersistThenMergeInSameTxnWithTimestamp()
		{
			ISession s = OpenSession();
			ITransaction tx = s.BeginTransaction();
			var entity = new TimestampedEntity {Id = "test", Name = "test"};
			s.Persist(entity);
			s.Merge(new TimestampedEntity {Id = "test", Name = "test-2"});

			try
			{
				// control operation...
				s.SaveOrUpdate(new TimestampedEntity {Id = "test", Name = "test-3"});
				Assert.Fail("saveOrUpdate() should fail here");
			}
			catch (NonUniqueObjectException)
			{
				// expected behavior
			}

			tx.Commit();
			s.Close();
		}

		[Test]
		public void PersistThenMergeInSameTxnWithVersion()
		{
			ISession s = OpenSession();
			ITransaction tx = s.BeginTransaction();
			var entity = new VersionedEntity {Id = "test", Name = "test"};
			s.Persist(entity);
			s.Merge(new VersionedEntity {Id = "test", Name = "test-2"});

			try
			{
				// control operation...
				s.SaveOrUpdate(new VersionedEntity {Id = "test", Name = "test-3"});
				Assert.Fail("saveOrUpdate() should fail here");
			}
			catch (NonUniqueObjectException)
			{
				// expected behavior
			}

			tx.Commit();
			s.Close();
		}

		[Test]
		public void RecursiveMergeTransient()
		{
			using (ISession s = OpenSession())
			{
				using (ITransaction tx = s.BeginTransaction())
				{
					var jboss = new Employer();
					var gavin = new Employee();
					jboss.Employees = new List<Employee> {gavin};
					s.Merge(jboss);
					s.Flush();
					jboss = s.CreateQuery("from Employer e join fetch e.Employees").UniqueResult<Employer>();
					Assert.That(NHibernateUtil.IsInitialized(jboss.Employees));
					Assert.That(jboss.Employees.Count, Is.EqualTo(1));
					s.Clear();
					IEnumerator<Employee> it = jboss.Employees.GetEnumerator();
					it.MoveNext();

					s.Merge(it.Current);
					tx.Commit();
				}
			}
		}
	}
}