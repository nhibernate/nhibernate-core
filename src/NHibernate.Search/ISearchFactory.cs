using NHibernate.Search.Reader;
using NHibernate.Search.Storage;

namespace NHibernate.Search
{
    /// <summary>
    /// Provide application wide operations as well as access to the underlying Lucene resources.
    /// </summary>
    public interface ISearchFactory
    {
        /// <summary>
        /// Provide the configured readerProvider strategy,
        /// hence access to a Lucene IndexReader
        /// </summary>
        IReaderProvider ReaderProvider { get; }

        /// <summary>
        /// Provide access to the DirectoryProviders (hence the Lucene Directories)
        /// for a given entity
        /// In most cases, the returned type will be a one element array.
        /// But if the given entity is configured to use sharded indexes, then multiple
        /// elements will be returned. In this case all of them should be considered.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        IDirectoryProvider[] GetDirectoryProviders(System.Type entity);

        /// <summary>
        /// Optimize all indexes
        /// </summary>
        void Optimize();

        /// <summary>
        /// Optimize the index holding <code>entityType</code>
        /// </summary>
        /// <param name="entityType"></param>
        void Optimize(System.Type entityType);
    }
}