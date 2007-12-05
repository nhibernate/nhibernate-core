namespace NHibernate.Search.Backend
{
    public enum WorkType
    {
        Add,
        Delete,
        Update,
        /// <summary>
        /// Used to remove a specific instance of a class from an index.
        /// </summary>
        Purge,
        /// <summary>
        /// Used to remove all instances of a class from an index.
        /// </summary>
        PurgeAll,
        /// <summary>
        /// Used for batch indexing.
        /// </summary>
        Index
    }
}