using System.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3069
{
    [TestFixture]
    public class Fixture : BugTestCase
    {
	    protected override void OnSetUp()
	    {
		    using (var session = OpenSession())
		    using (var tx = session.BeginTransaction())
		    {
			    var entity = new VersionableConcreate {Name = "Some Name"};
			    session.Save(entity);
			 
			    tx.Commit();
		    }
	    }

	    protected override void OnTearDown()
	    {
		    using (var session = OpenSession())
		    using (var tx = session.BeginTransaction())
		    {
			    session.Delete("from VersionableAbstract");
			    tx.Commit();
		    }
	    }

	    [Test]
        public void ShouldLockEntity()
        {
            using (var session = OpenSession())
            using (var tx = session.BeginTransaction())
            {
                var entity = session.Query<VersionableConcreate>().Single();
                session.Lock(entity, LockMode.Force); //exception is thrown here when incorrect update statement is generated
	            tx.Commit();
            }
        }
    }
}
