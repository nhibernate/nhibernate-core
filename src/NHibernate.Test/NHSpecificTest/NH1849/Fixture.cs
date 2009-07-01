using NHibernate.Dialect;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1849
{
	using Criterion;

	[TestFixture]
    public class Fixture:BugTestCase
    {
        protected override bool AppliesTo(NHibernate.Dialect.Dialect dialect)
        {
            return dialect is MsSql2005Dialect;
        }
        
        protected override void Configure(NHibernate.Cfg.Configuration configuration)
        {
            base.Configure(configuration);

            configuration.SetProperty("dialect", "NHibernate.Test.NHSpecificTest.NH1849.CustomDialect, NHibernate.Test");
        }

        /// <summary>
        /// This test may throw an ado exception due to the absence of a full text index,
        /// however the query should compile
        /// </summary>
        [Test,Ignore]
        public void ExecutesCustomSqlFunctionContains()
        {
            sessions.Statistics.Clear();
			using (ISession session = this.OpenSession())
			{
			    session.CreateQuery("from Customer c where contains(c.Name, :smth)")
					.SetString("smth","aaaa")
			        .List();

                Assert.That(sessions.Statistics.QueryExecutionCount, Is.EqualTo(1));
            }
        }
    }
}
