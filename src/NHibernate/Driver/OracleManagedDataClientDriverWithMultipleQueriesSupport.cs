using NHibernate.Engine;

namespace NHibernate.Driver
{
    /// <summary>
    /// Oracle Managed Driver implementation with support for Futures/MultiCriteria/MultiQuery.
    /// Proposed by Arturas Vitkauskas (http://stackoverflow.com/users/1951631/arturas-vitkauskas)
    /// http://stackoverflow.com/questions/10046461/nhibernate-multi-query-futures-with-oracle/14175635#14175635
    /// </summary>
    public class OracleManagedDataClientDriverWithMultipleQueriesSupport: 
        OracleManagedDataClientDriver
    {
        public override bool SupportsMultipleQueries
        {
            get { return true; }
        }

        public override IResultSetsCommand GetResultSetsCommand(ISessionImplementor session)
        {
            return new OracleManagedResultSetsCommandWithMultipleQueriesSupport(session);
        }
    }
}
