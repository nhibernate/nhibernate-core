using NHibernate.Cache;
using NHibernate.Cfg;
using NHibernate.Criterion;
using NHibernate.Transform;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3754
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		public override string BugNumber
		{
			get { return "NH3754"; }
		}

		private class TestEntity
		{
			public string Name { get; set; }
		}

		protected override void OnSetUp()
		{
			base.OnSetUp();
			cfg.Properties[Environment.CacheProvider] = typeof(HashtableCacheProvider).AssemblyQualifiedName;
			cfg.Properties[Environment.UseQueryCache] = "true";			
		}

		[Test]
		public void SecondLevelCacheWithResultTransformer()
		{
			using (ISession session = OpenSession())
			{
				using (ITransaction t = session.BeginTransaction())
				{
					User user = new User();
					user.Name = "Test";
					user.Id = 1;
					session.Save(user);
					session.Flush();
					session.Clear();
					var list = session.CreateCriteria<User>()
					                  .SetProjection(Projections.Property<User>(x => x.Name).As("Name")).SetResultTransformer(new AliasToBeanResultTransformer(typeof (TestEntity)))
					                  .SetCacheable(false)
					                  .List<TestEntity>();
					Assert.AreEqual(1, list.Count);
					Assert.AreEqual("Test", list[0].Name);

					session.Delete("from User");
					t.Commit();
				}
			}
		}		
	}
}