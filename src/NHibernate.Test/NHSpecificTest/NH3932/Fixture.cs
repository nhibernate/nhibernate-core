using System.Collections;
using NHibernate.Test.NHSpecificTest.NH3932.Model;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3932
{
	public abstract class Fixture : BugTestCase
	{
		private IParent storedParent;
		protected abstract bool CareAboutOrder { get; }

		[Test]
		public void ShouldKeepDirtyCollectionDirtyAfterMergingClone()
		{
			using (var s = OpenSession())
			{
				using (s.BeginTransaction())
				{
					s.Lock(storedParent, LockMode.None);
					storedParent.ClearChildren();
					Assert.IsTrue(s.IsDirty());

					var loadedClone = storedParent.Clone();
					s.Merge(loadedClone);
					Assert.IsTrue(s.IsDirty());
				}
			}
		}

		[Test]
		public void ShouldCareAboutOrder_MergeObjectNotInSession()
		{
			var parent = CreateParent(2);
			using (var s = OpenSession())
			{
				using (var tx = s.BeginTransaction())
				{
					s.Save(parent);
					tx.Commit();
				}
			}
			var parentClone = parent.Clone();
			parentClone.ReverseChildren();
			using (var s = OpenSession())
			{
				s.SessionFactory.Statistics.Clear();
				using (var tx = s.BeginTransaction())
				{
					s.Merge(parentClone);
					tx.Commit();
				}
				Assert.That(s.SessionFactory.Statistics.EntityUpdateCount, CareAboutOrder ? Is.EqualTo(1) : Is.EqualTo(0));
			}
		}

		[Test]
		public void ShouldCareAboutOrder_MergeObjectInSession()
		{
			var parent = CreateParent(2);
			using (var s = OpenSession())
			{
				using (var tx = s.BeginTransaction())
				{
					s.Save(parent);
					tx.Commit();
				}
			}
			var parentClone = parent.Clone();
			parentClone.ReverseChildren();
			using (var s = OpenSession())
			{
				s.SessionFactory.Statistics.Clear();
				using (var tx = s.BeginTransaction())
				{
					s.Lock(parent, LockMode.None);
					s.Merge(parentClone);
					tx.Commit();
				}
				Assert.That(s.SessionFactory.Statistics.EntityUpdateCount, CareAboutOrder ? Is.EqualTo(1) : Is.EqualTo(0));
			}
		}

		[Test]
		public void MergeCleanCloneShouldNotResultInUpdate()
		{
			var parentClone = storedParent.Clone();
			using (var s = OpenSession())
			{
				s.SessionFactory.Statistics.Clear();
				using (var tx = s.BeginTransaction())
				{
					s.Merge(parentClone);
					tx.Commit();
				}
				Assert.That(s.SessionFactory.Statistics.EntityUpdateCount, Is.EqualTo(0));
			}
		}

		[Test]
		public void MergeCleanCloneShouldNotMakeSessionDirty()
		{
			var parentClone = storedParent.Clone();
			using (var s = OpenSession())
			{
				s.SessionFactory.Statistics.Clear();
				using (var tx = s.BeginTransaction())
				{
					s.Merge(parentClone);
					Assert.That(s.IsDirty(), Is.False);
					tx.Commit();
				}
			}
		}

		[Test]
		public void MergeCloneOnNonCloneShouldNotResultInUpdate()
		{
			var parentClone = storedParent.Clone();
			using (var s = OpenSession())
			{
				s.SessionFactory.Statistics.Clear();
				using (var tx = s.BeginTransaction())
				{
					s.Lock(storedParent, LockMode.None);
					s.Merge(parentClone);
					tx.Commit();
				}
				Assert.That(s.SessionFactory.Statistics.EntityUpdateCount, Is.EqualTo(0));
			}
		}

		[Test]
		public void MergeCloneOnNonCloneShouldNotMakeSessionDirty()
		{
			var parentClone = storedParent.Clone();
			using (var s = OpenSession())
			{
				s.SessionFactory.Statistics.Clear();
				using (var tx = s.BeginTransaction())
				{
					s.Lock(storedParent, LockMode.None);
					s.Merge(parentClone);
					Assert.That(s.IsDirty(), Is.False);
					tx.Commit();
				}
			}
		}

		[Test]
		public void MoreElementsInTargetShouldBeTreatedAsDirty()
		{
			var parent = CreateParent(2);
			using (var s = OpenSession())
			{
				using (var tx = s.BeginTransaction())
				{
					s.Save(parent);
					tx.Commit();
				}
			}
			var parentClone = parent.Clone();
			parentClone.RemoveLastChild();
			using (var s = OpenSession())
			{
				s.SessionFactory.Statistics.Clear();
				using (var tx = s.BeginTransaction())
				{
					s.Merge(parentClone);
					tx.Commit();
				}
				Assert.That(s.SessionFactory.Statistics.EntityUpdateCount, Is.EqualTo(1));
			}
		}

		protected abstract IParent CreateParent(int numberOfChildren);

		protected override void OnSetUp()
		{
			storedParent = CreateParent(1);
			using (var s = OpenSession())
			{
				using (var tx = s.BeginTransaction())
				{
					s.Save(storedParent);
					tx.Commit();
				}
			}
		}

		protected override void OnTearDown()
		{
			using (var s = Sfi.OpenSession())
			{
				using (var tx = s.BeginTransaction())
				{
					s.Delete("from " + storedParent.GetType());
					tx.Commit();
				}
			}
		}

		protected override void Configure(Cfg.Configuration configuration)
		{
			configuration.SetProperty(Cfg.Environment.GenerateStatistics, "true");
		}

		protected override IList Mappings => new[]{"NHSpecificTest." + BugNumber + ".Model.Mappings.hbm.xml"};
	}
}