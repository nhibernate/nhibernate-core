using NHibernate.Cfg;
using NUnit.Framework;
using Environment = NHibernate.Cfg.Environment;

namespace NHibernate.Test.NHSpecificTest.GH1530
{
	[TestFixture]
	public abstract class FixtureBase : BugTestCase
	{
		private Parent _parent;

		protected override void OnSetUp()
		{
			using (var s = OpenSession())
			{
				using (var tx = s.BeginTransaction())
				{
					_parent = new Parent();
					_parent.AddChild(new Child());
					s.Save(_parent);
					tx.Commit();
				}
			}
		}

		[Test]
		public virtual void OneNewChildShouldOnlyCreateInsertWithNoUpdate_LockAndMerge()
		{
			var parentClone = _parent.MakeCopy();
			parentClone.AddChild(new Child());
			using (var s = OpenSession())
			{
				using (var tx = s.BeginTransaction())
				{
					s.Lock(_parent, LockMode.None);
					s.Merge(parentClone);
					s.SessionFactory.Statistics.Clear();
					tx.Commit();
				}

				Assert.That(s.SessionFactory.Statistics.PrepareStatementCount, Is.EqualTo(1));
			}
		}

		[Test]
		public virtual void OneNewChildShouldOnlyCreateInsertWithNoUpdate_Merge()
		{
			var parentClone = _parent.MakeCopy();
			parentClone.AddChild(new Child());
			using (var s = OpenSession())
			{
				using (var tx = s.BeginTransaction())
				{
					s.Merge(parentClone);
					s.SessionFactory.Statistics.Clear();
					tx.Commit();
				}

				Assert.That(s.SessionFactory.Statistics.PrepareStatementCount, Is.EqualTo(1));
			}
		}

		protected override void OnTearDown()
		{
			using (var s = OpenSession())
			{
				using (var tx = s.BeginTransaction())
				{
					s.Delete(s.Load<Parent>(_parent.Id));
					tx.Commit();
				}
			}
		}

		protected override void Configure(Configuration configuration)
		{
			configuration.SetProperty(Environment.GenerateStatistics, "true");
		}
	}

	[TestFixture]
	public class Fixture : FixtureBase
	{
		[Test]
		[KnownBug("#1530")]
		public override void OneNewChildShouldOnlyCreateInsertWithNoUpdate_Merge()
		{
			base.OneNewChildShouldOnlyCreateInsertWithNoUpdate_Merge();
		}

		[Test]
		[KnownBug("#1530")]
		public override void OneNewChildShouldOnlyCreateInsertWithNoUpdate_LockAndMerge()
		{
			base.OneNewChildShouldOnlyCreateInsertWithNoUpdate_LockAndMerge();
		}
	}

	[TestFixture]
	public class NonInverseFixture : FixtureBase
	{
		protected override string[] Mappings => new[] { "NonInverseMappings.hbm.xml" };
	}

}
