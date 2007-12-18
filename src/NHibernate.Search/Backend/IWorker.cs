using NHibernate.Search.Engine;

namespace NHibernate.Search.Backend
{
    /// <summary>
    /// Perform work for a given session. This implementation has to be multi threaded
    /// </summary>
    public interface IWorker
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="work"></param>
        /// <param name="session"></param>
        void PerformWork(Work work, object session);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="props"></param>
        /// <param name="searchFactoryImplementor"></param>
        void Initialize(object props, ISearchFactoryImplementor searchFactoryImplementor);
    }
}