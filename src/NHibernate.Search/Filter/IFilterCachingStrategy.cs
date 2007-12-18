namespace NHibernate.Search.Filter
{
    /// <summary>
    /// Defines the caching filter strategy
    /// </summary>
    public interface IFilterCachingStrategy
    {
        /// <summary>
        /// initialize the strategy from the properties
        /// The Properties must not be changed
        /// </summary>
        /// <param name="properties"></param>
        void Initialize(object properties);

        /// <summary>
        /// Retrieve the cached filter for a given key or null if not cached
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Lucene.Net.Search.Filter GetCachedFilter(FilterKey key);

        /// <summary>
        /// Propose a candidate filter for caching
        /// </summary>
        /// <param name="key"></param>
        /// <param name="filter"></param>
        void AddCachedFilter(FilterKey key, Lucene.Net.Search.Filter filter);
    }
}