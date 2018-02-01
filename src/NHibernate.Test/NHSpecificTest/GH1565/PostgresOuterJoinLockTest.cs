using NHibernate.Cfg;
using NHibernate.Dialect;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH1565
{
    [TestFixture]
    public class PostgresOuterJoinLockTest : BugTestCase
    {
	    [Test]
	    public void LockWithOuterJoin_ShouldBePossible()
	    {
		    using (var session = OpenSession())
		    {
			    using (var transaction = session.BeginTransaction())
			    {
				    var entity = session.Get<MainEntity>(id, LockMode.Upgrade);
				    Assert.That(entity.Id, Is.EqualTo(id));
				    transaction.Rollback();
			    }
		    }
	    }

		private int id;
        protected override void OnSetUp()
        {
            base.OnSetUp();
            using (var session = OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    session.FlushMode = FlushMode.Auto;
	                var entity = new MainEntity();
                    session.Save(entity);
                    transaction.Commit();
                    id = entity.Id;
                }
            }
        }

        protected override void OnTearDown()
        {
            base.OnTearDown();
            using (var session = OpenSession())
            {
                session.CreateSQLQuery("delete from MainEntity").ExecuteUpdate();
            }
        }

        protected override bool AppliesTo(Dialect.Dialect dialect)
        {
            return dialect is PostgreSQLDialect;
        }
    }

    public class MainEntity
    {
	    public virtual int Id { get; set; } = 0;

		public virtual string Data { get; set; }
    }
}
