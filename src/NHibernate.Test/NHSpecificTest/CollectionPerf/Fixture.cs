using NHibernate.Cfg;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.CollectionPerf
{
	public class Fixture : BugTestCase
	{
		private Parent parent;
		
		protected override void OnSetUp()
		{
			using (var s = OpenSession())
			{
				using (var tx = s.BeginTransaction())
				{
					parent = new Parent();
					parent.AddChild(new Child());
					s.Save(parent);
					tx.Commit();
				}				
			}
		}

		[Test]
		public void OneNewChildShouldOnlyCreateInsertWithNoUpdate_LockAndMerge()
		{
			var parentClone = parent.MakeCopy();
			parentClone.AddChild(new Child());
			using (var s = OpenSession())
			{
				using (var tx = s.BeginTransaction())
				{
					s.Lock(parent, LockMode.None);
					s.Merge(parentClone);
					s.SessionFactory.Statistics.Clear();
					tx.Commit();
				}
				Assert.That(s.SessionFactory.Statistics.PrepareStatementCount, Is.EqualTo(1));
			}
		}
		
		[Test]
		public void OneNewChildShouldOnlyCreateInsertWithNoUpdate_Merge()
		{
			var parentClone = parent.MakeCopy();
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
					s.Delete(parent);
					tx.Commit();
				}				
			}
		}
		
		protected override void Configure(Configuration configuration)
		{
			configuration.SetProperty(Environment.GenerateStatistics, "true");
		}
	}
}
