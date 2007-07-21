#if NET_2_0

namespace NHibernate.Impl
{
    /// <summary>
    /// Summary description for SqlClientBatchingBatcher.
    /// </summary>
    internal class SqlClientBatchingBatcher : CommandSetBatchingBatcher
    {
        internal SqlClientBatchingBatcher(ConnectionManager connectionManager)
            : base(connectionManager)
        {
        }

        protected override IDbCommandSet CreateCommandSet()
        {
            return new SqlClientCommandSet();
        }
    }
}

#endif