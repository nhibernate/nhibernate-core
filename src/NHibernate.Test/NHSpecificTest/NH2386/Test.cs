using System;
using log4net;
using log4net.Appender;
using log4net.Repository.Hierarchy;
using NHibernate.Dialect;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2386 {
    [TestFixture]
    public class Test : BugTestCase {
        private MemoryAppender memoryAppender;

        protected override bool AppliesTo(Dialect.Dialect dialect)
        {
            // This test uses the automatically generated timestamp type, which is a MSSQL feature.
            return dialect is MsSql2000Dialect;
        }

        protected override void OnTearDown() {
            if (memoryAppender != null) {
                var repository = (Hierarchy) LogManager.GetRepository(typeof(Test).Assembly);
                repository.Root.RemoveAppender(memoryAppender);
                memoryAppender = null;
            }
            base.OnTearDown();
        }

        [Test]
        public void TheTest() {
            using (ISession session = OpenSession()) {
                var organisation = new Organisation();
                session.SaveOrUpdate(organisation);
                session.Flush();

                organisation.TradingNames.Add(new TradingName(organisation)
                                              {Name = "Trading Name", StartDate = DateTime.Today});
                
                session.SaveOrUpdate(organisation);

                //this line below fails 
                //AbstractBatcher:0 - Could not execute command: UPDATE tblTrnOrganisation SET  WHERE OrganisationId = @p0 AND RVersion = @p1
                //System.Data.SqlClient.SqlException: Incorrect syntax near the keyword 'WHERE'.
                session.Flush();

                session.Delete(organisation);
                session.Flush();
            }
        }
    }
}